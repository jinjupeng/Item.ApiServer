<template>
  <el-dialog
    :model-value="visible"
    title="导入用户"
    width="600px"
    :before-close="handleClose"
    @update:model-value="$emit('update:visible', $event)"
  >
    <div class="import-content">
      <!-- 步骤指示器 -->
      <el-steps :active="currentStep" align-center>
        <el-step title="下载模板" />
        <el-step title="上传文件" />
        <el-step title="导入完成" />
      </el-steps>

      <!-- 步骤1：下载模板 -->
      <div v-if="currentStep === 0" class="step-content">
        <div class="template-info">
          <el-alert
            title="导入说明"
            type="info"
            :closable="false"
            show-icon
          >
            <template #default>
              <p>1. 请先下载用户导入模板</p>
              <p>2. 按照模板格式填写用户信息</p>
              <p>3. 保存为Excel文件后上传</p>
            </template>
          </el-alert>
        </div>

        <div class="template-download">
          <el-button type="primary" @click="downloadTemplate">
            <el-icon><Download /></el-icon>
            下载导入模板
          </el-button>
        </div>

        <div class="template-preview">
          <h4>模板格式预览：</h4>
          <el-table :data="templateData" border style="width: 100%">
            <el-table-column prop="username" label="用户名*" width="120" />
            <el-table-column prop="password" label="密码*" width="120" />
            <el-table-column prop="nickname" label="昵称" width="120" />
            <el-table-column prop="phone" label="手机号" width="130" />
            <el-table-column prop="email" label="邮箱" width="180" />
            <el-table-column prop="orgName" label="组织名称*" />
          </el-table>
          <p class="template-note">注：带*号的字段为必填项</p>
        </div>
      </div>

      <!-- 步骤2：上传文件 -->
      <div v-if="currentStep === 1" class="step-content">
        <div class="upload-area">
          <el-upload
            ref="uploadRef"
            class="upload-dragger"
            drag
            :auto-upload="false"
            :limit="1"
            :on-change="handleFileChange"
            :on-exceed="handleExceed"
            accept=".xlsx,.xls"
          >
            <el-icon class="el-icon--upload">
              <UploadFilled />
            </el-icon>
            <div class="el-upload__text">
              将文件拖到此处，或<em>点击上传</em>
            </div>
            <template #tip>
              <div class="el-upload__tip">
                只能上传 xlsx/xls 文件，且不超过 10MB
              </div>
            </template>
          </el-upload>
        </div>

        <!-- 文件信息 -->
        <div v-if="uploadFile" class="file-info">
          <el-card>
            <div class="file-item">
              <el-icon><Document /></el-icon>
              <span class="file-name">{{ uploadFile.name }}</span>
              <span class="file-size">{{ formatFileSize(uploadFile.size) }}</span>
              <el-button type="text" @click="removeFile">
                <el-icon><Delete /></el-icon>
              </el-button>
            </div>
          </el-card>
        </div>

        <!-- 验证结果 -->
        <div v-if="validationResult" class="validation-result">
          <el-alert
            :title="validationResult.success ? '文件验证通过' : '文件验证失败'"
            :type="validationResult.success ? 'success' : 'error'"
            :closable="false"
            show-icon
          >
            <template #default>
              <p v-if="validationResult.success">
                共 {{ validationResult.totalCount }} 条数据，可以导入 {{ validationResult.validCount }} 条
              </p>
              <div v-else>
                <p>发现 {{ validationResult.errors.length }} 个错误：</p>
                <ul>
                  <li v-for="error in validationResult.errors.slice(0, 5)" :key="error">
                    {{ error }}
                  </li>
                  <li v-if="validationResult.errors.length > 5">
                    还有 {{ validationResult.errors.length - 5 }} 个错误...
                  </li>
                </ul>
              </div>
            </template>
          </el-alert>
        </div>
      </div>

      <!-- 步骤3：导入完成 -->
      <div v-if="currentStep === 2" class="step-content">
        <div class="import-result">
          <el-result
            :icon="importResult.success ? 'success' : 'error'"
            :title="importResult.success ? '导入成功' : '导入失败'"
            :sub-title="importResult.message"
          >
            <template #extra>
              <div v-if="importResult.success" class="result-stats">
                <el-statistic title="成功导入" :value="importResult.successCount" />
                <el-statistic title="失败数量" :value="importResult.failCount" />
              </div>
              <div v-if="importResult.failDetails && importResult.failDetails.length > 0" class="fail-details">
                <h4>失败详情：</h4>
                <el-table :data="importResult.failDetails" border max-height="200">
                  <el-table-column prop="row" label="行号" width="80" />
                  <el-table-column prop="username" label="用户名" width="120" />
                  <el-table-column prop="error" label="错误信息" />
                </el-table>
              </div>
            </template>
          </el-result>
        </div>
      </div>
    </div>

    <template #footer>
      <div class="dialog-footer">
        <el-button v-if="currentStep > 0" @click="prevStep">上一步</el-button>
        <el-button @click="handleClose">
          {{ currentStep === 2 ? '完成' : '取消' }}
        </el-button>
        <el-button
          v-if="currentStep < 2"
          type="primary"
          :loading="loading"
          :disabled="!canNext"
          @click="nextStep"
        >
          {{ currentStep === 0 ? '下一步' : (loading ? '导入中...' : '开始导入') }}
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import { ElMessage, type UploadFile, type UploadFiles, type UploadInstance } from 'element-plus'
import { UserApi } from '@/api'
import { formatFileSize } from '@/utils'
import { Download, UploadFilled, Document, Delete } from '@element-plus/icons-vue'

