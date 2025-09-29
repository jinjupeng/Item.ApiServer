<template>
  <el-dialog
    v-model="visible"
    title="审计日志详情"
    width="800px"
    :before-close="handleClose"
  >
    <div v-if="auditLog" class="audit-detail">
      <!-- 基本信息 -->
      <el-card class="detail-card" shadow="never">
        <template #header>
          <span class="card-title">基本信息</span>
        </template>
        
        <el-descriptions :column="2" border>
          <el-descriptions-item label="日志ID">
            {{ auditLog.id }}
          </el-descriptions-item>
          
          <el-descriptions-item label="操作类型">
            <el-tag :type="getActionTagType(auditLog.action)" size="small">
              {{ getActionText(auditLog.action) }}
            </el-tag>
          </el-descriptions-item>
          
          <el-descriptions-item label="操作模块">
            {{ auditLog.module }}
          </el-descriptions-item>
          
          <el-descriptions-item label="操作结果">
            <el-tag
              :type="auditLog.result === 'Success' ? 'success' : 'danger'"
              size="small"
            >
              {{ auditLog.result === 'Success' ? '成功' : '失败' }}
            </el-tag>
          </el-descriptions-item>
          
          <el-descriptions-item label="操作时间">
            {{ formatDate(auditLog.createdAt) }}
          </el-descriptions-item>
          
          <el-descriptions-item label="耗时">
            <span v-if="auditLog.duration">{{ auditLog.duration }}ms</span>
            <span v-else>-</span>
          </el-descriptions-item>
        </el-descriptions>
      </el-card>
      
      <!-- 用户信息 -->
      <el-card class="detail-card" shadow="never">
        <template #header>
          <span class="card-title">用户信息</span>
        </template>
        
        <el-descriptions :column="2" border>
          <el-descriptions-item label="用户ID">
            {{ auditLog.userId || '-' }}
          </el-descriptions-item>
          
          <el-descriptions-item label="用户名">
            {{ auditLog.userName || '-' }}
          </el-descriptions-item>
          
          <el-descriptions-item label="IP地址">
            {{ auditLog.ipAddress || '-' }}
          </el-descriptions-item>
          
          <el-descriptions-item label="用户代理" :span="2">
            <div class="user-agent">
              {{ auditLog.userAgent || '-' }}
            </div>
          </el-descriptions-item>
        </el-descriptions>
      </el-card>
      
      <!-- 请求信息 -->
      <el-card class="detail-card" shadow="never">
        <template #header>
          <span class="card-title">请求信息</span>
        </template>
        
        <el-descriptions :column="2" border>
          <el-descriptions-item label="请求路径">
            {{ auditLog.requestPath || '-' }}
          </el-descriptions-item>
          
          <el-descriptions-item label="请求方法">
            <el-tag v-if="auditLog.requestMethod" :type="getMethodTagType(auditLog.requestMethod)" size="small">
              {{ auditLog.requestMethod }}
            </el-tag>
            <span v-else>-</span>
          </el-descriptions-item>
          
          <el-descriptions-item label="响应状态码">
            <el-tag
              v-if="auditLog.responseStatusCode"
              :type="getStatusCodeTagType(auditLog.responseStatusCode)"
              size="small"
            >
              {{ auditLog.responseStatusCode }}
            </el-tag>
            <span v-else>-</span>
          </el-descriptions-item>
          
          <el-descriptions-item label="实体类型">
            {{ auditLog.entityType || '-' }}
          </el-descriptions-item>
          
          <el-descriptions-item label="实体ID">
            {{ auditLog.entityId || '-' }}
          </el-descriptions-item>
        </el-descriptions>
      </el-card>
      
      <!-- 操作描述 -->
      <el-card class="detail-card" shadow="never">
        <template #header>
          <span class="card-title">操作描述</span>
        </template>
        
        <div class="description-content">
          {{ auditLog.description }}
        </div>
      </el-card>
      
      <!-- 错误信息 -->
      <el-card v-if="auditLog.errorMessage" class="detail-card" shadow="never">
        <template #header>
          <span class="card-title">错误信息</span>
        </template>
        
        <div class="error-content">
          {{ auditLog.errorMessage }}
        </div>
      </el-card>
      
      <!-- 请求数据 -->
      <el-card v-if="auditLog.requestData" class="detail-card" shadow="never">
        <template #header>
          <span class="card-title">请求数据</span>
        </template>
        
        <div class="json-content">
          <pre>{{ formatJson(auditLog.requestData) }}</pre>
        </div>
      </el-card>
      
      <!-- 数据变更 -->
      <el-card v-if="auditLog.oldData || auditLog.newData" class="detail-card" shadow="never">
        <template #header>
          <span class="card-title">数据变更</span>
        </template>
        
        <el-row :gutter="20">
          <el-col v-if="auditLog.oldData" :span="12">
            <div class="data-section">
              <h4>变更前数据</h4>
              <div class="json-content">
                <pre>{{ formatJson(auditLog.oldData) }}</pre>
              </div>
            </div>
          </el-col>
          
          <el-col v-if="auditLog.newData" :span="auditLog.oldData ? 12 : 24">
            <div class="data-section">
              <h4>变更后数据</h4>
              <div class="json-content">
                <pre>{{ formatJson(auditLog.newData) }}</pre>
              </div>
            </div>
          </el-col>
        </el-row>
      </el-card>
    </div>
    
    <template #footer>
      <el-button @click="handleClose">关闭</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { AuditLog } from '@/types'
