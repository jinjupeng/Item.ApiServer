import { ref, computed } from 'vue'
import type { Ref } from 'vue'

export interface UseRequestOptions<T> {
  immediate?: boolean
  onSuccess?: (data: T) => void
  onError?: (error: any) => void
}

/**
 * 通用API请求Hook
 */
export function useRequest<T = any>(
  requestFn: () => Promise<T>,
  options: UseRequestOptions<T> = {}
) {
  const { immediate = false, onSuccess, onError } = options
  
  const loading = ref(false)
  const error = ref<any>(null)
  const data = ref<T | null>(null)
  
  const isSuccess = computed(() => !loading.value && !error.value && data.value !== null)
  const isError = computed(() => !loading.value && error.value !== null)
  
  const execute = async (...args: any[]) => {
    try {
      loading.value = true
      error.value = null
      
      const result = await requestFn()
      data.value = result
      
      onSuccess?.(result)
      return result
    } catch (err) {
      error.value = err
      onError?.(err)
      throw err
    } finally {
      loading.value = false
    }
  }
  
  const reset = () => {
    loading.value = false
    error.value = null
    data.value = null
  }
  
  // 如果设置了immediate，立即执行
  if (immediate) {
    execute()
  }
  
  return {
    loading: loading as Ref<boolean>,
    error: error as Ref<any>,
    data: data as Ref<T | null>,
    isSuccess,
    isError,
    execute,
    reset
  }
}

/**
 * 分页请求Hook
 */
export function usePagination<T = any>(
  requestFn: (params: { page: number; size: number; [key: string]: any }) => Promise<{
    items: T[]
    total: number
  }>,
  options: {
    initialPage?: number
    initialSize?: number
    immediate?: boolean
  } = {}
) {
  const { initialPage = 1, initialSize = 10, immediate = false } = options
  
  const page = ref(initialPage)
  const size = ref(initialSize)
  const total = ref(0)
  const items = ref<T[]>([])
  const loading = ref(false)
  const error = ref<any>(null)
  
  const totalPages = computed(() => Math.ceil(total.value / size.value))
  const hasNext = computed(() => page.value < totalPages.value)
  const hasPrev = computed(() => page.value > 1)
  
  const execute = async (params: Record<string, any> = {}) => {
    try {
      loading.value = true
      error.value = null
      
      const result = await requestFn({
        page: page.value,
        size: size.value,
        ...params
      })
      
      items.value = result.items
      total.value = result.total
      
      return result
    } catch (err) {
      error.value = err
      throw err
    } finally {
      loading.value = false
    }
  }
  
  const changePage = (newPage: number) => {
    if (newPage >= 1 && newPage <= totalPages.value) {
      page.value = newPage
      execute()
    }
  }
  
  const changeSize = (newSize: number) => {
    size.value = newSize
    page.value = 1 // 重置到第一页
    execute()
  }
  
  const refresh = () => execute()
  const reset = () => {
    page.value = initialPage
    size.value = initialSize
    total.value = 0
    items.value = []
    error.value = null
  }
  
  if (immediate) {
    execute()
  }
  
  return {
    // 状态
    page,
    size,
    total,
    items,
    loading,
    error,
    
    // 计算属性
    totalPages,
    hasNext,
    hasPrev,
    
    // 方法
    execute,
    changePage,
    changeSize,
    refresh,
    reset
  }
}