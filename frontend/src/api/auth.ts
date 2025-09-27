import request from '@/utils/request'
import type {
  LoginDto,
  LoginResponse,
  RefreshTokenDto,
  ChangePasswordDto,
  ResetPasswordDto,
  CaptchaResponse,
  User,
  Permission,
  ApiResponse
} from '@/types'

// 认证相关API
export const authApi = {
  // 用户登录
  login(data: LoginDto): Promise<ApiResponse<LoginResponse>> {
    return request.post('/auth/login', data)
  },

  // 刷新令牌
  refreshToken(data: RefreshTokenDto): Promise<ApiResponse<LoginResponse>> {
    return request.post('/auth/refresh-token', data)
  },

  // 用户登出
  logout(): Promise<ApiResponse<void>> {
    return request.post('/auth/logout')
  },

  // 生成验证码
  generateCaptcha(): Promise<ApiResponse<CaptchaResponse>> {
    return request.get('/auth/captcha')
  },

  // 验证验证码
  validateCaptcha(key: string, code: string): Promise<ApiResponse<boolean>> {
    return request.post(`/auth/captcha/validate?key=${key}&code=${code}`)
  },

  // 发送重置密码验证码
  sendResetCode(usernameOrEmail: string): Promise<ApiResponse<void>> {
    return request.post('/auth/send-reset-code', usernameOrEmail)
  },

  // 重置密码
  resetPassword(data: ResetPasswordDto): Promise<ApiResponse<void>> {
    return request.post('/auth/reset-password', data)
  },

  // 修改密码
  changePassword(data: ChangePasswordDto): Promise<ApiResponse<void>> {
    return request.post('/auth/change-password', data)
  },

  // 获取当前用户信息
  getCurrentUserInfo(): Promise<ApiResponse<User>> {
    return request.get('/auth/user-info')
  },

  // 获取用户权限
  getUserPermissions(): Promise<ApiResponse<Permission[]>> {
    return request.get('/auth/permissions')
  },

  // 验证令牌
  validateToken(token: string): Promise<ApiResponse<boolean>> {
    return request.post('/auth/validate-token', token)
  }
}
