import type { App } from 'vue'
import { permission, role } from './permission'

/**
 * 注册全局指令
 */
export function setupDirectives(app: App) {
  app.directive('permission', permission)
  app.directive('role', role)
}

export { permission, role }
