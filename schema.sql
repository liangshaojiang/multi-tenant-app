-- 核心表结构

-- 全局用户表（不带租户ID）
CREATE TABLE IF NOT EXISTS `sys_user` (
  `id` varchar(64) NOT NULL COMMENT '主键ID',
  `account_name` varchar(100) NOT NULL COMMENT '登录账号，全库唯一',
  `real_name` varchar(100) DEFAULT NULL COMMENT '真实姓名',
  `password` varchar(200) NOT NULL COMMENT '密码哈希',
  `phone` varchar(20) DEFAULT NULL COMMENT '手机号',
  `email` varchar(100) DEFAULT NULL COMMENT '邮箱',
  `head_img` varchar(500) DEFAULT NULL COMMENT '头像URL',
  `sex` tinyint(1) DEFAULT NULL COMMENT '性别 0女 1男',
  `birthday` date DEFAULT NULL COMMENT '生日',
  `cancel_flag` tinyint(1) DEFAULT 0 COMMENT '注销标记 0正常 1注销',
  `user_type` tinyint(1) DEFAULT 1 COMMENT '用户类型 1后台 2前端 3其他',
  `create_time` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_account_name` (`account_name`),
  KEY `idx_phone` (`phone`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='用户表';

-- 租户表
CREATE TABLE IF NOT EXISTS `sys_tenant` (
  `id` varchar(64) NOT NULL COMMENT '租户ID',
  `name` varchar(200) NOT NULL COMMENT '租户全称',
  `short_name` varchar(100) DEFAULT NULL COMMENT '租户简称',
  `logo` varchar(500) DEFAULT NULL COMMENT 'Logo URL',
  `icon` varchar(500) DEFAULT NULL COMMENT '图标URL',
  `admin_user_id` varchar(64) NOT NULL COMMENT '租户管理员用户ID',
  `parent_id` varchar(64) DEFAULT NULL COMMENT '父租户ID，平台主租户为NULL',
  `enable` tinyint(1) DEFAULT 1 COMMENT '是否启用 0禁用 1启用',
  `hosts` json DEFAULT NULL COMMENT '绑定域名列表',
  `connection_string` varchar(500) DEFAULT NULL COMMENT '独立数据库连接串，为空则用共享库',
  `create_time` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  PRIMARY KEY (`id`),
  KEY `idx_admin_user` (`admin_user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='租户表';

-- 用户租户关系表（带租户ID）
CREATE TABLE IF NOT EXISTS `sys_user_tenant` (
  `id` varchar(64) NOT NULL COMMENT '主键',
  `user_id` varchar(64) NOT NULL COMMENT '用户ID',
  `tenant_id` varchar(64) NOT NULL COMMENT '租户ID',
  `real_name` varchar(100) DEFAULT NULL COMMENT '真实姓名（可覆盖用户表）',
  `phone` varchar(20) DEFAULT NULL COMMENT '手机号（可覆盖用户表）',
  `dept_id` varchar(64) DEFAULT NULL COMMENT '部门ID',
  `dept_name` varchar(200) DEFAULT NULL COMMENT '部门名称',
  `job_id` varchar(64) DEFAULT NULL COMMENT '岗位ID',
  `job_name` varchar(100) DEFAULT NULL COMMENT '岗位名称',
  `employee_no` varchar(50) DEFAULT NULL COMMENT '工号',
  `level` int DEFAULT 0 COMMENT '层级',
  `cancel_flag` tinyint(1) DEFAULT 0 COMMENT '是否注销 0正常 1注销',
  `create_time` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_user_tenant` (`user_id`, `tenant_id`),
  KEY `idx_tenant_id` (`tenant_id`),
  KEY `idx_dept_id` (`dept_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='用户租户关系表';

-- 部门表（带租户ID）
CREATE TABLE IF NOT EXISTS `sys_dept` (
  `id` varchar(64) NOT NULL COMMENT '部门ID',
  `tenant_id` varchar(64) NOT NULL COMMENT '租户ID',
  `parent_id` varchar(64) DEFAULT NULL COMMENT '父部门ID',
  `dept_name` varchar(200) NOT NULL COMMENT '部门名称',
  `sort` int DEFAULT 0 COMMENT '排序',
  `leader_user_id` varchar(64) DEFAULT NULL COMMENT '负责人用户ID',
  `phone` varchar(20) DEFAULT NULL COMMENT '联系电话',
  `email` varchar(100) DEFAULT NULL COMMENT '邮箱',
  `status` tinyint(1) DEFAULT 1 COMMENT '状态 0停用 1正常',
  `create_time` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `create_by_id` varchar(64) DEFAULT NULL COMMENT '创建人ID',
  PRIMARY KEY (`id`),
  KEY `idx_tenant_id` (`tenant_id`),
  KEY `idx_parent_id` (`parent_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='部门表';

-- 岗位表（带租户ID）
CREATE TABLE IF NOT EXISTS `sys_job` (
  `id` varchar(64) NOT NULL COMMENT '岗位ID',
  `tenant_id` varchar(64) NOT NULL COMMENT '租户ID',
  `job_code` varchar(64) DEFAULT NULL COMMENT '岗位编码',
  `job_name` varchar(100) NOT NULL COMMENT '岗位名称',
  `sort` int DEFAULT 0 COMMENT '排序',
  `status` tinyint(1) DEFAULT 1 COMMENT '状态 0停用 1正常',
  `create_time` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `create_by_id` varchar(64) DEFAULT NULL COMMENT '创建人ID',
  PRIMARY KEY (`id`),
  KEY `idx_tenant_id` (`tenant_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='岗位表';

-- 角色表（带租户ID）
CREATE TABLE IF NOT EXISTS `sys_role` (
  `id` varchar(64) NOT NULL COMMENT '角色ID',
  `tenant_id` varchar(64) NOT NULL COMMENT '租户ID',
  `role_name` varchar(100) NOT NULL COMMENT '角色名称',
  `role_code` varchar(64) DEFAULT NULL COMMENT '角色编码',
  `role_type` tinyint(1) DEFAULT 1 COMMENT '角色类型 1普通 2管理员',
  `data_scope_type` tinyint(1) DEFAULT 5 COMMENT '数据权限 1全部 2本部门及子部门 3本部门 4自定义 5仅本人',
  `sort` int DEFAULT 0 COMMENT '排序',
  `status` tinyint(1) DEFAULT 1 COMMENT '状态 0停用 1正常',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `create_time` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `create_by_id` varchar(64) DEFAULT NULL COMMENT '创建人ID',
  PRIMARY KEY (`id`),
  KEY `idx_tenant_id` (`tenant_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='角色表';

-- 用户角色关系表（带租户ID）
CREATE TABLE IF NOT EXISTS `sys_user_role` (
  `id` varchar(64) NOT NULL COMMENT '主键',
  `user_id` varchar(64) NOT NULL COMMENT '用户ID',
  `role_id` varchar(64) NOT NULL COMMENT '角色ID',
  `tenant_id` varchar(64) NOT NULL COMMENT '租户ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_user_role_tenant` (`user_id`, `role_id`, `tenant_id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_role_id` (`role_id`),
  KEY `idx_tenant_id` (`tenant_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='用户角色关系表';

-- 权限资源表（带租户ID）
CREATE TABLE IF NOT EXISTS `sys_permission` (
  `id` varchar(64) NOT NULL COMMENT '权限ID',
  `tenant_id` varchar(64) NOT NULL COMMENT '租户ID',
  `parent_id` varchar(64) DEFAULT NULL COMMENT '父权限ID',
  `permission_name` varchar(100) NOT NULL COMMENT '权限名称',
  `permission_code` varchar(200) DEFAULT NULL COMMENT '权限标识',
  `permission_type` tinyint(1) NOT NULL COMMENT '权限类型 1菜单 2按钮 3接口',
  `path` varchar(200) DEFAULT NULL COMMENT '路由路径',
  `component` varchar(200) DEFAULT NULL COMMENT '组件路径',
  `icon` varchar(100) DEFAULT NULL COMMENT '图标',
  `sort` int DEFAULT 0 COMMENT '排序',
  `visible` tinyint(1) DEFAULT 1 COMMENT '是否显示 0隐藏 1显示',
  `status` tinyint(1) DEFAULT 1 COMMENT '状态 0停用 1正常',
  `create_time` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `create_by_id` varchar(64) DEFAULT NULL COMMENT '创建人ID',
  PRIMARY KEY (`id`),
  KEY `idx_tenant_id` (`tenant_id`),
  KEY `idx_parent_id` (`parent_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='权限资源表';

-- 角色权限关系表（带租户ID）
CREATE TABLE IF NOT EXISTS `sys_role_permission` (
  `id` varchar(64) NOT NULL COMMENT '主键',
  `role_id` varchar(64) NOT NULL COMMENT '角色ID',
  `permission_id` varchar(64) NOT NULL COMMENT '权限ID',
  `tenant_id` varchar(64) NOT NULL COMMENT '租户ID',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_role_permission` (`role_id`, `permission_id`),
  KEY `idx_tenant_id` (`tenant_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='角色权限关系表';

-- 插入测试数据

-- 插入平台管理员用户
INSERT INTO `sys_user` (`id`, `account_name`, `real_name`, `password`, `phone`, `email`, `cancel_flag`, `user_type`)
VALUES
('1', 'admin', '系统管理员', '$2a$10$N.zmdr9k7uOCQb376NoUnuTJ8iAt6Z5EHsM8lE9lBOsl7iAt6Z5EH', '13800138000', 'admin@example.com', 0, 1),
('2', 'tenant1_admin', '租户1管理员', '$2a$10$N.zmdr9k7uOCQb376NoUnuTJ8iAt6Z5EHsM8lE9lBOsl7iAt6Z5EH', '13800138001', 'tenant1@example.com', 0, 1),
('3', 'tenant2_admin', '租户2管理员', '$2a$10$N.zmdr9k7uOCQb376NoUnuTJ8iAt6Z5EHsM8lE9lBOsl7iAt6Z5EH', '13800138002', 'tenant2@example.com', 0, 1),
('4', 'user1', '普通用户1', '$2a$10$N.zmdr9k7uOCQb376NoUnuTJ8iAt6Z5EHsM8lE9lBOsl7iAt6Z5EH', '13800138003', 'user1@example.com', 0, 1);

-- 插入租户（密码都是 123456 的 BCrypt 哈希）
INSERT INTO `sys_tenant` (`id`, `name`, `short_name`, `admin_user_id`, `parent_id`, `enable`, `hosts`)
VALUES
('platform', '平台租户', '平台', '1', NULL, 1, '["localhost", "127.0.0.1"]'),
('tenant_001', '测试企业A', '企业A', '2', 'platform', 1, '["tenant1.localhost"]'),
('tenant_002', '测试企业B', '企业B', '3', 'platform', 1, '["tenant2.localhost"]');

-- 插入用户租户关系
INSERT INTO `sys_user_tenant` (`id`, `user_id`, `tenant_id`, `real_name`, `phone`, `cancel_flag`)
VALUES
('ut_1', '1', 'platform', '系统管理员', '13800138000', 0),
('ut_2', '2', 'tenant_001', '租户1管理员', '13800138001', 0),
('ut_3', '3', 'tenant_002', '租户2管理员', '13800138002', 0),
('ut_4', '4', 'tenant_001', '普通用户1', '13800138003', 0),
('ut_5', '4', 'tenant_002', '普通用户1', '13800138003', 0);

-- 插入部门
INSERT INTO `sys_dept` (`id`, `tenant_id`, `parent_id`, `dept_name`, `sort`, `status`)
VALUES
('dept_t1_1', 'tenant_001', NULL, '企业A总部', 1, 1),
('dept_t1_2', 'tenant_001', 'dept_t1_1', '研发部', 2, 1),
('dept_t1_3', 'tenant_001', 'dept_t1_1', '市场部', 3, 1),
('dept_t2_1', 'tenant_002', NULL, '企业B总部', 1, 1),
('dept_t2_2', 'tenant_002', 'dept_t2_1', '技术部', 2, 1);

-- 插入角色
INSERT INTO `sys_role` (`id`, `tenant_id`, `role_name`, `role_code`, `role_type`, `data_scope_type`, `status`)
VALUES
('role_t1_1', 'tenant_001', '管理员', 'admin', 2, 1, 1),
('role_t1_2', 'tenant_001', '普通员工', 'staff', 1, 5, 1),
('role_t2_1', 'tenant_002', '管理员', 'admin', 2, 1, 1);

-- 插入用户角色关系
INSERT INTO `sys_user_role` (`id`, `user_id`, `role_id`, `tenant_id`)
VALUES
('ur_1', '2', 'role_t1_1', 'tenant_001'),
('ur_2', '4', 'role_t1_2', 'tenant_001'),
('ur_3', '3', 'role_t2_1', 'tenant_002'),
('ur_4', '4', 'role_t1_2', 'tenant_002');
