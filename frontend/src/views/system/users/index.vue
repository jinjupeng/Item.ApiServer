<template>
  <div class="users-page">
    <div class="page-header">
      <div class="header-title">
        <h2>用户管理</h2>
        <p>管理系统用户信息和权限</p>
      </div>
      <div class="header-actions">
        <el-button type="primary" @click="handleAdd" v-permission="['system:user:add']">
          <el-icon><Plus /></el-icon>
          新增用户
        </el-button>
      </div>
    </div>

    <!-- 搜索区域 -->
    <el-card class="search-card">
      <el-form :model="queryForm" :inline="true" label-width="80px">
        <el-form-item label="用户名">
          <el-input
            v-model="queryForm.username"
            placeholder="请输入用户名"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="昵称">
          <el-input
            v-model="queryForm.nickname"
            placeholder="请输入昵称"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="组织">
          <el-tree-select
            v-model="queryForm.orgId"
            :data="organizationTree"
            :props="{ label: 'orgName', value: 'id' }"
            placeholder="请选择组织"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="queryForm.status" placeholder="请选择状态" clearable style="width: 120px">
            <el-option label="启用" :value="1" />
            <el-option label="禁用" :value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            搜索
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            重置
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 表格区域 -->
    <el-card class="table-card">
      <div class="table-header">
        <div class="table-title">用户列表</div>
        <div class="table-actions">
          <el-button
            type="danger"
            :disabled="!selectedRows.length"
            @click="handleBatchDelete"
            v-permission="['system:user:delete']"
          >
            <el-icon><Delete /></el-icon>
            批量删除
          </el-button>
          <el-button @click="handleExport" v-permission="['system:user:export']">
            <el-icon><Download /></el-icon>
            导出
          </el-button>
          <el-button @click="handleImport" v-permission="['system:user:import']">
            <el-icon><Upload /></el-icon>
            导入
          </el-button>
        </div>
      </div>

      <el-table
        v-loading="loading"
        :data="tableData"
        @selection-change="handleSelectionChange"
        stripe
        style="width: 100%"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column label="头像" width="80">
          <template #default="{ row }">
            <el-avatar
              :size="40"
              :src="row.portrait"
              :style="{ backgroundColor: getAvatarColor(row.nickname || row.username) }"
            >
              {{ (row.nickname || row.username).charAt(0).toUpperCase() }}
            </el-avatar>
          </template>
        </el-table-column>
        <el-table-column prop="username" label="用户名" min-width="120" />
        <el-table-column prop="nickname" label="昵称" min-width="120" />
        <el-table-column prop="orgName" label="所属组织" min-width="150" />
        <el-table-column prop="phone" label="手机号" min-width="130" />
        <el-table-column prop="email" label="邮箱" min-width="180" />
        <el-table-column label="角色" min-width="150">
          <template #default="{ row }">
            <el-tag
              v-for="role in row.roles"
              :key="role"
              size="small"
              style="margin-right: 4px"
            >
              {{ role }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="状态" width="80">
          <template #default="{ row }">
            <el-switch
              v-model="row.status"
              :active-value="1"
              :inactive-value="0"
              @change="handleStatusChange(row)"
              v-permission="['system:user:edit']"
            />
          </template>
        </el-table-column>
        <el-table-column prop="createTime" label="创建时间" width="180">
          <template #default="{ row }">
            {{ formatDate(row.createTime) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button
              type="primary"
              size="small"
              @click="handleEdit(row)"
              v-permission="['system:user:edit']"
            >
              编辑
            </el-button>
            <el-button
              type="warning"
              size="small"
              @click="handleResetPassword(row)"
              v-permission="['system:user:reset']"
            >
              重置密码
            </el-button>
            <el-button
              type="danger"
              size="small"
              @click="handleDelete(row)"
              v-permission="['system:user:delete']"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="queryForm.pageIndex"
          v-model:page-size="queryForm.pageSize"
          :total="total"
          :page-sizes="[10, 20, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSearch"
          @current-change="handleSearch"
        />
      </div>
    </el-card>

    <!-- 用户表单对话框 -->
    <UserForm
      v-model:visible="formVisible"
      :form-data="formData"
      :is-edit="isEdit"
      @success="handleFormSuccess"
    />

    <!-- 导入对话框 -->
    <ImportDialog
      v-model:visible="importVisible"
      @success="handleImportSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { UserApi, OrganizationApi } from '@/api'
import { formatDate, getAvatarColor, confirm } from '@/utils'
import type { User, UserQueryDto, OrganizationTreeDto } from '@/types/api'
import UserForm from './components/UserForm.vue'
import ImportDialog from './components/ImportDialog.vue'
import { Plus, Search, Refresh, Delete, Download, Upload } from '@element-plus/icons-vue'

// 响应式数据
const loading = ref(false)
const tableData = ref<User[]>([])
const total = ref(0)
const selectedRows = ref<User[]>([])
const organizationTree = ref<OrganizationTreeDto[]>([])

// 表单相关
const formVisible = ref(false)
const formData = ref<any>({})
const isEdit = ref(false)

// 导入相关
const importVisible = ref(false)

// 查询表单
const queryForm = reactive<UserQueryDto>({
  pageIndex: 1,
  pageSize: 20,
  username: '',
  nickname: '',
  orgId: undefined,
  status: undefined
})

// 获取用户列表
const getUserList = async () => {
  try {
    loading.value = true
    const response = await UserApi.getUsers(queryForm)
    tableData.value = response.data.items
    total.value = response.data.total
  } catch (error) {
    console.error('获取用户列表失败:', error)
  } finally {
    loading.value = false
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

// 搜索
const handleSearch = () => {
  queryForm.pageIndex = 1
  getUserList()
}

// 重置
const handleReset = () => {
  Object.assign(queryForm, {
    pageIndex: 1,
    pageSize: 20,
    username: '',
    nickname: '',
    orgId: undefined,
    status: undefined
  })
  getUserList()
}

// 新增
const handleAdd = () => {
  formData.value = {}
  isEdit.value = false
  formVisible.value = true
}

// 编辑
const handleEdit = (row: User) => {
  formData.value = { ...row }
  isEdit.value = true
  formVisible.value = true
}

// 删除
const handleDelete = async (row: User) => {
  try {
    await confirm(`确定要删除用户 "${row.username}" 吗？`)
    await UserApi.deleteUser(row.id)
    ElMessage.success('删除成功')
    getUserList()
  } catch (error) {
    // 用户取消或删除失败
  }
}

// 批量删除
const handleBatchDelete = async () => {
  try {
    await confirm(`确定要删除选中的 ${selectedRows.value.length} 个用户吗？`)
    const ids = selectedRows.value.map(row => row.id)
    await UserApi.batchDeleteUsers(ids)
    ElMessage.success('批量删除成功')
    getUserList()
  } catch (error) {
    // 用户取消或删除失败
  }
}

// 状态变更
const handleStatusChange = async (row: User) => {
  try {
    await UserApi.updateUserStatus(row.id, row.status === 1)
    ElMessage.success('状态更新成功')
  } catch (error) {
    // 恢复原状态
    row.status = row.status === 1 ? 0 : 1
  }
}

// 重置密码
const handleResetPassword = async (row: User) => {
  try {
    await confirm(`确定要重置用户 "${row.username}" 的密码吗？`)
    await UserApi.resetUserPassword(row.id, '123456')
    ElMessage.success('密码重置成功，新密码为：123456')
  } catch (error) {
    // 用户取消或重置失败
  }
}

// 表格选择变更
const handleSelectionChange = (selection: User[]) => {
  selectedRows.value = selection
}

// 表单成功回调
const handleFormSuccess = () => {
  formVisible.value = false
  getUserList()
}

// 导出
const handleExport = async () => {
  try {
    await UserApi.exportUsers(queryForm)
    ElMessage.success('导出成功')
  } catch (error) {
    console.error('导出失败:', error)
  }
}

// 导入
const handleImport = () => {
  importVisible.value = true
}

// 导入成功回调
const handleImportSuccess = () => {
  importVisible.value = false
  getUserList()
}

// 初始化
onMounted(() => {
  getUserList()
  getOrganizationTree()
})
</script>

<style lang="scss" scoped>
.users-page {
  .page-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;

    .header-title {
      h2 {
        margin: 0 0 4px 0;
        font-size: 24px;
        font-weight: 600;
        color: var(--el-text-color-primary);
      }

      p {
        margin: 0;
        font-size: 14px;
        color: var(--el-text-color-secondary);
      }
    }
  }

  .search-card {
    margin-bottom: 20px;
    border-radius: 8px;
  }

  .table-card {
    border-radius: 8px;

    .table-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;

      .table-title {
        font-size: 16px;
        font-weight: 600;
        color: var(--el-text-color-primary);
      }

      .table-actions {
        display: flex;
        gap: 8px;
      }
    }

    .pagination-container {
      display: flex;
      justify-content: center;
      margin-top: 20px;
    }
  }
}

// 移动端适配
@media (max-width: 768px) {
  .users-page {
    .page-header {
      flex-direction: column;
      align-items: flex-start;
      gap: 16px;
    }

    .table-card {
      .table-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 12px;

        .table-actions {
          width: 100%;
          justify-content: flex-start;
          flex-wrap: wrap;
        }
      }
    }
  }
}
</style>
