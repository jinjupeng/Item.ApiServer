import type { Directive, DirectiveBinding } from 'vue'
import { useAuthStore } from '@/stores/auth'

/**
 * 权限指令
 * v-permission="['system:user:add']"
 * v-permission="'system:user:add'"
 */
export const permission: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding) {
    checkPermission(el, binding)
  },
  updated(el: HTMLElement, binding: DirectiveBinding) {
    checkPermission(el, binding)
  }
}

function checkPermission(el: HTMLElement, binding: DirectiveBinding) {
  const { value } = binding
  const authStore = useAuthStore()

  if (value) {
    let hasPermission = false
    
    if (Array.isArray(value)) {
      // 数组形式：需要满足其中任意一个权限
      hasPermission = value.some(permission => authStore.hasPermission(permission))
    } else if (typeof value === 'string') {
      // 字符串形式：需要满足该权限
      hasPermission = authStore.hasPermission(value)
    }

    if (!hasPermission) {
      // 没有权限则移除元素
      el.parentNode?.removeChild(el)
    }
  }
}

/**
 * 角色指令
 * v-role="['admin']"
 * v-role="'admin'"
 */
export const role: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding) {
    checkRole(el, binding)
  },
  updated(el: HTMLElement, binding: DirectiveBinding) {
    checkRole(el, binding)
  }
}

function checkRole(el: HTMLElement, binding: DirectiveBinding) {
  const { value } = binding
  const authStore = useAuthStore()

  if (value) {
    let hasRole = false
    
    if (Array.isArray(value)) {
      // 数组形式：需要满足其中任意一个角色
      hasRole = value.some(role => authStore.hasRole(role))
    } else if (typeof value === 'string') {
      // 字符串形式：需要满足该角色
      hasRole = authStore.hasRole(value)
    }

    if (!hasRole) {
      // 没有角色则移除元素
      el.parentNode?.removeChild(el)
    }
  }
}
