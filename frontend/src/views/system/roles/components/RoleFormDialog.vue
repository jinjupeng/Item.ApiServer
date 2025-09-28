<template>
  <el-dialog
    v-model="visible"
    :title="isEdit ? '编辑角色' : '新增角色'"
    width="500px"
    :before-close="handleClose"
  >
    <el-form
      ref="formRef"
      :model="form"
      :rules="rules"
      label-width="80px"
    >
      <el-form-item label="角色名称" prop="name">
        <el-input
          v-model="form.name"
          placeholder="请输入角色名称"
          clearable
        />
      </el-form-item>
      
      <el-form-item label="角色编码" prop="code">
        <el-input
          v-model="form.code"
          placeholder="请输入角色编码"
          :disabled="isEdit"
          clearable
        />
      </el-form-item>
      
      <el-form-item label="描述" prop="description">
        <el-input
          v-model="form.description"
          type="textarea"
          :rows="3"
          placeholder="请输入角色描述"
        />
      </el-form-item>
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
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage, FormInstance, FormRules } from 'element-plus'
import { rolesApi } from '@/api/roles'
import type { Role, CreateRoleDto, UpdateRoleDto } from '@/types'

interface Props {
  modelValue: boolean
  role?: Role | null
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

// 是否为编辑模式
const isEdit = computed(() => !!props.role)

// 表单数据
const form = reactive<CreateRoleDto & UpdateRoleDto>({
  name: '',
  code: '',
  description: ''
})

// 验证角色编码唯一性
const validateCode = async (rule: any, value: string, callback: any) => {
  if (!value) {
    callback()
    return
  }
  
  try {
    const excludeId = isEdit.value ? props.role!.id : undefined
    const response = await rolesApi.checkRoleName(value, excludeId)
    if (response.data) {
      callback(new Error('角色编码已存在'))
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
    { required: true, message: '请输入角色名称', trigger: 'blur' },
    { min: 2, max: 20, message: '角色名称长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  code: [
    { required: true, message: '请输入角色编码', trigger: 'blur' },
    { min: 2, max: 50, message: '角色编码长度在 2 到 50 个字符', trigger: 'blur' },
    { pattern: /^[a-zA-Z0-9_]+$/, message: '角色编码只能包含字母、数字和下划线', trigger: 'blur' },
    { asyncValidator: validateCode, trigger: 'blur' }
  ]
}

// 监听 modelValue 变化
watch(() => props.modelValue, (val) => {
  visible.value = val
  if (val) {
    initForm()
  }
})

// 监听 visible 变化
watch(visible, (val) => {
  emit('update:modelValue', val)
  if (!val) {
    resetForm()
  }
})

// 初始化表单
const initForm = () => {
  if (props.role) {
    // 编辑模式
    Object.assign(form, {
      name: props.role.name,
      code: props.role.code,
      description: props.role.description || ''
    })
  } else {
    // 新增模式
    resetForm()
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
    description: ''
  })
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
      // 编辑角色
      const updateData: UpdateRoleDto = {
        name: form.name,
        code: form.code,
        description: form.description
      }
      await rolesApi.updateRole(props.role!.id, updateData)
      ElMessage.success('角色更新成功')
    } else {
      // 新增角色
      const createData: CreateRoleDto = {
        name: form.name,
        code: form.code,
        description: form.description
      }
      await rolesApi.createRole(createData)
      ElMessage.success('角色创建成功')
    }
    
    emit('success')
    handleClose()
  } catch (error) {
    console.error('保存角色失败:', error)
  } finally {
    loading.value = false
  }
}
</script>
