<template>
  <div class="page-container">
    <!-- 搜索表单 -->
    <div class="search-form">
      <el-form
        ref="searchFormRef"
        :model="searchForm"
        :inline="true"
        label-width="80px"
      >
        <el-form-item label="关键词">
          <el-input
            v-model="searchForm.keyword"
            placeholder="操作描述/用户名"
            clearable
            style="width: 200px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        
        <el-form-item label="操作类型">
          <el-select
            v-model="searchForm.action"
            placeholder="请选择操作类型"
            clearable
            style="width: 120px"
          >
            <el-option label="创建" value="Create" />
            <el-option label="更新" value="Update" />
            <el-option label="删除" value="Delete" />
            <el-option label="查询" value="Read" />
            <el-option label="登录" value="Login" />
            <el-option label="登出" value="Logout" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="模块">
          <el-select
            v-model="searchForm.module"
            placeholder="请选择模块"
            clearable
            style="width: 120px"
          >
            <el-option label="用户管理" value="User" />
            <el-option label="角色管理" value="Role" />
            <el-option label="菜单管理" value="Menu" />
            <el-option label="权限管理" value="Permission" />
            <el-option label="系统设置" value="System" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="操作结果">
          <el-select
            v-model="searchForm.result"
            placeholder="请选择结果"
            clearable
            style="width: 100px"
          >
            <el-option label="成功" value="Success" />
            <el-option label="失败" value="Failed" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="时间范围">
          <el-date-picker
            v-model="dateRange"
            type="datetimerange"
            range-separator="至"
            start-placeholder="开始时间"
            end-placeholder="结束时间"
            format="YYYY-MM-DD HH:mm:ss"
            value-format="YYYY-MM-DD HH:mm:ss"
            style="width: 350px"
            @change="handleDateRangeChange"
          />
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">
            搜索
          </el-button>
          <el-button icon="Refresh" @click="handleReset">
            重置
          </el-button>
        </el-form-item>
      </el-form>
    </div>
    
    <!-- 操作工具栏 -->
    <div class="table-toolbar">
      <div class="toolbar-left">
        <el-button
          v-if="authStore.hasPermission('system:auditlog:export')"
          type="success"
          icon="Download"
          @click="handleExport"
          :loading="exportLoading"
        >
          导出日志
        </el-button>
        
        <el-button
          v-if="authStore.hasPermission('system:auditlog:cleanup')"
          type="warning"
          icon="Delete"
          @click="handleCleanup"
        >
          清理过期日志
        </el-button>
        
        <el-button
          type="info"
          icon="DataAnalysis"
          @click="handleShowStatistics"
        >
          统计信息
        </el-button>
      </div>
      
      <div class="toolbar-right">
        <el-button icon="Refresh" @click="getAuditLogs">
          刷新
        </el-button>
      </div>
    </div>
    
    <!-- 审计日志表格 -->
    <el-table
      v-loading="loading"
      :data="auditLogs"
      row-key="id"
    >
      <el-table-column prop="id" label="ID" width="200" show-overflow-tooltip />
      
      <el-table-column prop="action" label="操作类型" width="100">
        <template #default="{ row }">
          <el-tag :type="getActionTagType(row.action)" size="small">
            {{ getActionText(row.action) }}
          </el-tag>
        </template>
      </el-table-column>
      
      <el-table-column prop="module" label="模块" width="100" />
      
      <el-table-column prop="description" label="操作描述" min-width="200" show-overflow-tooltip />
      
      <el-table-column prop="userName" label="操作用户" width="120" />
      
      <el-table-column prop="ipAddress" label="IP地址" width="130" />
      
      <el-table-column label="操作结果" width="80">
        <template #default="{ row }">
          <el-tag
            :type="row.result === 'Success' ? 'success' : 'danger'"
            size="small"
          >
            {{ row.result === 'Success' ? '成功' : '失败' }}
          </el-tag>
        </template>
      </el-table-column>
      
      <el-table-column prop="duration" label="耗时(ms)" width="100">
        <template #default="{ row }">
          <span v-if="row.duration">{{ row.duration }}ms</span>
          <span v-else>-</span>
        </template>
      </el-table-column>
      
      <el-table-column prop="createdAt" label="操作时间" width="160">
        <template #default="{ row }">
          {{ formatDate(row.createdAt) }}
        </template>
      </el-table-column>
      
      <el-table-column label="操作" width="120" fixed="right">
        <template #default="{ row }">
          <el-button
            type="primary"
            size="small"
            link
            @click="handleViewDetail(row)"
          >
            详情
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    
    <!-- 分页 -->
    <el-pagination
      v-model:current-page="pagination.pageIndex"
      v-model:page-size="pagination.pageSize"
      :total="pagination.totalCount"
      :page-sizes="[10, 20, 50, 100]"
      layout="total, sizes, prev, pager, next, jumper"
      @size-change="getAuditLogs"
      @current-change="getAuditLogs"
    />
    
    <!-- 审计日志详情对话框 -->
    <AuditLogDetailDialog
      v-model="showDetailDialog"
      :audit-log="currentAuditLog"
    />
    
    <!-- 统计信息对话框 -->
    <AuditLogStatisticsDialog
      v-model="showStatisticsDialog"
    />
    
    <!-- 清理日志对话框 -->
    <el-dialog
      v-model="showCleanupDialog"
      title="清理过期日志"
      width="400px"
    >
      <el-form :model="cleanupForm" label-width="120px">
        <el-form-item label="保留天数">
          <el-input-number
            v-model="cleanupForm.retentionDays"
            :min="1"
            :max="365"
            controls-position="right"
          />
          <div class="form-tip">
            将删除 {{ cleanupForm.retentionDays }} 天前的所有审计日志
          </div>
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="showCleanupDialog = false">取消</el-button>
        <el-button
          type="danger"
          @click="confirmCleanup"
          :loading="cleanupLoading"
        >
          确认清理
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox, FormInstance } from 'element-plus'
import { useAuthStore } from '@/stores/auth'
import { auditLogsApi } from '@/api/auditLogs'
import AuditLogDetailDialog from './components/AuditLogDetailDialog.vue'
import AuditLogStatisticsDialog from './components/AuditLogStatisticsDialog.vue'
import type { AuditLog, AuditLogQueryDto } from '@/types'
import dayjs from 'dayjs'

