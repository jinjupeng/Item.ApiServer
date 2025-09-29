<template>
  <el-dialog
    v-model="visible"
    title="审计日志统计信息"
    width="900px"
    :before-close="handleClose"
  >
    <div class="statistics-container">
      <!-- 时间范围选择 -->
      <div class="date-range-selector">
        <el-date-picker
          v-model="dateRange"
          type="datetimerange"
          range-separator="至"
          start-placeholder="开始时间"
          end-placeholder="结束时间"
          format="YYYY-MM-DD HH:mm:ss"
          value-format="YYYY-MM-DD HH:mm:ss"
          style="width: 350px"
          @change="loadStatistics"
        />
        <el-button type="primary" @click="loadStatistics" :loading="loading">
          刷新统计
        </el-button>
      </div>
      
      <div v-if="statistics" class="statistics-content">
        <!-- 总体统计 -->
        <el-row :gutter="20" class="summary-cards">
          <el-col :span="6">
            <el-card class="stat-card">
              <div class="stat-item">
                <div class="stat-value total">{{ statistics.totalOperations }}</div>
                <div class="stat-label">总操作数</div>
              </div>
            </el-card>
          </el-col>
          
          <el-col :span="6">
            <el-card class="stat-card">
              <div class="stat-item">
                <div class="stat-value success">{{ statistics.successOperations }}</div>
                <div class="stat-label">成功操作</div>
              </div>
            </el-card>
          </el-col>
          
          <el-col :span="6">
            <el-card class="stat-card">
              <div class="stat-item">
                <div class="stat-value failed">{{ statistics.failedOperations }}</div>
                <div class="stat-label">失败操作</div>
              </div>
            </el-card>
          </el-col>
          
          <el-col :span="6">
            <el-card class="stat-card">
              <div class="stat-item">
                <div class="stat-value rate">{{ successRate }}%</div>
                <div class="stat-label">成功率</div>
              </div>
            </el-card>
          </el-col>
        </el-row>
        
        <!-- 详细统计 -->
        <el-row :gutter="20" class="detail-stats">
          <!-- 操作类型统计 -->
          <el-col :span="12">
            <el-card>
              <template #header>
                <span class="card-title">操作类型统计</span>
              </template>
              
              <div class="chart-container">
                <div v-if="Object.keys(statistics.actionStatistics).length === 0" class="no-data">
                  暂无数据
                </div>
                <div v-else class="stat-list">
                  <div
                    v-for="(count, action) in statistics.actionStatistics"
                    :key="action"
                    class="stat-list-item"
                  >
                    <div class="stat-list-label">
                      <el-tag :type="getActionTagType(action)" size="small">
                        {{ getActionText(action) }}
                      </el-tag>
                    </div>
                    <div class="stat-list-value">{{ count }}</div>
                    <div class="stat-list-bar">
                      <div
                        class="stat-list-bar-fill"
                        :style="{ width: getPercentage(count, statistics.totalOperations) + '%' }"
                      ></div>
                    </div>
                  </div>
                </div>
              </div>
            </el-card>
          </el-col>
          
          <!-- 模块统计 -->
          <el-col :span="12">
            <el-card>
              <template #header>
                <span class="card-title">模块统计</span>
              </template>
              
              <div class="chart-container">
                <div v-if="Object.keys(statistics.moduleStatistics).length === 0" class="no-data">
                  暂无数据
                </div>
                <div v-else class="stat-list">
                  <div
                    v-for="(count, module) in statistics.moduleStatistics"
                    :key="module"
                    class="stat-list-item"
                  >
                    <div class="stat-list-label">{{ module }}</div>
                    <div class="stat-list-value">{{ count }}</div>
                    <div class="stat-list-bar">
                      <div
                        class="stat-list-bar-fill"
                        :style="{ width: getPercentage(count, statistics.totalOperations) + '%' }"
                      ></div>
                    </div>
                  </div>
                </div>
              </div>
            </el-card>
          </el-col>
        </el-row>
        
        <el-row :gutter="20" class="detail-stats">
          <!-- 用户统计 -->
          <el-col :span="12">
            <el-card>
              <template #header>
                <span class="card-title">用户操作统计（Top 10）</span>
              </template>
              
              <div class="chart-container">
                <div v-if="Object.keys(statistics.userStatistics).length === 0" class="no-data">
                  暂无数据
                </div>
                <div v-else class="stat-list">
                  <div
                    v-for="(count, user) in topUserStatistics"
                    :key="user"
                    class="stat-list-item"
                  >
                    <div class="stat-list-label">{{ user || '匿名用户' }}</div>
                    <div class="stat-list-value">{{ count }}</div>
                    <div class="stat-list-bar">
                      <div
                        class="stat-list-bar-fill"
                        :style="{ width: getPercentage(count, statistics.totalOperations) + '%' }"
                      ></div>
                    </div>
                  </div>
                </div>
              </div>
            </el-card>
          </el-col>
          
          <!-- 日期统计 -->
          <el-col :span="12">
            <el-card>
              <template #header>
                <span class="card-title">日期统计</span>
              </template>
              
              <div class="chart-container">
                <div v-if="Object.keys(statistics.dateStatistics).length === 0" class="no-data">
                  暂无数据
                </div>
                <div v-else class="stat-list">
                  <div
                    v-for="(count, date) in sortedDateStatistics"
                    :key="date"
                    class="stat-list-item"
                  >
                    <div class="stat-list-label">{{ date }}</div>
                    <div class="stat-list-value">{{ count }}</div>
                    <div class="stat-list-bar">
                      <div
                        class="stat-list-bar-fill"
                        :style="{ width: getPercentage(count, Math.max(...Object.values(statistics.dateStatistics))) + '%' }"
                      ></div>
                    </div>
                  </div>
                </div>
              </div>
            </el-card>
          </el-col>
        </el-row>
      </div>
      
      <div v-else-if="!loading" class="no-statistics">
        <el-empty description="暂无统计数据" />
      </div>
      
      <div v-if="loading" class="loading-container">
        <el-skeleton :rows="8" animated />
      </div>
    </div>
    
    <template #footer>
      <el-button @click="handleClose">关闭</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { auditLogsApi } from '@/api/auditLogs'
