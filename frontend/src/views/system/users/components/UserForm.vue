<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? '编辑用户' : '新增用户'"
    width="600px"
    :before-close="handleClose"
    @update:model-value="$emit('update:visible', $event)"
  >
    <el-form
      ref="formRef"
      :model="form"
      :rules="rules"
      label-width="100px"
      size="default"
    >
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="用户名" prop="username">
            <el-input
              v-model="form.username"
              placeholder="请输入用户名"
              :disabled="isEdit"
              clearable
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="昵称" prop="nickname">
            <el-input
              v-model="form.nickname"
              placeholder="请输入昵称"
              clearable
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row v-if="!isEdit" :gutter="20">
        <el-col :span="12">
          <el-form-item label="密码" prop="password">
            <el-input
              v-model="form.password"
              type="password"
              placeholder="请输入密码"
              show-password
              clearable
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="确认密码" prop="confirmPassword">
            <el-input
              v-model="form.confirmPassword"
              type="password"
              placeholder="请再次输入密码"
              show-password
              clearable
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="手机号" prop="phone">
            <el-input
              v-model="form.phone"
              placeholder="请输入手机号"
              clearable
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="邮箱" prop="email">
            <el-input
              v-model="form.email"
              placeholder="请输入邮箱"
              clearable
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="所属组织" prop="orgId">
            <el-tree-select
              v-model="form.orgId"
              :data="organizationTree"
              :props="{ label: 'orgName', value: 'id' }"
              placeholder="请选择组织"
              clearable
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="用户角色" prop="roleIds">
            <el-select
              v-model="form.roleIds"
              multiple
              placeholder="请选择角色"
              style="width: 100%"
            >
              <el-option
                v-for="role in roleList"
                :key="role.id"
                :label="role.roleName"
                :value="role.id"
              />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="头像">
        <div class="avatar-upload">
          <el-upload
            class="avatar-uploader"
            :show-file-list="false"
            :before-upload="beforeAvatarUpload"
            :http-request="handleAvatarUpload"
          >
            <el-avatar
              v-if="form.portrait"
              :size="80"
              :src="form.portrait"
              fit="cover"
            />
            <el-icon v-else class="avatar-uploader-icon">
              <Plus />
            </el-icon>
          </el-upload>
          <div class="avatar-tips">
            <p>点击上传头像</p>
            <p>支持 JPG、PNG 格式，大小不超过 2MB</p>
          </div>
        </div>
      </el-form-item>
    </el-form>

    <template #footer>
      <div class="dialog-footer">
        <el-button @click="handleClose">取消</el-button>
        <el-button type="primary" :loading="loading" @click="handleSubmit">
          {{ loading ? '提交中...' : '确定' }}
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules, type UploadProps } from 'element-plus'
import { UserApi, RoleApi, OrganizationApi } from '@/api'
import { isEmail, isPhone } from '@/utils'
import type { CreateUserDto, UpdateUserDto, Role, OrganizationTreeDto } from '@/types/api'
import { Plus } from '@element-plus/icons-vue'

interface Props {
  visible: boolean
  formData: any
  isEdit: boolean
}

const props = defineProps<Props>()
const emit = defineEmits(['update:visible', 'success'])

const formRef = ref<FormInstance>()
const loading = ref(false)
const roleList = ref<Role[]>([])
const organizationTree = ref<OrganizationTreeDto[]>([])

// 表单数据
const form = reactive<CreateUserDto & UpdateUserDto & { confirmPassword?: string }>({
  username: '',
  password: '',
  confirmPassword: '',
  nickname: '',
  portrait: '',
  orgId: 0,
  phone: '',
  email: '',
  roleIds: []
})