const authStore = useAuthStore()

const searchFormRef = ref<FormInstance>()
const loading = ref(false)
const exportLoading = ref(false)
const cleanupLoading = ref(false)
const auditLogs = ref<AuditLog[]>([])
const showDetailDialog = ref(false)
const showStatisticsDialog = ref(false)
const showCleanupDialog = ref(false)
const currentAuditLog = ref<AuditLog | null>(null)
const dateRange = ref<[string, string] | null>(null)

// 搜索表单
const searchForm = reactive<AuditLogQueryDto>({
  keyword: '',
  action: undefined,
  module: undefined,
  result: undefined,
  dateFrom: undefined,
  dateTo: undefined,
  pageIndex: 1,
  pageSize: 20
})

// 分页信息
const pagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0
})

// 清理表单
const cleanupForm = reactive({
  retentionDays: 90
})

// 获取审计日志列表
const getAuditLogs = async () => {
  try {
    loading.value = true
    
    const params = {
      ...searchForm,
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize
    }
    
    const response = await auditLogsApi.getAuditLogs(params)
    const data = response.data
    
    auditLogs.value = data.items
    pagination.totalCount = data.totalCount
    pagination.totalPages = data.totalPages
  } catch (error) {
    console.error('获取审计日志列表失败:', error)
    ElMessage.error('获取审计日志列表失败')
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  pagination.pageIndex = 1
  getAuditLogs()
}

// 重置搜索
const handleReset = () => {
  if (searchFormRef.value) {
    searchFormRef.value.resetFields()
  }
  Object.assign(searchForm, {
    keyword: '',
    action: undefined,
    module: undefined,
    result: undefined,
    dateFrom: undefined,
    dateTo: undefined
  })
  dateRange.value = null
  handleSearch()
}

// 时间范围变化
const handleDateRangeChange = (value: [string, string] | null) => {
  if (value) {
    searchForm.dateFrom = value[0]
    searchForm.dateTo = value[1]
  } else {
    searchForm.dateFrom = undefined
    searchForm.dateTo = undefined
  }
}

// 查看详情
const handleViewDetail = (auditLog: AuditLog) => {
  currentAuditLog.value = auditLog
  showDetailDialog.value = true
}

// 导出日志
const handleExport = async () => {
  try {
    exportLoading.value = true
    
    const params = {
      searchTerm: searchForm.keyword,
      action: searchForm.action,
      module: searchForm.module,
      result: searchForm.result,
      dateFrom: searchForm.dateFrom,
      dateTo: searchForm.dateTo
    }
    
    const blob = await auditLogsApi.exportAuditLogs(params)
    
    // 为了解决中文乱码问题，需要添加UTF-8 BOM
    const utf8BOM = new Uint8Array([0xEF, 0xBB, 0xBF])
    const blobWithBOM = new Blob([utf8BOM, blob], { type: 'text/csv;charset=utf-8' })
    
    // 创建下载链接
    const url = window.URL.createObjectURL(blobWithBOM)
    const link = document.createElement('a')
    link.href = url
    link.download = `audit-logs-${dayjs().format('YYYY-MM-DD-HH-mm-ss')}.csv`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
    
    ElMessage.success('导出成功')
  } catch (error) {
    console.error('导出审计日志失败:', error)
    ElMessage.error('导出审计日志失败')
  } finally {
    exportLoading.value = false
  }
}

// 显示统计信息
const handleShowStatistics = () => {
  showStatisticsDialog.value = true
}

// 清理过期日志
const handleCleanup = () => {
  showCleanupDialog.value = true
}

// 确认清理
const confirmCleanup = async () => {
  try {
    await ElMessageBox.confirm(
      `确定要清理 ${cleanupForm.retentionDays} 天前的所有审计日志吗？此操作不可恢复！`,
      '警告',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    cleanupLoading.value = true
    
    const response = await auditLogsApi.cleanupExpiredLogs(cleanupForm.retentionDays)
    
    ElMessage.success(`清理完成，共删除 ${response.data} 条记录`)
    showCleanupDialog.value = false
    getAuditLogs()
  } catch (error) {
    if (error !== 'cancel') {
      console.error('清理审计日志失败:', error)
      ElMessage.error('清理审计日志失败')
    }
  } finally {
    cleanupLoading.value = false
  }
}

// 获取操作类型标签类型
const getActionTagType = (action: string) => {
  switch (action) {
    case 'Create':
      return 'success'
    case 'Update':
      return 'warning'
    case 'Delete':
      return 'danger'
    case 'Read':
      return 'info'
    case 'Login':
      return 'primary'
    case 'Logout':
      return 'info'
    default:
      return 'info'
  }
}

// 获取操作类型文本
const getActionText = (action: string) => {
  switch (action) {
    case 'Create':
      return '创建'
    case 'Update':
      return '更新'
    case 'Delete':
      return '删除'
    case 'Read':
      return '查询'
    case 'Login':
      return '登录'
    case 'Logout':
      return '登出'
    default:
      return action
  }
}

// 格式化日期
const formatDate = (date: string) => {
  return dayjs(date).format('YYYY-MM-DD HH:mm:ss')
}

onMounted(() => {
  getAuditLogs()
})
</script>

<style lang="scss" scoped>
.form-tip {
  font-size: 12px;
  color: #999;
  margin-top: 4px;
}

.el-pagination {
  margin-top: 20px;
  text-align: right;
}
</style>
