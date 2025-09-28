import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from './router'
import App from './App.vue'
import { useAuthStore } from './stores/auth'

// 样式
import 'element-plus/dist/index.css'
import './styles/index.scss'

// 进度条
import NProgress from 'nprogress'
import 'nprogress/nprogress.css'

// 配置进度条
NProgress.configure({ showSpinner: false })

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)

// 在挂载应用前进行初始化
async function initializeApp() {
  const authStore = useAuthStore()
  try {
    await authStore.init()
  } catch (error) {
    console.error('应用初始化失败:', error)
    // 根据需要处理初始化失败的情况，例如重定向到错误页面
  }

  app.use(router)
  app.mount('#app')
}

initializeApp()

