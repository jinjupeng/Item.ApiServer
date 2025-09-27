<template>
  <el-dialog
    v-model="visible"
    :title="dialogTitle"
    width="600px"
    :before-close="handleClose"
  >
    <el-form
      ref="formRef"
      :model="form"
      :rules="rules"
      label-width="80px"
    >
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="菜单名称" prop="name">
            <el-input
              v-model="form.name"
              placeholder="请输入菜单名称"
              clearable
            />
          </el-form-item>
        </el-col>
        
        <el-col :span="12">
          <el-form-item label="菜单编码" prop="code">
            <el-input
              v-model="form.code"
              placeholder="请输入菜单编码"
              clearable
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="菜单类型" prop="type">
            <el-select
              v-model="form.type"
              placeholder="请选择菜单类型"
              style="width: 100%"
              @change="handleTypeChange"
            >
              <el-option label="目录" :value="MenuType.Directory" />
              <el-option label="菜单" :value="MenuType.Menu" />
              <el-option label="按钮" :value="MenuType.Button" />
            </el-select>
          </el-form-item>
        </el-col>
        
        <el-col :span="12">
          <el-form-item label="父级菜单" prop="parentId">
            <el-tree-select
              v-model="form.parentId"
              :data="parentMenuOptions"
              :props="treeSelectProps"
              placeholder="请选择父级菜单"
              clearable
              check-strictly
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row v-if="form.type !== MenuType.Button" :gutter="20">
        <el-col :span="12">
          <el-form-item label="路由路径" prop="path">
            <el-input
              v-model="form.path"
              placeholder="请输入路由路径"
              clearable
            />
          </el-form-item>
        </el-col>
        
        <el-col :span="12">
          <el-form-item label="组件路径" prop="component">
            <el-input
              v-model="form.component"
              placeholder="请输入组件路径"
              clearable
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="菜单图标" prop="icon">
            <el-input
              v-model="form.icon"
              placeholder="请输入图标名称"
              clearable
            >
              <template #prepend>
                <el-icon v-if="form.icon">
                  <component :is="form.icon" />
                </el-icon>
              </template>
            </el-input>
          </el-form-item>
        </el-col>
        
        <el-col :span="12">
          <el-form-item label="排序" prop="sort">
            <el-input-number
              v-model="form.sort"
              :min="0"
              :max="9999"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    
    <template #footer>
      <div class="dialog-footer">
        <el-button @click="handleClose">取消</el-button>
        <el-button type="primary" :loading="loading" @click="handleSubmit">
          确定
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { ElMessage, FormInstance, FormRules } from 'element-plus'
import { menusApi } from '@/api/menus'
import type { Menu, CreateMenuDto, UpdateMenuDto, MenuType } from '@/types'
import { MenuType } from '@/types'

interface Props {
  modelValue: boolean
  menu?: Menu | null
  parentMenu?: Menu | null
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const formRef = ref<FormInstance>()
const loading = ref(false)
const visible = ref(false)
const parentMenuOptions = ref<Menu[]>([])

// 是否为编辑模式
const isEdit = computed(() => !!props.menu)

// 对话框标题
const dialogTitle = computed(() => {
  if (isEdit.value) {
    return '编辑菜单'
  } else if (props.parentMenu) {
    return `新增子菜单 - ${props.parentMenu.name}`
  } else {
    return '新增菜单'
  }
})

// 表单数据
const form = reactive<CreateMenuDto & UpdateMenuDto>({
  name: '',
  code: '',
  path: '',
  component: '',
  icon: '',
  parentId: undefined,
  sort: 0,
  type: MenuType.Menu
})

// 树形选择器属性配置
const treeSelectProps = {
  children: 'children',
  label: 'name',
  value: 'id',
  disabled: (data: Menu) => !data.isActive
}

// 验证菜单编码唯一性
const validateCode = async (rule: any, value: string, callback: any) => {
  if (!value) {
    callback()
    return
  }
  
  try {
    const excludeId = isEdit.value ? props.menu!.id : undefined
    const response = await menusApi.checkMenuCode(value, excludeId)
    if (response.data) {
      callback(new Error('菜单编码已存在'))
    } else {
      callback()
    }
  } catch (error) {
    callback()
  }
}

// 表单验证规则
const rules: FormRules = {
  name: [
    { required: true, message: '请输入菜单名称', trigger: 'blur' },
    { min: 2, max: 20, message: '菜单名称长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  code: [
    { required: true, message: '请输入菜单编码', trigger: 'blur' },
    { min: 2, max: 50, message: '菜单编码长度在 2 到 50 个字符', trigger: 'blur' },
    { pattern: /^[a-zA-Z0-9_:]+$/, message: '菜单编码只能包含字母、数字、下划线和冒号', trigger: 'blur' },
    { validator: validateCode, trigger: 'blur' }
  ],
  type: [
    { required: true, message: '请选择菜单类型', trigger: 'change' }
  ],
  path: [
    { required: true, message: '请输入路由路径', trigger: 'blur' }
  ],
  sort: [
    { required: true, message: '请输入排序值', trigger: 'blur' }
  ]
}

// 监听 modelValue 变化
watch(() => props.modelValue, (val) => {
  visible.value = val
  if (val) {
    initForm()
    getParentMenus()
  }
})

// 监听 visible 变化
watch(visible, (val) => {
  emit('update:modelValue', val)
  if (!val) {
    resetForm()
  }
})

// 菜单类型变化处理
const handleTypeChange = (type: MenuType) => {
  if (type === MenuType.Button) {
    // 按钮类型不需要路由和组件
    form.path = ''
    form.component = ''
  }
}

// 初始化表单
const initForm = () => {
  if (props.menu) {
    // 编辑模式
    Object.assign(form, {
      name: props.menu.name,
      code: props.menu.code,
      path: props.menu.path || '',
      component: props.menu.component || '',
      icon: props.menu.icon || '',
      parentId: props.menu.parentId,
      sort: props.menu.sort,
      type: props.menu.type
    })
  } else {
    // 新增模式
    resetForm()
    if (props.parentMenu) {
      form.parentId = props.parentMenu.id
    }
  }
}

// 重置表单
const resetForm = () => {
  if (formRef.value) {
    formRef.value.resetFields()
  }
  Object.assign(form, {
    name: '',
    code: '',
    path: '',
    component: '',
    icon: '',
    parentId: undefined,
    sort: 0,
    type: MenuType.Menu
  })
}

// 获取父级菜单选项
const getParentMenus = async () => {
  try {
    const response = await menusApi.getParentMenus()
    parentMenuOptions.value = response.data
  } catch (error) {
    console.error('获取父级菜单失败:', error)
  }
}

// 关闭对话框
const handleClose = () => {
  visible.value = false
}

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    loading.value = true
    
    if (isEdit.value) {
      // 编辑菜单
      const updateData: UpdateMenuDto = {
        name: form.name,
        code: form.code,
        path: form.path,
        component: form.component,
        icon: form.icon,
        parentId: form.parentId,
        sort: form.sort,
        type: form.type
      }
      await menusApi.updateMenu(props.menu!.id, updateData)
      ElMessage.success('菜单更新成功')
    } else {
      // 新增菜单
      const createData: CreateMenuDto = {
        name: form.name,
        code: form.code,
        path: form.path,
        component: form.component,
        icon: form.icon,
        parentId: form.parentId,
        sort: form.sort,
        type: form.type
      }
      await menusApi.createMenu(createData)
      ElMessage.success('菜单创建成功')
    }
    
    emit('success')
    handleClose()
  } catch (error) {
    console.error('保存菜单失败:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  getParentMenus()
})
</script>
