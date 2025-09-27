/**
 * Vue3 + TypeScript 通用类型定义
 */

// 基础工具类型
export type Nullable<T> = T | null
export type Optional<T> = T | undefined
export type NonNullable<T> = T extends null | undefined ? never : T

// 响应式数据类型
export type Ref<T> = import('vue').Ref<T>
export type ComputedRef<T> = import('vue').ComputedRef<T>
export type Reactive<T> = import('vue').UnwrapNestedRefs<T>

// 表单相关类型
export interface FormItemRule {
  required?: boolean
  message?: string
  trigger?: string | string[]
  min?: number
  max?: number
  pattern?: RegExp
  validator?: (rule: any, value: any, callback: any) => void
}

export type FormRules<T = any> = {
  [K in keyof T]?: FormItemRule[]
}

// 表格相关类型
export interface TableColumn<T = any> {
  prop: keyof T
  label: string
  width?: number | string
  minWidth?: number | string
  fixed?: boolean | 'left' | 'right'
  sortable?: boolean
  align?: 'left' | 'center' | 'right'
  formatter?: (row: T, column: any, cellValue: any, index: number) => string
  showOverflowTooltip?: boolean
}

// 分页参数类型
export interface PaginationParams {
  page: number
  size: number
}

// 查询参数基础类型
export interface BaseQuery extends PaginationParams {
  keyword?: string
  [key: string]: any
}

// 菜单/路由类型
export interface RouteMetaCustom {
  title?: string
  icon?: string
  hidden?: boolean
  keepAlive?: boolean
  roles?: string[]
  permissions?: string[]
}

// 组件实例类型
export type ComponentInstance<T = {}> = import('vue').ComponentPublicInstance<T>

// 通用CRUD操作类型
export interface CrudOptions<T = any> {
  create?: (data: T) => Promise<any>
  update?: (id: string | number, data: T) => Promise<any>
  delete?: (id: string | number) => Promise<any>
  list?: (params?: any) => Promise<{ items: T[]; total: number }>
}

// 选项类型
export interface SelectOption<T = any> {
  label: string
  value: T
  disabled?: boolean
  [key: string]: any
}

// 树形数据类型
export interface TreeNode<T = any> {
  id: string | number
  label: string
  children?: TreeNode<T>[]
  data?: T
  [key: string]: any
}

// 上传文件类型
export interface UploadFile {
  name: string
  url?: string
  status?: 'ready' | 'uploading' | 'success' | 'fail'
  percentage?: number
  raw?: File
  response?: any
  uid?: number
}

// 字典类型
export interface DictItem {
  label: string
  value: string | number
  type?: string
  sort?: number
  remark?: string
  status?: 0 | 1
}

// 主题类型
export type ThemeMode = 'light' | 'dark' | 'auto'

// 语言类型
export type Locale = 'zh-CN' | 'en-US'

// 布局类型
export type LayoutMode = 'vertical' | 'horizontal' | 'mix'