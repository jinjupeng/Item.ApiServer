import { createPinia } from 'pinia'

// 创建pinia实例
const pinia = createPinia()

export default pinia

// 导出所有store
export { useAuthStore } from './auth'
export { useAppStore } from './app'
