<template>
  <div class="app-footer">
    <div class="footer-content">
      <span class="copyright">
        © 2024 {{ appStore.settings.title }}. All rights reserved.
      </span>
      <div class="footer-links">
        <a href="#" class="footer-link">隐私政策</a>
        <span class="divider">|</span>
        <a href="#" class="footer-link">服务条款</a>
        <span class="divider">|</span>
        <a href="#" class="footer-link">技术支持</a>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAppStore } from '@/stores/app'

const appStore = useAppStore()
</script>

<style lang="scss" scoped>
.app-footer {
  height: var(--app-footer-height);
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: var(--el-bg-color);
  border-top: 1px solid var(--el-border-color-lighter);

  .footer-content {
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: 100%;
    max-width: 1200px;
    padding: 0 20px;
    font-size: 12px;
    color: var(--el-text-color-secondary);

    .copyright {
      font-weight: 500;
    }

    .footer-links {
      display: flex;
      align-items: center;
      gap: 8px;

      .footer-link {
        color: var(--el-text-color-secondary);
        text-decoration: none;
        transition: color var(--el-transition-duration);

        &:hover {
          color: var(--el-color-primary);
        }
      }

      .divider {
        color: var(--el-border-color-dark);
      }
    }
  }
}

// 移动端适配
@media (max-width: 768px) {
  .app-footer {
    .footer-content {
      flex-direction: column;
      gap: 8px;
      text-align: center;

      .footer-links {
        justify-content: center;
      }
    }
  }
}
</style>
