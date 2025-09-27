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
        <el-form-item label="菜单名称">
          <el-input
            v-model="searchForm.menuName"
            placeholder="菜单名称"
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
            <el-option label="启用" :value="true" />
            <el-option label="禁用" :value="false" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="类型">
          <el-select
            v-model="searchForm.menuType"
            placeholder="请选择类型"
            clearable
            style="width: 120px"
          >
            <el-option label="目录" :value="MenuType.Directory" />
            <el-option label="菜单" :value="MenuType.Menu" />
            <el-option label="按钮" :value="MenuType.Button" />
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
          v-if="authStore.hasPermission('system:menu:create')"
          type="primary"
          icon="Plus"
          @click="handleCreate"
        >
          新增菜单
        </el-button>
        
        <el-button icon="Sort" @click="expandAll">
          展开全部
        </el-button>
        
        <el-button icon="Sort" @click="collapseAll">
          收起全部
        </el-button>
      </div>
      
      <div class="toolbar-right">
        <el-button icon="Refresh" @click="getMenus">
          刷新
        </el-button>
      </div>
    </div>
    
    <!-- 菜单表格 -->
    <el-table
      ref="tableRef"
      v-loading="loading"
      :data="menus"
      row-key="id"
      :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
      :default-expand-all="false"
    >
      <el-table-column prop="menuName" label="菜单名称" min-width="200">
        <template #default="{ row }">
          <div class="menu-name">
            <el-icon v-if="row.icon" class="menu-icon">
              <component :is="row.icon" />
            </el-icon>
            <span>{{ row.menuName }}</span>
          </div>
        </template>
      </el-table-column>
      
      <el-table-column prop="menuCode" label="菜单编码" min-width="150" />
      
      <el-table-column label="类型" width="80">
        <template #default="{ row }">
          <el-tag
            :type="getMenuTypeTagType(row.menuType)"
            size="small"
          >
            {{ getMenuTypeText(row.menuType) }}
          </el-tag>
        </template>
      </el-table-column>
      
      <el-table-column prop="url" label="路由路径" min-width="150" show-overflow-tooltip />
      
      <el-table-column prop="component" label="组件路径" min-width="150" show-overflow-tooltip />
      
      <el-table-column prop="sort" label="排序" width="80" />
      
      <el-table-column label="状态" width="80">
        <template #default="{ row }">
          <el-tag
            :type="row.status ? 'success' : 'danger'"
            size="small"
          >
            {{ row.status ? '启用' : '禁用' }}
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
            v-if="authStore.hasPermission('system:menu:create')"
            type="success"
            size="small"
            link
            @click="handleCreateChild(row)"
          >
            新增
          </el-button>
          
          <el-button
            v-if="authStore.hasPermission('system:menu:update')"
            type="primary"
            size="small"
            link
            @click="handleEdit(row)"
          >
            编辑
          </el-button>
          
          <el-button
            v-if="authStore.hasPermission('system:menu:update')"
            :type="row.isActive ? 'warning' : 'success'"
            size="small"
            link
            @click="handleToggleStatus(row)"
          >
            {{ row.isActive ? '禁用' : '启用' }}
          </el-button>
          
          <el-button
            v-if="authStore.hasPermission('system:menu:delete')"
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
    
    <!-- 菜单表单对话框 -->
    <MenuFormDialog
      v-model="showMenuDialog"
      :menu="currentMenu"
      :parent-menu="parentMenu"
      @success="handleFormSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox, ElTable, FormInstance } from 'element-plus'
import { useAuthStore } from '@/stores/auth'
import { menusApi } from '@/api/menus'
import MenuFormDialog from './components/MenuFormDialog.vue'
import type { Menu, MenuQueryDto } from '@/types'
import { MenuType } from '@/types'
import dayjs from 'dayjs'

const authStore = useAuthStore()

