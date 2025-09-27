<template>
  <div class="tabs-view">
    <el-scrollbar ref="scrollbarRef" class="tabs-scrollbar">
      <div class="tabs-nav">
        <div
          v-for="tab in appStore.tabs"
          :key="tab.path"
          class="tabs-item"
          :class="{ 'is-active': tab.path === appStore.activeTab }"
          @click="handleTabClick(tab)"
          @contextmenu.prevent="handleContextMenu($event, tab)"
        >
          <span class="tab-title">{{ tab.title }}</span>
          <el-icon
            v-if="tab.closable !== false"
            class="tab-close"
            @click.stop="handleTabClose(tab)"
          >
            <Close />
          </el-icon>
        </div>
      </div>
    </el-scrollbar>

    <!-- 右键菜单 -->
    <div class="tabs-actions">
      <el-dropdown @command="handleDropdownCommand">
        <el-icon class="tabs-action-icon">
          <ArrowDown />
        </el-icon>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="refresh">
              <el-icon><Refresh /></el-icon>
              刷新当前
            </el-dropdown-item>
            <el-dropdown-item command="close-others">
              <el-icon><Remove /></el-icon>
              关闭其他
            </el-dropdown-item>
            <el-dropdown-item command="close-all">
              <el-icon><CircleClose /></el-icon>
              关闭所有
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>

    <!-- 右键上下文菜单 -->
    <ul
      v-show="contextMenuVisible"
      class="context-menu"
      :style="{ left: contextMenuLeft + 'px', top: contextMenuTop + 'px' }"
    >
      <li @click="refreshTab">
        <el-icon><Refresh /></el-icon>
        刷新
      </li>
      <li v-if="selectedTab?.closable !== false" @click="closeTab">
        <el-icon><Close /></el-icon>
        关闭
      </li>
      <li @click="closeOtherTabs">
        <el-icon><Remove /></el-icon>
        关闭其他
      </li>
      <li @click="closeAllTabs">
        <el-icon><CircleClose /></el-icon>
        关闭所有
      </li>
    </ul>
  </div>
</template>

<script setup lang="ts">
import { ref, nextTick, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAppStore } from '@/stores/app'
import { Close, ArrowDown, Refresh, Remove, CircleClose } from '@element-plus/icons-vue'

const router = useRouter()
const appStore = useAppStore()

const scrollbarRef = ref()
const contextMenuVisible = ref(false)
const contextMenuLeft = ref(0)
const contextMenuTop = ref(0)
const selectedTab = ref<any>(null)

// 标签页点击
const handleTabClick = (tab: any) => {
  if (tab.path !== router.currentRoute.value.path) {
    router.push(tab.path)
  }
  appStore.setActiveTab(tab.path)
}

// 关闭标签页
const handleTabClose = (tab: any) => {
  appStore.removeTab(tab.path)
  
  // 如果关闭的是当前标签页，跳转到其他标签页
  if (tab.path === router.currentRoute.value.path && appStore.tabs.length > 0) {
    const lastTab = appStore.tabs[appStore.tabs.length - 1]
    router.push(lastTab.path)
  }
}

// 右键菜单
const handleContextMenu = (event: MouseEvent, tab: any) => {
  event.preventDefault()
  contextMenuLeft.value = event.clientX
  contextMenuTop.value = event.clientY
  selectedTab.value = tab
  contextMenuVisible.value = true
}

// 下拉菜单命令
const handleDropdownCommand = (command: string) => {
  const currentTab = appStore.tabs.find(tab => tab.path === appStore.activeTab)
  if (!currentTab) return

  switch (command) {
    case 'refresh':
      refreshCurrentTab()
      break
    case 'close-others':
      appStore.removeOtherTabs(currentTab.path)
      break
    case 'close-all':
      appStore.removeAllTabs()
      if (appStore.tabs.length > 0) {
        router.push(appStore.tabs[0].path)
      } else {
        router.push('/')
      }
      break
  }
}

// 刷新标签页
const refreshTab = () => {
  if (selectedTab.value) {
    router.replace({
      path: '/redirect' + selectedTab.value.path,
      query: router.currentRoute.value.query
    })
  }
  closeContextMenu()
}

