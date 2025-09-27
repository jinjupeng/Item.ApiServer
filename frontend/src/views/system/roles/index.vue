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
            placeholder="角色名称/编码"
            clearable
            style="width: 200px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        
        <el-form-item label="状态">
          <el-select
            v-model="searchForm.isActive"
            placeholder="请选择状态"
            clearable
            style="width: 120px"
          >
            <el-option label="启用" :value="true" />
            <el-option label="禁用" :value="false" />
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
          v-if="authStore.hasPermission('system:role:create')"
          type="primary"
          icon="Plus"
          @click="handleCreate"
        >
          新增角色
        </el-button>
        
        <el-button
          v-if="authStore.hasPermission('system:role:delete')"
          type="danger"
          icon="Delete"
          :disabled="!selectedIds.length"
          @click="handleBatchDelete"
        >
          批量删除
        </el-button>
      </div>
      
      <div class="toolbar-right">
        <el-button icon="Refresh" @click="getRoles">
          刷新
        </el-button>
      </div>
    </div>
    
    <!-- 角色表格 -->
    <el-table
      v-loading="loading"
      :data="roles"
      @selection-change="handleSelectionChange"
    >
      <el-table-column type="selection" width="55" />
      
      <el-table-column prop="id" label="ID" width="80" />
      
      <el-table-column prop="name" label="角色名称" min-width="120" />
      
      <el-table-column prop="code" label="角色编码" min-width="120" />
      
      <el-table-column prop="description" label="描述" min-width="200" show-overflow-tooltip />
      
      <el-table-column label="状态" width="80">
        <template #default="{ row }">
          <el-tag
            :type="row.isActive ? 'success' : 'danger'"
            size="small"
          >
            {{ row.isActive ? '启用' : '禁用' }}
          </el-tag>
        </template>
      </el-table-column>
      
      <el-table-column prop="createdAt" label="创建时间" width="160">
        <template #default="{ row }">
          {{ formatDate(row.createdAt) }}
        </template>
      </el-table-column>
      
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button
            v-if="authStore.hasPermission('system:role:update')"
            type="primary"
            size="small"
            link
            @click="handleEdit(row)"
          >
            编辑
          </el-button>
          
          <el-button
            v-if="authStore.hasPermission('system:role:permission')"
            type="warning"
            size="small"
            link
            @click="handleAssignPermissions(row)"
          >
            分配权限
          </el-button>
          
          <el-button
            v-if="authStore.hasPermission('system:role:update')"
            :type="row.isActive ? 'warning' : 'success'"
            size="small"
            link
            @click="handleToggleStatus(row)"
          >
            {{ row.isActive ? '禁用' : '启用' }}
          </el-button>
          
          <el-button
            v-if="authStore.hasPermission('system:role:delete')"
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
      @size-change="getRoles"
      @current-change="getRoles"
    />
    
    <!-- 角色表单对话框 -->
    <RoleFormDialog
      v-model="showRoleDialog"
      :role="currentRole"
      @success="handleFormSuccess"
    />
    
    <!-- 权限分配对话框 -->
    <PermissionAssignDialog
      v-model="showPermissionDialog"
      :role="currentRole"
      @success="handleFormSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox, FormInstance } from 'element-plus'
import { useAuthStore } from '@/stores/auth'
import { rolesApi } from '@/api/roles'
import RoleFormDialog from './components/RoleFormDialog.vue'
import PermissionAssignDialog from './components/PermissionAssignDialog.vue'
import type { Role, RoleQueryDto } from '@/types'
import dayjs from 'dayjs'

const authStore = useAuthStore()

const searchFormRef = ref<FormInstance>()
const loading = ref(false)
const roles = ref<Role[]>([])
const selectedIds = ref<number[]>([])
const showRoleDialog = ref(false)
const showPermissionDialog = ref(false)
const currentRole = ref<Role | null>(null)

// 搜索表单
const searchForm = reactive<RoleQueryDto>({
  keyword: '',
  isActive: undefined,
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

// 获取角色列表
const getRoles = async () => {
  try {
    loading.value = true
    
    const params = {
      ...searchForm,
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize
    }
    
    const response = await rolesApi.getRoles(params)
    const data = response.data
    
    roles.value = data.items
    pagination.totalCount = data.totalCount
    pagination.totalPages = data.totalPages
  } catch (error) {
    console.error('获取角色列表失败:', error)
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  pagination.pageIndex = 1
  getRoles()
}

// 重置搜索
const handleReset = () => {
  if (searchFormRef.value) {
    searchFormRef.value.resetFields()
  }
  Object.assign(searchForm, {
    keyword: '',
    isActive: undefined
  })
  handleSearch()
}

// 选择变化
const handleSelectionChange = (selection: Role[]) => {
  selectedIds.value = selection.map(item => item.id)
}

// 新增角色
const handleCreate = () => {
  currentRole.value = null
  showRoleDialog.value = true
}

// 编辑角色
const handleEdit = (role: Role) => {
  currentRole.value = role
  showRoleDialog.value = true
}

// 分配权限
const handleAssignPermissions = (role: Role) => {
  currentRole.value = role
  showPermissionDialog.value = true
}

// 切换角色状态
const handleToggleStatus = async (role: Role) => {
  try {
    const newStatus = !role.isActive
    const action = newStatus ? '启用' : '禁用'
    
    await ElMessageBox.confirm(`确定要${action}角色"${role.name}"吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    await rolesApi.updateRoleStatus(role.id, newStatus)
    
    ElMessage.success(`${action}成功`)
    getRoles()
  } catch (error) {
    // 用户取消操作
  }
}

// 删除角色
const handleDelete = async (role: Role) => {
  try {
    await ElMessageBox.confirm(`确定要删除角色"${role.name}"吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    await rolesApi.deleteRole(role.id)
    
    ElMessage.success('删除成功')
    getRoles()
  } catch (error) {
    // 用户取消操作
  }
}

// 批量删除
const handleBatchDelete = async () => {
  try {
    await ElMessageBox.confirm(`确定要删除选中的 ${selectedIds.value.length} 个角色吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    // 批量删除
    await Promise.all(selectedIds.value.map(id => rolesApi.deleteRole(id)))
    
    ElMessage.success('批量删除成功')
    getRoles()
  } catch (error) {
    // 用户取消操作
  }
}

// 表单提交成功
const handleFormSuccess = () => {
  getRoles()
}

// 格式化日期
const formatDate = (date: string) => {
  return dayjs(date).format('YYYY-MM-DD HH:mm')
}

onMounted(() => {
  getRoles()
})
</script>

<style lang="scss" scoped>
.el-pagination {
  margin-top: 20px;
  text-align: right;
}
</style>
