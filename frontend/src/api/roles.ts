import request from '@/utils/request'
import type {
  Role,
  CreateRoleDto,
  UpdateRoleDto,
  RoleQueryDto,
  UserRoleAssignDto,
  RolePermissionDto,
  Permission,
  PagedResponse,
  ApiResponse
} from '@/types'

// 角色管理API
export const rolesApi = {
  // 获取角色列表
  getRoles(params: RoleQueryDto): Promise<ApiResponse<PagedResponse<Role>>> {
    return request.get('/roles', { params })
  },

  // 获取所有角色
  getAllRoles(): Promise<ApiResponse<Role[]>> {
    return request.get('/roles/all')
  },

  // 根据ID获取角色详情
  getRoleById(id: number): Promise<ApiResponse<Role>> {
    return request.get(`/roles/${id}`)
  },

  // 创建角色
  createRole(data: CreateRoleDto): Promise<ApiResponse<Role>> {
    return request.post('/roles', data)
  },

  // 更新角色
  updateRole(id: number, data: UpdateRoleDto): Promise<ApiResponse<Role>> {
    return request.put(`/roles/${id}`, data)
  },

  // 删除角色
  deleteRole(id: number): Promise<ApiResponse<void>> {
    return request.delete(`/roles/${id}`)
  },

  // 更新角色状态
  updateRoleStatus(id: number, status: boolean): Promise<ApiResponse<void>> {
    return request.patch(`/roles/${id}/status`, status)
  },

  // 检查角色名称是否存在
  checkRoleName(roleName: string, excludeId?: number): Promise<ApiResponse<boolean>> {
    const params = excludeId ? { roleName, excludeId } : { roleName }
    return request.get('/roles/check-name', { params })
  },

  // 获取用户的角色列表
  getUserRoles(userId: number): Promise<ApiResponse<Role[]>> {
    return request.get(`/roles/user/${userId}`)
  },

  // 为用户分配角色
  assignRolesToUser(data: UserRoleAssignDto): Promise<ApiResponse<void>> {
    return request.post('/roles/assign', data)
  },

  // 移除用户的角色
  removeRolesFromUser(userId: number, roleIds: number[]): Promise<ApiResponse<void>> {
    return request.delete(`/roles/user/${userId}/roles`, { data: roleIds })
  },

  // 获取角色权限
  getRolePermissions(id: number): Promise<ApiResponse<Permission[]>> {
    return request.get(`/roles/${id}/permissions`)
  },

  // 为角色分配权限
  assignPermissionsToRole(data: RolePermissionDto): Promise<ApiResponse<void>> {
    return request.post('/roles/permissions', data)
  },

  // 获取用户角色选择列表
  getRolesForUserAssign(userId: number): Promise<ApiResponse<Role[]>> {
    return request.get(`/roles/user/${userId}/assign-list`)
  }
}
