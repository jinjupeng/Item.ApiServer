<template>
  <div class="organization-management">
    <!-- 查询表单 -->
    <el-card class="search-card">
      <el-form
        ref="queryFormRef"
        :model="queryForm"
        :inline="true"
        label-width="80px"
      >
        <el-form-item label="组织名称" prop="orgName">
          <el-input
            v-model="queryForm.orgName"
            placeholder="请输入组织名称"
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

    <!-- 操作按钮和组织树 -->
    <el-card class="table-card">
      <div class="table-header">
        <el-button
          type="primary"
          :icon="Plus"
          @click="handleAdd"
          v-permission="['system:org:add']"
        >
          新增组织
        </el-button>
        <el-button
          :icon="Sort"
          @click="expandAll = !expandAll"
        >
          {{ expandAll ? '收起' : '展开' }}
        </el-button>
      </div>

      <!-- 组织树表格 -->
      <el-table
        v-loading="loading"
        :data="orgList"
        row-key="id"
        :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
        :default-expand-all="expandAll"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="orgName" label="组织名称" min-width="200" />
        <el-table-column prop="orgCode" label="组织编码" width="150" />
        <el-table-column prop="orgType" label="组织类型" width="120" align="center">
          <template #default="{ row }">
            <el-tag :type="getOrgTypeTag(row.orgType) as any">
              {{ getOrgTypeName(row.orgType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="leader" label="负责人" width="120" />
        <el-table-column prop="phone" label="联系电话" width="150" />
        <el-table-column prop="sort" label="排序" width="80" align="center" />
        <el-table-column prop="status" label="状态" width="100" align="center">
          <template #default="{ row }">
            <el-switch
              v-model="row.status"
              :disabled="!hasPermission(['system:org:edit'])"
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
              :icon="Plus"
              @click="handleAddChild(row)"
              v-permission="['system:org:add']"
            >
              新增
            </el-button>
            <el-button
              type="warning"
              size="small"
              :icon="Edit"
              @click="handleEdit(row)"
              v-permission="['system:org:edit']"
            >
              编辑
            </el-button>
            <el-button
              type="danger"
              size="small"
              :icon="Delete"
              @click="handleDelete(row)"
              v-permission="['system:org:delete']"
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
            <el-form-item label="上级组织" prop="parentId">
              <el-tree-select
                v-model="form.parentId"
                :data="orgTreeOptions"
                :props="{ value: 'id', label: 'orgName', children: 'children' }"
                placeholder="选择上级组织"
                check-strictly
                clearable
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="组织类型" prop="orgType">
              <el-select v-model="form.orgType" placeholder="请选择组织类型">
                <el-option label="公司" :value="1" />
                <el-option label="部门" :value="2" />
                <el-option label="小组" :value="3" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="组织名称" prop="orgName">
              <el-input v-model="form.orgName" placeholder="请输入组织名称" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="组织编码" prop="orgCode">
              <el-input v-model="form.orgCode" placeholder="请输入组织编码" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="负责人" prop="leader">
              <el-input v-model="form.leader" placeholder="请输入负责人" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="联系电话" prop="phone">
              <el-input v-model="form.phone" placeholder="请输入联系电话" />
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
            <el-form-item label="地址" prop="address">
              <el-input v-model="form.address" placeholder="请输入地址" />
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
        </el-row>

        <el-form-item label="描述" prop="description">
          <el-input
            v-model="form.description"
            type="textarea"
            :rows="3"
            placeholder="请输入组织描述"
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
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { Search, Refresh, Plus, Edit, Delete, Sort } from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'
import { formatDateTime } from '@/utils'
import type { Organization, OrganizationQuery, OrganizationForm } from '@/types/api'

// 权限检查
const authStore = useAuthStore()
const hasPermission = (permissions: string[]) => {
  return permissions.some(permission => authStore.hasPermission(permission))
}

// 响应式数据
const loading = ref(false)
const submitting = ref(false)
const expandAll = ref(true)
const orgList = ref<Organization[]>([])
const dialogVisible = ref(false)
const dialogTitle = ref('')

// 表单引用
const queryFormRef = ref<FormInstance>()
const formRef = ref<FormInstance>()

// 查询表单
const queryForm = reactive<OrganizationQuery>({
  orgName: '',
  status: undefined
})

// 编辑表单
const form = reactive<OrganizationForm>({
  id: undefined,
  parentId: null,
  orgName: '',
  orgCode: '',
  orgType: 1,
  leader: '',
  phone: '',
  email: '',
  address: '',
  description: '',
  sort: 0,
  status: true
})

// 表单验证规则
const rules: FormRules = {
  orgName: [
    { required: true, message: '请输入组织名称', trigger: 'blur' },
    { min: 2, max: 50, message: '组织名称长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  orgCode: [
    { required: true, message: '请输入组织编码', trigger: 'blur' },
    { pattern: /^[A-Za-z0-9_-]+$/, message: '组织编码只能包含字母、数字、下划线和横线', trigger: 'blur' }
  ],
  orgType: [
    { required: true, message: '请选择组织类型', trigger: 'change' }
  ],
  phone: [
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号码', trigger: 'blur' }
  ],
  email: [
    { type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur' }
  ]
}

// 组织树选项
const orgTreeOptions = computed(() => {
  const buildTree = (orgs: Organization[], parentId: number | null = null): any[] => {
    return orgs
      .filter(org => org.parentId === parentId)
      .map(org => ({
        id: org.id,
        orgName: org.orgName,
        children: buildTree(orgs, org.id)
      }))
  }
  
  return [
    { id: null, orgName: '根组织', children: buildTree(orgList.value) }
  ]
})

// 获取组织类型名称
const getOrgTypeName = (type: number) => {
  const typeMap: Record<number, string> = {
    1: '公司',
    2: '部门',
    3: '小组'
  }
  return typeMap[type] || '未知'
}

// 获取组织类型标签样式
const getOrgTypeTag = (type: number) => {
  const tagMap: Record<number, string> = {
    1: 'success',
    2: 'warning',
    3: 'info'
  }
  return tagMap[type] || ''
}

// 获取组织列表
const getOrgList = async () => {
  try {
    loading.value = true
    // TODO: 调用API获取组织列表
    // const { data } = await orgApi.getList(queryForm)
    // orgList.value = data
    
    // 模拟数据
    orgList.value = [
      {
        id: 1,
        parentId: null,
        orgName: '总公司',
        orgCode: 'COMPANY',
        orgType: 1,
        leader: '张三',
        phone: '13800138000',
        email: 'admin@company.com',
        address: '北京市朝阳区',
        description: '公司总部',
        sort: 1,
        status: true,
        createdAt: '2024-01-01 10:00:00',
        children: [
          {
            id: 11,
            parentId: 1,
            orgName: '技术部',
            orgCode: 'TECH',
            orgType: 2,
            leader: '李四',
            phone: '13800138001',
            email: 'tech@company.com',
            address: '北京市朝阳区',
            description: '技术研发部门',
            sort: 1,
            status: true,
            createdAt: '2024-01-01 10:00:00',
            children: [
              {
                id: 111,
                parentId: 11,
                orgName: '前端组',
                orgCode: 'FRONTEND',
                orgType: 3,
                leader: '王五',
                phone: '13800138002',
                email: 'frontend@company.com',
                address: '北京市朝阳区',
                description: '前端开发小组',
                sort: 1,
                status: true,
                createdAt: '2024-01-01 10:00:00'
              }
            ]
          }
        ]
      }
    ]
  } catch (error) {
    ElMessage.error('获取组织列表失败')
  } finally {
    loading.value = false
  }
}

// 查询
const handleQuery = () => {
  getOrgList()
}

// 重置查询
const resetQuery = () => {
  queryFormRef.value?.resetFields()
  handleQuery()
}

// 新增
const handleAdd = () => {
  dialogTitle.value = '新增组织'
  resetForm()
  dialogVisible.value = true
}

// 新增子组织
const handleAddChild = (row: Organization) => {
  dialogTitle.value = '新增子组织'
  resetForm()
  form.parentId = row.id
  form.orgType = row.orgType < 3 ? row.orgType + 1 : 3
  dialogVisible.value = true
}

// 编辑
const handleEdit = (row: Organization) => {
  dialogTitle.value = '编辑组织'
  resetForm()
  Object.assign(form, row)
  dialogVisible.value = true
}

// 删除
const handleDelete = async (row: Organization) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除组织"${row.orgName}"吗？`,
      '提示',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    // TODO: 调用删除API
    // await orgApi.delete(row.id)
    
    ElMessage.success('删除成功')
    getOrgList()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
    }
  }
}

// 状态变更
const handleStatusChange = async (row: Organization) => {
  try {
    // TODO: 调用状态变更API
    // await orgApi.updateStatus(row.id, row.status)
    
    ElMessage.success('状态更新成功')
  } catch (error) {
    // 恢复原状态
    row.status = !row.status
    ElMessage.error('状态更新失败')
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
      // await orgApi.update(form.id, form)
      ElMessage.success('更新成功')
    } else {
      // TODO: 调用新增API
      // await orgApi.create(form)
      ElMessage.success('新增成功')
    }
    
    dialogVisible.value = false
    getOrgList()
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
    orgName: '',
    orgCode: '',
    orgType: 1,
    leader: '',
    phone: '',
    email: '',
    address: '',
    description: '',
    sort: 0,
    status: true
  })
}

// 初始化
onMounted(() => {
  getOrgList()
})
</script>

<style scoped lang="scss">
.organization-management {
  .search-card {
    margin-bottom: 16px;
  }

  .table-card {
    .table-header {
      margin-bottom: 16px;
    }
  }

  .dialog-footer {
    text-align: right;
  }
}
</style>
