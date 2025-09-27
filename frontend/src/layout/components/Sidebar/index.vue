<template>
  <div class="sidebar">
    <!-- Logo区域 -->
    <div class="logo-container" :class="{ 'is-collapsed': appStore.sidebarCollapsed }">
      <router-link to="/" class="logo-link">
        <img src="/logo.png" alt="Logo" class="logo-image" />
        <span v-if="!appStore.sidebarCollapsed" class="logo-text">
          {{ appStore.settings.title }}
        </span>
      </router-link>
    </div>

    <!-- 菜单区域 -->
    <div class="menu-container">
      <el-scrollbar>
        <el-menu
          :default-active="activeMenu"
          :collapse="appStore.sidebarCollapsed"
          :unique-opened="true"
          :collapse-transition="false"
          mode="vertical"
          @select="handleMenuSelect"
        >
          <SidebarItem
            v-for="route in menuRoutes"
            :key="route.path"
            :item="route"
            :base-path="route.path"
          />
        </el-menu>
      </el-scrollbar>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAppStore } from '@/stores/app'
import { useAuthStore } from '@/stores/auth'
import SidebarItem from './SidebarItem.vue'

const route = useRoute()
const router = useRouter()
const appStore = useAppStore()
const authStore = useAuthStore()

// 当前激活的菜单
const activeMenu = computed(() => {
  const { meta, path } = route
  if (meta?.activeMenu) {
    return meta.activeMenu as string
  }
  return path
})

// 菜单路由（过滤掉隐藏的路由）
const menuRoutes = computed(() => {
  return router.getRoutes().filter(route => {
    return !route.meta?.hidden && route.children && route.children.length > 0
  })
})

// 菜单选择处理
const handleMenuSelect = (index: string) => {
  if (index !== route.path) {
    router.push(index)
  }
}
</script>

<style lang="scss" scoped>
.sidebar {
  height: 100%;
  display: flex;
  flex-direction: column;
  background-color: var(--el-bg-color);

  .logo-container {
    height: var(--app-header-height);
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 0 20px;
    border-bottom: 1px solid var(--el-border-color-lighter);
    transition: all var(--el-transition-duration);

    &.is-collapsed {
      padding: 0 12px;
    }

    .logo-link {
      display: flex;
      align-items: center;
      text-decoration: none;
      color: var(--el-text-color-primary);
      font-weight: 600;
      font-size: 18px;

      .logo-image {
        width: 32px;
        height: 32px;
        margin-right: 12px;
        border-radius: 6px;
      }

      .logo-text {
        white-space: nowrap;
        overflow: hidden;
        transition: opacity var(--el-transition-duration);
      }

      &:hover {
        color: var(--el-color-primary);
      }
    }
  }

  .menu-container {
    flex: 1;
    overflow: hidden;

    :deep(.el-scrollbar__view) {
      height: 100%;
    }

    .el-menu {
      border-right: none;
      height: 100%;
      background-color: transparent;

      .el-menu-item,
      .el-sub-menu__title {
        height: 48px;
        line-height: 48px;
        border-radius: 6px;
        margin: 2px 8px;
        transition: all var(--el-transition-duration);

        &:hover {
          background-color: var(--el-color-primary-light-9);
          color: var(--el-color-primary);
        }

        &.is-active {
          background-color: var(--el-color-primary-light-8);
          color: var(--el-color-primary);
          font-weight: 600;
          position: relative;

          &::after {
            content: '';
            position: absolute;
            right: 8px;
            top: 50%;
            transform: translateY(-50%);
            width: 3px;
            height: 20px;
            background-color: var(--el-color-primary);
            border-radius: 2px;
          }
        }

        .el-icon {
          margin-right: 8px;
          font-size: 18px;
        }
      }

      .el-sub-menu {
        .el-menu {
          background-color: transparent;

          .el-menu-item {
            padding-left: 48px !important;
            margin: 1px 8px;

            &.is-active {
              background-color: var(--el-color-primary-light-9);
              
              &::after {
                display: none;
              }
            }
          }
        }

        .el-sub-menu__title {
          .el-sub-menu__icon-arrow {
            transition: transform var(--el-transition-duration);
          }

          &:hover {
            .el-sub-menu__icon-arrow {
              color: var(--el-color-primary);
            }
          }
        }

        &.is-opened {
          .el-sub-menu__title {
            .el-sub-menu__icon-arrow {
              transform: rotateZ(90deg);
            }
          }
        }
      }

      // 收起状态下的样式
      &.el-menu--collapse {
        .el-menu-item,
        .el-sub-menu__title {
          padding: 0 !important;
          text-align: center;

          .el-icon {
            margin-right: 0;
          }

          span {
            display: none;
          }
        }

        .el-sub-menu {
          .el-sub-menu__title {
            .el-sub-menu__icon-arrow {
              display: none;
            }
          }
        }
      }
    }
  }
}

// 暗色主题适配
html.dark {
  .sidebar {
    .logo-container {
      border-bottom-color: var(--el-border-color);
    }
  }
}
</style>
