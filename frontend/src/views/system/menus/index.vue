<template>
  <div class="menu-management">
    <!-- 操作按钮 -->
    <el-card class="table-card">
      <div class="table-header">
        <el-button
          type="primary"
          :icon="Plus"
          @click="handleAdd"
          v-permission="['system:menu:add']"
        >
          新增菜单
        </el-button>
        <el-button
          :icon="Sort"
          @click="expandAll = !expandAll"
        >
          {{ expandAll ? '收起' : '展开' }}
        </el-button>
      </div>

      <!-- 菜单树表格 -->
      <el-table
        v-loading="loading"
        :data="menuList"
        row-key="id"
        :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
        :default-expand-all="expandAll"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="title" label="菜单名称" min-width="200">
          <template #default="{ row }">
            <el-icon v-if="row.icon" style="margin-right: 5px">
              <component :is="row.icon" />
            </el-icon>
            {{ row.title }}
          </template>
        </el-table-column>
        <el-table-column prop="path" label="路由路径" width="200" />
        <el-table-column prop="component" label="组件路径" width="200" />
        <el-table-column prop="sort" label="排序" width="80" align="center" />
        <el-table-column prop="status" label="状态" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="row.status ? 'success' : 'danger'">
              {{ row.status ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="hidden" label="显示状态" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="row.hidden ? 'warning' : 'success'">
              {{ row.hidden ? '隐藏' : '显示' }}
            </el-tag>
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
              :icon="Plus"
              @click="handleAddChild(row)"
              v-permission="['system:menu:add']"
            >
              新增
            </el-button>
            <el-button
              type="warning"
              size="small"
              :icon="Edit"
              @click="handleEdit(row)"
              v-permission="['system:menu:edit']"
            >
              编辑
            </el-button>
            <el-button
              type="danger"
              size="small"
              :icon="Delete"
              @click="handleDelete(row)"
              v-permission="['system:menu:delete']"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 新增/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="800px"
      :before-close="handleDialogClose"
    >
      <el-form
        ref="formRef"
        :model="form"
        :rules="rules"
        label-width="100px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="上级菜单" prop="parentId">
              <el-tree-select
                v-model="form.parentId"
                :data="menuTreeOptions"
                :props="{ value: 'id', label: 'title', children: 'children' }"
                placeholder="选择上级菜单"
                check-strictly
                clearable
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="菜单类型" prop="type">
              <el-radio-group v-model="form.type">
                <el-radio :label="1">目录</el-radio>
                <el-radio :label="2">菜单</el-radio>
                <el-radio :label="3">按钮</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="菜单名称" prop="title">
              <el-input v-model="form.title" placeholder="请输入菜单名称" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="菜单图标" prop="icon">
              <el-input v-model="form.icon" placeholder="请输入图标名称">
                <template #append>
                  <el-button @click="showIconDialog = true">选择</el-button>
                </template>
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20" v-if="form.type !== 3">
          <el-col :span="12">
            <el-form-item label="路由路径" prop="path">
              <el-input v-model="form.path" placeholder="请输入路由路径" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="组件路径" prop="component">
              <el-input v-model="form.component" placeholder="请输入组件路径" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20" v-if="form.type === 3">
          <el-col :span="12">
            <el-form-item label="权限标识" prop="permission">
              <el-input v-model="form.permission" placeholder="请输入权限标识" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="排序" prop="sort">
              <el-input-number v-model="form.sort" :min="0" :max="9999" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="状态" prop="status">
              <el-switch v-model="form.status" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="显示状态" prop="hidden">
              <el-switch v-model="form.hidden" active-text="隐藏" inactive-text="显示" />
            </el-form-item>
          </el-col>
        </el-row>
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

    <!-- 图标选择对话框 -->
    <el-dialog v-model="showIconDialog" title="选择图标" width="800px">
      <div class="icon-selector">
        <div class="icon-list">
          <div
            v-for="icon in iconList"
            :key="icon"
            class="icon-item"
            :class="{ active: form.icon === icon }"
            @click="selectIcon(icon)"
          >
            <el-icon><component :is="icon" /></el-icon>
            <span>{{ icon }}</span>
          </div>
        </div>
      </div>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="showIconDialog = false">取消</el-button>
          <el-button type="primary" @click="showIconDialog = false">确定</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { Plus, Edit, Delete, Sort } from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'
import { formatDateTime } from '@/utils'
import type { Menu, MenuForm } from '@/types/api'

// 权限检查
const authStore = useAuthStore()
const hasPermission = (permissions: string[]) => {
  return permissions.some(permission => authStore.hasPermission(permission))
}

// 响应式数据
const loading = ref(false)
const submitting = ref(false)
const expandAll = ref(true)
const menuList = ref<Menu[]>([])
const dialogVisible = ref(false)
const dialogTitle = ref('')
const showIconDialog = ref(false)

// 表单引用
const formRef = ref<FormInstance>()

// 编辑表单
const form = reactive<MenuForm>({
  id: undefined,
  parentId: null,
  title: '',
  path: '',
  component: '',
  icon: '',
  type: 1,
  sort: 0,
  status: true,
  hidden: false,
  permission: ''
})

// 表单验证规则
const rules: FormRules = {
  title: [
    { required: true, message: '请输入菜单名称', trigger: 'blur' },
    { min: 2, max: 50, message: '菜单名称长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  type: [
    { required: true, message: '请选择菜单类型', trigger: 'change' }
  ],
  path: [
    { required: true, message: '请输入路由路径', trigger: 'blur' }
  ],
  component: [
    { required: true, message: '请输入组件路径', trigger: 'blur' }
  ],
  permission: [
    { required: true, message: '请输入权限标识', trigger: 'blur' }
  ]
}

// 菜单树选项
const menuTreeOptions = computed(() => {
  const buildTree = (menus: Menu[], parentId: number | null = null): any[] => {
    return menus
      .filter(menu => menu.parentId === parentId && menu.type !== 3)
      .map(menu => ({
        id: menu.id,
        title: menu.title,
        children: buildTree(menus, menu.id)
      }))
  }
  
  return [
    { id: null, title: '根目录', children: buildTree(menuList.value) }
  ]
})

// 图标列表
const iconList = [
  'Dashboard', 'User', 'UserFilled', 'Menu', 'Setting', 'Lock', 'Unlock',
  'Document', 'Folder', 'FolderOpened', 'Files', 'Edit', 'Delete', 'Plus',
  'Search', 'Refresh', 'Download', 'Upload', 'Star', 'StarFilled'
]

// 获取菜单列表
const getMenuList = async () => {
  try {
    loading.value = true
    // TODO: 调用API获取菜单列表
    // const { data } = await menuApi.getList()
    // menuList.value = data
    
    // 模拟数据
    menuList.value = [
      {
        id: 1,
        parentId: null,
        title: '系统管理',
        path: '/system',
        component: 'Layout',
        icon: 'Setting',
        type: 1,
        sort: 1,
        status: true,
        hidden: false,
        createdAt: '2024-01-01 10:00:00',
        children: [
          {
            id: 11,
            parentId: 1,
            title: '用户管理',
            path: 'users',
            component: '@/views/system/users/index.vue',
            icon: 'User',
            type: 2,
            sort: 1,
            status: true,
            hidden: false,
            createdAt: '2024-01-01 10:00:00'
          },
          {
            id: 12,
            parentId: 1,
            title: '角色管理',
            path: 'roles',
            component: '@/views/system/roles/index.vue',
            icon: 'UserFilled',
            type: 2,
            sort: 2,
            status: true,
            hidden: false,
            createdAt: '2024-01-01 10:00:00'
          }
        ]
      }
    ]
  } catch (error) {
    ElMessage.error('获取菜单列表失败')
  } finally {
    loading.value = false
  }
}

// 新增
const handleAdd = () => {
  dialogTitle.value = '新增菜单'
  resetForm()
  dialogVisible.value = true
}

// 新增子菜单
const handleAddChild = (row: Menu) => {
  dialogTitle.value = '新增子菜单'
  resetForm()
  form.parentId = row.id
  form.type = row.type === 1 ? 2 : 3
  dialogVisible.value = true
}

// 编辑
const handleEdit = (row: Menu) => {
  dialogTitle.value = '编辑菜单'
  resetForm()
  Object.assign(form, row)
  dialogVisible.value = true
}

// 删除
const handleDelete = async (row: Menu) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除菜单"${row.title}"吗？`,
      '提示',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    // TODO: 调用删除API
    // await menuApi.delete(row.id)
    
    ElMessage.success('删除成功')
    getMenuList()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
    }
  }
}

// 选择图标
const selectIcon = (icon: string) => {
  form.icon = icon
}

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    submitting.value = true
    
    if (form.id) {
      // TODO: 调用更新API
      // await menuApi.update(form.id, form)
      ElMessage.success('更新成功')
    } else {
      // TODO: 调用新增API
      // await menuApi.create(form)
      ElMessage.success('新增成功')
    }
    
    dialogVisible.value = false
    getMenuList()
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
    parentId: null,
    title: '',
    path: '',
    component: '',
    icon: '',
    type: 1,
    sort: 0,
    status: true,
    hidden: false,
    permission: ''
  })
}

// 初始化
onMounted(() => {
  getMenuList()
})
</script>

<style scoped lang="scss">
.menu-management {
  .table-card {
    .table-header {
      margin-bottom: 16px;
    }
  }

  .dialog-footer {
    text-align: right;
  }

  .icon-selector {
    .icon-list {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
      gap: 10px;
      max-height: 400px;
      overflow-y: auto;

      .icon-item {
        display: flex;
        flex-direction: column;
        align-items: center;
        padding: 10px;
        border: 1px solid #dcdfe6;
        border-radius: 4px;
        cursor: pointer;
        transition: all 0.3s;

        &:hover {
          border-color: #409eff;
          background-color: #f0f9ff;
        }

        &.active {
          border-color: #409eff;
          background-color: #409eff;
          color: white;
        }

        .el-icon {
          font-size: 24px;
          margin-bottom: 5px;
        }

        span {
          font-size: 12px;
        }
      }
    }
  }
}
</style>
