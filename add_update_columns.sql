-- 给所有租户相关表添加 update_time 和 update_by_id 字段

ALTER TABLE `sys_dept` 
  ADD COLUMN `update_time` datetime DEFAULT NULL COMMENT '更新时间' AFTER `create_by_id`,
  ADD COLUMN `update_by_id` varchar(64) DEFAULT NULL COMMENT '更新人ID' AFTER `update_time`;

ALTER TABLE `sys_role` 
  ADD COLUMN `update_time` datetime DEFAULT NULL COMMENT '更新时间' AFTER `create_by_id`,
  ADD COLUMN `update_by_id` varchar(64) DEFAULT NULL COMMENT '更新人ID' AFTER `update_time`;

ALTER TABLE `sys_permission` 
  ADD COLUMN `update_time` datetime DEFAULT NULL COMMENT '更新时间' AFTER `create_by_id`,
  ADD COLUMN `update_by_id` varchar(64) DEFAULT NULL COMMENT '更新人ID' AFTER `update_time`;
