<template>
  <div class="dashboard">
    <div class="dashboard-header">
      <h1>仪表盘</h1>
      <p>欢迎使用RBAC权限管理系统</p>
    </div>
    
    <!-- 统计卡片 -->
    <el-row :gutter="20" class="stats-cards">
      <el-col :xs="24" :sm="12" :md="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon user">
              <el-icon><User /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ stats.userCount }}</div>
              <div class="stat-label">用户总数</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :xs="24" :sm="12" :md="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon role">
              <el-icon><UserFilled /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ stats.roleCount }}</div>
              <div class="stat-label">角色总数</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :xs="24" :sm="12" :md="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon menu">
              <el-icon><Menu /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ stats.menuCount }}</div>
              <div class="stat-label">菜单总数</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :xs="24" :sm="12" :md="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon online">
              <el-icon><Connection /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ stats.onlineCount }}</div>
              <div class="stat-label">在线用户</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    
    <!-- 快捷操作 -->
    <el-row :gutter="20" class="quick-actions">
      <el-col :xs="24" :md="12">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>快捷操作</span>
            </div>
          </template>
          
          <div class="action-buttons">
            <el-button
              v-if="authStore.hasPermission('system:user:create')"
              type="primary"
              icon="Plus"
              @click="$router.push('/system/users')"
            >
              新增用户
            </el-button>
            
            <el-button
              v-if="authStore.hasPermission('system:role:create')"
              type="success"
              icon="Plus"
              @click="$router.push('/system/roles')"
            >
              新增角色
            </el-button>
            
            <el-button
              v-if="authStore.hasPermission('system:menu:create')"
              type="warning"
              icon="Plus"
              @click="$router.push('/system/menus')"
            >
              新增菜单
            </el-button>
          </div>
        </el-card>
      </el-col>
      
      <el-col :xs="24" :md="12">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>系统信息</span>
            </div>
          </template>
          
          <div class="system-info">
            <div class="info-item">
              <span class="label">当前用户：</span>
              <span class="value">{{ authStore.userInfo?.realName }}</span>
            </div>
            <div class="info-item">
              <span class="label">用户角色：</span>
              <span class="value">
                <el-tag
                  v-for="role in authStore.userRoles"
                  :key="role.id"
                  size="small"
                  class="role-tag"
                >
                  {{ role.name }}
                </el-tag>
              </span>
            </div>
            <div class="info-item">
              <span class="label">登录时间：</span>
              <span class="value">{{ formatDate(new Date()) }}</span>
            </div>
            <div class="info-item">
              <span class="label">系统版本：</span>
              <span class="value">v1.0.0</span>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { User, UserFilled, Menu, Connection } from '@element-plus/icons-vue'
import dayjs from 'dayjs'

const authStore = useAuthStore()

// 统计数据
const stats = ref({
  userCount: 0,
  roleCount: 0,
  menuCount: 0,
  onlineCount: 0
})

// 格式化日期
const formatDate = (date: Date) => {
  return dayjs(date).format('YYYY-MM-DD HH:mm:ss')
}

// 获取统计数据
const getStats = async () => {
  // 这里可以调用API获取真实的统计数据
  // 暂时使用模拟数据
  stats.value = {
    userCount: 156,
    roleCount: 8,
    menuCount: 24,
    onlineCount: 12
  }
}

onMounted(() => {
  getStats()
})
</script>

<style lang="scss" scoped>
.dashboard {
  .dashboard-header {
    margin-bottom: 24px;
    
    h1 {
      font-size: 24px;
      font-weight: 600;
      color: #333;
      margin: 0 0 8px 0;
    }
    
    p {
      color: #666;
      margin: 0;
    }
  }
  
  .stats-cards {
    margin-bottom: 24px;
    
    .stat-card {
      .stat-content {
        display: flex;
        align-items: center;
        
        .stat-icon {
          width: 60px;
          height: 60px;
          border-radius: 8px;
          display: flex;
          align-items: center;
          justify-content: center;
          margin-right: 16px;
          
          .el-icon {
            font-size: 24px;
            color: #fff;
          }
          
          &.user {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          }
          
          &.role {
            background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
          }
          
          &.menu {
            background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
          }
          
          &.online {
            background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%);
          }
        }
        
        .stat-info {
          flex: 1;
          
          .stat-number {
            font-size: 28px;
            font-weight: 600;
            color: #333;
            line-height: 1;
            margin-bottom: 4px;
          }
          
          .stat-label {
            font-size: 14px;
            color: #666;
          }
        }
      }
    }
  }
  
  .quick-actions {
    .card-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
    }
    
    .action-buttons {
      display: flex;
      flex-wrap: wrap;
      gap: 12px;
    }
    
    .system-info {
      .info-item {
        display: flex;
        align-items: center;
        margin-bottom: 12px;
        
        &:last-child {
          margin-bottom: 0;
        }
        
        .label {
          width: 80px;
          color: #666;
          font-size: 14px;
        }
        
        .value {
          flex: 1;
          color: #333;
          font-size: 14px;
          
          .role-tag {
            margin-right: 4px;
          }
        }
      }
    }
  }
}

// 响应式设计
@media (max-width: 768px) {
  .dashboard {
    .stats-cards {
      .stat-card {
        margin-bottom: 16px;
      }
    }
    
    .quick-actions {
      .action-buttons {
        flex-direction: column;
        
        .el-button {
          width: 100%;
        }
      }
    }
  }
}
</style>