interface Props {
  visible: boolean
}

const props = defineProps<Props>()
const emit = defineEmits(['update:visible', 'success'])

const uploadRef = ref<UploadInstance>()
const currentStep = ref(0)
const loading = ref(false)
const uploadFile = ref<File | null>(null)

// 模板数据
const templateData = [
  {
    username: 'zhangsan',
    password: '123456',
    nickname: '张三',
    phone: '13800138000',
    email: 'zhangsan@example.com',
    orgName: '技术部'
  },
  {
    username: 'lisi',
    password: '123456',
    nickname: '李四',
    phone: '13800138001',
    email: 'lisi@example.com',
    orgName: '市场部'
  }
]

// 验证结果
const validationResult = ref<{
  success: boolean
  totalCount: number
  validCount: number
  errors: string[]
} | null>(null)

// 导入结果
const importResult = reactive({
  success: false,
  message: '',
  successCount: 0,
  failCount: 0,
  failDetails: [] as Array<{ row: number; username: string; error: string }>
})

// 是否可以进入下一步
const canNext = computed(() => {
  if (currentStep.value === 0) return true
  if (currentStep.value === 1) return uploadFile.value && validationResult.value?.success
  return false
})

// 下载模板
const downloadTemplate = () => {
  // 创建模板数据
  const headers = ['用户名*', '密码*', '昵称', '手机号', '邮箱', '组织名称*']
  const data = [
    ['zhangsan', '123456', '张三', '13800138000', 'zhangsan@example.com', '技术部'],
    ['lisi', '123456', '李四', '13800138001', 'lisi@example.com', '市场部']
  ]

  // 创建CSV内容
  const csvContent = [headers, ...data]
    .map(row => row.map(cell => `"${cell}"`).join(','))
    .join('\n')

  // 创建Blob并下载
  const blob = new Blob(['\ufeff' + csvContent], { type: 'text/csv;charset=utf-8;' })
  const link = document.createElement('a')
  link.href = URL.createObjectURL(blob)
  link.download = '用户导入模板.csv'
  link.click()
  URL.revokeObjectURL(link.href)

  ElMessage.success('模板下载成功')
}

// 文件变化处理
const handleFileChange = (file: UploadFile, files: UploadFiles) => {
  uploadFile.value = file.raw || null
  validationResult.value = null
  
  if (uploadFile.value) {
    validateFile()
  }
}

// 文件超出限制
const handleExceed = () => {
  ElMessage.warning('只能上传一个文件')
}

// 移除文件
const removeFile = () => {
  uploadRef.value?.clearFiles()
  uploadFile.value = null
  validationResult.value = null
}

