#!/bin/bash

# RBAC权限管理系统部署脚本
# 使用方法: ./deploy.sh [dev|prod]

set -e

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# 打印带颜色的消息
print_message() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_header() {
    echo -e "${BLUE}================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}================================${NC}"
}

# 检查Docker是否安装
check_docker() {
    if ! command -v docker &> /dev/null; then
        print_error "Docker未安装，请先安装Docker"
        exit 1
    fi
    
    if ! command -v docker-compose &> /dev/null; then
        print_error "Docker Compose未安装，请先安装Docker Compose"
        exit 1
    fi
    
    print_message "Docker环境检查通过"
}

# 检查环境变量文件
check_env_file() {
    local env_file="$1"
    if [ ! -f "$env_file" ]; then
        print_warning "环境变量文件 $env_file 不存在"
        if [ -f ".env.example" ]; then
            print_message "正在从 .env.example 创建 $env_file"
            cp .env.example "$env_file"
            print_warning "请编辑 $env_file 文件，配置正确的环境变量"
            return 1
        else
            print_error "找不到 .env.example 文件"
            exit 1
        fi
    fi
    return 0
}

# 创建必要的目录
create_directories() {
    print_message "创建必要的目录..."
    mkdir -p logs/nginx
    mkdir -p backups
    mkdir -p sql-scripts
    mkdir -p nginx/ssl
    print_message "目录创建完成"
}

# 开发环境部署
deploy_dev() {
    print_header "部署开发环境"
    
    check_env_file ".env.dev"
    if [ $? -eq 1 ]; then
        print_error "请先配置 .env.dev 文件"
        exit 1
    fi
    
    create_directories
    
    print_message "停止现有容器..."
    docker-compose -f docker-compose.yml -f docker-compose.override.yml down
    
    print_message "构建并启动开发环境..."
    docker-compose -f docker-compose.yml -f docker-compose.override.yml --env-file .env.dev up --build -d
    
    print_message "等待服务启动..."
    sleep 30
    
    print_message "检查服务状态..."
    docker-compose -f docker-compose.yml -f docker-compose.override.yml ps
    
    print_message "开发环境部署完成！"
    echo -e "${GREEN}前端地址: http://localhost:3000${NC}"
    echo -e "${GREEN}后端API: http://localhost:5000${NC}"
    echo -e "${GREEN}Swagger文档: http://localhost:5000/swagger${NC}"
    echo -e "${GREEN}MySQL: localhost:3307${NC}"
    echo -e "${GREEN}Redis: localhost:6380${NC}"
}

# 生产环境部署
deploy_prod() {
    print_header "部署生产环境"
    
    check_env_file ".env.prod"
    if [ $? -eq 1 ]; then
        print_error "请先配置 .env.prod 文件"
        exit 1
    fi
    
    create_directories
    
    # 检查SSL证书
    if [ ! -f "nginx/ssl/cert.pem" ] || [ ! -f "nginx/ssl/key.pem" ]; then
        print_warning "SSL证书不存在，将使用HTTP模式"
    fi
    
    print_message "停止现有容器..."
    docker-compose -f docker-compose.yml -f docker-compose.prod.yml down
    
    print_message "拉取最新镜像..."
    docker-compose -f docker-compose.yml -f docker-compose.prod.yml pull
    
    print_message "构建并启动生产环境..."
    docker-compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.prod up --build -d
    
    print_message "等待服务启动..."
    sleep 60
    
    print_message "检查服务状态..."
    docker-compose -f docker-compose.yml -f docker-compose.prod.yml ps
    
    print_message "生产环境部署完成！"
    echo -e "${GREEN}应用地址: http://localhost${NC}"
    echo -e "${GREEN}API地址: http://localhost:5000${NC}"
}

# 停止服务
stop_services() {
    print_header "停止所有服务"
    
    print_message "停止开发环境..."
    docker-compose -f docker-compose.yml -f docker-compose.override.yml down 2>/dev/null || true
    
    print_message "停止生产环境..."
    docker-compose -f docker-compose.yml -f docker-compose.prod.yml down 2>/dev/null || true
    
    print_message "服务已停止"
}

# 查看日志
view_logs() {
    local service="$1"
    if [ -z "$service" ]; then
        docker-compose logs -f
    else
        docker-compose logs -f "$service"
    fi
}

# 备份数据库
backup_database() {
    print_header "备份数据库"
    
    local backup_file="backups/mysql_backup_$(date +%Y%m%d_%H%M%S).sql"
    
    print_message "创建数据库备份: $backup_file"
    docker-compose exec mysql mysqldump -u root -p123456 --all-databases > "$backup_file"
    
    print_message "数据库备份完成: $backup_file"
}

# 显示帮助信息
show_help() {
    echo "RBAC权限管理系统部署脚本"
    echo ""
    echo "使用方法:"
    echo "  $0 dev          部署开发环境"
    echo "  $0 prod         部署生产环境"
    echo "  $0 stop         停止所有服务"
    echo "  $0 logs [服务名] 查看日志"
    echo "  $0 backup       备份数据库"
    echo "  $0 help         显示此帮助信息"
    echo ""
    echo "示例:"
    echo "  $0 dev                    # 部署开发环境"
    echo "  $0 prod                   # 部署生产环境"
    echo "  $0 logs apiserver         # 查看API服务日志"
    echo "  $0 backup                 # 备份数据库"
}

# 主函数
main() {
    case "$1" in
        "dev")
            check_docker
            deploy_dev
            ;;
        "prod")
            check_docker
            deploy_prod
            ;;
        "stop")
            stop_services
            ;;
        "logs")
            view_logs "$2"
            ;;
        "backup")
            backup_database
            ;;
        "help"|"--help"|"-h"|"")
            show_help
            ;;
        *)
            print_error "未知参数: $1"
            show_help
            exit 1
            ;;
    esac
}

# 执行主函数
main "$@"
