<template>
  <el-breadcrumb class="app-breadcrumb" separator="/">
    <transition-group name="breadcrumb">
      <el-breadcrumb-item
        v-for="(item, index) in breadcrumbs"
        :key="item.path || item.title"
        :to="item.path && index !== breadcrumbs.length - 1 ? { path: item.path } : undefined"
      >
        {{ item.title }}
      </el-breadcrumb-item>
    </transition-group>
  </el-breadcrumb>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useAppStore } from '@/stores/app'

const appStore = useAppStore()

const breadcrumbs = computed(() => appStore.breadcrumbs)
</script>

<style lang="scss" scoped>
.app-breadcrumb {
  display: inline-block;
  font-size: 14px;
  line-height: 50px;

  :deep(.el-breadcrumb__item) {
    .el-breadcrumb__inner {
      font-weight: 500;
      color: var(--el-text-color-regular);
      cursor: pointer;
      transition: color var(--el-transition-duration);

      &:hover {
        color: var(--el-color-primary);
      }

      &.is-link {
        color: var(--el-text-color-primary);
      }
    }

    &:last-child {
      .el-breadcrumb__inner {
        color: var(--el-text-color-secondary);
        cursor: default;

        &:hover {
          color: var(--el-text-color-secondary);
        }
      }
    }

    .el-breadcrumb__separator {
      color: var(--el-text-color-placeholder);
      margin: 0 8px;
    }
  }
}

// 面包屑动画
.breadcrumb-enter-active,
.breadcrumb-leave-active {
  transition: all 0.3s;
}

.breadcrumb-enter-from,
.breadcrumb-leave-to {
  opacity: 0;
  transform: translateX(20px);
}

.breadcrumb-leave-active {
  position: absolute;
}
</style>
