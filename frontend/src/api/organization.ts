import request from '@/utils/request'
import type {
  Organization,
  OrganizationTreeDto,
  CreateOrganizationDto,
  UpdateOrganizationDto,
  OrganizationQueryDto,
  ApiResult
} from '@/types/api'

/**
 * 组织管理API
 */
export class OrganizationApi {
  /**
   * 获取组织树
   */
  static getOrganizationTree(params?: OrganizationQueryDto): Promise<ApiResult<OrganizationTreeDto[]>> {
    return request.get('/organizations/tree', params)
  }

  /**
   * 根据ID获取组织详情
   */
  static getOrganizationById(id: number): Promise<ApiResult<Organization>> {
    return request.get(`/organizations/${id}`)
  }

  /**
   * 创建组织
   */
  static createOrganization(data: CreateOrganizationDto): Promise<ApiResult<number>> {
    return request.post('/organizations', data)
  }

  /**
   * 更新组织
   */
  static updateOrganization(id: number, data: UpdateOrganizationDto): Promise<ApiResult> {
    return request.put(`/organizations/${id}`, data)
  }

  /**
   * 删除组织
   */
  static deleteOrganization(id: number): Promise<ApiResult> {
    return request.delete(`/organizations/${id}`)
  }

  /**
   * 更新组织状态
   */
  static updateOrganizationStatus(id: number, status: boolean): Promise<ApiResult> {
    return request.patch(`/organizations/${id}/status`, status)
  }

  /**
   * 更新组织排序
   */
  static updateOrganizationSort(id: number, sort: number): Promise<ApiResult> {
    return request.patch(`/organizations/${id}/sort`, sort)
  }

  /**
   * 检查组织编码是否存在
   */
  static checkOrgCode(orgCode: string, excludeId?: number): Promise<ApiResult<boolean>> {
    const params: any = { orgCode }
    if (excludeId) {
      params.excludeId = excludeId
    }
    return request.get('/organizations/check-code', params)
  }

  /**
   * 获取父组织列表
   */
  static getParentOrganizations(): Promise<ApiResult<Organization[]>> {
    return request.get('/organizations/parents')
  }

  /**
   * 根据父ID获取子组织
   */
  static getChildOrganizations(parentId: number): Promise<ApiResult<Organization[]>> {
    return request.get(`/organizations/parent/${parentId}/children`)
  }

  /**
   * 获取组织下的用户数量
   */
  static getOrganizationUserCount(orgId: number): Promise<ApiResult<number>> {
    return request.get(`/organizations/${orgId}/user-count`)
  }

  /**
   * 移动组织位置
   */
  static moveOrganization(id: number, targetId: number, position: 'before' | 'after' | 'inner'): Promise<ApiResult> {
    return request.post(`/organizations/${id}/move`, { targetId, position })
  }

  /**
   * 批量更新组织排序
   */
  static batchUpdateOrganizationSort(organizations: { id: number; sort: number }[]): Promise<ApiResult> {
    return request.post('/organizations/batch-sort', organizations)
  }

  /**
   * 获取组织路径
   */
  static getOrganizationPath(id: number): Promise<ApiResult<Organization[]>> {
    return request.get(`/organizations/${id}/path`)
  }

  /**
   * 获取组织统计信息
   */
  static getOrganizationStats(id: number): Promise<ApiResult<any>> {
    return request.get(`/organizations/${id}/stats`)
  }

  /**
   * 导出组织数据
   */
  static exportOrganizations(): Promise<void> {
    return request.download('/organizations/export', undefined, 'organizations.xlsx')
  }

  /**
   * 导入组织数据
   */
  static importOrganizations(file: File): Promise<ApiResult> {
    const formData = new FormData()
    formData.append('file', file)
    return request.upload('/organizations/import', formData)
  }
}
