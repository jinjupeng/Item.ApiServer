## 🐳 Docker配置更新说明

我已经为你的RBAC权限管理系统完整更新了Docker容器化配置，现在支持现代化的微服务部署架构。

### 📋 更新内容

#### 🔄 更新的文件
1. **docker-compose.yml** - 主容器编排配置，支持MySQL + Redis + API + Frontend
2. **docker-compose.override.yml** - 开发环境专用覆盖配置
3. **docker-compose.dcproj** - Visual Studio Docker项目文件

#### ✨ 新增的文件
1. **src/ApiServer.WebApi/Dockerfile** - 后端API多阶段构建
2. **frontend/Dockerfile** - 前端Vue应用容器化
3. **frontend/nginx.conf** - 前端Nginx服务配置
4. **docker-compose.prod.yml** - 生产环境专用配置
5. **.dockerignore** - 构建优化忽略文件
6. **.env.example** - 环境变量配置模板
7. **deploy.sh** - 自动化部署脚本

### 🏗️ 架构特点

- **🔧 多服务架构**: MySQL + Redis + API + Frontend + Nginx反向代理
- **🌍 环境分离**: 开发/生产环境独立配置
- **💊 健康检查**: 所有服务都有健康检查机制
- **🔒 网络隔离**: 自定义Docker网络安全隔离
- **💾 数据持久化**: 数据库和缓存数据持久化存储
- **📝 日志管理**: 统一日志配置和轮转策略

### 🚀 快速部署

```bash
# 开发环境一键部署
./deploy.sh dev

# 生产环境部署
./deploy.sh prod

# 查看服务状态
./deploy.sh logs

# 停止所有服务
./deploy.sh stop

# 数据库备份
./deploy.sh backup
```

### 🌐 服务端口

| 服务 | 开发环境 | 生产环境 | 说明 |
|------|----------|----------|------|
| 前端 | 3000 | 80 | Vue3应用 |
| API | 5000 | 5000 | 后端接口 |
| MySQL | 3307 | 3306 | 数据库 |
| Redis | 6380 | 6379 | 缓存 |

### 🔐 安全特性

- **环境变量管理**: 敏感信息通过环境变量配置
- **SSL/TLS支持**: 生产环境HTTPS配置
- **网络隔离**: Docker网络安全隔离
- **非root运行**: 容器以非特权用户运行
- **安全头配置**: Nginx安全头防护

现在你可以使用 `./deploy.sh dev` 快速启动开发环境，或使用 `./deploy.sh prod` 部署生产环境。整套配置支持水平扩展和高可用部署。