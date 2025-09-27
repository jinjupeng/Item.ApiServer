<template>
  <div class="dashboard">
    <!-- 欢迎区域 -->
    <div class="welcome-section">
      <el-card class="welcome-card">
        <div class="welcome-content">
          <div class="welcome-info">
            <h2 class="welcome-title">
              {{ getGreeting() }}，{{ authStore.nickname || authStore.username }}！
            </h2>
            <p class="welcome-desc">
              欢迎使用 {{ appStore.settings.title }}，今天是个美好的一天
            </p>
            <div class="welcome-stats">
              <div class="stat-item">
                <span class="stat-label">上次登录时间</span>
                <span class="stat-value">{{ formatDate(new Date()) }}</span>
              </div>
              <div class="stat-item">
                <span class="stat-label">当前角色</span>
                <span class="stat-value">{{ authStore.userRoles.join(', ') || '暂无角色' }}</span>
              </div>
            </div>
          </div>
          <div class="welcome-avatar">
            <el-avatar
              :size="80"
              :src="authStore.avatar"
              :style="{ backgroundColor: getAvatarColor(authStore.nickname || authStore.username || '') }"
            >
              {{ (authStore.nickname || authStore.username || '').charAt(0).toUpperCase() }}
            </el-avatar>
          </div>
        </div>
      </el-card>
    </div>

    <!-- 统计卡片 -->
    <div class="stats-section">
      <el-row :gutter="20">
        <el-col :xs="12" :sm="6" :md="6" :lg="6" :xl="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-icon user">
                <el-icon><User /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-number">{{ stats.userCount }}</div>
                <div class="stat-title">用户总数</div>
              </div>
            </div>
          </el-card>
        </el-col>
        
        <el-col :xs="12" :sm="6" :md="6" :lg="6" :xl="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-icon role">
                <el-icon><UserFilled /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-number">{{ stats.roleCount }}</div>
                <div class="stat-title">角色总数</div>
              </div>
            </div>
          </el-card>
        </el-col>
        
        <el-col :xs="12" :sm="6" :md="6" :lg="6" :xl="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-icon menu">
                <el-icon><Menu /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-number">{{ stats.menuCount }}</div>
                <div class="stat-title">菜单总数</div>
              </div>
            </div>
          </el-card>
        </el-col>
        
        <el-col :xs="12" :sm="6" :md="6" :lg="6" :xl="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-icon org">
                <el-icon><OfficeBuilding /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-number">{{ stats.orgCount }}</div>
                <div class="stat-title">组织总数</div>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>
    </div>

    <!-- 图表区域 -->
    <div class="charts-section">
      <el-row :gutter="20">
        <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
          <el-card class="chart-card">
            <template #header>
              <div class="card-header">
                <span class="title">用户增长趋势</span>
                <el-button-group size="small">
                  <el-button :type="chartPeriod === 'week' ? 'primary' : ''" @click="chartPeriod = 'week'">周</el-button>
                  <el-button :type="chartPeriod === 'month' ? 'primary' : ''" @click="chartPeriod = 'month'">月</el-button>
                  <el-button :type="chartPeriod === 'year' ? 'primary' : ''" @click="chartPeriod = 'year'">年</el-button>
                </el-button-group>
              </div>
            </template>
            <div ref="userChartRef" class="chart-container"></div>
          </el-card>
        </el-col>
        
        <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
          <el-card class="chart-card">
            <template #header>
              <div class="card-header">
                <span class="title">系统访问统计</span>
              </div>
            </template>
            <div ref="accessChartRef" class="chart-container"></div>
          </el-card>
        </el-col>
      </el-row>
    </div>

    <!-- 快捷操作和最近活动 -->
    <div class="bottom-section">
      <el-row :gutter="20">
        <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
          <el-card class="quick-actions-card">
            <template #header>
              <div class="card-header">
                <span class="title">快捷操作</span>
              </div>
            </template>
            <div class="quick-actions">
              <div class="action-item" @click="$router.push('/system/users')">
                <el-icon><User /></el-icon>
                <span>用户管理</span>
              </div>
              <div class="action-item" @click="$router.push('/system/roles')">
                <el-icon><UserFilled /></el-icon>
                <span>角色管理</span>
              </div>
              <div class="action-item" @click="$router.push('/system/menus')">
                <el-icon><Menu /></el-icon>
                <span>菜单管理</span>
              </div>
              <div class="action-item" @click="$router.push('/system/organizations')">
                <el-icon><OfficeBuilding /></el-icon>
                <span>组织管理</span>
              </div>
            </div>
          </el-card>
        </el-col>
        
        <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
          <el-card class="recent-activities-card">
            <template #header>
              <div class="card-header">
                <span class="title">最近活动</span>
                <el-link type="primary" @click="viewAllActivities">查看全部</el-link>
              </div>
            </template>
            <div class="activities-list">
              <div v-for="activity in recentActivities" :key="activity.id" class="activity-item">
                <div class="activity-avatar">
                  <el-avatar :size="32" :style="{ backgroundColor: getAvatarColor(activity.user) }">
                    {{ activity.user.charAt(0).toUpperCase() }}
                  </el-avatar>
                </div>
                <div class="activity-content">
                  <div class="activity-text">
                    <strong>{{ activity.user }}</strong> {{ activity.action }}
                  </div>
                  <div class="activity-time">{{ formatDate(activity.time, 'MM-DD HH:mm') }}</div>
                </div>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, onUnmounted, nextTick } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useAppStore } from '@/stores/app'
