<template>
  <div class="page-container">
    <!-- 搜索表单 -->
    <div class="search-form">
      <el-form
        ref="searchFormRef"
        :model="searchForm"
        :inline="true"
        label-width="80px"
      >
        <el-form-item label="关键词">
          <el-input
            v-model="searchForm.keyword"
            placeholder="用户名/姓名/邮箱"
            clearable
            style="width: 200px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        
        <el-form-item label="状态">
          <el-select
            v-model="searchForm.status"
            placeholder="请选择状态"
            clearable
            style="width: 120px"
          >
            <el-option label="正常" :value="UserStatus.Active" />
            <el-option label="禁用" :value="UserStatus.Inactive" />
            <el-option label="锁定" :value="UserStatus.Locked" />
          </el-select>
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">
            搜索
          </el-button>
          <el-button icon="Refresh" @click="handleReset">
            重置
          </el-button>
        </el-form-item>
      </el-form>
    </div>
    
    <!-- 操作工具栏 -->
    <div class="table-toolbar">
      <div class="toolbar-left">
        <el-button
          v-if="authStore.hasPermission('system:user:create')"
          type="primary"
          icon="Plus"
          @click="handleCreate"
        >
          新增用户
        </el-button>
        
        <el-button
          v-if="authStore.hasPermission('system:user:delete')"
          type="danger"
          icon="Delete"
          :disabled="!selectedIds.length"
          @click="handleBatchDelete"
        >
          批量删除
        </el-button>
      </div>
      
      <div class="toolbar-right">
        <el-button icon="Refresh" @click="getUsers">
          刷新
        </el-button>
      </div>
    </div>
    
    <!-- 用户表格 -->
    <el-table
      v-loading="loading"
      :data="users"
      @selection-change="handleSelectionChange"
    >
      <el-table-column type="selection" width="55" />
      
      <el-table-column prop="id" label="ID" width="80" />
      
      <el-table-column prop="userName" label="用户名" min-width="120" />
      
      <el-table-column prop="nickName" label="姓名" min-width="100" />
      
      <el-table-column prop="email" label="邮箱" min-width="180" />
      
      <el-table-column prop="phone" label="手机号" min-width="120" />
      
      <el-table-column label="角色" min-width="150">
        <template #default="{ row }">
          <el-tag
            v-for="role in row.roles"
            :key="role.id"
            size="small"
            class="role-tag"
          >
            {{ role.name }}
          </el-tag>
        </template>
      </el-table-column>
      
      <el-table-column label="状态" width="80">
        <template #default="{ row }">
          <el-tag
            :type="getStatusTagType(row.status)"
            size="small"
          >
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>
      </el-table-column>
      
      <el-table-column prop="createdAt" label="创建时间" width="160">
        <template #default="{ row }">
          {{ formatDate(row.createdAt) }}
        </template>
      </el-table-column>
      
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button
            v-if="authStore.hasPermission('system:user:update')"
            type="primary"
            size="small"
            link
            @click="handleEdit(row)"
          >
            编辑
          </el-button>
          
          <el-button
            v-if="authStore.hasPermission('system:user:update')"
            :type="row.status === UserStatus.Active ? 'warning' : 'success'"
            size="small"
            link
            @click="handleToggleStatus(row)"
          >
            {{ row.status === UserStatus.Active ? '禁用' : '启用' }}
          </el-button>
          
          <el-button
            v-if="authStore.hasPermission('system:user:update')"
            type="info"
            size="small"
            link
            @click="handleResetPassword(row)"
          >
            重置密码
          </el-button>
          
          <el-button
            v-if="authStore.hasPermission('system:user:delete')"
            type="danger"
            size="small"
            link
            @click="handleDelete(row)"
          >
            删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    
    <!-- 分页 -->
    <el-pagination
      v-model:current-page="pagination.pageIndex"
      v-model:page-size="pagination.pageSize"
      :total="pagination.totalCount"
      :page-sizes="[10, 20, 50, 100]"
      layout="total, sizes, prev, pager, next, jumper"
      @size-change="getUsers"
      @current-change="getUsers"
    />
    
    <!-- 用户表单对话框 -->
    <UserFormDialog
      v-model="showUserDialog"
      :user="currentUser"
      @success="handleFormSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox, FormInstance } from 'element-plus'
import { useAuthStore } from '@/stores/auth'
import { usersApi } from '@/api/users'
import UserFormDialog from './components/UserFormDialog.vue'
import type { User, UserQueryDto } from '@/types'
import { UserStatus } from '@/types'
import dayjs from 'dayjs'
import { ResetPasswordDto } from '@/types'

