<template>
  <el-dialog
    v-model="visible"
    :title="isEdit ? '编辑用户' : '新增用户'"
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
          <el-form-item label="用户名" prop="userName">
            <el-input
              v-model="form.userName"
              placeholder="请输入用户名"
              :disabled="isEdit"
              clearable
            />
          </el-form-item>
        </el-col>
        
        <el-col :span="12">
          <el-form-item label="姓名" prop="nickName">
            <el-input
              v-model="form.nickName"
              placeholder="请输入姓名"
              clearable
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="邮箱" prop="email">
            <el-input
              v-model="form.email"
              placeholder="请输入邮箱"
              clearable
            />
          </el-form-item>
        </el-col>
        
        <el-col :span="12">
          <el-form-item label="手机号" prop="phone">
            <el-input
              v-model="form.phone"
              placeholder="请输入手机号"
              clearable
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-form-item v-if="!isEdit" label="密码" prop="password">
        <el-input
          v-model="form.password"
          type="password"
          placeholder="请输入密码"
          show-password
          clearable
        />
      </el-form-item>
      
      <el-form-item label="角色" prop="roleIds">
        <el-select
          v-model="form.roleIds"
          placeholder="请选择角色"
          multiple
          style="width: 100%"
        >
          <el-option
            v-for="role in roles"
            :key="role.id"
            :label="role.name"
            :value="role.id"
          />
        </el-select>
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
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { ElMessage, FormInstance, FormRules } from 'element-plus'
import { usersApi } from '@/api/users'
import { rolesApi } from '@/api/roles'
import type { User, CreateUserDto, UpdateUserDto, Role } from '@/types'

interface Props {
  modelValue: boolean
  user?: User | null
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
const roles = ref<Role[]>([])

// 是否为编辑模式
const isEdit = computed(() => !!props.user)

// 表单数据
const form = reactive<CreateUserDto & UpdateUserDto>({
  userName: '',
  email: '',
  nickName: '',
  phone: '',
  password: '',
  roleIds: []
})

// 验证邮箱格式
const validateEmail = (rule: any, value: string, callback: any) => {
  if (value && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
    callback(new Error('请输入正确的邮箱格式'))
  } else {
    callback()
  }
}

// 验证手机号格式
const validatePhone = (rule: any, value: string, callback: any) => {
  if (value && !/^1[3-9]\d{9}$/.test(value)) {
    callback(new Error('请输入正确的手机号格式'))
  } else {
    callback()
  }
}

// 表单验证规则
const rules: FormRules = {
  userName: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 3, max: 20, message: '用户名长度在 3 到 20 个字符', trigger: 'blur' },
    { pattern: /^[a-zA-Z0-9_]+$/, message: '用户名只能包含字母、数字和下划线', trigger: 'blur' }
  ],
  nickName: [
    { required: true, message: '请输入姓名', trigger: 'blur' },
    { min: 2, max: 10, message: '姓名长度在 2 到 10 个字符', trigger: 'blur' }
  ],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'blur' },
    { validator: validateEmail, trigger: 'blur' }
  ],
  phone: [
    { validator: validatePhone, trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 6, max: 20, message: '密码长度在 6 到 20 个字符', trigger: 'blur' }
  ],
  roleIds: [
    { required: true, message: '请选择角色', trigger: 'change' }
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
  if (props.user) {
    // 编辑模式
    Object.assign(form, {
      userName: props.user.userName,
      email: props.user.email,
      nickName: props.user.nickName,
      phone: props.user.phone || '',
      roleIds: props.user.roles?.map(role => role.id) || []
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
    userName: '',
    email: '',
    nickName: '',
    phone: '',
    password: '',
    roleIds: []
  })
}

// 获取角色列表
const getRoles = async () => {
  try {
    const response = await rolesApi.getAllRoles()
    roles.value = response.data.filter(role => role.isActive)
  } catch (error) {
    console.error('获取角色列表失败:', error)
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
      // 编辑用户
      const updateData: UpdateUserDto = {
        email: form.email,
        nickName: form.nickName,
        phone: form.phone,
        roleIds: form.roleIds
      }
      await usersApi.updateUser(props.user!.id, updateData)
      ElMessage.success('用户更新成功')
    } else {
      // 新增用户
      const createData: CreateUserDto = {
        userName: form.userName,
        email: form.email,
        nickName: form.nickName,
        phone: form.phone,
        password: form.password,
        roleIds: form.roleIds
      }
      await usersApi.createUser(createData)
      ElMessage.success('用户创建成功')
    }
    
    emit('success')
    handleClose()
  } catch (error) {
    console.error('保存用户失败:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  getRoles()
})
</script>
