import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/api/auth'
import { menusApi } from '@/api/menus'
import type { User, Permission, Menu, LoginDto, LoginResponse } from '@/types'
import Cookies from 'js-cookie'

const TOKEN_KEY = 'access_token'
const REFRESH_TOKEN_KEY = 'refresh_token'
const USER_INFO_KEY = 'user_info'

export const useAuthStore = defineStore('auth', () => {
  // 状态
  const token = ref<string>(Cookies.get(TOKEN_KEY) || '')
  const refreshToken = ref<string>(Cookies.get(REFRESH_TOKEN_KEY) || '')
  const userInfo = ref<User | null>(null)
  const permissions = ref<Permission[]>([])
  const userMenus = ref<Menu[]>([])

  // 计算属性
  const isLoggedIn = computed(() => !!token.value)
  const userRoles = computed(() => userInfo.value?.roles || [])
  const permissionCodes = computed(() => permissions.value.map(p => p.code))

  // 获取用户菜单
  const fetchUserMenus = async () => {
    if (!isLoggedIn.value) return
    try {
      const response = await menusApi.getCurrentUserMenuTree()
      userMenus.value = response.data || []
    } catch (error) {
      console.error('获取用户菜单失败:', error)
      userMenus.value = []
    }
  }

  // 初始化
  const init = async () => {
    const savedUserInfo = localStorage.getItem(USER_INFO_KEY)
    if (savedUserInfo) {
      try {
        userInfo.value = JSON.parse(savedUserInfo)
      } catch (error) {
        console.error('解析用户信息失败:', error)
        localStorage.removeItem(USER_INFO_KEY)
      }
    }

    if (isLoggedIn.value) {
      await Promise.all([
        getUserPermissions(),
        fetchUserMenus()
      ])
    }
  }
  // init()

  // 登录
  const login = async (loginData: LoginDto): Promise<LoginResponse> => {
    try {
      console.log('开始登录请求...')
      const response = await authApi.login(loginData)
      const loginResult = response.data

      console.log('登录响应:', loginResult)

      // 保存token
      token.value = loginResult.accessToken
      refreshToken.value = loginResult.refreshToken
      userInfo.value = loginResult.userInfo

      console.log('保存token到状态:', loginResult.accessToken)

      // 持久化存储
      Cookies.set(TOKEN_KEY, loginResult.accessToken, { expires: 7 })
      Cookies.set(REFRESH_TOKEN_KEY, loginResult.refreshToken, { expires: 30 })
      localStorage.setItem(USER_INFO_KEY, JSON.stringify(loginResult.userInfo))

      console.log('Token已保存到Cookie和localStorage')
      console.log('当前token状态:', token.value)

      // 获取用户权限和菜单
      await Promise.all([
        getUserPermissions(),
        fetchUserMenus()
      ])

      return loginResult
    } catch (error) {
      console.error('登录失败:', error)
      throw error
    }
  }

  // 登出
  const logout = async () => {
    try {
      if (token.value) {
        await authApi.logout()
      }
    } catch (error) {
      console.error('登出请求失败:', error)
    } finally {
      // 清除所有状态和存储
      token.value = ''
      refreshToken.value = ''
      userInfo.value = null
      permissions.value = []
      userMenus.value = []

      Cookies.remove(TOKEN_KEY)
      Cookies.remove(REFRESH_TOKEN_KEY)
      localStorage.removeItem(USER_INFO_KEY)
    }
  }

  // 刷新token
  const refreshAccessToken = async (): Promise<boolean> => {
    try {
      if (!refreshToken.value) {
        throw new Error('没有刷新令牌')
      }

      const response = await authApi.refreshToken({ refreshToken: refreshToken.value })
      const loginResult = response.data

      // 更新token
      token.value = loginResult.accessToken
      refreshToken.value = loginResult.refreshToken
      userInfo.value = loginResult.userInfo

      // 更新存储
      Cookies.set(TOKEN_KEY, loginResult.accessToken, { expires: 7 })
      Cookies.set(REFRESH_TOKEN_KEY, loginResult.refreshToken, { expires: 30 })
      localStorage.setItem(USER_INFO_KEY, JSON.stringify(loginResult.userInfo))

      return true
    } catch (error) {
      console.error('刷新token失败:', error)
      await logout()
      return false
    }
  }

  // 获取当前用户信息
  const getCurrentUserInfo = async () => {
    try {
      const response = await authApi.getCurrentUserInfo()
      userInfo.value = response.data
      localStorage.setItem(USER_INFO_KEY, JSON.stringify(response.data))
    } catch (error) {
      console.error('获取用户信息失败:', error)
      throw error
    }
  }

  // 获取用户权限
  const getUserPermissions = async () => {
    try {
      console.log('获取用户权限，用户名:', userInfo.value?.username)
      const response = await authApi.getUserPermissions()
      permissions.value = response.data
      console.log('获取到的权限数据:', permissions.value)
      console.log('权限代码列表:', permissionCodes.value)
    } catch (error) {
      console.error('获取用户权限失败:', error)
      permissions.value = []
    }
  }
  // 获取用户菜单
  const getUserMenus = async () => {
    try {
      if (!userInfo.value?.username) return

      const response = await menusApi.getUserMenuTreeByUsername(userInfo.value.username)
      userMenus.value = response.data
    } catch (error) {
      console.error('获取用户菜单失败:', error)
      userMenus.value = []
    }
  }

  // 检查权限
  const hasPermission = (permissionCode: string): boolean => {
    console.log('检查权限:', permissionCode)
    console.log('用户权限列表:', permissionCodes.value)
    
    // 临时解决方案：如果用户已登录，允许访问（用于开发调试）
    if (isLoggedIn.value) {
      console.log('用户已登录，临时允许访问 (开发模式)')
      return true
    }
    
    const hasAccess = permissionCodes.value.includes(permissionCode)
    console.log('权限检查结果:', hasAccess)
    return hasAccess
  }

  // 检查角色
  const hasRole = (roleCode: string): boolean => {
    return userRoles.value.some(role => role.code === roleCode)
  }

  // 检查是否为超级管理员
  const isSuperAdmin = computed(() => hasRole('super_admin'))

  return {
    // 状态
    token,
    refreshToken,
    userInfo,
    permissions,
    userMenus,
    
    // 计算属性
    isLoggedIn,
    userRoles,
    permissionCodes,
    isSuperAdmin,
    
    // 方法
    init,
    login,
    logout,
    refreshAccessToken,
    getCurrentUserInfo,
    getUserPermissions,
    getUserMenus,
    hasPermission,
    hasRole
  }
})
