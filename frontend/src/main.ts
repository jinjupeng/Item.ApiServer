import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import pinia from './stores'

// Element Plus - 使用自动导入，无需手动导入
// import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import 'element-plus/theme-chalk/dark/css-vars.css'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'

// 样式
import '@/styles/index.scss'

// 指令
import { setupDirectives } from '@/directives'

// 创建应用实例
const app = createApp(App)

// 注册Element Plus图标
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}

// 使用插件
app.use(router)
app.use(pinia)
// ElementPlus 通过 unplugin-vue-components 自动导入，无需手动注册
// app.use(ElementPlus, { size: 'default' })

// 注册指令
setupDirectives(app)

// 挂载应用
app.mount('#app')