// 验证文件
const validateFile = async () => {
  if (!uploadFile.value) return

  try {
    loading.value = true
    
    // 模拟文件验证
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // 模拟验证结果
    const isValid = Math.random() > 0.3 // 70% 概率验证通过
    
    if (isValid) {
      validationResult.value = {
        success: true,
        totalCount: 10,
        validCount: 10,
        errors: []
      }
    } else {
      validationResult.value = {
        success: false,
        totalCount: 10,
        validCount: 7,
        errors: [
          '第3行：用户名不能为空',
          '第5行：邮箱格式不正确',
          '第7行：组织名称不存在'
        ]
      }
    }
  } catch (error) {
    ElMessage.error('文件验证失败')
  } finally {
    loading.value = false
  }
}

// 上一步
const prevStep = () => {
  if (currentStep.value > 0) {
    currentStep.value--
  }
}

// 下一步
const nextStep = async () => {
  if (currentStep.value === 0) {
    currentStep.value = 1
  } else if (currentStep.value === 1) {
    await importUsers()
  }
}

// 导入用户
const importUsers = async () => {
  if (!uploadFile.value) return

  try {
    loading.value = true
    
    // 调用导入API
    const response = await UserApi.importUsers(uploadFile.value)
    
    // 模拟导入结果
    const success = Math.random() > 0.2 // 80% 概率成功
    
    if (success) {
      importResult.success = true
      importResult.message = '用户导入完成'
      importResult.successCount = 8
      importResult.failCount = 2
      importResult.failDetails = [
        { row: 3, username: 'test1', error: '用户名已存在' },
        { row: 7, username: 'test2', error: '邮箱格式不正确' }
      ]
    } else {
      importResult.success = false
      importResult.message = '导入过程中发生错误'
      importResult.successCount = 0
      importResult.failCount = 10
    }
    
    currentStep.value = 2
    
    if (importResult.success) {
      ElMessage.success('导入完成')
    }
  } catch (error) {
    ElMessage.error('导入失败')
    importResult.success = false
    importResult.message = '导入失败，请稍后重试'
    currentStep.value = 2
  } finally {
    loading.value = false
  }
}

// 关闭对话框
const handleClose = () => {
  // 重置状态
  currentStep.value = 0
  uploadFile.value = null
  validationResult.value = null
  Object.assign(importResult, {
    success: false,
    message: '',
    successCount: 0,
    failCount: 0,
    failDetails: []
  })
  
  if (uploadRef.value) {
    uploadRef.value.clearFiles()
  }
  
  emit('update:visible', false)
  
  // 如果导入成功，通知父组件刷新
  if (currentStep.value === 2 && importResult.success) {
    emit('success')
  }
}
</script>

<style lang="scss" scoped>
.import-content {
  .el-steps {
    margin-bottom: 30px;
  }

  .step-content {
    min-height: 300px;
    padding: 20px 0;

    .template-info {
      margin-bottom: 20px;

      :deep(.el-alert__content) {
        p {
          margin: 4px 0;
        }
      }
    }

    .template-download {
      text-align: center;
      margin: 30px 0;
    }

    .template-preview {
      h4 {
        margin: 20px 0 10px 0;
        color: var(--el-text-color-primary);
      }

      .template-note {
        margin: 10px 0 0 0;
        font-size: 12px;
        color: var(--el-text-color-secondary);
      }
    }

    .upload-area {
      margin-bottom: 20px;

      .upload-dragger {
        width: 100%;
      }
    }

    .file-info {
      margin-bottom: 20px;

      .file-item {
        display: flex;
        align-items: center;
        gap: 12px;
        padding: 12px;

        .el-icon {
          font-size: 24px;
          color: var(--el-color-primary);
        }

        .file-name {
          flex: 1;
          font-weight: 500;
        }

        .file-size {
          font-size: 12px;
          color: var(--el-text-color-secondary);
        }
      }
    }

    .validation-result {
      margin-top: 20px;

      ul {
        margin: 8px 0 0 0;
        padding-left: 20px;

        li {
          margin: 4px 0;
          font-size: 14px;
        }
      }
    }

    .import-result {
      text-align: center;

      .result-stats {
        display: flex;
        justify-content: center;
        gap: 40px;
        margin: 20px 0;
      }

      .fail-details {
        margin-top: 20px;
        text-align: left;

        h4 {
          margin-bottom: 10px;
          color: var(--el-text-color-primary);
        }
      }
    }
  }
}

.dialog-footer {
  text-align: right;
}
</style>