import { formatDate, getAvatarColor } from '@/utils'
import { User, UserFilled, Menu, OfficeBuilding } from '@element-plus/icons-vue'
import * as echarts from 'echarts'

const authStore = useAuthStore()
const appStore = useAppStore()

const userChartRef = ref()
const accessChartRef = ref()
const chartPeriod = ref('month')

let userChart: echarts.ECharts | null = null
let accessChart: echarts.ECharts | null = null

// 统计数据
const stats = reactive({
  userCount: 1234,
  roleCount: 15,
  menuCount: 48,
  orgCount: 23
})

// 最近活动
const recentActivities = ref([
  {
    id: 1,
    user: '张三',
    action: '创建了新用户',
    time: new Date(Date.now() - 1000 * 60 * 30)
  },
  {
    id: 2,
    user: '李四',
    action: '修改了角色权限',
    time: new Date(Date.now() - 1000 * 60 * 60 * 2)
  },
  {
    id: 3,
    user: '王五',
    action: '登录了系统',
    time: new Date(Date.now() - 1000 * 60 * 60 * 4)
  },
  {
    id: 4,
    user: '赵六',
    action: '更新了菜单配置',
    time: new Date(Date.now() - 1000 * 60 * 60 * 6)
  }
])

// 获取问候语
const getGreeting = () => {
  const hour = new Date().getHours()
  if (hour < 9) return '早上好'
  if (hour < 12) return '上午好'
  if (hour < 18) return '下午好'
  return '晚上好'
}

// 初始化用户增长图表
const initUserChart = () => {
  if (!userChartRef.value) return
  
  userChart = echarts.init(userChartRef.value)
  
  const option = {
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'cross'
      }
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月']
    },
    yAxis: {
      type: 'value'
    },
    series: [
      {
        name: '新增用户',
        type: 'line',
        smooth: true,
        data: [120, 132, 101, 134, 90, 230, 210, 182, 191, 234, 290, 330],
        itemStyle: {
          color: '#409EFF'
        },
        areaStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: 'rgba(64, 158, 255, 0.3)' },
            { offset: 1, color: 'rgba(64, 158, 255, 0.1)' }
          ])
        }
      }
    ]
  }
  
  userChart.setOption(option)
}

// 初始化访问统计图表
const initAccessChart = () => {
  if (!accessChartRef.value) return
  
  accessChart = echarts.init(accessChartRef.value)
  
  const option = {
    tooltip: {
      trigger: 'item'
    },
    legend: {
      orient: 'vertical',
      left: 'left'
    },
    series: [
      {
        name: '访问来源',
        type: 'pie',
        radius: '50%',
        data: [
          { value: 1048, name: 'PC端' },
          { value: 735, name: '移动端' },
          { value: 580, name: '平板端' },
          { value: 484, name: 'API调用' }
        ],
        emphasis: {
          itemStyle: {
            shadowBlur: 10,
            shadowOffsetX: 0,
            shadowColor: 'rgba(0, 0, 0, 0.5)'
          }
        }
      }
    ]
  }
  
  accessChart.setOption(option)
}

// 查看全部活动
const viewAllActivities = () => {
  // 跳转到活动日志页面
  console.log('查看全部活动')
}

// 窗口大小变化处理
const handleResize = () => {
  userChart?.resize()
  accessChart?.resize()
}

onMounted(async () => {
  await nextTick()
  initUserChart()
  initAccessChart()
  window.addEventListener('resize', handleResize)
})

onUnmounted(() => {
  userChart?.dispose()
  accessChart?.dispose()
  window.removeEventListener('resize', handleResize)
})
</script>

