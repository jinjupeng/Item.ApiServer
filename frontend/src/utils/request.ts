import axios, { AxiosInstance, InternalAxiosRequestConfig, AxiosResponse } from 'axios'
import { ElMessage } from 'element-plus'
import router from '@/router'
import NProgress from 'nprogress'
import Cookies from 'js-cookie'

// 创建axios实例
const service: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api',
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json;charset=utf-8'
  },
  withCredentials: false
})

// 获取token的函数
const getToken = (): string | null => {
  // 首先尝试从cookie获取
  const tokenFromCookie = Cookies.get('access_token')
  if (tokenFromCookie) {
    return tokenFromCookie
  }
  
  // 如果cookie中没有，尝试从localStorage获取用户信息中的token
  try {
    const userInfo = localStorage.getItem('user_info')
    if (userInfo) {
      const parsed = JSON.parse(userInfo)
      return parsed.token || null
    }
  } catch (error) {
    console.error('解析用户信息失败:', error)
  }
  
  return null
}

// 请求拦截器
service.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // 开始进度条
    NProgress.start()
    
    // 添加认证token
    const token = getToken()
    console.log('请求拦截器 - 获取到的token:', token ? `${token.substring(0, 20)}...` : 'null')
    
    if (token) {
      config.headers = config.headers || {}
      config.headers.Authorization = `Bearer ${token}`
      console.log('请求拦截器 - 已添加Authorization头')
    } else {
      console.log('请求拦截器 - 没有找到token')
    }
    
    console.log('请求配置:', {
      url: config.url,
      method: config.method,
      headers: config.headers
    })
    
    return config
  },
  (error) => {
    NProgress.done()
    console.error('请求错误:', error)
    return Promise.reject(error)
  }
)

// 响应拦截器
service.interceptors.response.use(
  (response: AxiosResponse) => {
    NProgress.done()
    
    const { data } = response
    
    // 如果是文件下载等特殊响应，直接返回
    if (response.config.responseType === 'blob') {
      return response
    }
    
    // 处理业务逻辑错误
    if (data.success === false) {
      ElMessage.error(data.message || '请求失败')
      return Promise.reject(new Error(data.message || '请求失败'))
    }
    
    return data
  },
  async (error) => {
    NProgress.done()
    
    const { response } = error
    
    console.error('API请求错误:', error)
    
    if (response) {
      const { status, data } = response
      
      switch (status) {
        case 401:
          // 未授权，清除token并跳转到登录页
          ElMessage.error('登录已过期，请重新登录')
          // 直接清除所有认证信息
          Cookies.remove('access_token')
          Cookies.remove('refresh_token')
          localStorage.removeItem('user_info')
          router.push('/login')
          break
        case 403:
          ElMessage.error('没有权限访问该资源')
          break
        case 404:
          ElMessage.error('请求的资源不存在')
          break
        case 500:
          ElMessage.error('服务器内部错误')
          break
        default:
          ElMessage.error(data?.message || `请求失败 (${status})`)
      }
    } else if (error.code === 'ECONNABORTED') {
      ElMessage.error('请求超时，请稍后重试')
    } else if (error.code === 'ERR_NETWORK') {
      ElMessage.error('无法连接到服务器，请确保后端服务已启动 (http://localhost:5000)')
    } else {
      ElMessage.error(`网络错误: ${error.message}`)
    }
    
    return Promise.reject(error)
  }
)

export default service
