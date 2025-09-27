import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useAppStore = defineStore('app', () => {
  // 侧边栏状态
  const sidebarCollapsed = ref(false)
  
  // 设备类型
  const device = ref<'desktop' | 'mobile'>('desktop')
  
  // 主题模式
  const isDark = ref(false)
  
  // 语言
  const language = ref('zh-cn')
  
  // 页面加载状态
  const pageLoading = ref(false)
  
  // 全局加载状态
  const globalLoading = ref(false)
  
  // 面包屑导航
  const breadcrumbs = ref<Array<{ title: string; path?: string }>>([])
  
  // 标签页
  const tabs = ref<Array<{ title: string; path: string; name: string; closable?: boolean }>>([])
  const activeTab = ref('')
  
  // 系统设置
  const settings = ref({
    title: 'ApiServer 管理系统',
    logo: '/logo.png',
    fixedHeader: true,
    showTabs: true,
    showBreadcrumb: true,
    showFooter: true,
    layout: 'default', // default | top | mix
    theme: 'light', // light | dark
    primaryColor: '#409EFF',
    componentSize: 'default' // large | default | small
  })

  // 切换侧边栏
  const toggleSidebar = () => {
    sidebarCollapsed.value = !sidebarCollapsed.value
  }

  // 设置设备类型
  const setDevice = (deviceType: 'desktop' | 'mobile') => {
    device.value = deviceType
    if (deviceType === 'mobile') {
      sidebarCollapsed.value = true
    }
  }

  // 切换主题
  const toggleTheme = () => {
    isDark.value = !isDark.value
    settings.value.theme = isDark.value ? 'dark' : 'light'
    
    // 更新HTML类名
    const html = document.documentElement
    if (isDark.value) {
      html.classList.add('dark')
    } else {
      html.classList.remove('dark')
    }
  }

  // 设置语言
  const setLanguage = (lang: string) => {
    language.value = lang
  }

  // 设置页面加载状态
  const setPageLoading = (loading: boolean) => {
    pageLoading.value = loading
  }

  // 设置全局加载状态
  const setGlobalLoading = (loading: boolean) => {
    globalLoading.value = loading
  }

  // 设置面包屑
  const setBreadcrumbs = (crumbs: Array<{ title: string; path?: string }>) => {
    breadcrumbs.value = crumbs
  }

  // 添加标签页
  const addTab = (tab: { title: string; path: string; name: string; closable?: boolean }) => {
    const existingTab = tabs.value.find(t => t.path === tab.path)
    if (!existingTab) {
      tabs.value.push({ closable: true, ...tab })
    }
    activeTab.value = tab.path
  }

  // 移除标签页
  const removeTab = (path: string) => {
    const index = tabs.value.findIndex(tab => tab.path === path)
    if (index > -1) {
      tabs.value.splice(index, 1)
      
      // 如果移除的是当前激活的标签页，切换到其他标签页
      if (activeTab.value === path && tabs.value.length > 0) {
        const newIndex = index >= tabs.value.length ? tabs.value.length - 1 : index
        activeTab.value = tabs.value[newIndex].path
      }
    }
  }

  // 移除其他标签页
  const removeOtherTabs = (path: string) => {
    tabs.value = tabs.value.filter(tab => tab.path === path || !tab.closable)
    activeTab.value = path
  }

  // 移除所有标签页
  const removeAllTabs = () => {
    tabs.value = tabs.value.filter(tab => !tab.closable)
    if (tabs.value.length > 0) {
      activeTab.value = tabs.value[0].path
    } else {
      activeTab.value = ''
    }
  }

  // 设置激活标签页
  const setActiveTab = (path: string) => {
    activeTab.value = path
  }

  // 更新设置
  const updateSettings = (newSettings: Partial<typeof settings.value>) => {
    settings.value = { ...settings.value, ...newSettings }
  }

  // 重置应用状态
  const resetApp = () => {
    sidebarCollapsed.value = false
    device.value = 'desktop'
    isDark.value = false
    language.value = 'zh-cn'
    pageLoading.value = false
    globalLoading.value = false
    breadcrumbs.value = []
    tabs.value = []
    activeTab.value = ''
  }

  return {
    // 状态
    sidebarCollapsed,
    device,
    isDark,
    language,
    pageLoading,
    globalLoading,
    breadcrumbs,
    tabs,
    activeTab,
    settings,

    // 方法
    toggleSidebar,
    setDevice,
    toggleTheme,
    setLanguage,
    setPageLoading,
    setGlobalLoading,
    setBreadcrumbs,
    addTab,
    removeTab,
    removeOtherTabs,
    removeAllTabs,
    setActiveTab,
    updateSettings,
    resetApp
  }
})
