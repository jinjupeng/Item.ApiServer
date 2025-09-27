# ApiServer 前端管理系统

基于 Vue 3 + TypeScript + Element Plus 构建的现代化企业级权限管理系统前端。

## ✨ 特性

- 🚀 **现代化技术栈**: Vue 3 + TypeScript + Vite
- 🎨 **优雅UI**: Element Plus + 自定义主题
- 🔐 **完整权限**: RBAC权限模型，细粒度权限控制
- 📱 **响应式设计**: 支持桌面端和移动端
- 🌙 **主题切换**: 支持亮色/暗色主题
- 🔄 **状态管理**: Pinia状态管理
- 📊 **数据可视化**: ECharts图表
- 🛡️ **类型安全**: 完整的TypeScript类型定义
- 🎯 **代码规范**: ESLint + Prettier
- 📦 **组件化**: 高度可复用的组件设计

## 🏗️ 技术架构

### 核心技术栈
- **框架**: Vue 3.4+
- **语言**: TypeScript 5.3+
- **构建工具**: Vite 5.0+
- **UI框架**: Element Plus 2.4+
- **状态管理**: Pinia 2.1+
- **路由**: Vue Router 4.2+
- **HTTP客户端**: Axios 1.6+
- **图表**: ECharts 5.4+
- **工具库**: Lodash-es, Day.js, VueUse

### 开发工具
- **代码检查**: ESLint + TypeScript ESLint
- **代码格式化**: Prettier
- **Git钩子**: Husky + lint-staged
- **包管理**: npm/yarn/pnpm

## 📁 项目结构

```
frontend/
├── public/                 # 静态资源
├── src/
│   ├── api/               # API接口
│   │   ├── auth.ts        # 认证接口
│   │   ├── user.ts        # 用户接口
│   │   ├── role.ts        # 角色接口
│   │   ├── menu.ts        # 菜单接口
│   │   └── index.ts       # 接口统一导出
│   ├── assets/            # 资源文件
│   ├── components/        # 公共组件
│   ├── directives/        # 自定义指令
│   │   ├── permission.ts  # 权限指令
│   │   └── index.ts       # 指令统一导出
│   ├── layout/            # 布局组件
│   │   ├── components/    # 布局子组件
│   │   │   ├── Header/    # 顶部导航
│   │   │   ├── Sidebar/   # 侧边栏
│   │   │   ├── Tabs/      # 标签页
│   │   │   └── Footer/    # 底部
│   │   └── index.vue      # 主布局
│   ├── router/            # 路由配置
│   │   └── index.ts       # 路由定义
│   ├── stores/            # 状态管理
│   │   ├── auth.ts        # 认证状态
│   │   ├── app.ts         # 应用状态
│   │   └── index.ts       # Store统一导出
│   ├── styles/            # 样式文件
│   │   ├── variables.scss # CSS变量
│   │   ├── mixins.scss    # SCSS混入
│   │   ├── reset.scss     # 重置样式
│   │   ├── common.scss    # 通用样式
│   │   └── index.scss     # 样式入口
│   ├── types/             # 类型定义
│   │   └── api.ts         # API类型
│   ├── utils/             # 工具函数
│   │   ├── request.ts     # HTTP请求封装
│   │   └── index.ts       # 工具函数
│   ├── views/             # 页面组件
│   │   ├── auth/          # 认证页面
│   │   │   └── Login.vue  # 登录页
│   │   ├── dashboard/     # 仪表盘
│   │   │   └── index.vue  # 首页
│   │   ├── system/        # 系统管理
│   │   │   ├── users/     # 用户管理
│   │   │   ├── roles/     # 角色管理
│   │   │   ├── menus/     # 菜单管理
│   │   │   └── orgs/      # 组织管理
│   │   └── error/         # 错误页面
│   ├── App.vue            # 根组件
│   └── main.ts            # 应用入口
├── .env                   # 环境变量
├── .env.development       # 开发环境变量
├── .env.production        # 生产环境变量
├── index.html             # HTML模板
├── package.json           # 项目配置
├── tsconfig.json          # TypeScript配置
├── vite.config.ts         # Vite配置
└── README.md              # 项目说明
```

## 🚀 快速开始

### 环境要求
- Node.js >= 16.0.0
- npm >= 8.0.0 或 yarn >= 1.22.0

### 安装依赖
```bash
# 使用 npm
npm install

# 使用 yarn
yarn install

# 使用 pnpm
pnpm install
```

### 开发运行
```bash
# 启动开发服务器
npm run dev

# 或使用 yarn
yarn dev

# 或使用 pnpm
pnpm dev
```

访问 http://localhost:8080

### 构建部署
```bash
# 构建生产版本
npm run build

# 预览构建结果
npm run preview
```

## 🔧 配置说明

### 环境变量
- `VITE_APP_TITLE`: 应用标题
- `VITE_APP_VERSION`: 应用版本
- `VITE_API_BASE_URL`: API基础URL

