<template>
  <el-dialog
    v-model="visible"
    title="分配权限"
    width="600px"
    :before-close="handleClose"
  >
    <div class="permission-assign">
      <div class="role-info">
        <el-descriptions :column="2">
          <el-descriptions-item label="角色名称">
            {{ role?.name }}
          </el-descriptions-item>
          <el-descriptions-item label="角色编码">
            {{ role?.code }}
          </el-descriptions-item>
        </el-descriptions>
      </div>
      
      <div class="menu-tree-container">
        <div class="tree-header">
          <span>菜单权限</span>
          <div class="tree-actions">
            <el-button size="small" @click="expandAll">
              展开全部
            </el-button>
            <el-button size="small" @click="collapseAll">
              收起全部
            </el-button>
            <el-button size="small" @click="checkAll">
              全选
            </el-button>
            <el-button size="small" @click="uncheckAll">
              取消全选
            </el-button>
          </div>
        </div>
        
        <el-tree
          ref="menuTreeRef"
          v-loading="treeLoading"
          :data="menuTree"
          :props="treeProps"
          :default-checked-keys="checkedKeys"
          :default-expanded-keys="expandedKeys"
          show-checkbox
          node-key="id"
          check-strictly
          class="menu-tree"
        >
          <template #default="{ node, data }">
            <div class="tree-node">
              <el-icon v-if="data.icon" class="node-icon">
                <component :is="data.icon" />
              </el-icon>
              <span class="node-label">{{ data.menuName }}</span>
              <el-tag
                v-if="data.type !== undefined"
                :type="getMenuTypeTagType(data.menuType)"
                size="small"
                class="node-type"
              >
                {{ getMenuTypeText(data.menuType) }}
              </el-tag>
            </div>
          </template>
        </el-tree>
      </div>
    </div>
    
    <template #footer>
      <div class="dialog-footer">
        <el-button @click="handleClose">取消</el-button>
        <el-button type="primary" :loading="loading" @click="handleSubmit">
          确定
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, watch, nextTick } from 'vue'
import { ElMessage, ElTree } from 'element-plus'
import { menusApi } from '@/api/menus'
import type { Role, Menu } from '@/types'
import { MenuType } from '@/types'

interface Props {
  modelValue: boolean
  role?: Role | null
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const menuTreeRef = ref<InstanceType<typeof ElTree>>()
const loading = ref(false)
const treeLoading = ref(false)
const visible = ref(false)
const menuTree = ref<Menu[]>([])
const checkedKeys = ref<number[]>([])
const expandedKeys = ref<number[]>([])

// 树形组件属性配置
const treeProps = {
  children: 'children',
  label: 'name',
  disabled: (data: any) => !data.status
}

// 监听 modelValue 变化
watch(() => props.modelValue, (val) => {
  visible.value = val
  if (val && props.role) {
    loadMenuTree()
    loadRoleMenus()
  }
})

// 监听 visible 变化
watch(visible, (val) => {
  emit('update:modelValue', val)
  if (!val) {
    resetData()
  }
})

// 加载菜单树
const loadMenuTree = async () => {
  try {
    treeLoading.value = true
    
    const [treeResponse, expandedResponse] = await Promise.all([
      menusApi.getMenuTreeForRoleAssign(props.role?.id),
      menusApi.getExpandedKeys()
    ])
    
    menuTree.value = treeResponse.data
    expandedKeys.value = expandedResponse.data.map(key => parseInt(key))
  } catch (error) {
    console.error('加载菜单树失败:', error)
  } finally {
    treeLoading.value = false
  }
}

// 加载角色已有菜单
const loadRoleMenus = async () => {
  try {
    if (!props.role) return
    
    const response = await menusApi.getCheckedKeysByRoleId(props.role.id)
    checkedKeys.value = response.data.map(key => parseInt(key))
    
    // 等待树渲染完成后设置选中状态
    await nextTick()
    if (menuTreeRef.value) {
      menuTreeRef.value.setCheckedKeys(checkedKeys.value)
    }
  } catch (error) {
    console.error('加载角色菜单失败:', error)
  }
}

// 展开全部
const expandAll = () => {
  if (menuTreeRef.value) {
    const allKeys = getAllNodeKeys(menuTree.value)
    expandedKeys.value = allKeys
    allKeys.forEach(key => {
      menuTreeRef.value!.store.nodesMap[key]?.expand()
    })
  }
}

// 收起全部
const collapseAll = () => {
  if (menuTreeRef.value) {
    expandedKeys.value = []
    const allKeys = getAllNodeKeys(menuTree.value)
    allKeys.forEach(key => {
      menuTreeRef.value!.store.nodesMap[key]?.collapse()
    })
  }
}

// 全选
const checkAll = () => {
  if (menuTreeRef.value) {
    const allKeys = getAllNodeKeys(menuTree.value)
    menuTreeRef.value.setCheckedKeys(allKeys)
  }
}

// 取消全选
const uncheckAll = () => {
  if (menuTreeRef.value) {
    menuTreeRef.value.setCheckedKeys([])
  }
}

// 获取所有节点的key
const getAllNodeKeys = (nodes: Menu[]): number[] => {
  const keys: number[] = []
  
  const traverse = (nodeList: Menu[]) => {
    nodeList.forEach(node => {
      keys.push(node.id)
      if (node.children && node.children.length > 0) {
        traverse(node.children)
      }
    })
  }
  
  traverse(nodes)
  return keys
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

// 重置数据
const resetData = () => {
  menuTree.value = []
  checkedKeys.value = []
  expandedKeys.value = []
}

// 关闭对话框
const handleClose = () => {
  visible.value = false
}

// 提交表单
const handleSubmit = async () => {
  if (!props.role || !menuTreeRef.value) return
  
  try {
    loading.value = true
    
    // 获取选中的菜单ID
    const checkedMenuIds = menuTreeRef.value.getCheckedKeys() as number[]
    
    // 保存角色菜单权限
    await menusApi.saveRoleMenuPermissions(props.role.id, checkedMenuIds)
    
    ElMessage.success('权限分配成功')
    emit('success')
    handleClose()
  } catch (error) {
    console.error('保存权限失败:', error)
  } finally {
    loading.value = false
  }
}
</script>

<style lang="scss" scoped>
.permission-assign {
  .role-info {
    margin-bottom: 20px;
  }
  
  .menu-tree-container {
    .tree-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 12px;
      padding-bottom: 8px;
      border-bottom: 1px solid #ebeef5;
      
      .tree-actions {
        display: flex;
        gap: 8px;
      }
    }
    
    .menu-tree {
      max-height: 400px;
      overflow-y: auto;
      border: 1px solid #ebeef5;
      border-radius: 4px;
      padding: 8px;
      
      .tree-node {
        display: flex;
        align-items: center;
        flex: 1;
        
        .node-icon {
          margin-right: 6px;
          color: #606266;
        }
        
        .node-label {
          flex: 1;
          margin-right: 8px;
        }
        
        .node-type {
          font-size: 12px;
        }
      }
    }
  }
}
</style>
