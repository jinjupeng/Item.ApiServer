<template>
  <el-dialog
    v-model="visible"
    title="忘记密码"
    width="400px"
    :before-close="handleClose"
  >
    <el-steps :active="currentStep" align-center>
      <el-step title="验证身份" />
      <el-step title="重置密码" />
      <el-step title="完成" />
    </el-steps>
    
    <div class="step-content">
      <!-- 步骤1：验证身份 -->
      <div v-if="currentStep === 0">
        <el-form
          ref="step1FormRef"
          :model="step1Form"
          :rules="step1Rules"
          label-width="100px"
        >
          <el-form-item label="用户名/邮箱" prop="usernameOrEmail">
            <el-input
              v-model="step1Form.usernameOrEmail"
              placeholder="请输入用户名或邮箱"
              clearable
            />
          </el-form-item>
        </el-form>
        
        <div class="step-buttons">
          <el-button @click="handleClose">取消</el-button>
          <el-button type="primary" :loading="loading" @click="sendResetCode">
            发送验证码
          </el-button>
        </div>
      </div>
      
      <!-- 步骤2：重置密码 -->
      <div v-if="currentStep === 1">
        <el-form
          ref="step2FormRef"
          :model="step2Form"
          :rules="step2Rules"
          label-width="100px"
        >
          <el-form-item label="验证码" prop="code">
            <el-input
              v-model="step2Form.code"
              placeholder="请输入验证码"
              clearable
            />
          </el-form-item>
          
          <el-form-item label="新密码" prop="newPassword">
            <el-input
              v-model="step2Form.newPassword"
              type="password"
              placeholder="请输入新密码"
              show-password
              clearable
            />
          </el-form-item>
          
          <el-form-item label="确认密码" prop="confirmPassword">
            <el-input
              v-model="step2Form.confirmPassword"
              type="password"
              placeholder="请再次输入新密码"
              show-password
              clearable
            />
          </el-form-item>
        </el-form>
        
        <div class="step-buttons">
          <el-button @click="currentStep = 0">上一步</el-button>
          <el-button type="primary" :loading="loading" @click="resetPassword">
            重置密码
          </el-button>
        </div>
      </div>
      
      <!-- 步骤3：完成 -->
      <div v-if="currentStep === 2" class="success-content">
        <el-result
          icon="success"
          title="密码重置成功"
          sub-title="您的密码已成功重置，请使用新密码登录"
        >
          <template #extra>
            <el-button type="primary" @click="handleClose">
              返回登录
            </el-button>
          </template>
        </el-result>
      </div>
    </div>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue'
import { ElMessage, FormInstance, FormRules } from 'element-plus'
import { authApi } from '@/api/auth'
import type { ResetPasswordDto } from '@/types'

interface Props {
  modelValue: boolean
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const step1FormRef = ref<FormInstance>()
const step2FormRef = ref<FormInstance>()
const loading = ref(false)
const visible = ref(false)
const currentStep = ref(0)

const step1Form = reactive({
  usernameOrEmail: ''
})

const step2Form = reactive<ResetPasswordDto>({
  usernameOrEmail: '',
  code: '',
  newPassword: '',
  confirmPassword: ''
})

// 验证确认密码
const validateConfirmPassword = (rule: any, value: string, callback: any) => {
  if (value !== step2Form.newPassword) {
    callback(new Error('两次输入的密码不一致'))
  } else {
    callback()
  }
}

const step1Rules: FormRules = {
  usernameOrEmail: [
    { required: true, message: '请输入用户名或邮箱', trigger: 'blur' }
  ]
}

const step2Rules: FormRules = {
  code: [
    { required: true, message: '请输入验证码', trigger: 'blur' }
  ],
  newPassword: [
    { required: true, message: '请输入新密码', trigger: 'blur' },
    { min: 6, max: 20, message: '密码长度在 6 到 20 个字符', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, message: '请确认新密码', trigger: 'blur' },
    { validator: validateConfirmPassword, trigger: 'blur' }
  ]
}

// 监听 modelValue 变化
watch(() => props.modelValue, (val) => {
  visible.value = val
})

// 监听 visible 变化
watch(visible, (val) => {
  emit('update:modelValue', val)
  if (!val) {
    resetForm()
  }
})

// 重置表单
const resetForm = () => {
  currentStep.value = 0
  step1Form.usernameOrEmail = ''
  Object.assign(step2Form, {
    usernameOrEmail: '',
    code: '',
    newPassword: '',
    confirmPassword: ''
  })
  
  if (step1FormRef.value) {
    step1FormRef.value.resetFields()
  }
  if (step2FormRef.value) {
    step2FormRef.value.resetFields()
  }
}

// 关闭对话框
const handleClose = () => {
  visible.value = false
}

// 发送重置验证码
const sendResetCode = async () => {
  if (!step1FormRef.value) return
  
  try {
    await step1FormRef.value.validate()
    loading.value = true
    
    await authApi.sendResetCode(step1Form.usernameOrEmail)
    
    step2Form.usernameOrEmail = step1Form.usernameOrEmail
    currentStep.value = 1
    
    ElMessage.success('验证码已发送，请查收')
  } catch (error) {
    console.error('发送验证码失败:', error)
  } finally {
    loading.value = false
  }
}

// 重置密码
const resetPassword = async () => {
  if (!step2FormRef.value) return
  
  try {
    await step2FormRef.value.validate()
    loading.value = true
    
    await authApi.resetPassword(step2Form)
    
    currentStep.value = 2
    ElMessage.success('密码重置成功')
  } catch (error) {
    console.error('重置密码失败:', error)
  } finally {
    loading.value = false
  }
}
</script>

<style lang="scss" scoped>
.step-content {
  margin: 24px 0;
  min-height: 200px;
  
  .step-buttons {
    text-align: right;
    margin-top: 24px;
    
    .el-button {
      margin-left: 8px;
    }
  }
  
  .success-content {
    text-align: center;
  }
}
</style>