// 刷新当前标签页
const refreshCurrentTab = () => {
  const currentPath = router.currentRoute.value.path
  router.replace({
    path: '/redirect' + currentPath,
    query: router.currentRoute.value.query
  })
}

// 关闭标签页
const closeTab = () => {
  if (selectedTab.value) {
    handleTabClose(selectedTab.value)
  }
  closeContextMenu()
}

// 关闭其他标签页
const closeOtherTabs = () => {
  if (selectedTab.value) {
    appStore.removeOtherTabs(selectedTab.value.path)
    router.push(selectedTab.value.path)
  }
  closeContextMenu()
}

// 关闭所有标签页
const closeAllTabs = () => {
  appStore.removeAllTabs()
  if (appStore.tabs.length > 0) {
    router.push(appStore.tabs[0].path)
  } else {
    router.push('/')
  }
  closeContextMenu()
}

// 关闭右键菜单
const closeContextMenu = () => {
  contextMenuVisible.value = false
  selectedTab.value = null
}

// 点击其他地方关闭右键菜单
const handleClickOutside = () => {
  closeContextMenu()
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<style lang="scss" scoped>
.tabs-view {
  height: var(--app-tabs-height);
  display: flex;
  align-items: center;
  background-color: var(--el-bg-color);
  border-bottom: 1px solid var(--el-border-color-lighter);
  position: relative;

  .tabs-scrollbar {
    flex: 1;
    height: 100%;

    :deep(.el-scrollbar__view) {
      height: 100%;
    }

    :deep(.el-scrollbar__bar) {
      display: none;
    }
  }

  .tabs-nav {
    display: flex;
    align-items: center;
    height: 100%;
    padding: 0 8px;
    gap: 4px;

    .tabs-item {
      display: flex;
      align-items: center;
      height: 32px;
      padding: 0 12px;
      background-color: var(--el-fill-color-light);
      border: 1px solid var(--el-border-color-lighter);
      border-radius: 6px;
      cursor: pointer;
      transition: all var(--el-transition-duration);
      white-space: nowrap;
      user-select: none;

      &:hover {
        background-color: var(--el-fill-color);
        border-color: var(--el-border-color);
      }

      &.is-active {
        background-color: var(--el-color-primary);
        border-color: var(--el-color-primary);
        color: #fff;

        .tab-close {
          color: rgba(255, 255, 255, 0.8);

          &:hover {
            color: #fff;
            background-color: rgba(255, 255, 255, 0.2);
          }
        }
      }

      .tab-title {
        font-size: 12px;
        font-weight: 500;
        margin-right: 6px;
      }

      .tab-close {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 16px;
        height: 16px;
        border-radius: 50%;
        font-size: 12px;
        color: var(--el-text-color-secondary);
        transition: all var(--el-transition-duration);

        &:hover {
          background-color: var(--el-fill-color-dark);
          color: var(--el-text-color-primary);
        }
      }
    }
  }

  .tabs-actions {
    padding: 0 12px;
    border-left: 1px solid var(--el-border-color-lighter);

    .tabs-action-icon {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 24px;
      height: 24px;
      border-radius: 4px;
      cursor: pointer;
      color: var(--el-text-color-secondary);
      transition: all var(--el-transition-duration);

      &:hover {
        background-color: var(--el-fill-color-light);
        color: var(--el-text-color-primary);
      }
    }
  }

  .context-menu {
    position: fixed;
    z-index: 9999;
    background-color: var(--el-bg-color-overlay);
    border: 1px solid var(--el-border-color);
    border-radius: 6px;
    box-shadow: var(--el-box-shadow-light);
    padding: 4px 0;
    margin: 0;
    list-style: none;
    min-width: 120px;

    li {
      display: flex;
      align-items: center;
      padding: 8px 16px;
      font-size: 14px;
      color: var(--el-text-color-primary);
      cursor: pointer;
      transition: background-color var(--el-transition-duration);

      &:hover {
        background-color: var(--el-fill-color-light);
      }

      .el-icon {
        margin-right: 8px;
        font-size: 16px;
      }
    }
  }
}
</style>
