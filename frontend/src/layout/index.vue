<template>
  <div class="app-layout">
    <el-container>
      <!-- 侧边栏 -->
      <el-aside :width="sidebarWidth" class="sidebar-container">
        <div class="logo-container">
          <img src="/vite.svg" alt="Logo" class="logo" />
          <h1 v-if="!appStore.sidebarCollapsed" class="title">RBAC管理系统</h1>
        </div>
        <Sidebar />
      </el-aside>
      
      <!-- 主内容区 -->
      <el-container>
        <!-- 顶部导航 -->
        <el-header class="header-container">
          <Navbar />
        </el-header>
        
        <!-- 内容区域 -->
        <el-main class="main-container">
          <router-view v-slot="{ Component }">
            <transition name="fade-transform" mode="out-in">
              <component :is="Component" />
            </transition>
          </router-view>
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useAppStore } from '@/stores/app'
import Sidebar from './components/Sidebar.vue'
import Navbar from './components/Navbar.vue'

const appStore = useAppStore()

const sidebarWidth = computed(() => {
  return appStore.sidebarCollapsed ? '64px' : '200px'
})
</script>

<style lang="scss" scoped>
.app-layout {
  height: 100vh;
  
  .el-container {
    height: 100%;
  }
  
  .sidebar-container {
    background: #304156;
    transition: width 0.28s;
    
    .logo-container {
      display: flex;
      align-items: center;
      padding: 16px;
      background: #2b2f3a;
      
      .logo {
        width: 32px;
        height: 32px;
        margin-right: 12px;
      }
      
      .title {
        color: #fff;
        font-size: 16px;
        font-weight: 600;
        margin: 0;
        white-space: nowrap;
        overflow: hidden;
      }
    }
  }
  
  .header-container {
    background: #fff;
    border-bottom: 1px solid #e4e7ed;
    padding: 0;
    height: 60px;
    line-height: 60px;
  }
  
  .main-container {
    background: #f0f2f5;
    padding: 20px;
    overflow-y: auto;
  }
}

// 页面切换动画
.fade-transform-enter-active,
.fade-transform-leave-active {
  transition: all 0.3s;
}

.fade-transform-enter-from {
  opacity: 0;
  transform: translateX(-30px);
}

.fade-transform-leave-to {
  opacity: 0;
  transform: translateX(30px);
}
</style>
