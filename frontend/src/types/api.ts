// API响应基础类型
export interface ApiResult<T = any> {
  success: boolean
  message: string
  data: T
  code: number
  timestamp: string
}

// 分页查询基础类型
export interface PagedQuery {
  pageIndex: number
  pageSize: number
}

// 分页结果类型
export interface PagedResult<T> {
  items: T[]
  total: number
  pageIndex: number
  pageSize: number
  totalPages: number
}

// 用户相关类型
export interface User {
  id: number
  username: string
  nickname?: string
  portrait?: string
  orgId: number
  orgName?: string
  status: UserStatus
  phone?: string
  email?: string
  createTime: string
  roles?: string[]
}

export interface CreateUserDto {
  username: string
  password: string
  nickname?: string
  portrait?: string
  orgId: number
  phone?: string
  email?: string
  roleIds?: number[]
}

export interface UpdateUserDto {
  nickname?: string
  portrait?: string
  orgId: number
  phone?: string
  email?: string
  roleIds?: number[]
}

export interface UserQueryDto extends PagedQuery {
  username?: string
  nickname?: string
  orgId?: number
  status?: UserStatus
  startTime?: string
  endTime?: string
}

export interface ChangePasswordDto {
  oldPassword: string
  newPassword: string
  confirmPassword: string
}

export interface ResetPasswordDto {
  usernameOrEmail: string
  newPassword: string
  verificationCode: string
}

export enum UserStatus {
  Disabled = 0,
  Enabled = 1,
  Active = 1,
  Inactive = 2,
  Locked = 3
}

// 角色相关类型
export interface Role {
  id: number
  roleName: string
  roleDesc?: string
  status: boolean
  createdAt: string
  userCount?: number
}

export interface CreateRoleDto {
  roleName: string
  roleCode: string
  description?: string
  status: boolean
}

export interface UpdateRoleDto {
  roleName: string
  roleCode: string
  description?: string
  status: boolean
}

export interface RoleQuery {
  roleName?: string
  status?: boolean
}

export interface RoleForm {
  id?: number
  roleName: string
  roleDesc?: string
  status: boolean
}

export interface RoleQueryDto extends PagedQuery {
  roleName?: string
  roleCode?: string
  status?: boolean
}

export interface UserRoleAssignDto {
  userId: number
  roleIds: number[]
}

export interface RolePermissionDto {
  roleId: number
  menuIds: number[]
  apiIds: number[]
}

// 菜单相关类型
export interface Menu {
  id: number
  title: string
  path?: string
  component?: string
  parentId?: number | null
  icon?: string
  type: number
  sort: number
  status: boolean
  hidden: boolean
  permission?: string
  createdAt: string
  children?: Menu[]
}

export interface MenuTreeDto extends Menu {
  children: MenuTreeDto[]
  checked?: boolean
}

export interface CreateMenuDto {
  menuName: string
  menuCode: string
  parentId?: number
  url?: string
  icon?: string
  menuType: MenuType
  sort: number
  status: boolean
}

export interface UpdateMenuDto {
  menuName: string
  menuCode: string
  parentId?: number
  url?: string
  icon?: string
  menuType: MenuType
  sort: number
  status: boolean
}

export interface MenuForm {
  id?: number
  parentId?: number | null
  title: string
  path?: string
  component?: string
  icon?: string
  type: number
  sort: number
  status: boolean
  hidden: boolean
  permission?: string
}

export interface MenuQueryDto {
  menuName?: string
  status?: boolean
}

export enum MenuType {
  Directory = 0,
  Menu = 1,
  Button = 2
}

// 组织相关类型
export interface Organization {
  id: number
  orgName: string
  orgCode: string
  parentId?: number | null
  orgType: number
  leader?: string
  phone?: string
  email?: string
  address?: string
  description?: string
  sort: number
  status: boolean
  createdAt: string
  children?: Organization[]
}

export interface OrganizationTreeDto extends Organization {
  children: OrganizationTreeDto[]
}

export interface CreateOrganizationDto {
  orgName: string
  orgCode: string
  parentId?: number
  sort: number
  status: boolean
}

export interface UpdateOrganizationDto {
  orgName: string
  orgCode: string
  parentId?: number
  sort: number
  status: boolean
}

export interface OrganizationQuery {
  orgName?: string
  status?: boolean
}

export interface OrganizationForm {
  id?: number
  parentId?: number | null
  orgName: string
  orgCode: string
  orgType: number
  leader?: string
  phone?: string
  email?: string
  address?: string
  description?: string
  sort: number
  status: boolean
}

export interface OrganizationQueryDto {
  orgName?: string
  status?: boolean
}

// 认证相关类型
export interface LoginDto {
  username: string
  password: string
  captchaKey?: string
  captchaCode?: string
  rememberMe?: boolean
}

export interface LoginResponseDto {
  accessToken: string
  refreshToken: string
  expiresIn: number
  userInfo: UserInfoDto
}

export interface UserInfoDto {
  userId: number
  username: string
  nickname: string
  avatar?: string
  email?: string
  phone?: string
  orgId: number
  orgName?: string
  roles: string[]
  lastLoginTime?: string
  createTime?: string
  gender?: number
  birthday?: string
  bio?: string
}

export interface RefreshTokenDto {
  refreshToken: string
}

export interface UserPermissionDto {
  userId: number
  menus: MenuPermissionDto[]
  apis: string[]
}

export interface MenuPermissionDto {
  id: number
  name: string
  code: string
  icon?: string
  path?: string
  parentId?: number
  sort: number
  children: MenuPermissionDto[]
}

export interface CaptchaDto {
  key: string
  image: string
}

// 系统配置类型
export interface SystemConfig {
  id: number
  configKey: string
  configValue: string
  configName: string
  configType: string
  description?: string
  status: boolean
  createTime: string
}

// 字典类型
export interface Dictionary {
  id: number
  dictType: string
  dictLabel: string
  dictValue: string
  dictSort: number
  status: boolean
  createTime: string
}

// API权限类型
export interface Api {
  id: number
  apiName: string
  url: string
  method: string
  description?: string
  status: boolean
  createTime: string
}
