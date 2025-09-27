import request from '@/utils/request'
import type {
  User,
  CreateUserDto,
  UpdateUserDto,
  UserQueryDto,
  PagedResult,
  ApiResult
} from '@/types/api'

/**
 * 用户管理API
 */
export class UserApi {
  /**
   * 获取用户列表（分页）
   */
  static getUsers(params: UserQueryDto): Promise<ApiResult<PagedResult<User>>> {
    return request.get('/users', params)
  }

  /**
   * 获取所有用户
   */
  static getAllUsers(): Promise<ApiResult<User[]>> {
    return request.get('/users/all')
  }

  /**
   * 根据ID获取用户详情
   */
  static getUserById(id: number): Promise<ApiResult<User>> {
    return request.get(`/users/${id}`)
  }

  /**
   * 创建用户
   */
  static createUser(data: CreateUserDto): Promise<ApiResult<number>> {
    return request.post('/users', data)
  }

  /**
   * 更新用户
   */
  static updateUser(id: number, data: UpdateUserDto): Promise<ApiResult> {
    return request.put(`/users/${id}`, data)
  }

  /**
   * 删除用户
   */
  static deleteUser(id: number): Promise<ApiResult> {
    return request.delete(`/users/${id}`)
  }

  /**
   * 批量删除用户
   */
  static batchDeleteUsers(ids: number[]): Promise<ApiResult> {
    return request.delete('/users/batch', { ids })
  }

  /**
   * 更新用户状态
   */
  static updateUserStatus(id: number, status: boolean): Promise<ApiResult> {
    return request.patch(`/users/${id}/status`, status)
  }

  /**
   * 重置用户密码
   */
  static resetUserPassword(id: number, newPassword: string): Promise<ApiResult> {
    return request.post(`/users/${id}/reset-password`, { newPassword })
  }

  /**
   * 检查用户名是否存在
   */
  static checkUsername(username: string, excludeId?: number): Promise<ApiResult<boolean>> {
    const params: any = { username }
    if (excludeId) {
      params.excludeId = excludeId
    }
    return request.get('/users/check-username', params)
  }

  /**
   * 检查邮箱是否存在
   */
  static checkEmail(email: string, excludeId?: number): Promise<ApiResult<boolean>> {
    const params: any = { email }
    if (excludeId) {
      params.excludeId = excludeId
    }
    return request.get('/users/check-email', params)
  }

  /**
   * 获取用户的角色列表
   */
  static getUserRoles(userId: number): Promise<ApiResult<any[]>> {
    return request.get(`/users/${userId}/roles`)
  }

  /**
   * 为用户分配角色
   */
  static assignRolesToUser(userId: number, roleIds: number[]): Promise<ApiResult> {
    return request.post(`/users/${userId}/roles`, { roleIds })
  }

  /**
   * 导出用户数据
   */
  static exportUsers(params?: UserQueryDto): Promise<void> {
    return request.download('/users/export', params, 'users.xlsx')
  }

  /**
   * 导入用户数据
   */
  static importUsers(file: File): Promise<ApiResult> {
    const formData = new FormData()
    formData.append('file', file)
    return request.upload('/users/import', formData)
  }
}