### 代理配置
开发环境下，Vite会自动代理API请求到后端服务：
```typescript
// vite.config.ts
server: {
  proxy: {
    '/api': {
      target: 'http://localhost:5000',
      changeOrigin: true,
      rewrite: (path) => path.replace(/^\/api/, '/api/v1')
    }
  }
}
```

## 🎯 功能模块

### 认证授权
- ✅ 用户登录/登出
- ✅ JWT令牌管理
- ✅ 权限验证
- ✅ 路由守卫
- ✅ 权限指令

### 用户管理
- ✅ 用户列表查询
- ✅ 用户增删改查
- ✅ 用户状态管理
- ✅ 密码重置
- ✅ 批量操作
- ✅ 数据导入导出

### 角色管理
- ✅ 角色列表管理
- ✅ 角色权限分配
- ✅ 用户角色关联
- ✅ 角色状态控制

### 菜单管理
- ✅ 菜单树形结构
- ✅ 菜单增删改查
- ✅ 菜单排序
- ✅ 权限控制
- ✅ 动态路由

### 组织管理
- ✅ 组织树形结构
- ✅ 组织层级管理
- ✅ 用户组织关联

### 系统功能
- ✅ 仪表盘统计
- ✅ 主题切换
- ✅ 多标签页
- ✅ 面包屑导航
- ✅ 全屏模式
- ✅ 响应式布局

## 🎨 UI组件

### 布局组件
- **AppLayout**: 主布局容器
- **AppHeader**: 顶部导航栏
- **AppSidebar**: 侧边栏菜单
- **AppTabs**: 多标签页
- **AppBreadcrumb**: 面包屑导航
- **AppFooter**: 底部信息

### 业务组件
- **UserForm**: 用户表单
- **ImportDialog**: 数据导入对话框
- **PermissionTree**: 权限树选择器

### 通用组件
- **PageContainer**: 页面容器
- **SearchForm**: 搜索表单
- **DataTable**: 数据表格
- **FormDialog**: 表单对话框

## 🔐 权限控制

### 路由权限
```typescript
// 路由元信息
meta: {
  title: '用户管理',
  permissions: ['system:user:list'],
  roles: ['admin']
}
```

### 组件权限
```vue
<!-- 权限指令 -->
<el-button v-permission="['system:user:add']">新增</el-button>
<el-button v-role="['admin']">管理员功能</el-button>
```

### API权限
```typescript
// 请求拦截器自动添加认证头
headers: {
  Authorization: `Bearer ${token}`
}
```

## 📊 状态管理

### Auth Store
- 用户信息管理
- 登录状态控制
- 权限数据缓存
- 令牌自动刷新

### App Store
- 应用配置管理
- 主题状态控制
- 布局状态管理
- 标签页管理

## 🛠️ 开发规范

### 代码规范
- 使用 TypeScript 严格模式
- 遵循 Vue 3 Composition API
- 统一的命名规范
- 完整的类型定义

### 组件规范
- 单一职责原则
- Props 类型定义
- Emits 事件定义
- 完整的注释文档

### 样式规范
- SCSS 预处理器
- BEM 命名规范
- CSS 变量统一管理
- 响应式设计

## 🔄 与后端对接

### API接口规范
- RESTful API设计
- 统一响应格式
- 错误码标准化
- 分页查询规范

### 数据类型匹配
- 完整的TypeScript类型定义
- 与后端DTO一一对应
- 枚举值统一管理
- 日期格式标准化

## 📈 性能优化

### 构建优化
- Vite 快速构建
- 代码分割
- 资源压缩
- Tree Shaking

### 运行时优化
- 组件懒加载
- 图片懒加载
- 虚拟滚动
- 缓存策略

## 🐛 调试指南

### 开发工具
- Vue DevTools
- TypeScript 类型检查
- ESLint 代码检查
- 浏览器调试工具

### 常见问题
1. **路由权限问题**: 检查用户权限和路由配置
2. **API请求失败**: 检查网络和后端服务状态
3. **样式问题**: 检查CSS变量和主题配置
4. **类型错误**: 检查TypeScript类型定义

## 📝 更新日志

### v2.0.0 (2024-01-01)
- 🎉 全新架构升级到Vue 3 + TypeScript
- ✨ 新增暗色主题支持
- 🔧 优化权限控制机制
- 📱 增强移动端适配
- 🚀 性能优化和体验提升

## 🤝 贡献指南

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 打开 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 👥 团队

- **前端开发**: Vue 3 + TypeScript + Element Plus
- **后端开发**: .NET Core + Clean Architecture
- **UI设计**: 现代化企业级设计语言

## 📞 联系我们

- 项目地址: [GitHub](https://github.com/jinjupeng/Item.ApiServer)
- 问题反馈: [Issues](https://github.com/jinjupeng/Item.ApiServer/issues)

---

⭐ 如果这个项目对你有帮助，请给我们一个星标！
