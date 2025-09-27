import request from '@/utils/request'
import type {
  Menu,
  MenuTreeDto,
  CreateMenuDto,
  UpdateMenuDto,
  MenuQueryDto,
  ApiResult
} from '@/types/api'

/**
 * 菜单管理API
 */
export class MenuApi {
  /**
   * 获取菜单树
   */
  static getMenuTree(params?: MenuQueryDto): Promise<ApiResult<MenuTreeDto[]>> {
    return request.get('/menus/tree', params)
  }

  /**
   * 根据ID获取菜单详情
   */
  static getMenuById(id: number): Promise<ApiResult<Menu>> {
    return request.get(`/menus/${id}`)
  }

  /**
   * 创建菜单
   */
  static createMenu(data: CreateMenuDto): Promise<ApiResult<number>> {
    return request.post('/menus', data)
  }

  /**
   * 更新菜单
   */
  static updateMenu(id: number, data: UpdateMenuDto): Promise<ApiResult> {
    return request.put(`/menus/${id}`, data)
  }

  /**
   * 删除菜单
   */
  static deleteMenu(id: number): Promise<ApiResult> {
    return request.delete(`/menus/${id}`)
  }

  /**
   * 更新菜单状态
   */
  static updateMenuStatus(id: number, status: boolean): Promise<ApiResult> {
    return request.patch(`/menus/${id}/status`, status)
  }

  /**
   * 更新菜单排序
   */
  static updateMenuSort(id: number, sort: number): Promise<ApiResult> {
    return request.patch(`/menus/${id}/sort`, sort)
  }

  /**
   * 检查菜单编码是否存在
   */
  static checkMenuCode(menuCode: string, excludeId?: number): Promise<ApiResult<boolean>> {
    const params: any = { menuCode }
    if (excludeId) {
      params.excludeId = excludeId
    }
    return request.get('/menus/check-code', params)
  }

  /**
   * 获取父菜单列表
   */
  static getParentMenus(): Promise<ApiResult<Menu[]>> {
    return request.get('/menus/parents')
  }

  /**
   * 根据父ID获取子菜单
   */
  static getChildMenus(parentId: number): Promise<ApiResult<Menu[]>> {
    return request.get(`/menus/parent/${parentId}/children`)
  }

  /**
   * 获取用户菜单树
   */
  static getUserMenuTree(userId: number): Promise<ApiResult<MenuTreeDto[]>> {
    return request.get(`/menus/user/${userId}/tree`)
  }

  /**
   * 根据用户名获取用户菜单树
   */
  static getUserMenuTreeByUsername(username: string): Promise<ApiResult<MenuTreeDto[]>> {
    return request.get(`/menus/user/${username}/tree-by-name`)
  }

  /**
   * 根据角色ID获取角色菜单
   */
  static getMenusByRoleId(roleId: number): Promise<ApiResult<Menu[]>> {
    return request.get(`/menus/role/${roleId}`)
  }

  /**
   * 获取菜单选择树（用于角色分配菜单权限）
   */
  static getMenuTreeForRoleAssign(roleId?: number): Promise<ApiResult<MenuTreeDto[]>> {
    const params = roleId ? { roleId } : undefined
    return request.get('/menus/assign-tree', params)
  }

  /**
   * 保存角色菜单权限
   */
  static saveRoleMenuPermissions(roleId: number, menuIds: number[]): Promise<ApiResult> {
    return request.post(`/menus/role/${roleId}/permissions`, menuIds)
  }

  /**
   * 获取展开的菜单Keys
   */
  static getExpandedKeys(): Promise<ApiResult<string[]>> {
    return request.get('/menus/expanded-keys')
  }

  /**
   * 获取角色已选中的菜单Keys
   */
  static getCheckedKeysByRoleId(roleId: number): Promise<ApiResult<string[]>> {
    return request.get(`/menus/role/${roleId}/checked-keys`)
  }

  /**
   * 批量更新菜单排序
   */
  static batchUpdateMenuSort(menus: { id: number; sort: number }[]): Promise<ApiResult> {
    return request.post('/menus/batch-sort', menus)
  }

  /**
   * 移动菜单位置
   */
  static moveMenu(id: number, targetId: number, position: 'before' | 'after' | 'inner'): Promise<ApiResult> {
    return request.post(`/menus/${id}/move`, { targetId, position })
  }

  /**
   * 复制菜单
   */
  static copyMenu(id: number, targetParentId?: number): Promise<ApiResult<number>> {
    return request.post(`/menus/${id}/copy`, { targetParentId })
  }

  /**
   * 导出菜单数据
   */
  static exportMenus(): Promise<void> {
    return request.download('/menus/export', undefined, 'menus.json')
  }

  /**
   * 导入菜单数据
   */
  static importMenus(file: File): Promise<ApiResult> {
    const formData = new FormData()
    formData.append('file', file)
    return request.upload('/menus/import', formData)
  }
}
