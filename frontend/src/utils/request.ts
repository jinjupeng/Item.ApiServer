import axios from 'axios'
import type { AxiosInstance, InternalAxiosRequestConfig, AxiosResponse } from 'axios'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useAuthStore } from '@/stores/auth'
import router from '@/router'
import type { ApiResult } from '@/types/api'

// 创建axios实例
const service: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json;charset=UTF-8'
  }
})

// 请求拦截器
service.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const authStore = useAuthStore()
    
    // 添加认证token
    if (authStore.token) {
      config.headers = config.headers || {}
      config.headers.Authorization = `Bearer ${authStore.token}`
    }
    
    return config
  },
  (error) => {
    console.error('Request error:', error)
    return Promise.reject(error)
  }
)

// 响应拦截器
service.interceptors.response.use(
  (response: AxiosResponse<ApiResult>) => {
    const { data } = response
    
    // 如果是文件下载等特殊情况，直接返回
    if (response.config.responseType === 'blob') {
      return response
    }
    
    // 检查业务状态码
    if (data.success) {
      return data as any
    } else {
      // 业务错误处理
      ElMessage.error(data.message || '请求失败')
      return Promise.reject(new Error(data.message || '请求失败'))
    }
  },
  async (error) => {
    console.error('Response error:', error)
    
    const { response } = error
    const authStore = useAuthStore()
    
    if (response) {
      const { status, data } = response
      
      switch (status) {
        case 401:
          // 未授权，清除token并跳转到登录页
          ElMessage.error('登录已过期，请重新登录')
          authStore.logout()
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
    } else if (error.message === 'Network Error') {
      ElMessage.error('网络连接失败，请检查网络')
    } else {
      ElMessage.error('请求失败，请稍后重试')
    }
    
    return Promise.reject(error)
  }
)

// 封装请求方法
class Request {
  // GET请求
  get<T = any>(url: string, params?: any): Promise<ApiResult<T>> {
    return service.get(url, { params })
  }
  
  // POST请求
  post<T = any>(url: string, data?: any): Promise<ApiResult<T>> {
    return service.post(url, data)
  }
  
  // PUT请求
  put<T = any>(url: string, data?: any): Promise<ApiResult<T>> {
    return service.put(url, data)
  }
  
  // DELETE请求
  delete<T = any>(url: string, params?: any): Promise<ApiResult<T>> {
    return service.delete(url, { params })
  }
  
  // PATCH请求
  patch<T = any>(url: string, data?: any): Promise<ApiResult<T>> {
    return service.patch(url, data)
  }
  
  // 文件上传
  upload<T = any>(url: string, formData: FormData): Promise<ApiResult<T>> {
    return service.post(url, formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
  }
  
  // 文件下载
  download(url: string, params?: any, filename?: string): Promise<void> {
    return service.get(url, {
      params,
      responseType: 'blob'
    }).then((response: any) => {
      const blob = new Blob([response.data])
      const downloadUrl = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = downloadUrl
      link.download = filename || 'download'
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      window.URL.revokeObjectURL(downloadUrl)
    })
  }
}

export default new Request()