const searchFormRef = ref<FormInstance>()
const tableRef = ref<InstanceType<typeof ElTable>>()
const loading = ref(false)
const menus = ref<Menu[]>([])
const showMenuDialog = ref(false)
const currentMenu = ref<Menu | null>(null)
const parentMenu = ref<Menu | null>(null)

// 搜索表单
const searchForm = reactive<MenuQueryDto>({
  menuName: '',
  status: undefined,
  menuType: undefined
})

// 获取菜单列表
const getMenus = async () => {
  try {
    loading.value = true
    
    const response = await menusApi.getMenuTree(searchForm)
    menus.value = response.data
  } catch (error) {
    console.error('获取菜单列表失败:', error)
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  getMenus()
}

// 重置搜索
const handleReset = () => {
  if (searchFormRef.value) {
    searchFormRef.value.resetFields()
  }
  Object.assign(searchForm, {
    menuName: '',
    status: undefined,
    menuType: undefined
  })
  handleSearch()
}

// 展开全部
const expandAll = () => {
  if (tableRef.value) {
    const expandRows = (rows: Menu[]) => {
      rows.forEach(row => {
        tableRef.value!.toggleRowExpansion(row, true)
        if (row.children && row.children.length > 0) {
          expandRows(row.children)
        }
      })
    }
    expandRows(menus.value)
  }
}

// 收起全部
const collapseAll = () => {
  if (tableRef.value) {
    const collapseRows = (rows: Menu[]) => {
      rows.forEach(row => {
        tableRef.value!.toggleRowExpansion(row, false)
        if (row.children && row.children.length > 0) {
          collapseRows(row.children)
        }
      })
    }
    collapseRows(menus.value)
  }
}

// 新增菜单
const handleCreate = () => {
  currentMenu.value = null
  parentMenu.value = null
  showMenuDialog.value = true
}

// 新增子菜单
const handleCreateChild = (menu: Menu) => {
  currentMenu.value = null
  parentMenu.value = menu
  showMenuDialog.value = true
}

// 编辑菜单
const handleEdit = (menu: Menu) => {
  currentMenu.value = menu
  parentMenu.value = null
  showMenuDialog.value = true
}

// 切换菜单状态
const handleToggleStatus = async (menu: Menu) => {
  try {
    const newStatus = !menu.status
    const action = newStatus ? '启用' : '禁用'
    
    await ElMessageBox.confirm(`确定要${action}菜单"${menu.menuName}"吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    await menusApi.updateMenuStatus(menu.id, newStatus)
    
    ElMessage.success(`${action}成功`)
    getMenus()
  } catch (error) {
    // 用户取消操作
  }
}

// 删除菜单
const handleDelete = async (menu: Menu) => {
  try {
    await ElMessageBox.confirm(`确定要删除菜单"${menu.menuName}"吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    await menusApi.deleteMenu(menu.id)
    
    ElMessage.success('删除成功')
    getMenus()
  } catch (error) {
    // 用户取消操作
  }
}

// 表单提交成功
const handleFormSuccess = () => {
  getMenus()
}

// 获取菜单类型标签类型
const getMenuTypeTagType = (type: MenuType) => {
  switch (type) {
    case MenuType.Directory:
      return 'info'
    case MenuType.Menu:
      return 'success'
    case MenuType.Button:
      return 'warning'
    default:
      return 'info'
  }
}

// 获取菜单类型文本
const getMenuTypeText = (type: MenuType) => {
  switch (type) {
    case MenuType.Directory:
      return '目录'
    case MenuType.Menu:
      return '菜单'
    case MenuType.Button:
      return '按钮'
    default:
      return '未知'
  }
}

// 格式化日期
const formatDate = (date: string) => {
  return dayjs(date).format('YYYY-MM-DD HH:mm')
}

onMounted(() => {
  getMenus()
})
</script>

<style lang="scss" scoped>
.menu-name {
  display: flex;
  align-items: center;
  
  .menu-icon {
    margin-right: 6px;
    color: #606266;
  }
}
</style>
