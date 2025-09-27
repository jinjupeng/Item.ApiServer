<template>
  <div class="login-container">
    <div class="login-background">
      <div class="bg-shapes">
        <div class="shape shape-1"></div>
        <div class="shape shape-2"></div>
        <div class="shape shape-3"></div>
      </div>
    </div>

    <div class="login-content">
      <div class="login-form-container">
        <!-- Logo和标题 -->
        <div class="login-header">
          <img src="@/assets/logo.png" alt="Logo" class="logo" />
          <h1 class="title">{{ appStore.settings.title }}</h1>
          <p class="subtitle">企业级权限管理系统</p>
        </div>

        <!-- 登录表单 -->
        <el-form
          ref="loginFormRef"
          :model="loginForm"
          :rules="loginRules"
          class="login-form"
          size="large"
          @keyup.enter="handleLogin"
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
                style="flex: 1"
              />
              <div class="captcha-image" @click="refreshCaptcha">
                <img v-if="captcha.image" :src="captcha.image" alt="验证码" />
                <span v-else>点击获取</span>
              </div>
            </div>
          </el-form-item>

          <!-- 记住我 -->
          <el-form-item>
            <div class="login-options">
              <el-checkbox v-model="loginForm.rememberMe">
                记住我
              </el-checkbox>
              <el-link type="primary" @click="handleForgotPassword">
                忘记密码？
              </el-link>
            </div>
          </el-form-item>

          <!-- 登录按钮 -->
          <el-form-item>
            <el-button
              type="primary"
              size="large"
              :loading="loading"
              class="login-button"
              @click="handleLogin"
            >
              {{ loading ? '登录中...' : '登录' }}
            </el-button>
          </el-form-item>
        </el-form>

        <!-- 其他登录方式 -->
        <div class="other-login">
          <div class="divider">
            <span>其他登录方式</span>
          </div>
          <div class="social-login">
            <el-tooltip content="微信登录" placement="top">
              <div class="social-item wechat">
                <el-icon><ChatDotRound /></el-icon>
              </div>
            </el-tooltip>
            <el-tooltip content="QQ登录" placement="top">
              <div class="social-item qq">
                <el-icon><User /></el-icon>
              </div>
            </el-tooltip>
            <el-tooltip content="钉钉登录" placement="top">
              <div class="social-item dingtalk">
                <el-icon><OfficeBuilding /></el-icon>
              </div>
            </el-tooltip>
          </div>
        </div>
      </div>

      <!-- 版权信息 -->
      <div class="login-footer">
        <p>© 2024 ApiServer. All rights reserved.</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { useAuthStore } from '@/stores/auth'
import { useAppStore } from '@/stores/app'
import { AuthApi } from '@/api'
import type { LoginDto, CaptchaDto } from '@/types/api'
import { 
  User, 
  Lock, 
  Picture, 
  ChatDotRound, 
  OfficeBuilding 
} from '@element-plus/icons-vue'

const router = useRouter()
const authStore = useAuthStore()
const appStore = useAppStore()

const loginFormRef = ref<FormInstance>()
const loading = ref(false)
const showCaptcha = ref(false)

// 登录表单
const loginForm = reactive<LoginDto>({
  username: 'admin',
  password: '123456',
  captchaKey: '',
  captchaCode: '',
  rememberMe: false
})

// 验证码
const captcha = reactive<CaptchaDto>({
  key: '',
  image: ''
})

// 表单验证规则
const loginRules: FormRules = {
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 2, max: 20, message: '用户名长度在 2 到 20 个字符', trigger: 'blur' }
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
    const response = await AuthApi.generateCaptcha()
    captcha.key = response.data.key
    captcha.image = response.data.image
    loginForm.captchaKey = response.data.key
  } catch (error) {
    console.error('获取验证码失败:', error)
  }
}

// 刷新验证码
const refreshCaptcha = () => {
  getCaptcha()
  loginForm.captchaCode = ''
}

// 登录处理
const handleLogin = async () => {
  if (!loginFormRef.value) return

  try {
    await loginFormRef.value.validate()
    loading.value = true

    const response = await authStore.login(loginForm)
    
    ElMessage.success('登录成功')
    
    // 跳转到首页或之前访问的页面
    const redirect = router.currentRoute.value.query.redirect as string
    router.push(redirect || '/')
    
  } catch (error: any) {
    console.error('登录失败:', error)
    
    // 如果是验证码错误，刷新验证码
    if (error.message?.includes('验证码')) {
      refreshCaptcha()
    }
    
    // 登录失败次数过多时显示验证码
    if (!showCaptcha.value && error.message?.includes('次数')) {
      showCaptcha.value = true
      await getCaptcha()
    }
  } finally {
    loading.value = false
  }
}

// 忘记密码
const handleForgotPassword = () => {
  ElMessage.info('请联系管理员重置密码')
}