import type { AuditLogStatistics } from '@/types'

interface Props {
  modelValue: boolean
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

const loading = ref(false)
const statistics = ref<AuditLogStatistics | null>(null)
const dateRange = ref<[string, string] | null>(null)

// 成功率
const successRate = computed(() => {
  if (!statistics.value || statistics.value.totalOperations === 0) {
    return 0
  }
  return Math.round((statistics.value.successOperations / statistics.value.totalOperations) * 100)
})

// Top 10 用户统计
const topUserStatistics = computed(() => {
  if (!statistics.value) return {}
  
  const entries = Object.entries(statistics.value.userStatistics)
  const sorted = entries.sort(([, a], [, b]) => b - a)
  const top10 = sorted.slice(0, 10)
  
  return Object.fromEntries(top10)
})

// 排序后的日期统计
const sortedDateStatistics = computed(() => {
  if (!statistics.value) return {}
  
  const entries = Object.entries(statistics.value.dateStatistics)
  const sorted = entries.sort(([a], [b]) => b.localeCompare(a))
  
  return Object.fromEntries(sorted)
})

// 监听对话框显示状态
watch(visible, (newValue) => {
  if (newValue) {
    loadStatistics()
  }
})

// 加载统计信息
const loadStatistics = async () => {
  try {
    loading.value = true
    
    let startDate: string | undefined
    let endDate: string | undefined
    
    if (dateRange.value) {
      startDate = dateRange.value[0]
      endDate = dateRange.value[1]
    }
    
    const response = await auditLogsApi.getAuditLogStatistics(startDate, endDate)
    statistics.value = response.data
  } catch (error) {
    console.error('获取统计信息失败:', error)
    ElMessage.error('获取统计信息失败')
  } finally {
    loading.value = false
  }
}

// 关闭对话框
const handleClose = () => {
  visible.value = false
}

// 获取百分比
const getPercentage = (value: number, total: number) => {
  if (total === 0) return 0
  return Math.round((value / total) * 100)
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
</script>

<style lang="scss" scoped>
.statistics-container {
  .date-range-selector {
    display: flex;
    align-items: center;
    gap: 12px;
    margin-bottom: 20px;
    padding: 16px;
    background-color: #f5f7fa;
    border-radius: 6px;
  }
  
  .summary-cards {
    margin-bottom: 20px;
    
    .stat-card {
      text-align: center;
      
      .stat-item {
        .stat-value {
          font-size: 28px;
          font-weight: bold;
          margin-bottom: 8px;
          
          &.total {
            color: #409eff;
          }
          
          &.success {
            color: #67c23a;
          }
          
          &.failed {
            color: #f56c6c;
          }
          
          &.rate {
            color: #e6a23c;
          }
        }
        
        .stat-label {
          font-size: 14px;
          color: #909399;
        }
      }
    }
  }
  
  .detail-stats {
    margin-bottom: 20px;
    
    &:last-child {
      margin-bottom: 0;
    }
    
    .card-title {
      font-weight: 600;
      color: #303133;
    }
  }
  
  .chart-container {
    height: 300px;
    overflow-y: auto;
    
    .no-data {
      display: flex;
      align-items: center;
      justify-content: center;
      height: 100%;
      color: #909399;
      font-size: 14px;
    }
    
    .stat-list {
      .stat-list-item {
        display: flex;
        align-items: center;
        padding: 8px 0;
        border-bottom: 1px solid #f0f0f0;
        
        &:last-child {
          border-bottom: none;
        }
        
        .stat-list-label {
          flex: 1;
          font-size: 14px;
          color: #303133;
        }
        
        .stat-list-value {
          width: 60px;
          text-align: right;
          font-weight: 600;
          color: #409eff;
          margin-right: 12px;
        }
        
        .stat-list-bar {
          width: 100px;
          height: 6px;
          background-color: #f0f0f0;
          border-radius: 3px;
          overflow: hidden;
          
          .stat-list-bar-fill {
            height: 100%;
            background-color: #409eff;
            transition: width 0.3s ease;
          }
        }
      }
    }
  }
  
  .no-statistics {
    text-align: center;
    padding: 40px 0;
  }
  
  .loading-container {
    padding: 20px;
  }
}
</style>
