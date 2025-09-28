import request from '@/utils/request'
import type {
  Menu,
  CreateMenuDto,
  UpdateMenuDto,
  MenuQueryDto,
  ApiResponse
} from '@/types'

// 菜单管理API
export const menusApi = {
  // 获取菜单树
  getMenuTree(params?: MenuQueryDto): Promise<ApiResponse<Menu[]>> {
    return request.get('/menus/tree', { params })
  },

  // 根据ID获取菜单详情
  getMenuById(id: number): Promise<ApiResponse<Menu>> {
    return request.get(`/menus/${id}`)
  },

  // 创建菜单
  createMenu(data: CreateMenuDto): Promise<ApiResponse<Menu>> {
    return request.post('/menus', data)
  },

  // 更新菜单
  updateMenu(id: number, data: UpdateMenuDto): Promise<ApiResponse<Menu>> {
    return request.put(`/menus/${id}`, data)
  },

  // 删除菜单
  deleteMenu(id: number): Promise<ApiResponse<void>> {
    return request.delete(`/menus/${id}`)
  },

  // 更新菜单状态
  updateMenuStatus(id: number, status: boolean): Promise<ApiResponse<void>> {
    return request.patch(`/menus/${id}/status`, status)
  },

  // 更新菜单排序
  updateMenuSort(id: number, sort: number): Promise<ApiResponse<void>> {
    return request.patch(`/menus/${id}/sort`, sort)
  },

  // 检查菜单编码是否存在
  checkMenuCode(menuCode: string, excludeId?: number): Promise<ApiResponse<boolean>> {
    const params = excludeId ? { menuCode, excludeId } : { menuCode }
    return request.get('/menus/check-code', { params })
  },

  // 获取父菜单列表
  getParentMenus(): Promise<ApiResponse<Menu[]>> {
    return request.get('/menus/parents')
  },

  // 根据父ID获取子菜单
  getChildMenus(parentId: number): Promise<ApiResponse<Menu[]>> {
    return request.get(`/menus/parent/${parentId}/children`)
  },

  // 获取用户菜单树
  getUserMenuTree(userId: number): Promise<ApiResponse<Menu[]>> {
    return request.get(`/menus/user/${userId}/tree`)
  },

  // 根据用户名获取用户菜单树
  getUserMenuTreeByUsername(username: string): Promise<ApiResponse<Menu[]>> {
    return request.get(`/menus/user/${username}/tree-by-name`)
  },

  // 根据角色ID获取角色菜单
  getMenusByRoleId(roleId: number): Promise<ApiResponse<Menu[]>> {
    return request.get(`/menus/role/${roleId}`)
  },

  // 获取菜单选择树（用于角色分配菜单权限）
  getMenuTreeForRoleAssign(roleId?: number): Promise<ApiResponse<Menu[]>> {
    const params = roleId ? { roleId } : {}
    return request.get('/menus/assign-tree', { params })
  },

  // 保存角色菜单权限
  saveRoleMenuPermissions(roleId: number, menuIds: number[]): Promise<ApiResponse<void>> {
    return request.post(`/menus/role/${roleId}/permissions`, menuIds)
  },

  // 获取展开的菜单Keys
  getExpandedKeys(): Promise<ApiResponse<string[]>> {
    return request.get('/menus/expanded-keys')
  },

  // 获取角色已选中的菜单Keys
  getCheckedKeysByRoleId(roleId: number): Promise<ApiResponse<string[]>> {
    return request.get(`/menus/role/${roleId}/checked-keys`)
  },

  // 获取当前用户菜单树
  getCurrentUserMenuTree(): Promise<ApiResponse<Menu[]>> {
    return request.get('/menus/my-tree')
  }
}