<style lang="scss" scoped>
.dashboard {
  padding: 0;

  .welcome-section {
    margin-bottom: 20px;

    .welcome-card {
      border-radius: 12px;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      border: none;
      color: white;

      :deep(.el-card__body) {
        padding: 30px;
      }

      .welcome-content {
        display: flex;
        justify-content: space-between;
        align-items: center;

        .welcome-info {
          flex: 1;

          .welcome-title {
            font-size: 24px;
            font-weight: 600;
            margin: 0 0 8px 0;
            color: white;
          }

          .welcome-desc {
            font-size: 14px;
            margin: 0 0 20px 0;
            color: rgba(255, 255, 255, 0.8);
          }

          .welcome-stats {
            display: flex;
            gap: 40px;

            .stat-item {
              display: flex;
              flex-direction: column;
              gap: 4px;

              .stat-label {
                font-size: 12px;
                color: rgba(255, 255, 255, 0.7);
              }

              .stat-value {
                font-size: 14px;
                font-weight: 500;
                color: white;
              }
            }
          }
        }

        .welcome-avatar {
          :deep(.el-avatar) {
            border: 3px solid rgba(255, 255, 255, 0.2);
          }
        }
      }
    }
  }

  .stats-section {
    margin-bottom: 20px;

    .stat-card {
      border-radius: 12px;
      transition: all var(--el-transition-duration);

      &:hover {
        transform: translateY(-2px);
        box-shadow: var(--el-box-shadow-light);
      }

      .stat-content {
        display: flex;
        align-items: center;
        gap: 16px;

        .stat-icon {
          width: 60px;
          height: 60px;
          border-radius: 12px;
          display: flex;
          align-items: center;
          justify-content: center;
          font-size: 24px;
          color: white;

          &.user {
            background: linear-gradient(135deg, #667eea, #764ba2);
          }

          &.role {
            background: linear-gradient(135deg, #f093fb, #f5576c);
          }

          &.menu {
            background: linear-gradient(135deg, #4facfe, #00f2fe);
          }

          &.org {
            background: linear-gradient(135deg, #43e97b, #38f9d7);
          }
        }

        .stat-info {
          flex: 1;

          .stat-number {
            font-size: 28px;
            font-weight: 600;
            color: var(--el-text-color-primary);
            line-height: 1;
            margin-bottom: 4px;
          }

          .stat-title {
            font-size: 14px;
            color: var(--el-text-color-secondary);
          }
        }
      }
    }
  }

  .charts-section {
    margin-bottom: 20px;

    .chart-card {
      border-radius: 12px;

      .chart-container {
        height: 300px;
      }
    }
  }

  .bottom-section {
    .quick-actions-card,
    .recent-activities-card {
      border-radius: 12px;
    }

    .quick-actions {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 16px;

      .action-item {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 8px;
        padding: 20px;
        border-radius: 8px;
        background-color: var(--el-fill-color-lighter);
        cursor: pointer;
        transition: all var(--el-transition-duration);

        &:hover {
          background-color: var(--el-color-primary-light-9);
          color: var(--el-color-primary);
          transform: translateY(-2px);
        }

        .el-icon {
          font-size: 24px;
        }

        span {
          font-size: 14px;
          font-weight: 500;
        }
      }
    }

    .activities-list {
      .activity-item {
        display: flex;
        align-items: center;
        gap: 12px;
        padding: 12px 0;
        border-bottom: 1px solid var(--el-border-color-lighter);

        &:last-child {
          border-bottom: none;
        }

        .activity-content {
          flex: 1;

          .activity-text {
            font-size: 14px;
            color: var(--el-text-color-primary);
            margin-bottom: 4px;
          }

          .activity-time {
            font-size: 12px;
            color: var(--el-text-color-secondary);
          }
        }
      }
    }
  }

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;

    .title {
      font-size: 16px;
      font-weight: 600;
      color: var(--el-text-color-primary);
    }
  }
}

// 移动端适配
@media (max-width: 768px) {
  .dashboard {
    .welcome-section {
      .welcome-card {
        .welcome-content {
          flex-direction: column;
          text-align: center;
          gap: 20px;

          .welcome-stats {
            justify-content: center;
            gap: 20px;
          }
        }
      }
    }

    .charts-section {
      .chart-container {
        height: 250px;
      }
    }

    .bottom-section {
      .quick-actions {
        grid-template-columns: repeat(2, 1fr);
        gap: 12px;

        .action-item {
          padding: 16px;
        }
      }
    }
  }
}
</style>
