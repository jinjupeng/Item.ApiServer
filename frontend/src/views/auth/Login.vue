<template>
  <div class="login-container">
    <div class="login-form">
      <div class="login-header">
        <img src="/vite.svg" alt="Logo" class="logo" />
        <h1 class="title">RBAC权限管理系统</h1>
        <p class="subtitle">欢迎登录</p>
      </div>
      
      <el-form
        ref="loginFormRef"
        :model="loginForm"
        :rules="loginRules"
        class="login-form-content"
        size="large"
      >
        <el-form-item prop="username">
          <el-input
            v-model="loginForm.username"
            placeholder="请输入用户名"
            prefix-icon="User"
            clearable
          />
        </el-form-item>
        
        <el-form-item prop="password">
          <el-input
            v-model="loginForm.password"
            type="password"
            placeholder="请输入密码"
            prefix-icon="Lock"
            show-password
            clearable
            @keyup.enter="handleLogin"
          />
        </el-form-item>
        
        <!-- 验证码 -->
        <el-form-item v-if="showCaptcha" prop="captchaCode">
          <div class="captcha-container">
            <el-input
              v-model="loginForm.captchaCode"
              placeholder="请输入验证码"
              prefix-icon="Picture"
              clearable
              style="flex: 1; margin-right: 12px;"
            />
            <div class="captcha-image" @click="refreshCaptcha">
              <img v-if="captchaImage" :src="captchaImage" alt="验证码" />
              <div v-else class="captcha-placeholder">点击获取验证码</div>
            </div>
          </div>
        </el-form-item>
        
        <el-form-item>
          <div class="login-options">
            <el-checkbox v-model="loginForm.rememberMe">
              记住我
            </el-checkbox>
            <el-link type="primary" @click="showForgotPassword = true">
              忘记密码？
            </el-link>
          </div>
        </el-form-item>
        
        <el-form-item>
          <el-button
            type="primary"
            size="large"
            style="width: 100%"
            :loading="loading"
            @click="handleLogin"
          >
            登录
          </el-button>
        </el-form-item>
      </el-form>
    </div>
    
    <!-- 忘记密码对话框 -->
    <ForgotPasswordDialog v-model="showForgotPassword" />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage, FormInstance, FormRules } from 'element-plus'
import { useAuthStore } from '@/stores/auth'
import { authApi } from '@/api/auth'
import ForgotPasswordDialog from '@/components/ForgotPasswordDialog.vue'
import type { LoginDto } from '@/types'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const loginFormRef = ref<FormInstance>()
const loading = ref(false)
const showCaptcha = ref(false)
const captchaImage = ref('')
const showForgotPassword = ref(false)

// 登录表单
const loginForm = reactive<LoginDto>({
  username: '',
  password: '',
  captchaKey: '',
  captchaCode: '',
  rememberMe: false
})

// 表单验证规则
const loginRules: FormRules = {
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 3, max: 20, message: '用户名长度在 3 到 20 个字符', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 6, max: 20, message: '密码长度在 6 到 20 个字符', trigger: 'blur' }
  ],
  captchaCode: [
    { required: true, message: '请输入验证码', trigger: 'blur' },
    { len: 4, message: '验证码长度为 4 位', trigger: 'blur' }
  ]
}

// 获取验证码
const getCaptcha = async () => {
  try {
    const response = await authApi.generateCaptcha()
    const captcha = response.data
    captchaImage.value = `data:image/png;base64,${captcha.image}`
    loginForm.captchaKey = captcha.key
    showCaptcha.value = true
  } catch (error) {
    console.error('获取验证码失败:', error)
  }
}

// 刷新验证码
const refreshCaptcha = () => {
  getCaptcha()
}

// 处理登录
const handleLogin = async () => {
  if (!loginFormRef.value) return
  
  try {
    await loginFormRef.value.validate()
    loading.value = true
    
    await authStore.login(loginForm)
    
    ElMessage.success('登录成功')
    
    // 跳转到目标页面或首页
    const redirect = route.query.redirect as string
    router.push(redirect || '/')
  } catch (error: any) {
    console.error('登录失败:', error)
    
    // 如果是验证码错误，刷新验证码
    if (error.message?.includes('验证码')) {
      refreshCaptcha()
      loginForm.captchaCode = ''
    }
    
    // 登录失败多次后显示验证码
    if (!showCaptcha.value && error.message?.includes('验证码')) {
      getCaptcha()
    }
  } finally {
    loading.value = false
  }
}

// 组件挂载时检查是否需要验证码
onMounted(() => {
  // 可以根据需要决定是否默认显示验证码
  // getCaptcha()
})
</script>

<style lang="scss" scoped>
.login-container {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  
  .login-form {
    width: 400px;
    padding: 40px;
    background: #fff;
    border-radius: 8px;
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
    
    .login-header {
      text-align: center;
      margin-bottom: 32px;
      
      .logo {
        width: 64px;
        height: 64px;
        margin-bottom: 16px;
      }
      
      .title {
        font-size: 24px;
        font-weight: 600;
        color: #333;
        margin: 0 0 8px 0;
      }
      
      .subtitle {
        font-size: 14px;
        color: #666;
        margin: 0;
      }
    }
    
    .login-form-content {
      .captcha-container {
        display: flex;
        align-items: center;
        
        .captcha-image {
          width: 120px;
          height: 40px;
          border: 1px solid #dcdfe6;
          border-radius: 4px;
          cursor: pointer;
          display: flex;
          align-items: center;
          justify-content: center;
          background: #f5f7fa;
          
          img {
            max-width: 100%;
            max-height: 100%;
          }
          
          .captcha-placeholder {
            font-size: 12px;
            color: #999;
            text-align: center;
          }
          
          &:hover {
            border-color: #409eff;
          }
        }
      }
      
      .login-options {
        display: flex;
        align-items: center;
        justify-content: space-between;
        width: 100%;
      }
    }
  }
}

// 响应式设计
@media (max-width: 480px) {
  .login-container {
    padding: 20px;
    
    .login-form {
      width: 100%;
      padding: 24px;
    }
  }
}
</style>