import dayjs from 'dayjs'

interface Props {
  modelValue: boolean
  auditLog: AuditLog | null
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const visible = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

// 关闭对话框
const handleClose = () => {
  visible.value = false
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

// 获取HTTP方法标签类型
const getMethodTagType = (method: string) => {
  switch (method.toUpperCase()) {
    case 'GET':
      return 'primary'
    case 'POST':
      return 'success'
    case 'PUT':
      return 'warning'
    case 'DELETE':
      return 'danger'
    case 'PATCH':
      return 'info'
    default:
      return 'info'
  }
}

// 获取状态码标签类型
const getStatusCodeTagType = (statusCode: number) => {
  if (statusCode >= 200 && statusCode < 300) {
    return 'success'
  } else if (statusCode >= 300 && statusCode < 400) {
    return 'info'
  } else if (statusCode >= 400 && statusCode < 500) {
    return 'warning'
  } else if (statusCode >= 500) {
    return 'danger'
  }
  return 'info'
}

// 格式化日期
const formatDate = (date: string) => {
  return dayjs(date).format('YYYY-MM-DD HH:mm:ss')
}

// 格式化JSON
const formatJson = (jsonString: string) => {
  try {
    const obj = JSON.parse(jsonString)
    return JSON.stringify(obj, null, 2)
  } catch {
    return jsonString
  }
}
</script>

<style lang="scss" scoped>
.audit-detail {
  .detail-card {
    margin-bottom: 16px;
    
    &:last-child {
      margin-bottom: 0;
    }
    
    .card-title {
      font-weight: 600;
      color: #303133;
    }
  }
  
  .user-agent {
    word-break: break-all;
    line-height: 1.5;
  }
  
  .description-content {
    padding: 12px;
    background-color: #f5f7fa;
    border-radius: 4px;
    line-height: 1.6;
  }
  
  .error-content {
    padding: 12px;
    background-color: #fef0f0;
    border: 1px solid #fbc4c4;
    border-radius: 4px;
    color: #f56c6c;
    line-height: 1.6;
  }
  
  .json-content {
    background-color: #f5f7fa;
    border: 1px solid #dcdfe6;
    border-radius: 4px;
    padding: 12px;
    max-height: 300px;
    overflow-y: auto;
    
    pre {
      margin: 0;
      font-family: 'Courier New', Courier, monospace;
      font-size: 12px;
      line-height: 1.4;
      color: #606266;
      white-space: pre-wrap;
      word-break: break-all;
    }
  }
  
  .data-section {
    h4 {
      margin: 0 0 12px 0;
      font-size: 14px;
      font-weight: 600;
      color: #303133;
    }
  }
}

:deep(.el-descriptions__label) {
  font-weight: 600;
}
</style>