// 验证规则
const rules: FormRules = {
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 2, max: 20, message: '用户名长度在 2 到 20 个字符', trigger: 'blur' },
    {
      validator: (rule: any, value: any, callback: any) => {
        if (value && !props.isEdit) {
          UserApi.checkUsername(value).then((response: any) => {
            if (response.data) {
              callback(new Error('用户名已存在'))
            } else {
              callback()
            }
          }).catch((error: any) => {
            callback(error)
          })
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ],
  password: [
    { required: !props.isEdit, message: '请输入密码', trigger: 'blur' },
    { min: 6, max: 20, message: '密码长度在 6 到 20 个字符', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: !props.isEdit, message: '请再次输入密码', trigger: 'blur' },
    {
      validator: (rule, value) => {
        if (value !== form.password) {
          throw new Error('两次输入的密码不一致')
        }
      },
      trigger: 'blur'
    }
  ],
  nickname: [
    { max: 20, message: '昵称长度不能超过 20 个字符', trigger: 'blur' }
  ],
  phone: [
    {
      validator: (rule, value) => {
        if (value && !isPhone(value)) {
          throw new Error('请输入正确的手机号')
        }
      },
      trigger: 'blur'
    },
    {
      validator: async (rule, value) => {
        if (value) {
          const excludeId = props.isEdit ? props.formData.id : undefined
          const response = await UserApi.checkEmail(value, excludeId)
          if (response.data) {
            throw new Error('手机号已存在')
          }
        }
      },
      trigger: 'blur'
    }
  ],
  email: [
    {
      validator: (rule, value) => {
        if (value && !isEmail(value)) {
          throw new Error('请输入正确的邮箱地址')
        }
      },
      trigger: 'blur'
    },
    {
      validator: async (rule, value) => {
        if (value) {
          const excludeId = props.isEdit ? props.formData.id : undefined
          const response = await UserApi.checkEmail(value, excludeId)
          if (response.data) {
            throw new Error('邮箱已存在')
          }
        }
      },
      trigger: 'blur'
    }
  ],
  orgId: [
    { required: true, message: '请选择所属组织', trigger: 'change' }
  ]
}

// 监听表单数据变化
watch(
  () => props.formData,
  (newData) => {
    if (newData && Object.keys(newData).length > 0) {
      Object.assign(form, {
        ...newData,
        roleIds: newData.roleIds || []
      })
    }
  },
  { immediate: true, deep: true }
)

// 监听对话框显示状态
watch(
  () => props.visible,
  (visible) => {
    if (visible) {
      // 重置表单
      if (!props.isEdit) {
        resetForm()
      }
    }
  }
)

// 重置表单
const resetForm = () => {
  Object.assign(form, {
    username: '',
    password: '',
    confirmPassword: '',
    nickname: '',
    portrait: '',
    orgId: 0,
    phone: '',
    email: '',
    roleIds: []
  })
  formRef.value?.clearValidate()
}

// 获取角色列表
const getRoleList = async () => {
  try {
    const response = await RoleApi.getAllRoles()
    roleList.value = response.data
  } catch (error) {
    console.error('获取角色列表失败:', error)
  }
}

// 获取组织树
const getOrganizationTree = async () => {
  try {
    const response = await OrganizationApi.getOrganizationTree()
    organizationTree.value = response.data
  } catch (error) {
    console.error('获取组织树失败:', error)
  }
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

// 头像上传处理
const handleAvatarUpload = async (options: any) => {
  try {
    // 这里应该调用文件上传API
    // const response = await FileApi.upload(options.file)
    // form.portrait = response.data.url
    
    // 临时处理：转换为base64
    const reader = new FileReader()
    reader.onload = (e) => {
      form.portrait = e.target?.result as string
    }
    reader.readAsDataURL(options.file)
    
    ElMessage.success('头像上传成功')
  } catch (error) {
    ElMessage.error('头像上传失败')
  }
}

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
    loading.value = true

    if (props.isEdit) {
      // 编辑用户
      const { confirmPassword, ...updateData } = form
      await UserApi.updateUser(props.formData.id, updateData)
      ElMessage.success('用户更新成功')
    } else {
      // 新增用户
      const { confirmPassword, ...createData } = form
      await UserApi.createUser(createData)
      ElMessage.success('用户创建成功')
    }

    emit('success')
  } catch (error) {
    console.error('提交失败:', error)
  } finally {
    loading.value = false
  }
}

// 关闭对话框
const handleClose = () => {
  emit('update:visible', false)
}

onMounted(() => {
  getRoleList()
  getOrganizationTree()
})
</script>

<style lang="scss" scoped>
.avatar-upload {
  display: flex;
  align-items: center;
  gap: 16px;

  .avatar-uploader {
    :deep(.el-upload) {
      border: 1px dashed var(--el-border-color);
      border-radius: 6px;
      cursor: pointer;
      position: relative;
      overflow: hidden;
      transition: var(--el-transition-duration);
      width: 80px;
      height: 80px;
      display: flex;
      align-items: center;
      justify-content: center;

      &:hover {
        border-color: var(--el-color-primary);
      }
    }

    .avatar-uploader-icon {
      font-size: 28px;
      color: #8c939d;
    }
  }

  .avatar-tips {
    p {
      margin: 0;
      font-size: 12px;
      color: var(--el-text-color-secondary);
      line-height: 1.5;

      &:first-child {
        font-weight: 500;
        color: var(--el-text-color-regular);
      }
    }
  }
}

.dialog-footer {
  text-align: right;
}
</style>
