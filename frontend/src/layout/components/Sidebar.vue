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
  // 这里可以根据用户菜单权限动态生成菜单
  // 暂时使用静态路由配置
  return [
    {
      path: '/dashboard',
      name: 'Dashboard',
      meta: {
        title: '仪表盘',
        icon: 'Dashboard'
      }
    },
    {
      path: '/system',
      name: 'System',
      meta: {
        title: '系统管理',
        icon: 'Setting'
      },
      children: [
        {
          path: '/system/users',
          name: 'SystemUsers',
          meta: {
            title: '用户管理',
            icon: 'User'
          }
        },
        {
          path: '/system/roles',
          name: 'SystemRoles',
          meta: {
            title: '角色管理',
            icon: 'UserFilled'
          }
        },
        {
          path: '/system/menus',
          name: 'SystemMenus',
          meta: {
            title: '菜单管理',
            icon: 'Menu'
          }
        }
      ]
    }
  ]
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
