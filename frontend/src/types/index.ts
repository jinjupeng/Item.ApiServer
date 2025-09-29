// 通用响应类型
export interface ApiResponse<T = any> {
  success: boolean
  message: string
  data: T
  code?: number
}

// 分页响应类型
export interface PagedResponse<T = any> {
  items: T[]
  totalCount: number
  pageIndex: number
  pageSize: number
  totalPages: number
}

// 分页查询参数
export interface PagedQuery {
  pageIndex?: number
  pageSize?: number
  keyword?: string
  sortField?: string
  sortOrder?: 'asc' | 'desc'
}

// 用户相关类型
export interface User {
  id: number
  userName: string
  email: string
  nickName: string
  phone?: string
  avatar?: string
  status: UserStatus
  organizationId?: number
  organizationName?: string
  roles: Role[]
  createdAt: string
  updatedAt: string
}

export interface CreateUserDto {
  userName: string
  email: string
  nickName: string
  phone?: string
  password: string
  organizationId?: number
  roleIds: number[]
}

export interface UpdateUserDto {
  email: string
  nickName: string
  phone?: string
  organizationId?: number
  roleIds: number[]
}

export interface UserQueryDto extends PagedQuery {
  status?: UserStatus
  organizationId?: number
}

export enum UserStatus {
  Inactive = 0,
  Active = 1,
  Locked = 2
}

// 角色相关类型
export interface Role {
  id: number
  name: string
  code: string
  description?: string
  isActive: boolean
  permissions: Permission[]
  createdAt: string
  updatedAt: string
}

export interface CreateRoleDto {
  name: string
  code: string
  description?: string
}

export interface UpdateRoleDto {
  name: string
  code: string
  description?: string
}

export interface RoleQueryDto extends PagedQuery {
  isActive?: boolean
}

export interface UserRoleAssignDto {
  userId: number
  roleIds: number[]
}

export interface RolePermissionDto {
  roleId: number
  permissionIds: number[]
}

// 权限相关类型
export interface Permission {
  id: number
  name: string
  code: string
  description?: string
  type: PermissionType
  resource?: string
  action?: string
  createdAt: string
}

export enum PermissionType {
  Menu = 0,
  Button = 1,
  Api = 2
}

// 菜单相关类型
export interface Menu {
  id: number
  menuName: string
  menuCode: string
  url?: string
  component?: string
  icon?: string
  menuPid?: number
  sort: number
  status: boolean
  menuType: MenuType
  children?: Menu[]
  createdAt: string
  updatedAt: string
}

export interface CreateMenuDto {
  menuName: string
  menuCode: string
  url?: string
  component?: string
  icon?: string
  menuPid?: number
  sort: number
  menuType: MenuType
}

export interface UpdateMenuDto {
  menuName: string
  menuCode: string
  url?: string
  component?: string
  icon?: string
  menuPid?: number
  sort: number
  menuType: MenuType
}

export interface MenuQueryDto {
  menuName?: string
  status?: boolean
  menuType?: MenuType
}

export enum MenuType {
  Directory = 0,
  Menu = 1,
  Button = 2
}

// 认证相关类型
export interface LoginDto {
  username: string
  password: string
  captchaKey?: string
  captchaCode?: string
  rememberMe?: boolean
}
// 登录响应
export interface LoginResponse {
  accessToken: string
  refreshToken: string
  tokenType: string
  expiresIn: number
  userInfo: User
}

export interface RefreshTokenDto {
  refreshToken: string
}
export interface ChangePasswordDto {
  oldPassword: string
  newPassword: string
  confirmPassword: string
}

export interface ResetPasswordDto {
  userNameOrEmail: string
  code: string
  newPassword: string
  confirmPassword: string
}

export interface CaptchaResponse {
  key: string
  image: string
}

// 组织相关类型
export interface Organization {
  id: number
  name: string
  code: string
  parentId?: number
  sort: number
  isActive: boolean
  children?: Organization[]
  createdAt: string
  updatedAt: string
}

// 审计日志相关类型
export interface AuditLog {
  id: string
  action: string
  module: string
  description: string
  oldData?: string
  newData?: string
  result: string
  errorMessage?: string
  userId?: string
  userName?: string
  ipAddress?: string
  userAgent?: string
  requestPath?: string
  requestMethod?: string
  requestData?: string
  responseStatusCode?: number
  duration?: number
  entityId?: string
  entityType?: string
  createdAt: string
}

export interface AuditLogQueryDto extends PagedQuery {
  action?: string
  module?: string
  userId?: number
  result?: string
  dateFrom?: string
  dateTo?: string
  ipAddress?: string
  entityType?: string
}

export interface AuditLogExportDto {
  searchTerm?: string
  action?: string
  module?: string
  userId?: number
  result?: string
  dateFrom?: string
  dateTo?: string
  ipAddress?: string
  entityType?: string
}

export interface AuditLogStatistics {
  totalOperations: number
  successOperations: number
  failedOperations: number
  actionStatistics: Record<string, number>
  moduleStatistics: Record<string, number>
  userStatistics: Record<string, number>
  dateStatistics: Record<string, number>
}

export interface CreateAuditLogDto {
  action: string
  module: string
  description: string
  oldData?: string
  newData?: string
  result: string
  errorMessage?: string
  entityId?: string
  entityType?: string
}
