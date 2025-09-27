<template>
  <div v-if="!item.meta?.hidden">
    <!-- 有子菜单的情况 -->
    <el-sub-menu
      v-if="hasChildren"
      :index="item.path"
      :popper-append-to-body="true"
    >
      <template #title>
        <el-icon v-if="item.meta?.icon">
          <component :is="item.meta.icon" />
        </el-icon>
        <span>{{ item.meta?.title }}</span>
      </template>
      
      <SidebarItem
        v-for="child in visibleChildren"
        :key="child.path"
        :item="child"
        :base-path="resolvePath(child.path)"
      />
    </el-sub-menu>

    <!-- 单个菜单项 -->
    <el-menu-item
      v-else
      :index="resolvePath(item.path)"
      @click="handleClick"
    >
      <el-icon v-if="item.meta?.icon">
        <component :is="item.meta.icon" />
      </el-icon>
      <template #title>
        <span>{{ item.meta?.title }}</span>
      </template>
    </el-menu-item>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

interface Props {
  item: RouteRecordRaw
  basePath: string
}

const props = defineProps<Props>()
const router = useRouter()
const authStore = useAuthStore()

// 可见的子菜单
const visibleChildren = computed(() => {
  if (!props.item.children) return []
  
  return props.item.children.filter(child => {
    // 过滤隐藏的菜单
    if (child.meta?.hidden) return false
    
    // 权限检查
    if (child.meta?.permissions) {
      const permissions = child.meta.permissions as string[]
      return permissions.some(permission => authStore.hasPermission(permission))
    }
    
    // 角色检查
    if (child.meta?.roles) {
      const roles = child.meta.roles as string[]
      return roles.some(role => authStore.hasRole(role))
    }
    
    return true
  })
})

// 是否有子菜单
const hasChildren = computed(() => {
  return visibleChildren.value.length > 0
})

// 解析路径
const resolvePath = (routePath: string) => {
  if (routePath.startsWith('/')) {
    return routePath
  }
  return `${props.basePath}/${routePath}`.replace(/\/+/g, '/')
}

// 点击处理
const handleClick = () => {
  const path = resolvePath(props.item.path)
  if (path !== router.currentRoute.value.path) {
    router.push(path)
  }
}
</script>
