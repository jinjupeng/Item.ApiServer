<template>
  <div class="profile-container">
    <el-row :gutter="20">
      <!-- 左侧个人信息卡片 -->
      <el-col :span="8">
        <el-card class="profile-card">
          <div class="profile-header">
            <div class="avatar-section">
              <el-avatar :size="80" :src="userInfo.avatar" class="avatar">
                <span v-if="!userInfo.avatar">{{ getAvatarText(userInfo.nickname || userInfo.username) }}</span>
              </el-avatar>
              <el-button type="text" class="change-avatar" @click="showAvatarDialog = true">
                更换头像
              </el-button>
            </div>
            <div class="user-info">
              <h3>{{ userInfo.nickname || userInfo.username }}</h3>
              <p class="username">@{{ userInfo.username }}</p>
              <p class="org-info">{{ userInfo.orgName }}</p>
            </div>
          </div>
          
          <el-divider />
          
          <div class="profile-stats">
            <div class="stat-item">
              <div class="stat-value">{{ loginCount }}</div>
              <div class="stat-label">登录次数</div>
            </div>
            <div class="stat-item">
              <div class="stat-value">{{ userInfo.lastLoginTime ? formatDate(userInfo.lastLoginTime) : '未知' }}</div>
              <div class="stat-label">最后登录</div>
            </div>
            <div class="stat-item">
              <div class="stat-value">{{ userInfo.createTime ? formatDate(userInfo.createTime) : '未知' }}</div>
              <div class="stat-label">注册时间</div>
            </div>
          </div>
        </el-card>
      </el-col>

      <!-- 右侧信息编辑 -->
      <el-col :span="16">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>个人信息</span>
              <el-button type="primary" @click="handleEdit" v-if="!isEditing">
                编辑信息
              </el-button>
              <div v-else>
                <el-button type="success" @click="handleSave" :loading="saving">
                  保存
                </el-button>
                <el-button @click="handleCancel">取消</el-button>
              </div>
            </div>
          </template>

          <el-form
            ref="formRef"
            :model="form"
            :rules="rules"
            label-width="100px"
            :disabled="!isEditing"
          >
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="用户名" prop="username">
                  <el-input v-model="form.username" disabled />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="昵称" prop="nickname">
                  <el-input v-model="form.nickname" placeholder="请输入昵称" />
                </el-form-item>
              </el-col>
            </el-row>

            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="邮箱" prop="email">
                  <el-input v-model="form.email" placeholder="请输入邮箱" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="手机号" prop="phone">
                  <el-input v-model="form.phone" placeholder="请输入手机号" />
                </el-form-item>
              </el-col>
            </el-row>

            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="性别" prop="gender">
                  <el-radio-group v-model="form.gender">
                    <el-radio :label="1">男</el-radio>
                    <el-radio :label="2">女</el-radio>
                    <el-radio :label="0">保密</el-radio>
                  </el-radio-group>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="生日" prop="birthday">
                  <el-date-picker
                    v-model="form.birthday"
                    type="date"
                    placeholder="选择生日"
                    style="width: 100%"
                  />
                </el-form-item>
              </el-col>
            </el-row>

            <el-form-item label="个人简介" prop="bio">
              <el-input
                v-model="form.bio"
                type="textarea"
                :rows="4"
                placeholder="请输入个人简介"
              />
            </el-form-item>
          </el-form>
        </el-card>

        <!-- 密码修改卡片 -->
        <el-card class="password-card">
          <template #header>
            <span>修改密码</span>
          </template>

          <el-form
            ref="passwordFormRef"
            :model="passwordForm"
            :rules="passwordRules"
            label-width="100px"
          >
            <el-form-item label="当前密码" prop="oldPassword">
              <el-input
                v-model="passwordForm.oldPassword"
                type="password"
                placeholder="请输入当前密码"
                show-password
              />
            </el-form-item>
            
            <el-form-item label="新密码" prop="newPassword">
              <el-input
                v-model="passwordForm.newPassword"
                type="password"
                placeholder="请输入新密码"
                show-password
              />
            </el-form-item>
            
            <el-form-item label="确认密码" prop="confirmPassword">
              <el-input
                v-model="passwordForm.confirmPassword"
                type="password"
                placeholder="请再次输入新密码"
                show-password
              />
            </el-form-item>

            <el-form-item>
              <el-button type="primary" @click="handleChangePassword" :loading="changingPassword">
                修改密码
              </el-button>
              <el-button @click="resetPasswordForm">重置</el-button>
            </el-form-item>
          </el-form>
        </el-card>
      </el-col>
    </el-row>

    <!-- 头像上传对话框 -->
    <el-dialog v-model="showAvatarDialog" title="更换头像" width="500px">
      <div class="avatar-upload">
        <el-upload
          class="avatar-uploader"
          :action="uploadUrl"
          :headers="uploadHeaders"
          :show-file-list="false"
          :on-success="handleAvatarSuccess"
          :before-upload="beforeAvatarUpload"
        >
          <img v-if="newAvatar" :src="newAvatar" class="avatar-preview" />
          <el-icon v-else class="avatar-uploader-icon"><Plus /></el-icon>
        </el-upload>
        <div class="upload-tips">
          <p>支持 JPG、PNG 格式，文件大小不超过 2MB</p>
          <p>建议上传 200x200 像素的正方形图片</p>
        </div>
      </div>
      
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="showAvatarDialog = false">取消</el-button>
          <el-button type="primary" @click="handleSaveAvatar" :disabled="!newAvatar">
            保存
          </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, type FormInstance, type FormRules, type UploadProps } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'
