import { ref, reactive } from 'vue'
import type { FormInstance } from 'element-plus'
import { ElMessage } from 'element-plus'

/**
 * CRUD操作通用组合式函数
 * 用于替代Vue2中的MixinCUD组件
 */
export function useCrud<T extends Record<string, any> = Record<string, any>>() {
  // 对话框状态
  const dialogVisible = ref(false)
  const dialogTitle = ref('')
  const dialogLoading = ref(false)
  
  // 表单引用
  const dialogFormRef = ref<FormInstance>()
  const queryFormRef = ref<FormInstance>()
  
  // 表单数据
  const dialogForm = reactive({} as T)
  
  // 提交查询表单
  const submitQueryForm = (callback?: () => void) => {
    callback?.()
  }
  
  // 重置查询表单
  const resetQueryForm = (callback?: () => void) => {
    queryFormRef.value?.resetFields()
    callback?.()
  }
  
  // 处理新增
  const handleAdd = (title: string = '新增', formData?: Partial<T>) => {
    dialogVisible.value = true
    dialogTitle.value = title
    resetDialogForm(formData)
  }
  
  // 处理编辑
  const handleEdit = (row: T, title: string = '编辑') => {
    dialogVisible.value = true
    dialogTitle.value = title
    Object.assign(dialogForm, row)
  }
  
  // 重置对话框表单
  const resetDialogForm = (defaultData?: Partial<T>) => {
    Object.keys(dialogForm).forEach(key => {
      const value = (dialogForm as any)[key]
      if (typeof value === 'string') {
        ;(dialogForm as any)[key] = ''
      } else if (Array.isArray(value)) {
        ;(dialogForm as any)[key] = []
      } else {
        ;(dialogForm as any)[key] = null
      }
    })
    
    if (defaultData) {
      Object.assign(dialogForm, defaultData)
    }
  }
  
  // 提交对话框表单
  const submitDialogForm = async (callback: (formData: T) => Promise<void>) => {
    if (!dialogFormRef.value) return
    
    try {
      await dialogFormRef.value.validate()
      dialogLoading.value = true
      await callback(dialogForm as T)
      dialogVisible.value = false
      ElMessage.success('操作成功')
    } catch (error) {
      console.error('表单提交失败:', error)
    } finally {
      dialogLoading.value = false
    }
  }
  
  // 关闭对话框
  const closeDialog = () => {
    dialogVisible.value = false
    dialogFormRef.value?.resetFields()
  }
  
  return {
    // 状态
    dialogVisible,
    dialogTitle,
    dialogLoading,
    dialogForm,
    
    // 引用
    dialogFormRef,
    queryFormRef,
    
    // 方法
    submitQueryForm,
    resetQueryForm,
    handleAdd,
    handleEdit,
    resetDialogForm,
    submitDialogForm,
    closeDialog
  }
}