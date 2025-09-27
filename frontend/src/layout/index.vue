<template>
  <div class="app-layout" :class="{ 'is-mobile': appStore.device === 'mobile' }">
    <!-- 侧边栏 -->
    <div
      class="sidebar-container"
      :class="{ 'is-collapsed': appStore.sidebarCollapsed }"
    >
      <AppSidebar />
    </div>

    <!-- 主内容区域 -->
    <div class="main-container">
      <!-- 顶部导航栏 -->
      <div class="header-container">
        <AppHeader />
      </div>

      <!-- 标签页 -->
      <div v-if="appStore.settings.showTabs" class="tabs-container">
        <AppTabs />
      </div>

      <!-- 面包屑导航 -->
      <div v-if="appStore.settings.showBreadcrumb" class="breadcrumb-container">
        <AppBreadcrumb />
      </div>

      <!-- 页面内容 -->
      <div class="content-container">
        <router-view v-slot="{ Component, route }">
          <transition name="fade-transform" mode="out-in">
            <keep-alive :include="cachedViews">
              <component :is="Component" :key="route.path" />
            </keep-alive>
          </transition>
        </router-view>
      </div>

      <!-- 底部 -->
      <div v-if="appStore.settings.showFooter" class="footer-container">
        <AppFooter />
      </div>
    </div>

    <!-- 移动端遮罩 -->
    <div
      v-if="appStore.device === 'mobile' && !appStore.sidebarCollapsed"
      class="mobile-mask"
      @click="appStore.toggleSidebar"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted } from 'vue'
import { useAppStore } from '@/stores/app'
import AppSidebar from './components/Sidebar/index.vue'
import AppHeader from './components/Header/index.vue'
import AppTabs from './components/Tabs/index.vue'
import AppBreadcrumb from './components/Breadcrumb/index.vue'
import AppFooter from './components/Footer/index.vue'

const appStore = useAppStore()

// 缓存的视图组件
const cachedViews = computed(() => {
  return appStore.tabs.map(tab => tab.name)
})

// 监听窗口大小变化
const handleResize = () => {
  const width = window.innerWidth
  if (width < 768) {
    appStore.setDevice('mobile')
  } else {
    appStore.setDevice('desktop')
  }
}

onMounted(() => {
  handleResize()
  window.addEventListener('resize', handleResize)
})

onUnmounted(() => {
  window.removeEventListener('resize', handleResize)
})
</script>

<style lang="scss" scoped>
.app-layout {
  display: flex;
  height: 100vh;
  width: 100%;

  .sidebar-container {
    position: fixed;
    top: 0;
    left: 0;
    height: 100vh;
    width: var(--app-sidebar-width);
    background-color: var(--el-bg-color);
    border-right: 1px solid var(--el-border-color-lighter);
    transition: width var(--el-transition-duration);
    z-index: 1001;

    &.is-collapsed {
      width: var(--app-sidebar-collapsed-width);
    }
  }

  .main-container {
    flex: 1;
    margin-left: var(--app-sidebar-width);
    display: flex;
    flex-direction: column;
    transition: margin-left var(--el-transition-duration);

    .header-container {
      height: var(--app-header-height);
      background-color: var(--el-bg-color);
      border-bottom: 1px solid var(--el-border-color-lighter);
      position: sticky;
      top: 0;
      z-index: 1000;
    }

    .tabs-container {
      height: var(--app-tabs-height);
      background-color: var(--el-bg-color);
      border-bottom: 1px solid var(--el-border-color-lighter);
    }

    .breadcrumb-container {
      padding: 12px 20px;
      background-color: var(--el-bg-color);
      border-bottom: 1px solid var(--el-border-color-lighter);
    }

    .content-container {
      flex: 1;
      padding: 20px;
      background-color: var(--el-bg-color-page);
      overflow-y: auto;
      @include scrollbar;
    }

    .footer-container {
      height: var(--app-footer-height);
      background-color: var(--el-bg-color);
      border-top: 1px solid var(--el-border-color-lighter);
    }
  }

  .mobile-mask {
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    background-color: rgba(0, 0, 0, 0.3);
    z-index: 1000;
  }

  // 移动端适配
  &.is-mobile {
    .sidebar-container {
      transform: translateX(-100%);
      
      &:not(.is-collapsed) {
        transform: translateX(0);
      }
    }

    .main-container {
      margin-left: 0;
    }
  }

  // 侧边栏收起时的样式调整
  .sidebar-container.is-collapsed + .main-container {
    margin-left: var(--app-sidebar-collapsed-width);
  }
}

// 页面切换动画
.fade-transform-enter-active,
.fade-transform-leave-active {
  transition: all 0.3s;
}

.fade-transform-enter-from {
  opacity: 0;
  transform: translateX(30px);
}

.fade-transform-leave-to {
  opacity: 0;
  transform: translateX(-30px);
}
</style>
