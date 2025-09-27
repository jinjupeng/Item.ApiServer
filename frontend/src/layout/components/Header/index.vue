<template>
  <div class="app-header">
    <div class="header-left">
      <!-- 菜单折叠按钮 -->
      <div class="hamburger-container" @click="appStore.toggleSidebar">
        <el-icon class="hamburger" :class="{ 'is-active': !appStore.sidebarCollapsed }">
          <Fold v-if="!appStore.sidebarCollapsed" />
          <Expand v-else />
        </el-icon>
      </div>

      <!-- 面包屑导航 -->
      <div v-if="!appStore.settings.showBreadcrumb" class="breadcrumb-container">
        <AppBreadcrumb />
      </div>
    </div>

    <div class="header-right">
      <!-- 全屏按钮 -->
      <div class="header-item" @click="toggleFullscreen">
        <el-tooltip content="全屏" placement="bottom">
          <el-icon>
            <FullScreen v-if="!isFullscreen" />
            <Aim v-else />
          </el-icon>
        </el-tooltip>
      </div>

      <!-- 主题切换 -->
      <div class="header-item" @click="appStore.toggleTheme">
        <el-tooltip :content="appStore.isDark ? '切换到亮色主题' : '切换到暗色主题'" placement="bottom">
          <el-icon>
            <Sunny v-if="appStore.isDark" />
            <Moon v-else />
          </el-icon>
        </el-tooltip>
      </div>

      <!-- 消息通知 -->
      <div class="header-item">
        <el-badge :value="notificationCount" :hidden="notificationCount === 0">
          <el-tooltip content="消息通知" placement="bottom">
            <el-icon>
              <Bell />
            </el-icon>
          </el-tooltip>
        </el-badge>
      </div>

      <!-- 用户信息 -->
      <el-dropdown class="user-dropdown" @command="handleUserCommand">
        <div class="user-info">
          <el-avatar
            :size="32"
            :src="authStore.avatar"
            :style="{ backgroundColor: getAvatarColor(authStore.nickname || authStore.username || '') }"
          >
            {{ (authStore.nickname || authStore.username || '').charAt(0).toUpperCase() }}
          </el-avatar>
          <span class="username">{{ authStore.nickname || authStore.username }}</span>
          <el-icon class="dropdown-icon">
            <ArrowDown />
          </el-icon>
        </div>
        
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="profile">
              <el-icon><User /></el-icon>
              个人中心
            </el-dropdown-item>
            <el-dropdown-item command="settings">
              <el-icon><Setting /></el-icon>
              系统设置
            </el-dropdown-item>
            <el-dropdown-item divided command="logout">
              <el-icon><SwitchButton /></el-icon>
              退出登录
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessageBox } from 'element-plus'
import { useAppStore } from '@/stores/app'
import { useAuthStore } from '@/stores/auth'
import { getAvatarColor } from '@/utils'
import AppBreadcrumb from '../Breadcrumb/index.vue'
import {
  Fold,
  Expand,
  FullScreen,
  Aim,
  Sunny,
  Moon,
  Bell,
  User,
  Setting,
  SwitchButton,
  ArrowDown
} from '@element-plus/icons-vue'

const router = useRouter()
const appStore = useAppStore()
const authStore = useAuthStore()

// 全屏状态
const isFullscreen = ref(false)

// 通知数量
const notificationCount = ref(3)

// 切换全屏
const toggleFullscreen = () => {
  if (!document.fullscreenElement) {
    document.documentElement.requestFullscreen()
    isFullscreen.value = true
  } else {
    document.exitFullscreen()
    isFullscreen.value = false
  }
}

// 监听全屏状态变化
const handleFullscreenChange = () => {
  isFullscreen.value = !!document.fullscreenElement
}

// 用户下拉菜单命令处理
const handleUserCommand = async (command: string) => {
  switch (command) {
    case 'profile':
      router.push('/profile')
      break
    case 'settings':
      // 打开设置对话框
      break
    case 'logout':
      try {
        await ElMessageBox.confirm('确定要退出登录吗？', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        })
        await authStore.logout()
      } catch (error) {
        // 用户取消
      }
      break
  }
}

onMounted(() => {
  document.addEventListener('fullscreenchange', handleFullscreenChange)
})

onUnmounted(() => {
  document.removeEventListener('fullscreenchange', handleFullscreenChange)
})
</script>

<style lang="scss" scoped>
.app-header {
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 20px;
  background-color: var(--el-bg-color);

  .header-left {
    display: flex;
    align-items: center;

    .hamburger-container {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 40px;
      height: 40px;
      border-radius: 6px;
      cursor: pointer;
      transition: all var(--el-transition-duration);

      &:hover {
        background-color: var(--el-fill-color-light);
      }

      .hamburger {
        font-size: 18px;
        color: var(--el-text-color-primary);
        transition: transform var(--el-transition-duration);

        &.is-active {
          transform: rotate(180deg);
        }
      }
    }

    .breadcrumb-container {
      margin-left: 20px;
    }
  }

  .header-right {
    display: flex;
    align-items: center;
    gap: 16px;

    .header-item {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 40px;
      height: 40px;
      border-radius: 6px;
      cursor: pointer;
      transition: all var(--el-transition-duration);

      &:hover {
        background-color: var(--el-fill-color-light);
      }

      .el-icon {
        font-size: 18px;
        color: var(--el-text-color-primary);
      }
    }

    .user-dropdown {
      .user-info {
        display: flex;
        align-items: center;
        padding: 8px 12px;
        border-radius: 6px;
        cursor: pointer;
        transition: all var(--el-transition-duration);

        &:hover {
          background-color: var(--el-fill-color-light);
        }

        .username {
          margin: 0 8px;
          font-size: 14px;
          font-weight: 500;
          color: var(--el-text-color-primary);
          white-space: nowrap;
        }

        .dropdown-icon {
          font-size: 12px;
          color: var(--el-text-color-secondary);
          transition: transform var(--el-transition-duration);
        }

        &:hover .dropdown-icon {
          transform: rotate(180deg);
        }
      }
    }
  }
}

// 移动端适配
@media (max-width: 768px) {
  .app-header {
    padding: 0 16px;

    .header-right {
      gap: 12px;

      .header-item {
        width: 36px;
        height: 36px;

        .el-icon {
          font-size: 16px;
        }
      }

      .user-dropdown .user-info {
        padding: 6px 8px;

        .username {
          display: none;
        }
      }
    }
  }
}
</style>
