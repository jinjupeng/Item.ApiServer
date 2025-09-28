import request from '@/utils/request'
import type {
  User,
  CreateUserDto,
  UpdateUserDto,
  UserQueryDto,
  UserStatus,
  PagedResponse,
  ApiResponse,
  ResetPasswordDto
} from '@/types'

// 用户管理API
export const usersApi = {
  // 获取用户列表
  getUsers(params: UserQueryDto): Promise<ApiResponse<PagedResponse<User>>> {
    return request.get('/users', { params })
  },

  // 根据ID获取用户详情
  getUserById(id: number): Promise<ApiResponse<User>> {
    return request.get(`/users/${id}`)
  },

  // 创建用户
  createUser(data: CreateUserDto): Promise<ApiResponse<User>> {
    return request.post('/users', data)
  },

  // 更新用户
  updateUser(id: number, data: UpdateUserDto): Promise<ApiResponse<User>> {
    return request.put(`/users/${id}`, data)
  },

  // 删除用户
  deleteUser(id: number): Promise<ApiResponse<void>> {
    return request.delete(`/users/${id}`)
  },

  // 更新用户状态
  updateUserStatus(id: number, status: UserStatus): Promise<ApiResponse<void>> {
    return request.patch(`/users/${id}/status`, status)
  },

  // 重置用户密码
  resetPassword(id: number, resetPassword: ResetPasswordDto): Promise<ApiResponse<void>> {
    return request.post(`/users/${id}/reset-password`, resetPassword)
  }
}
