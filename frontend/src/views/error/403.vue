<template>
  <div class="error-page">
    <div class="error-content">
      <div class="error-image">
        <img src="/images/403.svg" alt="403" />
      </div>
      <div class="error-info">
        <h1 class="error-code">403</h1>
        <h2 class="error-title">无权限访问</h2>
        <p class="error-desc">
          抱歉，您没有权限访问此页面，请联系管理员获取权限
        </p>
        <div class="error-actions">
          <el-button type="primary" @click="goHome">
            <el-icon><HomeFilled /></el-icon>
            返回首页
          </el-button>
          <el-button @click="goBack">
            <el-icon><Back /></el-icon>
            返回上页
          </el-button>
          <el-button type="warning" @click="contactAdmin">
            <el-icon><Service /></el-icon>
            联系管理员
          </el-button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { HomeFilled, Back, Service } from '@element-plus/icons-vue'

const router = useRouter()

const goHome = () => {
  router.push('/')
}

const goBack = () => {
  router.go(-1)
}

const contactAdmin = () => {
  ElMessage.info('请联系系统管理员获取相应权限')
}
</script>

<style lang="scss" scoped>
.error-page {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
  padding: 20px;

  .error-content {
    display: flex;
    align-items: center;
    gap: 60px;
    max-width: 800px;
    background: rgba(255, 255, 255, 0.95);
    backdrop-filter: blur(20px);
    border-radius: 20px;
    padding: 60px;
    box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);

    .error-image {
      flex-shrink: 0;

      img {
        width: 300px;
        height: 300px;
        object-fit: contain;
      }
    }

    .error-info {
      flex: 1;
      text-align: center;

      .error-code {
        font-size: 120px;
        font-weight: 700;
        color: var(--el-color-warning);
        margin: 0;
        line-height: 1;
        text-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
      }

      .error-title {
        font-size: 32px;
        font-weight: 600;
        color: var(--el-text-color-primary);
        margin: 20px 0 16px 0;
      }

      .error-desc {
        font-size: 16px;
        color: var(--el-text-color-secondary);
        margin: 0 0 40px 0;
        line-height: 1.6;
      }

      .error-actions {
        display: flex;
        justify-content: center;
        gap: 16px;
        flex-wrap: wrap;

        .el-button {
          padding: 12px 24px;
          font-size: 16px;
          border-radius: 8px;
        }
      }
    }
  }
}

// 移动端适配
@media (max-width: 768px) {
  .error-page {
    .error-content {
      flex-direction: column;
      gap: 40px;
      padding: 40px 20px;
      text-align: center;

      .error-image {
        img {
          width: 200px;
          height: 200px;
        }
      }

      .error-info {
        .error-code {
          font-size: 80px;
        }

        .error-title {
          font-size: 24px;
        }

        .error-desc {
          font-size: 14px;
        }

        .error-actions {
          flex-direction: column;
          align-items: center;

          .el-button {
            width: 200px;
          }
        }
      }
    }
  }
}
</style>
