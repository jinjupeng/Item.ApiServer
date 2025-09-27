import dayjs from 'dayjs'
import relativeTime from 'dayjs/plugin/relativeTime'
import { ElMessage, ElMessageBox } from 'element-plus'

// 扩展dayjs插件
dayjs.extend(relativeTime)

/**
 * 日期格式化
 */
export const formatDate = (date: string | Date, format = 'YYYY-MM-DD HH:mm:ss') => {
  if (!date) return ''
  return dayjs(date).format(format)
}

/**
 * 日期时间格式化
 */
export const formatDateTime = (date: string | Date, format = 'YYYY-MM-DD HH:mm:ss') => {
  if (!date) return ''
  return dayjs(date).format(format)
}

/**
 * 相对时间
 */
export const fromNow = (date: string | Date) => {
  if (!date) return ''
  return dayjs(date).fromNow()
}

/**
 * 防抖函数
 */
export const debounce = <T extends (...args: any[]) => any>(
  func: T,
  wait: number
): ((...args: Parameters<T>) => void) => {
  let timeout: NodeJS.Timeout
  return (...args: Parameters<T>) => {
    clearTimeout(timeout)
    timeout = setTimeout(() => func.apply(this, args), wait)
  }
}

/**
 * 节流函数
 */
export const throttle = <T extends (...args: any[]) => any>(
  func: T,
  limit: number
): ((...args: Parameters<T>) => void) => {
  let inThrottle: boolean
  return (...args: Parameters<T>) => {
    if (!inThrottle) {
      func.apply(this, args)
      inThrottle = true
      setTimeout(() => (inThrottle = false), limit)
    }
  }
}

/**
 * 深拷贝
 */
export const deepClone = <T>(obj: T): T => {
  if (obj === null || typeof obj !== 'object') return obj
  if (obj instanceof Date) return new Date(obj.getTime()) as unknown as T
  if (obj instanceof Array) return obj.map(item => deepClone(item)) as unknown as T
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
 * 生成唯一ID
 */
export const generateId = () => {
  return Math.random().toString(36).substr(2, 9)
}

/**
 * 下载文件
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
 * 格式化文件大小
 */
export const formatFileSize = (bytes: number) => {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB', 'TB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

/**
 * 获取文件扩展名
 */
export const getFileExtension = (filename: string) => {
  return filename.slice(((filename.lastIndexOf('.') - 1) >>> 0) + 2)
}

/**
 * 验证邮箱
 */
export const isEmail = (email: string) => {
  const reg = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/
  return reg.test(email)
}

/**
 * 验证手机号
 */
export const isPhone = (phone: string) => {
  const reg = /^1[3-9]\d{9}$/
  return reg.test(phone)
}

/**
 * 验证身份证号
 */
export const isIdCard = (idCard: string) => {
  const reg = /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/
  return reg.test(idCard)
}

/**
 * 树形数据转换为扁平数组
 */
export const treeToFlat = <T extends { children?: T[] }>(tree: T[]): T[] => {
  const result: T[] = []
  const traverse = (nodes: T[]) => {
    nodes.forEach(node => {
      const { children, ...rest } = node
      result.push(rest as T)
      if (children && children.length > 0) {
        traverse(children)
      }
    })
  }
  traverse(tree)
  return result
}

/**
 * 扁平数组转换为树形数据
 */
export const flatToTree = <T extends { id: number; parentId?: number }>(
  flat: T[],
  parentId?: number
): (T & { children?: T[] })[] => {
  const result: (T & { children?: T[] })[] = []
  
  flat.forEach(item => {
    if (item.parentId === parentId) {
      const children = flatToTree(flat, item.id)
      if (children.length > 0) {
        result.push({ ...item, children })
      } else {
        result.push({ ...item })
      }
    }
  })
  
  return result
}

/**
 * 确认对话框
 */
export const confirm = (message: string, title = '提示') => {
  return ElMessageBox.confirm(message, title, {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  })
}

/**
 * 成功提示
 */
export const success = (message: string) => {
  ElMessage.success(message)
}

/**
 * 错误提示
 */
export const error = (message: string) => {
  ElMessage.error(message)
}

/**
 * 警告提示
 */
export const warning = (message: string) => {
  ElMessage.warning(message)
}

/**
 * 信息提示
 */
export const info = (message: string) => {
  ElMessage.info(message)
}

/**
 * 复制到剪贴板
 */
export const copyToClipboard = async (text: string) => {
  try {
    await navigator.clipboard.writeText(text)
    success('复制成功')
  } catch (error) {
    // 降级方案
    const textArea = document.createElement('textarea')
    textArea.value = text
    document.body.appendChild(textArea)
    textArea.select()
    document.execCommand('copy')
    document.body.removeChild(textArea)
    success('复制成功')
  }
}

/**
 * 获取随机颜色
 */
export const getRandomColor = () => {
  const colors = [
    '#409EFF', '#67C23A', '#E6A23C', '#F56C6C', '#909399',
    '#FF6B6B', '#4ECDC4', '#45B7D1', '#96CEB4', '#FFEAA7',
    '#DDA0DD', '#98D8C8', '#F7DC6F', '#BB8FCE', '#85C1E9'
  ]
  return colors[Math.floor(Math.random() * colors.length)]
}

/**
 * 获取头像背景色
 */
export const getAvatarColor = (name: string) => {
  const colors = [
    '#f56a00', '#7265e6', '#ffbf00', '#00a2ae', '#87d068',
    '#108ee9', '#f50', '#2db7f5', '#722ed1', '#eb2f96'
  ]
  let hash = 0
  for (let i = 0; i < name.length; i++) {
    hash = name.charCodeAt(i) + ((hash << 5) - hash)
  }
  return colors[Math.abs(hash) % colors.length]
}

/**
 * 获取文件图标
 */
export const getFileIcon = (filename: string) => {
  const ext = getFileExtension(filename).toLowerCase()
  const iconMap: Record<string, string> = {
    pdf: 'document',
    doc: 'document',
    docx: 'document',
    xls: 'document',
    xlsx: 'document',
    ppt: 'document',
    pptx: 'document',
    txt: 'document',
    jpg: 'picture',
    jpeg: 'picture',
    png: 'picture',
    gif: 'picture',
    bmp: 'picture',
    svg: 'picture',
    mp4: 'video-play',
    avi: 'video-play',
    mov: 'video-play',
    mp3: 'headset',
    wav: 'headset',
    zip: 'folder',
    rar: 'folder',
    '7z': 'folder'
  }
  return iconMap[ext] || 'document'
}
