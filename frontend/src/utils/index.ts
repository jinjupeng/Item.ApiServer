import dayjs from 'dayjs'

/**
 * 格式化日期
 * @param date 日期
 * @param format 格式
 * @returns 格式化后的日期字符串
 */
export const formatDate = (date: string | Date, format = 'YYYY-MM-DD HH:mm:ss') => {
  return dayjs(date).format(format)
}

/**
 * 防抖函数
 * @param func 要防抖的函数
 * @param wait 等待时间
 * @returns 防抖后的函数
 */
export const debounce = <T extends (...args: any[]) => any>(
  func: T,
  wait: number
): ((...args: Parameters<T>) => void) => {
  let timeout: NodeJS.Timeout | null = null
  
  return (...args: Parameters<T>) => {
    if (timeout) {
      clearTimeout(timeout)
    }
    
    timeout = setTimeout(() => {
      func.apply(null, args)
    }, wait)
  }
}

/**
 * 节流函数
 * @param func 要节流的函数
 * @param wait 等待时间
 * @returns 节流后的函数
 */
export const throttle = <T extends (...args: any[]) => any>(
  func: T,
  wait: number
): ((...args: Parameters<T>) => void) => {
  let timeout: NodeJS.Timeout | null = null
  let previous = 0
  
  return (...args: Parameters<T>) => {
    const now = Date.now()
    const remaining = wait - (now - previous)
    
    if (remaining <= 0 || remaining > wait) {
      if (timeout) {
        clearTimeout(timeout)
        timeout = null
      }
      previous = now
      func.apply(null, args)
    } else if (!timeout) {
      timeout = setTimeout(() => {
        previous = Date.now()
        timeout = null
        func.apply(null, args)
      }, remaining)
    }
  }
}

/**
 * 深拷贝
 * @param obj 要拷贝的对象
 * @returns 拷贝后的对象
 */
export const deepClone = <T>(obj: T): T => {
  if (obj === null || typeof obj !== 'object') {
    return obj
  }
  
  if (obj instanceof Date) {
    return new Date(obj.getTime()) as unknown as T
  }
  
  if (obj instanceof Array) {
    return obj.map(item => deepClone(item)) as unknown as T
  }
  
  if (typeof obj === 'object') {
    const clonedObj = {} as T
    for (const key in obj) {
      if (obj.hasOwnProperty(key)) {
        clonedObj[key] = deepClone(obj[key])
      }
    }
    return clonedObj
  }
  
  return obj
}

/**
 * 生成UUID
 * @returns UUID字符串
 */
export const generateUUID = (): string => {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
    const r = (Math.random() * 16) | 0
    const v = c === 'x' ? r : (r & 0x3) | 0x8
    return v.toString(16)
  })
}

/**
 * 下载文件
 * @param blob 文件blob
 * @param filename 文件名
 */
export const downloadFile = (blob: Blob, filename: string) => {
  const url = window.URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = filename
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
  window.URL.revokeObjectURL(url)
}

/**
 * 获取文件扩展名
 * @param filename 文件名
 * @returns 扩展名
 */
export const getFileExtension = (filename: string): string => {
  return filename.slice(((filename.lastIndexOf('.') - 1) >>> 0) + 2)
}

/**
 * 格式化文件大小
 * @param bytes 字节数
 * @returns 格式化后的大小
 */
export const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 Bytes'
  
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

/**
 * 树形数据转换为平铺数据
 * @param tree 树形数据
 * @param childrenKey 子节点key
 * @returns 平铺数据
 */
export const treeToFlat = <T extends Record<string, any>>(
  tree: T[],
  childrenKey = 'children'
): T[] => {
  const result: T[] = []
  
  const traverse = (nodes: T[], parent?: T) => {
    nodes.forEach(node => {
      const item = { ...node }
      if (parent) {
        item.parentId = parent.id
      }
      delete item[childrenKey]
      result.push(item)
      
      if (node[childrenKey] && node[childrenKey].length > 0) {
        traverse(node[childrenKey], node)
      }
    })
  }
  
  traverse(tree)
  return result
}

/**
 * 平铺数据转换为树形数据
 * @param flat 平铺数据
 * @param idKey ID字段key
 * @param parentIdKey 父ID字段key
 * @param childrenKey 子节点key
 * @returns 树形数据
 */
export const flatToTree = <T extends Record<string, any>>(
  flat: T[],
  idKey = 'id',
  parentIdKey = 'parentId',
  childrenKey = 'children'
): T[] => {
  const tree: T[] = []
  const map = new Map<any, T>()
  
  // 创建映射
  flat.forEach(item => {
    map.set(item[idKey], { ...item, [childrenKey]: [] })
  })
  
  // 构建树形结构
  flat.forEach(item => {
    const node = map.get(item[idKey])!
    const parentId = item[parentIdKey]
    
    if (parentId && map.has(parentId)) {
      const parent = map.get(parentId)!
      parent[childrenKey].push(node)
    } else {
      tree.push(node)
    }
  })
  
  return tree
}
