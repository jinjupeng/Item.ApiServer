<template>
  <div class="role-management">
    <!-- 查询表单 -->
    <el-card class="search-card">
      <el-form
        ref="queryFormRef"
        :model="queryForm"
        :inline="true"
        label-width="80px"
      >
        <el-form-item label="角色名称" prop="roleName">
          <el-input
            v-model="queryForm.roleName"
            placeholder="请输入角色名称"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="状态" prop="status">
          <el-select
            v-model="queryForm.status"
            placeholder="请选择状态"
            clearable
            style="width: 120px"
          >
            <el-option label="启用" :value="true" />
            <el-option label="禁用" :value="false" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" :icon="Search" @click="handleQuery">
            查询
          </el-button>
          <el-button :icon="Refresh" @click="resetQuery">
            重置
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 操作按钮 -->
    <el-card class="table-card">
      <div class="table-header">
        <el-button
          type="primary"
          :icon="Plus"
          @click="handleAdd"
          v-permission="['system:role:add']"
        >
          新增角色
        </el-button>
      </div>

      <!-- 数据表格 -->
      <el-table
        v-loading="loading"
        :data="roleList"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="roleName" label="角色名称" width="150" />
        <el-table-column prop="roleDesc" label="角色描述" min-width="200" />
        <el-table-column prop="status" label="状态" width="100" align="center">
          <template #default="{ row }">
            <el-switch
              v-model="row.status"
              :disabled="!hasPermission(['system:role:edit'])"
              @change="handleStatusChange(row)"
            />
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" align="center" fixed="right">
          <template #default="{ row }">
            <el-button
              type="primary"
              size="small"
              :icon="Edit"
              @click="handleEdit(row)"
              v-permission="['system:role:edit']"
            >
              编辑
            </el-button>
            <el-button
              type="warning"
              size="small"
              :icon="Key"
              @click="handlePermission(row)"
              v-permission="['system:role:permission']"
            >
              权限
            </el-button>
            <el-button
              type="danger"
              size="small"
              :icon="Delete"
              @click="handleDelete(row)"
              v-permission="['system:role:delete']"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="pagination.page"
          v-model:page-size="pagination.size"
          :total="pagination.total"
          :page-sizes="[10, 20, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleQuery"
          @current-change="handleQuery"
        />
      </div>
    </el-card>

    <!-- 新增/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="600px"
      :before-close="handleDialogClose"
    >
      <el-form
        ref="formRef"
        :model="form"
        :rules="rules"
        label-width="80px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="角色名称" prop="roleName">
              <el-input v-model="form.roleName" placeholder="请输入角色名称" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="状态" prop="status">
              <el-switch v-model="form.status" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="角色描述" prop="roleDesc">
          <el-input
            v-model="form.roleDesc"
            type="textarea"
            :rows="3"
            placeholder="请输入角色描述"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="handleDialogClose">取消</el-button>
          <el-button type="primary" @click="handleSubmit" :loading="submitting">
            确定
          </el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 权限分配对话框 -->
    <el-dialog
      v-model="permissionDialogVisible"
      title="权限分配"
      width="800px"
    >
      <el-tabs v-model="activeTab">
        <el-tab-pane label="菜单权限" name="menu">
          <el-tree
            ref="menuTreeRef"
            :data="menuTree"
            :props="{ children: 'children', label: 'title' }"
            show-checkbox
            node-key="id"
            :default-checked-keys="checkedMenus"
            :default-expand-all="true"
          />
        </el-tab-pane>
      </el-tabs>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="permissionDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handlePermissionSubmit">
            确定
          </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { Search, Refresh, Plus, Edit, Delete, Key } from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'
import { formatDateTime } from '@/utils'
import type { Role, RoleQuery, RoleForm } from '@/types/api'

// 权限检查
const authStore = useAuthStore()
const hasPermission = (permissions: string[]) => {
  return permissions.some(permission => authStore.hasPermission(permission))
}

// 响应式数据
const loading = ref(false)
const submitting = ref(false)
const roleList = ref<Role[]>([])
const dialogVisible = ref(false)
const dialogTitle = ref('')
const permissionDialogVisible = ref(false)
const activeTab = ref('menu')
const menuTree = ref<Array<{ id: number; title: string; children?: Array<{ id: number; title: string }> }>>([])
const checkedMenus = ref<number[]>([])
const currentRole = ref<Role | null>(null)

// 表单引用
const queryFormRef = ref<FormInstance>()
const formRef = ref<FormInstance>()
const menuTreeRef = ref()

// 查询表单
const queryForm = reactive<RoleQuery>({
  roleName: '',
  status: undefined
})

// 分页
const pagination = reactive({
  page: 1,
  size: 10,
  total: 0
})

// 编辑表单
const form = reactive<RoleForm>({
  id: undefined,
  roleName: '',
  roleDesc: '',
  status: true
})

