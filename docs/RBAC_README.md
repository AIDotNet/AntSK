# 角色基础授权系统 (Role-Based Access Control)

## 概述

本系统实现了完整的角色基础授权功能，支持将权限绑定到角色，角色再绑定给用户，提供灵活的权限管理能力。

## 数据库结构

### 核心表

1. **Roles (角色表)**
   - Id: 角色ID (主键)
   - Name: 角色名称
   - Code: 角色编码 (用于授权验证)
   - Description: 角色描述
   - IsEnabled: 是否启用
   - CreateTime: 创建时间

2. **Permissions (权限表)**
   - Id: 权限ID (主键)
   - Name: 权限名称
   - Code: 权限编码 (用于授权验证)
   - Type: 权限类型 (Menu-菜单权限, Operation-操作权限)
   - Description: 权限描述
   - CreateTime: 创建时间

3. **RolePermissions (角色权限关联表)**
   - Id: 关联ID (主键)
   - RoleId: 角色ID
   - PermissionId: 权限ID
   - CreateTime: 创建时间

4. **UserRoles (用户角色关联表)**
   - Id: 关联ID (主键)
   - UserId: 用户ID
   - RoleId: 角色ID
   - CreateTime: 创建时间

## 默认角色和权限

系统首次运行时会自动初始化以下角色和权限：

### 默认角色

1. **AntSKAdmin (管理员)**
   - 拥有系统所有权限
   - 可以管理用户、角色、权限等

2. **AntSKUser (普通用户)**
   - 拥有基本功能权限
   - 包括：聊天、应用、知识库

### 默认权限

系统包含以下菜单权限：
- chat: 聊天
- app: 应用
- kms: 知识库
- plugins.apilist: API管理
- plugins.funlist: 函数管理
- modelmanager.modellist: 模型管理
- setting.user: 用户管理
- setting.role: 角色管理
- setting.chathistory: 聊天记录
- setting.delkms: 删除向量表

## 使用说明

### 1. 角色管理

访问 `/setting/rolelist` 可以进行角色管理：
- 查看所有角色
- 创建新角色
- 编辑角色信息
- 为角色分配权限
- 删除角色

### 2. 用户管理

访问 `/setting/userlist` 可以进行用户管理：
- 创建用户时可以分配角色
- 编辑用户时可以修改角色分配
- 用户可以拥有多个角色

### 3. 授权验证

系统支持两种授权方式：

**方式一：基于角色的授权**
```csharp
@attribute [Authorize(Roles = "AntSKAdmin")]
```

**方式二：基于多角色的授权**
```csharp
@attribute [Authorize(Roles = "AntSKAdmin,AntSKUser")]
```

## 技术实现

### 认证流程

1. 用户登录时，系统从数据库加载用户的角色和权限
2. 将所有角色添加到用户的Claims中
3. 将权限列表存储到UserSession中供后续使用
4. 支持多个角色同时验证

### 向后兼容

- 保留了原有的MenuRole字段，支持逐步迁移
- 硬编码的管理员账号继续使用AntSKAdmin角色
- 新用户如果没有分配角色，默认使用AntSKUser角色

## 安全性

1. **角色验证**: 只有AntSKAdmin角色可以访问角色和用户管理页面
2. **级联删除**: 删除角色时会自动删除相关的角色权限和用户角色关联
3. **启用状态**: 角色支持启用/禁用状态，禁用的角色不会加载权限
4. **多层级保护**: 数据库层、业务层、UI层都有权限验证

## 扩展建议

1. **操作权限**: 可以在Permissions表中添加Type为"Operation"的权限，用于控制具体操作
2. **权限缓存**: 可以考虑添加权限缓存机制，提高性能
3. **审计日志**: 可以添加角色和权限变更的审计日志
4. **批量操作**: 可以添加批量分配角色、批量分配权限的功能

## 常见问题

**Q: 如何添加新的权限？**
A: 可以通过代码在InitRolesAndPermissions方法中添加，或者后续可以开发权限管理UI。

**Q: 用户可以有多个角色吗？**
A: 可以。系统支持一个用户拥有多个角色，最终拥有所有角色的权限并集。

**Q: 如何处理权限冲突？**
A: 系统采用"允许优先"策略，只要用户的任一角色拥有某项权限，用户就拥有该权限。

**Q: 删除角色会影响已登录的用户吗？**
A: 会的。用户下次登录时会重新加载角色和权限，已删除的角色将不再生效。