// 社交登录
const handleSocialLogin = (type: string) => {
  ElMessage.info(`${type}登录功能开发中...`)
}

onMounted(() => {
  // 如果需要验证码，自动获取
  if (showCaptcha.value) {
    getCaptcha()
  }
})
</script>

<style lang="scss" scoped>
.login-container {
  position: relative;
  width: 100vw;
  height: 100vh;
  overflow: hidden;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);

  .login-background {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    overflow: hidden;

    .bg-shapes {
      position: relative;
      width: 100%;
      height: 100%;

      .shape {
        position: absolute;
        border-radius: 50%;
        background: rgba(255, 255, 255, 0.1);
        backdrop-filter: blur(10px);
        animation: float 6s ease-in-out infinite;

        &.shape-1 {
          width: 200px;
          height: 200px;
          top: 10%;
          left: 10%;
          animation-delay: 0s;
        }

        &.shape-2 {
          width: 150px;
          height: 150px;
          top: 60%;
          right: 15%;
          animation-delay: 2s;
        }

        &.shape-3 {
          width: 100px;
          height: 100px;
          bottom: 20%;
          left: 20%;
          animation-delay: 4s;
        }
      }
    }
  }

  .login-content {
    position: relative;
    z-index: 10;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    min-height: 100vh;
    padding: 20px;

    .login-form-container {
      width: 100%;
      max-width: 400px;
      background: rgba(255, 255, 255, 0.95);
      backdrop-filter: blur(20px);
      border-radius: 16px;
      padding: 40px;
      box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
      border: 1px solid rgba(255, 255, 255, 0.2);

      .login-header {
        text-align: center;
        margin-bottom: 32px;

        .logo {
          width: 64px;
          height: 64px;
          border-radius: 12px;
          margin-bottom: 16px;
        }

        .title {
          font-size: 24px;
          font-weight: 600;
          color: var(--el-text-color-primary);
          margin: 0 0 8px 0;
        }

        .subtitle {
          font-size: 14px;
          color: var(--el-text-color-secondary);
          margin: 0;
        }
      }

      .login-form {
        .captcha-container {
          display: flex;
          gap: 12px;
          align-items: center;

          .captcha-image {
            width: 100px;
            height: 40px;
            border: 1px solid var(--el-border-color);
            border-radius: 6px;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            background-color: var(--el-fill-color-light);
            transition: all var(--el-transition-duration);

            &:hover {
              border-color: var(--el-color-primary);
            }

            img {
              width: 100%;
              height: 100%;
              object-fit: cover;
              border-radius: 4px;
            }

            span {
              font-size: 12px;
              color: var(--el-text-color-secondary);
            }
          }
        }

        .login-options {
          display: flex;
          justify-content: space-between;
          align-items: center;
          width: 100%;
        }

        .login-button {
          width: 100%;
          height: 48px;
          font-size: 16px;
          font-weight: 600;
          border-radius: 8px;
        }
      }

      .other-login {
        margin-top: 24px;

        .divider {
          position: relative;
          text-align: center;
          margin: 20px 0;

          &::before {
            content: '';
            position: absolute;
            top: 50%;
            left: 0;
            right: 0;
            height: 1px;
            background-color: var(--el-border-color-lighter);
          }

          span {
            background-color: rgba(255, 255, 255, 0.95);
            padding: 0 16px;
            font-size: 12px;
            color: var(--el-text-color-secondary);
          }
        }

        .social-login {
          display: flex;
          justify-content: center;
          gap: 16px;

          .social-item {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            transition: all var(--el-transition-duration);
            border: 1px solid var(--el-border-color-light);

            &:hover {
              transform: translateY(-2px);
              box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
            }

            &.wechat {
              background-color: #07c160;
              color: white;
              border-color: #07c160;
            }

            &.qq {
              background-color: #12b7f5;
              color: white;
              border-color: #12b7f5;
            }

            &.dingtalk {
              background-color: #2eabff;
              color: white;
              border-color: #2eabff;
            }

            .el-icon {
              font-size: 18px;
            }
          }
        }
      }
    }

    .login-footer {
      margin-top: 32px;
      text-align: center;

      p {
        font-size: 12px;
        color: rgba(255, 255, 255, 0.8);
        margin: 0;
      }
    }
  }
}

@keyframes float {
  0%, 100% {
    transform: translateY(0px);
  }
  50% {
    transform: translateY(-20px);
  }
}

// 移动端适配
@media (max-width: 768px) {
  .login-container {
    .login-content {
      padding: 16px;

      .login-form-container {
        padding: 24px;
        border-radius: 12px;

        .login-header {
          margin-bottom: 24px;

          .logo {
            width: 48px;
            height: 48px;
          }

          .title {
            font-size: 20px;
          }
        }
      }
    }
  }
}
</style>
