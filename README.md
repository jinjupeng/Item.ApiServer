# API Server - Clean Architecture Implementation

## 项目概述

本项目是一个基于 .NET 8.0 和 Clean Architecture 原则的企业级 API 服务器，已完成从传统三层架构到现代化多项目架构的重构，遵循领域驱动设计(DDD)和.NET Core最佳实践。

## 重构成果

### 原始项目问题
- 单体项目结构，职责不清晰
- 命名不规范 (BLL.BLL, DAL.DAL 命名空间)
- 实体命名问题 (sys_user 格式)
- 缺乏现代化架构设计

### 重构后改进
- **清晰的多项目架构**: 5个独立项目，职责明确
- **标准C#命名规范**: 符合.NET最佳实践
- **Clean Architecture**: 依赖倒置，关注点分离
- **现代化技术栈**: .NET 8.0 + EF Core 8.0
- **企业级特性**: 仓储模式、工作单元、依赖注入

## 项目架构

### 多项目结构
```
├── ApiServer.Domain/               # 领域层 - 核心业务实体
│   ├── Common/BaseEntity.cs        # 实体基类 (审计字段)
│   ├── Entities/User.cs            # 用户实体
│   ├── Entities/Role.cs            # 角色实体
│   └── Enums/UserStatus.cs         # 业务枚举
├── ApiServer.Shared/               # 共享层 - 通用组件
│   ├── Common/ApiResult.cs         # 统一响应格式
│   ├── Common/PagedResult.cs       # 分页结果
│   └── JsonConverters/             # JSON转换器
├── ApiServer.Application/          # 应用层 - 业务协调
│   ├── DTOs/User/                  # 数据传输对象
│   ├── Interfaces/Services/        # 服务接口
│   └── Interfaces/Repositories/    # 仓储接口
├── ApiServer.Infrastructure/       # 基础设施层 - 数据访问
│   ├── Data/ApplicationDbContext.cs # EF Core上下文
│   ├── Repositories/               # 仓储实现
│   ├── Services/                   # 服务实现
│   └── UnitOfWork/                 # 工作单元
└── ApiServer.WebApi/               # 表示层 - API控制器
    ├── Controllers/                # RESTful API
    ├── Middlewares/                # 中间件
    └── Extensions/                 # 服务扩展
```

## 技术栈

### 核心框架
- **.NET 8.0**: 最新LTS版本
- **ASP.NET Core Web API**: RESTful API框架
- **Entity Framework Core 8.0**: 现代ORM框架

### 数据存储
- **MySQL 8.0+**: 主数据库
- **Pomelo.EntityFrameworkCore.MySql**: MySQL提供程序

### 开发工具
- **Mapster**: 高性能对象映射
- **FluentValidation**: 流畅的数据验证
- **MediatR**: 中介者模式实现
- **Serilog**: 结构化日志记录

### 安全认证
- **JWT Bearer**: 无状态认证
- **ASP.NET Core Identity**: 认证授权框架

### API文档
- **Swagger/OpenAPI 3.0**: 自动API文档
- **Swashbuckle.AspNetCore**: Swagger集成

## 使用指南

### 运行条件

1. ASP.NET Core 3.1
2. VS 2019

### 示例

给出更多使用演示和截图，并贴出相应代码。

## 版本历史

- 0.0.1

  - 初始化项目
- 1.0.1
  - 升级到3.1版本


## TODO

- [X] ASP.NET Core集成JWT
- [X] 集成Swagger UI
- [X] 基于策略模式的权限分配
- [X] 添加Serilog日志
- [X] Token平滑刷新
- [X] 添加EntityFrameworkCore ORM框架、事务一致性
- [X] 集成MemoryCache、Redis缓存
- [X] 升级到Core 3.1
- [X] 集成消息队列RabbitMQ
- [X] 集成阿里云OSS
- [X] 实现微信登录接口
- [X] 请求限流
- [X] 集成FluentValidation

## 关于作者

- [jinjupeng](https://github.com/jinjupeng/)

- 微信公众号（程序猿知多少）![微信公众号](https://some-images.oss-cn-hangzhou.aliyuncs.com/images/qrcode_for_gh_7a3c5972baba_258.jpg)

## License

MIT
