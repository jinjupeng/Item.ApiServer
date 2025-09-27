/**
 * Vue3 + TypeScript 通用工具函数
 */

// 类型判断工具
export const is = {
  string: (val: unknown): val is string => typeof val === 'string',
  number: (val: unknown): val is number => typeof val === 'number',
  boolean: (val: unknown): val is boolean => typeof val === 'boolean',
  function: (val: unknown): val is Function => typeof val === 'function',
  object: (val: unknown): val is object => 
    val !== null && typeof val === 'object' && !Array.isArray(val),
  array: (val: unknown): val is any[] => Array.isArray(val),
  date: (val: unknown): val is Date => val instanceof Date,
  promise: (val: unknown): val is Promise<any> => 
    val instanceof Promise || (is.object(val) && is.function((val as any).then)),
  null: (val: unknown): val is null => val === null,
  undefined: (val: unknown): val is undefined => val === undefined,
  nullOrUndefined: (val: unknown): val is null | undefined => 
    val === null || val === undefined,
  empty: (val: unknown): boolean => {
    if (is.nullOrUndefined(val)) return true
    if (is.string(val)) return val.trim() === ''
    if (is.array(val)) return val.length === 0
    if (is.object(val)) return Object.keys(val).length === 0
    return false
  }
}

// 数组工具
export const arrayUtils = {
  // 去重
  unique: <T>(arr: T[]): T[] => [...new Set(arr)],
  
  // 按属性去重
  uniqueBy: <T, K extends keyof T>(arr: T[], key: K): T[] => {
    const seen = new Set()
    return arr.filter(item => {
      const val = item[key]
      if (seen.has(val)) return false
      seen.add(val)
      return true
    })
  },
  
  // 分组
  groupBy: <T, K extends keyof T>(arr: T[], key: K): Record<string, T[]> => {
    return arr.reduce((groups, item) => {
      const group = String(item[key])
      groups[group] = groups[group] || []
      groups[group].push(item)
      return groups
    }, {} as Record<string, T[]>)
  },
  
  // 树形结构扁平化
  flatten: <T extends { children?: T[] }>(
    arr: T[], 
    childrenKey: keyof T = 'children'
  ): T[] => {
    const result: T[] = []
    const stack = [...arr]
    
    while (stack.length) {
      const node = stack.pop()!
      result.push(node)
      if (node[childrenKey] && Array.isArray(node[childrenKey])) {
        stack.push(...(node[childrenKey] as T[]))
      }
    }
    
    return result
  },
  
  // 扁平数组转树形结构
  toTree: <T extends { id: any; parentId?: any }>(
    arr: T[],
    options: {
      idKey?: keyof T
      parentIdKey?: keyof T
      childrenKey?: string
      rootValue?: any
    } = {}
  ): T[] => {
    const {
      idKey = 'id',
      parentIdKey = 'parentId',
      childrenKey = 'children',
      rootValue = null
    } = options
    
    const tree: T[] = []
    const childrenMap: Map<any, T[]> = new Map()
    
    // 构建子节点映射
    arr.forEach(item => {
      const parentId = item[parentIdKey]
      if (!childrenMap.has(parentId)) {
        childrenMap.set(parentId, [])
      }
      childrenMap.get(parentId)!.push(item)
    })
    
    // 递归构建树
    const buildTree = (parentId: any): T[] => {
      const children = childrenMap.get(parentId) || []
      return children.map(item => ({
        ...item,
        [childrenKey]: buildTree(item[idKey])
      }))
    }
    
    return buildTree(rootValue)
  }
}

// 对象工具
export const objectUtils = {
  // 深拷贝
  deepClone: <T>(obj: T): T => {
    if (obj === null || typeof obj !== 'object') return obj
    if (obj instanceof Date) return new Date(obj.getTime()) as T
    if (obj instanceof Array) return obj.map(item => objectUtils.deepClone(item)) as T
    if (typeof obj === 'object') {
      const clonedObj = {} as T
      for (const key in obj) {
        if (obj.hasOwnProperty(key)) {
          clonedObj[key] = objectUtils.deepClone(obj[key])
        }
      }
      return clonedObj
    }
    return obj
  },
  
  // 对象比较
  isEqual: (obj1: any, obj2: any): boolean => {
    if (obj1 === obj2) return true
    if (obj1 == null || obj2 == null) return obj1 === obj2
    if (typeof obj1 !== typeof obj2) return false
    if (typeof obj1 !== 'object') return obj1 === obj2
    
    const keys1 = Object.keys(obj1)
    const keys2 = Object.keys(obj2)
    
    if (keys1.length !== keys2.length) return false
    
    for (const key of keys1) {
      if (!keys2.includes(key)) return false
      if (!objectUtils.isEqual(obj1[key], obj2[key])) return false
    }
    
    return true
  },
  
  // 删除对象中的空值
  omitEmpty: <T extends Record<string, any>>(obj: T): Partial<T> => {
    const result: Partial<T> = {}
    for (const key in obj) {
      if (!is.empty(obj[key])) {
        result[key] = obj[key]
      }
    }
    return result
  },
  
  // 选择对象属性
  pick: <T extends Record<string, any>, K extends keyof T>(
    obj: T,
    keys: K[]
  ): Pick<T, K> => {
    const result = {} as Pick<T, K>
    keys.forEach(key => {
      if (key in obj) {
        result[key] = obj[key]
      }
    })
    return result
  },
  
  // 排除对象属性
  omit: <T extends Record<string, any>, K extends keyof T>(
    obj: T,
    keys: K[]
  ): Omit<T, K> => {
    const result = { ...obj }
    keys.forEach(key => {
      delete result[key]
    })
    return result
  }
}