import { formatDate, getAvatarColor } from '@/utils'

// Store
const authStore = useAuthStore()

// 响应式数据
const isEditing = ref(false)
const saving = ref(false)
const changingPassword = ref(false)
const showAvatarDialog = ref(false)
const newAvatar = ref('')
const loginCount = ref(156) // 模拟数据

// 表单引用
const formRef = ref<FormInstance>()
const passwordFormRef = ref<FormInstance>()

// 用户信息
const userInfo = computed(() => authStore.userInfo || {
  username: 'admin',
  nickname: '管理员',
  email: 'admin@example.com',
  phone: '13800138000',
  orgName: '技术部',
  avatar: '',
  lastLoginTime: '2024-01-15 10:30:00',
  createTime: '2024-01-01 09:00:00'
})

// 个人信息表单
const form = reactive({
  username: '',
  nickname: '',
  email: '',
  phone: '',
  gender: 0,
  birthday: '',
  bio: ''
})

// 密码修改表单
const passwordForm = reactive({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})

// 表单验证规则
const rules: FormRules = {
  nickname: [
    { required: true, message: '请输入昵称', trigger: 'blur' },
    { min: 2, max: 20, message: '昵称长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'blur' },
    { type: 'email', message: '请输入正确的邮箱格式', trigger: 'blur' }
  ],
  phone: [
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号码', trigger: 'blur' }
  ]
}

// 密码验证规则
const passwordRules: FormRules = {
  oldPassword: [
    { required: true, message: '请输入当前密码', trigger: 'blur' }
  ],
  newPassword: [
    { required: true, message: '请输入新密码', trigger: 'blur' },
    { min: 6, max: 20, message: '密码长度在 6 到 20 个字符', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, message: '请再次输入新密码', trigger: 'blur' },
    {
      validator: (rule, value, callback) => {
        if (value !== passwordForm.newPassword) {
          callback(new Error('两次输入的密码不一致'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ]
}

// 上传配置
const uploadUrl = '/api/upload/avatar'
const uploadHeaders = computed(() => ({
  Authorization: `Bearer ${authStore.token}`
}))

// 获取头像文字
const getAvatarText = (name: string) => {
  return name ? name.charAt(0).toUpperCase() : 'U'
}

// 编辑信息
const handleEdit = () => {
  isEditing.value = true
  // 填充表单数据
  Object.assign(form, {
    username: userInfo.value.username,
    nickname: userInfo.value.nickname,
    email: userInfo.value.email || '',
    phone: userInfo.value.phone || '',
    gender: (userInfo.value as any).gender || 0,
    birthday: (userInfo.value as any).birthday || '',
    bio: (userInfo.value as any).bio || ''
  })
}

// 保存信息
const handleSave = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    saving.value = true
    
    // TODO: 调用API更新用户信息
    // await userApi.updateProfile(form)
    
    // 模拟API调用
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    ElMessage.success('个人信息更新成功')
    isEditing.value = false
    
    // 更新store中的用户信息
    // authStore.updateUserInfo(form)
    
  } catch (error) {
    if (error !== false) {
      ElMessage.error('更新失败')
    }
  } finally {
    saving.value = false
  }
}

// 取消编辑
const handleCancel = () => {
  isEditing.value = false
  formRef.value?.resetFields()
}

// 修改密码
const handleChangePassword = async () => {
  if (!passwordFormRef.value) return
  
  try {
    await passwordFormRef.value.validate()
    changingPassword.value = true
    
    // TODO: 调用API修改密码
    // await authApi.changePassword(passwordForm)
    
    // 模拟API调用
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    ElMessage.success('密码修改成功')
    resetPasswordForm()
    
  } catch (error) {
    if (error !== false) {
      ElMessage.error('密码修改失败')
    }
  } finally {
    changingPassword.value = false
  }
}

// 重置密码表单
const resetPasswordForm = () => {
  passwordFormRef.value?.resetFields()
  Object.assign(passwordForm, {
    oldPassword: '',
    newPassword: '',
    confirmPassword: ''
  })
}

// 头像上传前验证
const beforeAvatarUpload: UploadProps['beforeUpload'] = (file) => {
  const isJPG = file.type === 'image/jpeg' || file.type === 'image/png'
  const isLt2M = file.size / 1024 / 1024 < 2

  if (!isJPG) {
    ElMessage.error('头像只能是 JPG/PNG 格式!')
    return false
  }
  if (!isLt2M) {
    ElMessage.error('头像大小不能超过 2MB!')
    return false
  }
  return true
}

// 头像上传成功
const handleAvatarSuccess: UploadProps['onSuccess'] = (response) => {
  if (response.success) {
    newAvatar.value = response.data.url
  } else {
    ElMessage.error('头像上传失败')
  }
}

// 保存头像
const handleSaveAvatar = async () => {
  try {
    // TODO: 调用API更新头像
    // await userApi.updateAvatar(newAvatar.value)
    
    ElMessage.success('头像更新成功')
    showAvatarDialog.value = false
    newAvatar.value = ''
    
    // 更新用户信息
    // authStore.updateUserInfo({ avatar: newAvatar.value })
    
  } catch (error) {
    ElMessage.error('头像更新失败')
  }
}

// 初始化
onMounted(() => {
  // 加载用户详细信息
  // loadUserProfile()
})
</script>

<style scoped lang="scss">
.profile-container {
  padding: 20px;
}

.profile-card {
  .profile-header {
    text-align: center;
    
    .avatar-section {
      margin-bottom: 20px;
      
      .avatar {
        display: block;
        margin: 0 auto 10px;
        background-color: v-bind('getAvatarColor(userInfo.nickname || userInfo.username)');
      }
      
      .change-avatar {
        font-size: 12px;
        color: #409eff;
      }
    }
    
    .user-info {
      h3 {
        margin: 0 0 5px;
        font-size: 20px;
        font-weight: 600;
      }
      
      .username {
        margin: 0 0 5px;
        color: #909399;
        font-size: 14px;
      }
      
      .org-info {
        margin: 0;
        color: #606266;
        font-size: 14px;
      }
    }
  }
  
  .profile-stats {
    display: flex;
    justify-content: space-around;
    
    .stat-item {
      text-align: center;
      
      .stat-value {
        font-size: 18px;
        font-weight: 600;
        color: #303133;
        margin-bottom: 5px;
      }
      
      .stat-label {
        font-size: 12px;
        color: #909399;
      }
    }
  }
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.password-card {
  margin-top: 20px;
}

.avatar-upload {
  text-align: center;
  
  .avatar-uploader {
    :deep(.el-upload) {
      border: 1px dashed #d9d9d9;
      border-radius: 6px;
      cursor: pointer;
      position: relative;
      overflow: hidden;
      transition: all 0.3s;
      
      &:hover {
        border-color: #409eff;
      }
    }
  }
  
  .avatar-preview {
    width: 178px;
    height: 178px;
    display: block;
  }
  
  .avatar-uploader-icon {
    font-size: 28px;
    color: #8c939d;
    width: 178px;
    height: 178px;
    line-height: 178px;
    text-align: center;
  }
  
  .upload-tips {
    margin-top: 15px;
    
    p {
      margin: 5px 0;
      font-size: 12px;
      color: #909399;
    }
  }
}

.dialog-footer {
  text-align: right;
}
</style>