const authStore = useAuthStore()

const searchFormRef = ref<FormInstance>()
const loading = ref(false)
const users = ref<User[]>([])
const selectedIds = ref<number[]>([])
const showUserDialog = ref(false)
const currentUser = ref<User | null>(null)

// 搜索表单
const searchForm = reactive<UserQueryDto>({
  keyword: '',
  status: undefined,
  pageIndex: 1,
  pageSize: 20
})

// 分页信息
const pagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0
})

// 获取用户列表
const getUsers = async () => {
  try {
    loading.value = true
    
    const params = {
      ...searchForm,
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize
    }
    
    const response = await usersApi.getUsers(params)
    const data = response.data
    
    users.value = data.items
    pagination.totalCount = data.totalCount
    pagination.totalPages = data.totalPages
  } catch (error) {
    console.error('获取用户列表失败:', error)
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  pagination.pageIndex = 1
  getUsers()
}

// 重置搜索
const handleReset = () => {
  if (searchFormRef.value) {
    searchFormRef.value.resetFields()
  }
  Object.assign(searchForm, {
    keyword: '',
    status: undefined
  })
  handleSearch()
}

// 选择变化
const handleSelectionChange = (selection: User[]) => {
  selectedIds.value = selection.map(item => item.id)
}

// 新增用户
const handleCreate = () => {
  currentUser.value = null
  showUserDialog.value = true
}

// 编辑用户
const handleEdit = (user: User) => {
  currentUser.value = user
  showUserDialog.value = true
}

// 切换用户状态
const handleToggleStatus = async (user: User) => {
  try {
    const newStatus = user.status === UserStatus.Active ? UserStatus.Inactive : UserStatus.Active
    const action = newStatus === UserStatus.Active ? '启用' : '禁用'
    
    await ElMessageBox.confirm(`确定要${action}用户"${user.nickName}"吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    await usersApi.updateUserStatus(user.id, newStatus)
    
    ElMessage.success(`${action}成功`)
    getUsers()
  } catch (error) {
    // 用户取消操作
  }
}

// 重置密码
const handleResetPassword = async (user: User) => {
  try {
    await ElMessageBox.confirm(`确定要重置用户"${user.userName}"的密码吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    const newPassword = '123456' // 默认密码
    const payload: ResetPasswordDto = {
      newPassword: newPassword,
      confirmPassword: newPassword,
      userNameOrEmail: user.userName,
      code: '' // 后端不需要验证码，传空字符串即可
    }
    await usersApi.resetPassword(user.id, payload)
    
    ElMessage.success(`密码重置成功`)
  } catch (error) {
    // 用户取消操作
  }
}

// 删除用户
const handleDelete = async (user: User) => {
  try {
    await ElMessageBox.confirm(`确定要删除用户"${user.nickName}"吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    await usersApi.deleteUser(user.id)
    
    ElMessage.success('删除成功')
    getUsers()
  } catch (error) {
    // 用户取消操作
  }
}

// 批量删除
const handleBatchDelete = async () => {
  try {
    await ElMessageBox.confirm(`确定要删除选中的 ${selectedIds.value.length} 个用户吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    // 批量删除
    await Promise.all(selectedIds.value.map(id => usersApi.deleteUser(id)))
    
    ElMessage.success('批量删除成功')
    getUsers()
  } catch (error) {
    // 用户取消操作
  }
}

// 表单提交成功
const handleFormSuccess = () => {
  getUsers()
}

// 获取状态标签类型
const getStatusTagType = (status: UserStatus) => {
  switch (status) {
    case UserStatus.Active:
      return 'success'
    case UserStatus.Inactive:
      return 'danger'
    case UserStatus.Locked:
      return 'warning'
    default:
      return 'info'
  }
}

// 获取状态文本
const getStatusText = (status: UserStatus) => {
  switch (status) {
    case UserStatus.Active:
      return '正常'
    case UserStatus.Inactive:
      return '禁用'
    case UserStatus.Locked:
      return '锁定'
    default:
      return '未知'
  }
}

// 格式化日期
const formatDate = (date: string) => {
  return dayjs(date).format('YYYY-MM-DD HH:mm')
}

onMounted(() => {
  getUsers()
})
</script>

<style lang="scss" scoped>
.role-tag {
  margin-right: 4px;
  margin-bottom: 2px;
}

.el-pagination {
  margin-top: 20px;
  text-align: right;
}
</style>
