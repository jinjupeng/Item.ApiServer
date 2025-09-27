<template>
  <div id="app">
    <router-view />
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useAppStore } from '@/stores/app'

const appStore = useAppStore()

onMounted(() => {
  // 初始化主题
  const isDark = localStorage.getItem('theme') === 'dark'
  if (isDark) {
    appStore.toggleTheme()
  }
})
</script>

<style lang="scss">
#app {
  height: 100%;
  width: 100%;
}
</style>