// 表单验证规则
const rules: FormRules = {
  roleName: [
    { required: true, message: '请输入角色名称', trigger: 'blur' },
    { min: 2, max: 32, message: '角色名称长度在 2 到 32 个字符', trigger: 'blur' }
  ],
  roleDesc: [
    { max: 128, message: '角色描述不能超过 128 个字符', trigger: 'blur' }
  ]
}

// 查询角色列表
const getRoleList = async () => {
  try {
    loading.value = true
    // TODO: 调用API获取角色列表
    // const { data } = await roleApi.getList({
    //   ...queryForm,
    //   page: pagination.page,
    //   size: pagination.size
    // })
    // roleList.value = data.list
    // pagination.total = data.total
    
    // 模拟数据
    roleList.value = [
      {
        id: 1,
        roleName: '超级管理员',
        roleDesc: '系统超级管理员，拥有所有权限',
        status: true,
        createdAt: '2024-01-01 10:00:00'
      },
      {
        id: 2,
        roleName: '普通用户',
        roleDesc: '普通用户角色',
        status: true,
        createdAt: '2024-01-02 10:00:00'
      }
    ]
    pagination.total = 2
  } catch (error) {
    ElMessage.error('获取角色列表失败')
  } finally {
    loading.value = false
  }
}

// 查询
const handleQuery = () => {
  pagination.page = 1
  getRoleList()
}

// 重置查询
const resetQuery = () => {
  queryFormRef.value?.resetFields()
  handleQuery()
}

// 新增
const handleAdd = () => {
  dialogTitle.value = '新增角色'
  resetForm()
  dialogVisible.value = true
}

// 编辑
const handleEdit = (row: Role) => {
  dialogTitle.value = '编辑角色'
  resetForm()
  Object.assign(form, row)
  dialogVisible.value = true
}

// 删除
const handleDelete = async (row: Role) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除角色"${row.roleName}"吗？`,
      '提示',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    // TODO: 调用删除API
    // await roleApi.delete(row.id)
    
    ElMessage.success('删除成功')
    getRoleList()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
    }
  }
}

// 状态变更
const handleStatusChange = async (row: Role) => {
  try {
    // TODO: 调用状态变更API
    // await roleApi.updateStatus(row.id, row.status)
    
    ElMessage.success('状态更新成功')
  } catch (error) {
    // 恢复原状态
    row.status = !row.status
    ElMessage.error('状态更新失败')
  }
}

// 权限分配
const handlePermission = async (row: Role) => {
  currentRole.value = row
  
  try {
    // TODO: 获取菜单树和已选中的权限
    // const { data } = await roleApi.getPermissions(row.id)
    // menuTree.value = data.menuTree
    // checkedMenus.value = data.checkedMenus
    
    // 模拟数据
    menuTree.value = [
      {
        id: 1,
        title: '系统管理',
        children: [
          { id: 11, title: '用户管理' },
          { id: 12, title: '角色管理' },
          { id: 13, title: '菜单管理' }
        ]
      }
    ]
    checkedMenus.value = [11, 12]
    
    permissionDialogVisible.value = true
  } catch (error) {
    ElMessage.error('获取权限信息失败')
  }
}

// 提交权限
const handlePermissionSubmit = async () => {
  if (!currentRole.value) return
  
  try {
    const checkedKeys = menuTreeRef.value.getCheckedKeys()
    const halfCheckedKeys = menuTreeRef.value.getHalfCheckedKeys()
    const allCheckedKeys = [...checkedKeys, ...halfCheckedKeys]
    
    // TODO: 调用权限分配API
    // await roleApi.assignPermissions(currentRole.value.id, allCheckedKeys)
    
    ElMessage.success('权限分配成功')
    permissionDialogVisible.value = false
  } catch (error) {
    ElMessage.error('权限分配失败')
  }
}

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    submitting.value = true
    
    if (form.id) {
      // TODO: 调用更新API
      // await roleApi.update(form.id, form)
      ElMessage.success('更新成功')
    } else {
      // TODO: 调用新增API
      // await roleApi.create(form)
      ElMessage.success('新增成功')
    }
    
    dialogVisible.value = false
    getRoleList()
  } catch (error) {
    if (error !== false) {
      ElMessage.error(form.id ? '更新失败' : '新增失败')
    }
  } finally {
    submitting.value = false
  }
}

// 关闭对话框
const handleDialogClose = () => {
  formRef.value?.resetFields()
  dialogVisible.value = false
}

// 重置表单
const resetForm = () => {
  Object.assign(form, {
    id: undefined,
    roleName: '',
    roleDesc: '',
    status: true
  })
}

// 初始化
onMounted(() => {
  getRoleList()
})
</script>

<style scoped lang="scss">
.role-management {
  .search-card {
    margin-bottom: 16px;
  }

  .table-card {
    .table-header {
      margin-bottom: 16px;
    }
  }

  .pagination-container {
    display: flex;
    justify-content: center;
    margin-top: 20px;
  }

  .dialog-footer {
    text-align: right;
  }
}
</style>
