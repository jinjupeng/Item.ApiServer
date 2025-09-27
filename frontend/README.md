# RBAC权限管理系统 - 前端

基于Vue3 + Vite + TypeScript + Element Plus构建的RBAC权限管理系统前端应用。

## 技术栈

- **框架**: Vue 3.4+ (Composition API)
- **构建工具**: Vite 5.0+
- **语言**: TypeScript 5.3+
- **UI组件库**: Element Plus 2.4+
- **状态管理**: Pinia 2.1+
- **路由**: Vue Router 4.2+
- **HTTP客户端**: Axios 1.6+
- **样式**: SCSS
- **图标**: Element Plus Icons

## 功能特性

### 🔐 认证授权
- 用户登录/登出
- JWT Token认证
- 自动刷新Token
- 验证码支持
- 忘记密码功能

### 👥 用户管理
- 用户列表查询
- 用户增删改查
- 用户状态管理
- 密码重置
- 角色分配

### 🎭 角色管理
- 角色列表查询
- 角色增删改查
- 角色状态管理
- 权限分配
- 菜单权限配置

### 📋 菜单管理
- 菜单树形展示
- 菜单增删改查
- 菜单类型管理（目录/菜单/按钮）
- 菜单排序
- 图标配置

### 🛡️ 权限控制
- 基于角色的权限控制(RBAC)
- 路由权限守卫
- 按钮级权限控制
- 菜单权限过滤

### 🎨 界面特性
- 响应式设计
- 暗色主题支持
- 多语言支持(预留)
- 面包屑导航
- 页面切换动画

## 项目结构

```
src/
├── api/                    # API接口
│   ├── auth.ts            # 认证相关接口
│   ├── users.ts           # 用户管理接口
│   ├── roles.ts           # 角色管理接口
│   └── menus.ts           # 菜单管理接口
├── components/            # 公共组件
│   ├── ChangePasswordDialog.vue
│   └── ForgotPasswordDialog.vue
├── layout/                # 布局组件
│   ├── index.vue          # 主布局
│   ├── RouteView.vue      # 路由视图
│   └── components/        # 布局子组件
│       ├── Navbar.vue     # 顶部导航
│       ├── Sidebar.vue    # 侧边栏
│       └── SidebarItem.vue
├── router/                # 路由配置
│   └── index.ts
├── stores/                # 状态管理
│   ├── auth.ts            # 认证状态
│   └── app.ts             # 应用状态
├── styles/                # 样式文件
│   └── index.scss         # 全局样式
├── types/                 # 类型定义
│   └── index.ts
├── utils/                 # 工具函数
│   ├── index.ts           # 通用工具
│   └── request.ts         # HTTP请求封装
├── views/                 # 页面组件
│   ├── auth/              # 认证页面
│   ├── dashboard/         # 仪表盘
│   ├── error/             # 错误页面
│   └── system/            # 系统管理
│       ├── users/         # 用户管理
│       ├── roles/         # 角色管理
│       └── menus/         # 菜单管理
├── App.vue                # 根组件
└── main.ts                # 入口文件
```

## 快速开始

### 环境要求

- Node.js 18+
- npm 9+ 或 yarn 1.22+

### 安装依赖

```bash
npm install
# 或
yarn install
```

### 开发环境

```bash
npm run dev
# 或
yarn dev
```

访问 http://localhost:3000

### 构建生产版本

```bash
npm run build
# 或
yarn build
```

### 预览生产版本

```bash
npm run preview
# 或
yarn preview
```

## 环境配置

### 环境变量

创建 `.env.local` 文件配置本地环境变量：

```bash
# API基础URL
VITE_API_BASE_URL=http://localhost:5000/api

# 应用标题
VITE_APP_TITLE=RBAC权限管理系统
```

### 代理配置

开发环境下，Vite会自动代理 `/api` 请求到后端服务器。

## 默认账户

根据后端数据库初始化，系统提供以下默认账户：

| 用户名 | 密码 | 角色 | 说明 |
|--------|------|------|------|
| admin | 123456 | 超级管理员 | 拥有所有权限 |
| manager | 123456 | 管理员 | 拥有管理权限 |
| user | 123456 | 普通用户 | 基础权限 |

## 开发指南

### 代码规范

项目使用 ESLint + Prettier 进行代码规范检查：

```bash
# 检查代码规范
npm run lint

# 自动修复
npm run lint --fix
```

### 类型检查

```bash
# TypeScript类型检查
npm run type-check
```

### 组件开发

1. 使用 Composition API
2. 使用 TypeScript 进行类型约束
3. 遵循 Vue 3 最佳实践
4. 使用 Element Plus 组件库

### API接口

所有API接口都在 `src/api/` 目录下，使用统一的请求封装：

```typescript
import request from '@/utils/request'

export const getUserList = (params: UserQueryDto) => {
  return request.get('/users', { params })
}
```

### 权限控制

#### 路由权限

```typescript
// 在路由meta中配置权限
{
  path: '/system/users',
  meta: {
    requiresAuth: true,
    permission: 'system:user:list'
  }
}
```

#### 组件权限

```vue
<template>
  <el-button
    v-if="authStore.hasPermission('system:user:create')"
    @click="handleCreate"
  >
    新增用户
  </el-button>
</template>
```

## 部署

### Nginx配置示例

```nginx
server {
    listen 80;
    server_name your-domain.com;
    root /var/www/html;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /api {
        proxy_pass http://backend-server:5000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

## 浏览器支持

- Chrome >= 87
- Firefox >= 78
- Safari >= 14
- Edge >= 88

## 许可证

MIT License

## 贡献

欢迎提交 Issue 和 Pull Request！

## 更新日志

### v1.0.0 (2024-01-01)

- 🎉 初始版本发布
- ✨ 完整的RBAC权限管理功能
- 🎨 现代化的UI界面
- 📱 响应式设计支持
