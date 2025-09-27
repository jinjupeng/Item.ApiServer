import request from '@/utils/request'
import type {
  LoginDto,
  LoginResponseDto,
  RefreshTokenDto,
  ChangePasswordDto,
  ResetPasswordDto,
  UserInfoDto,
  UserPermissionDto,
  CaptchaDto,
  ApiResult
} from '@/types/api'

/**
 * 认证相关API
 */
export class AuthApi {
  /**
   * 用户登录
   */
  static login(data: LoginDto): Promise<ApiResult<LoginResponseDto>> {
    return request.post('/auth/login', data)
  }

  /**
   * 刷新令牌
   */
  static refreshToken(data: RefreshTokenDto): Promise<ApiResult<LoginResponseDto>> {
    return request.post('/auth/refresh-token', data)
  }

  /**
   * 用户登出
   */
  static logout(): Promise<ApiResult> {
    return request.post('/auth/logout')
  }

  /**
   * 生成验证码
   */
  static generateCaptcha(): Promise<ApiResult<CaptchaDto>> {
    return request.get('/auth/captcha')
  }

  /**
   * 验证验证码
   */
  static validateCaptcha(key: string, code: string): Promise<ApiResult<boolean>> {
    return request.post(`/auth/captcha/validate?key=${key}&code=${code}`)
  }

  /**
   * 发送重置密码验证码
   */
  static sendResetCode(usernameOrEmail: string): Promise<ApiResult> {
    return request.post('/auth/send-reset-code', usernameOrEmail)
  }

  /**
   * 重置密码
   */
  static resetPassword(data: ResetPasswordDto): Promise<ApiResult> {
    return request.post('/auth/reset-password', data)
  }

  /**
   * 修改密码
   */
  static changePassword(data: ChangePasswordDto): Promise<ApiResult> {
    return request.post('/auth/change-password', data)
  }

  /**
   * 获取当前用户信息
   */
  static getCurrentUserInfo(): Promise<ApiResult<UserInfoDto>> {
    return request.get('/auth/user-info')
  }

  /**
   * 获取用户权限
   */
  static getUserPermissions(): Promise<ApiResult<UserPermissionDto>> {
    return request.get('/auth/permissions')
  }

  /**
   * 验证令牌
   */
  static validateToken(token: string): Promise<ApiResult<boolean>> {
    return request.post('/auth/validate-token', token)
  }
}
