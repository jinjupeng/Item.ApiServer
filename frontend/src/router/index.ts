import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useAppStore } from '@/stores/app'
import NProgress from 'nprogress'
import 'nprogress/nprogress.css'

// 配置NProgress
NProgress.configure({ showSpinner: false })

// 基础路由（无需权限）
const constantRoutes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/auth/Login.vue'),
    meta: {
      title: '登录',
      hidden: true
    }
  },
  {
    path: '/404',
    name: 'NotFound',
    component: () => import('@/views/error/404.vue'),
    meta: {
      title: '页面不存在',
      hidden: true
    }
  },
  {
    path: '/403',
    name: 'Forbidden',
    component: () => import('@/views/error/403.vue'),
    meta: {
      title: '无权限访问',
      hidden: true
    }
  }
]

// 异步路由（需要权限）
const asyncRoutes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'Layout',
    component: () => import('@/layout/index.vue'),
    redirect: '/dashboard',
    children: [
      {
        path: 'dashboard',
        name: 'Dashboard',
        component: () => import('@/views/dashboard/index.vue'),
        meta: {
          title: '仪表盘',
          icon: 'Dashboard',
          affix: true
        }
      }
    ]
  },
  {
    path: '/system',
    name: 'System',
    component: () => import('@/layout/index.vue'),
    meta: {
      title: '系统管理',
      icon: 'Setting'
    },
    children: [
      {
        path: 'users',
        name: 'SystemUsers',
        component: () => import('@/views/system/users/index.vue'),
        meta: {
          title: '用户管理',
          icon: 'User',
          permissions: ['system:user:list']
        }
      },
      {
        path: 'roles',
        name: 'SystemRoles',
        component: () => import('@/views/system/roles/index.vue'),
        meta: {
          title: '角色管理',
          icon: 'UserFilled',
          permissions: ['system:role:list']
        }
      },
      {
        path: 'menus',
        name: 'SystemMenus',
        component: () => import('@/views/system/menus/index.vue'),
        meta: {
          title: '菜单管理',
          icon: 'Menu',
          permissions: ['system:menu:list']
        }
      },
      {
        path: 'organizations',
        name: 'SystemOrganizations',
        component: () => import('@/views/system/organizations/index.vue'),
        meta: {
          title: '组织管理',
          icon: 'OfficeBuilding',
          permissions: ['system:org:list']
        }
      }
    ]
  },
  {
    path: '/profile',
    name: 'Profile',
    component: () => import('@/layout/index.vue'),
    meta: {
      hidden: true
    },
    children: [
      {
        path: '',
        name: 'UserProfile',
        component: () => import('@/views/profile/index.vue'),
        meta: {
          title: '个人中心',
          icon: 'User'
        }
      }
    ]
  }
]

// 创建路由实例
const router = createRouter({
  history: createWebHistory(),
  routes: [...constantRoutes, ...asyncRoutes, {
    path: '/:pathMatch(.*)*',
    redirect: '/404'
  }],
  scrollBehavior: () => ({ left: 0, top: 0 })
})

// 白名单路由
const whiteList = ['/login', '/404', '/403']

// 路由守卫
router.beforeEach(async (to, from, next) => {
  NProgress.start()
  
  const authStore = useAuthStore()
  const appStore = useAppStore()
  
  // 设置页面标题
  document.title = to.meta?.title ? `${to.meta.title} - ${appStore.settings.title}` : appStore.settings.title
  
  if (authStore.isLoggedIn) {
    if (to.path === '/login') {
      // 已登录用户访问登录页，重定向到首页
      next('/')
    } else {
      // 检查是否有用户信息
      if (!authStore.userInfo) {
        try {
          await authStore.loadUserInfo()
          await authStore.loadUserPermissions()
          next()
        } catch (error) {
          console.error('Load user info failed:', error)
          await authStore.logout()
          next('/login')
        }
      } else {
        // 权限检查
        if (to.meta?.permissions) {
          const hasPermission = (to.meta.permissions as string[]).some(permission => 
            authStore.hasPermission(permission)
          )
          if (hasPermission) {
            next()
          } else {
            next('/403')
          }
        } else {
          next()
        }
      }
    }
  } else {
    // 未登录
    if (whiteList.includes(to.path)) {
      next()
    } else {
      next('/login')
    }
  }
})

router.afterEach((to) => {
  NProgress.done()
  
  const appStore = useAppStore()
  
  // 添加标签页
  if (to.meta?.title && to.path !== '/login') {
    appStore.addTab({
      title: to.meta.title as string,
      path: to.path,
      name: to.name as string,
      closable: !to.meta?.affix
    })
  }
  
  // 生成面包屑
  const breadcrumbs: Array<{ title: string; path?: string }> = []
  const matched = to.matched.filter(item => item.meta?.title)
  
  matched.forEach((route, index) => {
    if (route.meta?.title) {
      breadcrumbs.push({
        title: route.meta.title as string,
        path: index === matched.length - 1 ? undefined : route.path
      })
    }
  })
  
  appStore.setBreadcrumbs(breadcrumbs)
})

export default router
