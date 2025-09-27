import request from '@/utils/request'
import type {
  Role,
  CreateRoleDto,
  UpdateRoleDto,
  RoleQueryDto,
  UserRoleAssignDto,
  RolePermissionDto,
  PagedResult,
  ApiResult
} from '@/types/api'

/**
 * 角色管理API
 */
export class RoleApi {
  /**
   * 获取角色列表（分页）
   */
  static getRoles(params: RoleQueryDto): Promise<ApiResult<PagedResult<Role>>> {
    return request.get('/roles', params)
  }

  /**
   * 获取所有角色
   */
  static getAllRoles(): Promise<ApiResult<Role[]>> {
    return request.get('/roles/all')
  }

  /**
   * 根据ID获取角色详情
   */
  static getRoleById(id: number): Promise<ApiResult<Role>> {
    return request.get(`/roles/${id}`)
  }

  /**
   * 创建角色
   */
  static createRole(data: CreateRoleDto): Promise<ApiResult<number>> {
    return request.post('/roles', data)
  }

  /**
   * 更新角色
   */
  static updateRole(id: number, data: UpdateRoleDto): Promise<ApiResult> {
    return request.put(`/roles/${id}`, data)
  }

  /**
   * 删除角色
   */
  static deleteRole(id: number): Promise<ApiResult> {
    return request.delete(`/roles/${id}`)
  }

  /**
   * 批量删除角色
   */
  static batchDeleteRoles(ids: number[]): Promise<ApiResult> {
    return request.delete('/roles/batch', { ids })
  }

  /**
   * 更新角色状态
   */
  static updateRoleStatus(id: number, status: boolean): Promise<ApiResult> {
    return request.patch(`/roles/${id}/status`, status)
  }

  /**
   * 检查角色名称是否存在
   */
  static checkRoleName(roleName: string, excludeId?: number): Promise<ApiResult<boolean>> {
    const params: any = { roleName }
    if (excludeId) {
      params.excludeId = excludeId
    }
    return request.get('/roles/check-name', params)
  }

  /**
   * 检查角色编码是否存在
   */
  static checkRoleCode(roleCode: string, excludeId?: number): Promise<ApiResult<boolean>> {
    const params: any = { roleCode }
    if (excludeId) {
      params.excludeId = excludeId
    }
    return request.get('/roles/check-code', params)
  }

  /**
   * 为用户分配角色
   */
  static assignRolesToUser(data: UserRoleAssignDto): Promise<ApiResult> {
    return request.post('/roles/assign', data)
  }

  /**
   * 获取角色的权限
   */
  static getRolePermissions(roleId: number): Promise<ApiResult<any>> {
    return request.get(`/roles/${roleId}/permissions`)
  }

  /**
   * 分配角色权限
   */
  static assignRolePermissions(data: RolePermissionDto): Promise<ApiResult> {
    return request.post('/roles/permissions', data)
  }

  /**
   * 获取角色下的用户列表
   */
  static getRoleUsers(roleId: number): Promise<ApiResult<any[]>> {
    return request.get(`/roles/${roleId}/users`)
  }

  /**
   * 从角色中移除用户
   */
  static removeUserFromRole(roleId: number, userId: number): Promise<ApiResult> {
    return request.delete(`/roles/${roleId}/users/${userId}`)
  }

  /**
   * 复制角色
   */
  static copyRole(roleId: number, newRoleName: string): Promise<ApiResult<number>> {
    return request.post(`/roles/${roleId}/copy`, { newRoleName })
  }

  /**
   * 导出角色数据
   */
  static exportRoles(params?: RoleQueryDto): Promise<void> {
    return request.download('/roles/export', params, 'roles.xlsx')
  }
}
