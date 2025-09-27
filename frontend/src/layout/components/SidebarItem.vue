<template>
  <div v-if="!item.meta?.hideInMenu">
    <!-- 有子菜单的情况 -->
    <el-sub-menu
      v-if="hasChildren"
      :index="resolvePath(item.path)"
      popper-append-to-body
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
import { useAuthStore } from '@/stores/auth'

interface MenuItem {
  path: string
  name: string
  meta?: {
    title?: string
    icon?: string
    hideInMenu?: boolean
    permission?: string
  }
  children?: MenuItem[]
}

interface Props {
  item: MenuItem
  basePath: string
}

const props = defineProps<Props>()
const authStore = useAuthStore()

// 解析路径
const resolvePath = (routePath: string) => {
  if (routePath.startsWith('/')) {
    return routePath
  }
  return `${props.basePath}/${routePath}`.replace(/\/+/g, '/')
}

// 可见的子菜单
const visibleChildren = computed(() => {
  if (!props.item.children) return []
  
  return props.item.children.filter(child => {
    // 检查是否隐藏
    if (child.meta?.hideInMenu) return false
    
    // 检查权限
    if (child.meta?.permission && !authStore.hasPermission(child.meta.permission)) {
      return false
    }
    
    return true
  })
})

// 是否有子菜单
const hasChildren = computed(() => {
  return visibleChildren.value.length > 0
})
</script>
