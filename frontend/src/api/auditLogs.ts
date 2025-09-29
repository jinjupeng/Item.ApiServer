import request from '@/utils/request'
import type {
  AuditLog,
  AuditLogQueryDto,
  AuditLogExportDto,
  AuditLogStatistics,
  CreateAuditLogDto,
  PagedResponse,
  ApiResponse
} from '@/types'

// 审计日志管理API
export const auditLogsApi = {
  // 获取审计日志分页列表
  getAuditLogs(params: AuditLogQueryDto): Promise<ApiResponse<PagedResponse<AuditLog>>> {
    return request.get('/auditlogs', { params })
  },

  // 根据ID获取审计日志详情
  getAuditLogById(id: string): Promise<ApiResponse<AuditLog>> {
    return request.get(`/auditlogs/${id}`)
  },

  // 获取当前用户操作日志
  getMyAuditLogs(params: AuditLogQueryDto): Promise<ApiResponse<PagedResponse<AuditLog>>> {
    return request.get('/auditlogs/my-logs', { params })
  },

  // 获取指定用户的操作日志
  getUserAuditLogs(userId: number, params: AuditLogQueryDto): Promise<ApiResponse<PagedResponse<AuditLog>>> {
    return request.get(`/auditlogs/user/${userId}`, { params })
  },

  // 获取模块操作日志
  getModuleAuditLogs(module: string, params: AuditLogQueryDto): Promise<ApiResponse<PagedResponse<AuditLog>>> {
    return request.get(`/auditlogs/module/${module}`, { params })
  },

  // 导出审计日志
  exportAuditLogs(params: AuditLogExportDto): Promise<Blob> {
    return request.get('/auditlogs/export', { 
      params,
      responseType: 'blob'
    }).then((response: any) => response.data)
  },

  // 获取审计日志统计信息
  getAuditLogStatistics(startDate?: string, endDate?: string): Promise<ApiResponse<AuditLogStatistics>> {
    const params: any = {}
    if (startDate) params.startDate = startDate
    if (endDate) params.endDate = endDate
    return request.get('/auditlogs/statistics', { params })
  },

  // 清理过期审计日志
  cleanupExpiredLogs(retentionDays: number = 90): Promise<ApiResponse<number>> {
    return request.delete('/auditlogs/cleanup', { 
      params: { retentionDays }
    })
  },

  // 手动记录审计日志
  logAudit(data: CreateAuditLogDto): Promise<ApiResponse<boolean>> {
    return request.post('/auditlogs', data)
  }
}
