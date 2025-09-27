import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { AuthApi } from '@/api'
import type { LoginDto, UserInfoDto, UserPermissionDto, MenuPermissionDto } from '@/types/api'
import Cookies from 'js-cookie'
import router from '@/router'

const TOKEN_KEY = 'access_token'
const REFRESH_TOKEN_KEY = 'refresh_token'
const USER_INFO_KEY = 'user_info'

export const useAuthStore = defineStore('auth', () => {
  // 状态
  const token = ref<string>(Cookies.get(TOKEN_KEY) || '')
  const refreshToken = ref<string>(Cookies.get(REFRESH_TOKEN_KEY) || '')
  const userInfo = ref<UserInfoDto | null>(null)
  const permissions = ref<UserPermissionDto | null>(null)
  const menuRoutes = ref<MenuPermissionDto[]>([])

  // 计算属性
  const isLoggedIn = computed(() => !!token.value)
  const userRoles = computed(() => userInfo.value?.roles || [])
  const userId = computed(() => userInfo.value?.userId)
  const username = computed(() => userInfo.value?.username)
  const nickname = computed(() => userInfo.value?.nickname)
  const avatar = computed(() => userInfo.value?.avatar)

  // 登录
  const login = async (loginData: LoginDto) => {
    try {
      const response = await AuthApi.login(loginData)
      const { accessToken, refreshToken: newRefreshToken, userInfo: user } = response.data

      // 保存token
      token.value = accessToken
      refreshToken.value = newRefreshToken
      userInfo.value = user

      // 持久化存储
      Cookies.set(TOKEN_KEY, accessToken, { expires: 7 })
      Cookies.set(REFRESH_TOKEN_KEY, newRefreshToken, { expires: 30 })
      localStorage.setItem(USER_INFO_KEY, JSON.stringify(user))

      // 获取用户权限
      await loadUserPermissions()

      return response
    } catch (error) {
      throw error
    }
  }

  // 登出
  const logout = async () => {
    try {
      if (token.value) {
        await AuthApi.logout()
      }
    } catch (error) {
      console.error('Logout error:', error)
    } finally {
      // 清除状态
      token.value = ''
      refreshToken.value = ''
      userInfo.value = null
      permissions.value = null
      menuRoutes.value = []

      // 清除存储
      Cookies.remove(TOKEN_KEY)
      Cookies.remove(REFRESH_TOKEN_KEY)
      localStorage.removeItem(USER_INFO_KEY)

      // 跳转到登录页
      router.push('/login')
    }
  }

  // 刷新token
  const refreshAccessToken = async () => {
    try {
      if (!refreshToken.value) {
        throw new Error('No refresh token')
      }

      const response = await AuthApi.refreshToken({ refreshToken: refreshToken.value })
      const { accessToken, refreshToken: newRefreshToken } = response.data

      token.value = accessToken
      refreshToken.value = newRefreshToken

      Cookies.set(TOKEN_KEY, accessToken, { expires: 7 })
      Cookies.set(REFRESH_TOKEN_KEY, newRefreshToken, { expires: 30 })

      return response
    } catch (error) {
      // 刷新失败，清除所有状态
      await logout()
      throw error
    }
  }

  // 加载用户信息
  const loadUserInfo = async () => {
    try {
      const response = await AuthApi.getCurrentUserInfo()
      userInfo.value = response.data
      localStorage.setItem(USER_INFO_KEY, JSON.stringify(response.data))
      return response
    } catch (error) {
      throw error
    }
  }

  // 加载用户权限
  const loadUserPermissions = async () => {
    try {
      const response = await AuthApi.getUserPermissions()
      permissions.value = response.data
      menuRoutes.value = response.data.menus
      return response
    } catch (error) {
      console.error('Load permissions error:', error)
      throw error
    }
  }

  // 修改密码
  const changePassword = async (passwordData: any) => {
    try {
      const response = await AuthApi.changePassword(passwordData)
      return response
    } catch (error) {
      throw error
    }
  }

  // 检查权限
  const hasPermission = (permission: string): boolean => {
    if (!permissions.value) return false
    
    // 检查API权限
    return permissions.value.apis.includes(permission)
  }

  // 检查角色
  const hasRole = (role: string): boolean => {
    return userRoles.value.includes(role)
  }

  // 检查菜单权限
  const hasMenuPermission = (menuCode: string): boolean => {
    if (!permissions.value) return false
    
    const findMenu = (menus: MenuPermissionDto[], code: string): boolean => {
      for (const menu of menus) {
        if (menu.code === code) return true
        if (menu.children && findMenu(menu.children, code)) return true
      }
      return false
    }
    
    return findMenu(permissions.value.menus, menuCode)
  }

  // 初始化用户信息（从本地存储恢复）
  const initUserInfo = () => {
    const savedUserInfo = localStorage.getItem(USER_INFO_KEY)
    if (savedUserInfo) {
      try {
        userInfo.value = JSON.parse(savedUserInfo)
      } catch (error) {
        console.error('Parse user info error:', error)
        localStorage.removeItem(USER_INFO_KEY)
      }
    }
  }

  // 验证token有效性
  const validateToken = async () => {
    if (!token.value) return false
    
    try {
      const response = await AuthApi.validateToken(token.value)
      return response.data
    } catch (error) {
      return false
    }
  }

  // 初始化
  initUserInfo()

  return {
    // 状态
    token,
    refreshToken,
    userInfo,
    permissions,
    menuRoutes,
    
    // 计算属性
    isLoggedIn,
    userRoles,
    userId,
    username,
    nickname,
    avatar,
    
    // 方法
    login,
    logout,
    refreshAccessToken,
    loadUserInfo,
    loadUserPermissions,
    changePassword,
    hasPermission,
    hasRole,
    hasMenuPermission,
    validateToken
  }
})
