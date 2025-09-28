<template>
  <el-menu
    :default-active="activeMenu"
    :collapse="appStore.sidebarCollapsed"
    :unique-opened="false"
    :collapse-transition="false"
    mode="vertical"
    background-color="#304156"
    text-color="#bfcbd9"
    active-text-color="#409eff"
    router
  >
    <SidebarItem
      v-for="route in menuRoutes"
      :key="route.path"
      :item="route"
      :base-path="route.path"
    />
  </el-menu>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { useAppStore } from '@/stores/app'
import { useAuthStore } from '@/stores/auth'
import SidebarItem from './SidebarItem.vue'

const route = useRoute()
const appStore = useAppStore()
const authStore = useAuthStore()

// 当前激活的菜单
const activeMenu = computed(() => {
  const { meta, path } = route
  if (meta.activeMenu) {
    return meta.activeMenu as string
  }
  return path
})

// 菜单路由（基于用户权限过滤）
const menuRoutes = computed(() => {
  // 根据用户菜单权限动态生成菜单路由
  const transformMenu = (menu: any) => {
    const route: any = {
      path: menu.url || `/m-${menu.id}`,
      name: menu.menuCode,
      meta: {
        title: menu.menuName,
        icon: menu.icon
      }
    }
    if (menu.children && menu.children.length > 0) {
      route.children = menu.children.map(transformMenu)
    }
    return route
  }
  return authStore.userMenus.map(transformMenu)
})
</script>

<style lang="scss" scoped>
.el-menu {
  border: none;
  height: calc(100vh - 64px);
  overflow-y: auto;
  
  &:not(.el-menu--collapse) {
    width: 200px;
  }
}
</style>