// 字符串工具
export const stringUtils = {
  // 首字母大写
  capitalize: (str: string): string => 
    str.charAt(0).toUpperCase() + str.slice(1),
  
  // 驼峰转下划线
  camelToSnake: (str: string): string =>
    str.replace(/[A-Z]/g, letter => `_${letter.toLowerCase()}`),
  
  // 下划线转驼峰
  snakeToCamel: (str: string): string =>
    str.replace(/_([a-z])/g, (_, letter) => letter.toUpperCase()),
  
  // 生成随机字符串
  random: (length = 8): string => {
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789'
    let result = ''
    for (let i = 0; i < length; i++) {
      result += chars.charAt(Math.floor(Math.random() * chars.length))
    }
    return result
  },
  
  // 模板字符串替换
  template: (str: string, data: Record<string, any>): string => {
    return str.replace(/{(\w+)}/g, (match, key) => 
      data.hasOwnProperty(key) ? String(data[key]) : match
    )
  }
}

// 数字工具
export const numberUtils = {
  // 格式化数字
  format: (num: number, decimals = 2): string => {
    return num.toLocaleString('zh-CN', {
      minimumFractionDigits: decimals,
      maximumFractionDigits: decimals
    })
  },
  
  // 文件大小格式化
  formatFileSize: (bytes: number): string => {
    if (bytes === 0) return '0 B'
    const k = 1024
    const sizes = ['B', 'KB', 'MB', 'GB', 'TB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
  },
  
  // 生成随机数
  random: (min = 0, max = 100): number => 
    Math.floor(Math.random() * (max - min + 1)) + min,
  
  // 保留小数位
  toFixed: (num: number, decimals = 2): number => 
    Math.round(num * Math.pow(10, decimals)) / Math.pow(10, decimals)
}

// URL工具
export const urlUtils = {
  // 获取URL参数
  getQuery: (url?: string): Record<string, string> => {
    const queryString = url ? url.split('?')[1] : window.location.search.slice(1)
    if (!queryString) return {}
    
    return queryString.split('&').reduce((params, param) => {
      const [key, value] = param.split('=')
      params[decodeURIComponent(key)] = decodeURIComponent(value || '')
      return params
    }, {} as Record<string, string>)
  },
  
  // 构建查询字符串
  buildQuery: (params: Record<string, any>): string => {
    const query = Object.entries(params)
      .filter(([_, value]) => !is.nullOrUndefined(value))
      .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`)
      .join('&')
    
    return query ? `?${query}` : ''
  },
  
  // 合并URL
  join: (...parts: string[]): string => {
    return parts
      .map((part, index) => {
        if (index === 0) return part.replace(/\/+$/, '')
        return part.replace(/^\/+/, '').replace(/\/+$/, '')
      })
      .filter(Boolean)
      .join('/')
  }
}

// 浏览器工具
export const browserUtils = {
  // 复制到剪贴板
  copyToClipboard: async (text: string): Promise<boolean> => {
    try {
      if (navigator.clipboard) {
        await navigator.clipboard.writeText(text)
        return true
      } else {
        // 兼容旧版浏览器
        const textArea = document.createElement('textarea')
        textArea.value = text
        textArea.style.position = 'fixed'
        textArea.style.opacity = '0'
        document.body.appendChild(textArea)
        textArea.select()
        const successful = document.execCommand('copy')
        document.body.removeChild(textArea)
        return successful
      }
    } catch (error) {
      console.error('复制失败:', error)
      return false
    }
  },
  
  // 下载文件
  downloadFile: (url: string, filename?: string): void => {
    const link = document.createElement('a')
    link.href = url
    link.download = filename || url.split('/').pop() || 'download'
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
  },
  
  // 检测设备类型
  isMobile: (): boolean => {
    return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(
      navigator.userAgent
    )
  },
  
  // 全屏操作
  fullscreen: {
    enter: (element: Element = document.documentElement) => {
      if (element.requestFullscreen) {
        element.requestFullscreen()
      }
    },
    exit: () => {
      if (document.exitFullscreen) {
        document.exitFullscreen()
      }
    },
    isFullscreen: (): boolean => {
      return !!document.fullscreenElement
    }
  }
}