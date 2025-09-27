import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useAppStore = defineStore('app', () => {
  // 侧边栏状态
  const sidebarCollapsed = ref(false)
  
  // 设备类型
  const device = ref<'desktop' | 'mobile'>('desktop')
  
  // 主题
  const theme = ref<'light' | 'dark'>('light')
  
  // 语言
  const language = ref('zh-CN')
  
  // 页面加载状态
  const pageLoading = ref(false)
  
  // 切换侧边栏
  const toggleSidebar = () => {
    sidebarCollapsed.value = !sidebarCollapsed.value
  }
  
  // 设置设备类型
  const setDevice = (deviceType: 'desktop' | 'mobile') => {
    device.value = deviceType
  }
  
  // 切换主题
  const toggleTheme = () => {
    theme.value = theme.value === 'light' ? 'dark' : 'light'
  }
  
  // 设置语言
  const setLanguage = (lang: string) => {
    language.value = lang
  }
  
  // 设置页面加载状态
  const setPageLoading = (loading: boolean) => {
    pageLoading.value = loading
  }
  
  return {
    // 状态
    sidebarCollapsed,
    device,
    theme,
    language,
    pageLoading,
    
    // 方法
    toggleSidebar,
    setDevice,
    toggleTheme,
    setLanguage,
    setPageLoading
  }
})
