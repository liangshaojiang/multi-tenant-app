export interface TenantOption {
  tenantId: string
  tenantName: string
  shortName?: string | null
}

export interface CompanyOption {
  companyId: string
  companyName: string
  shortName?: string | null
}

export interface TenantCompanyGroup {
  tenantId: string
  tenantName: string
  deploymentType: string
  gatewayUrl?: string | null
  companies: CompanyOption[]
}

export interface LoginRequest {
  accountName: string
  password: string
  companyId?: string
}

export interface SwitchCompanyRequest {
  companyId: string
}

export interface LoginResponse {
  accessToken: string
  refreshToken: string
  expiresAt: string
  userId: string
  accountName: string
  realName: string
  tenantId: string
  tenantName: string
  companyId: string
  companyName: string
  companyGroups: TenantCompanyGroup[]
}

export interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
}

export interface CurrentUserInfo {
  userId: string
  tenantId: string
  isAuthenticated: boolean
}

export interface DepartmentItem {
  id: string
  tenantId: string
  deptName: string
  deptType: number
  parentId?: string | null
  taxNumber?: string | null
  secondTaxNumber?: string | null
  secondDeptName?: string | null
  sort?: number
  status?: boolean
  leaderUserId?: string | null
  phone?: string | null
  email?: string | null
}

export interface DeptTreeNode {
  id: string
  deptName: string
  deptType: number
  parentId?: string | null
  taxNumber?: string | null
  secondTaxNumber?: string | null
  secondDeptName?: string | null
  sort: number
  status: boolean
  leaderUserId?: string | null
  phone?: string | null
  email?: string | null
  children: DeptTreeNode[]
}

export interface DepartmentCreateRequest {
  deptName: string
  deptType: number
  parentId?: string | null
  taxNumber?: string | null
  secondTaxNumber?: string | null
  secondDeptName?: string | null
  sort: number
}

export interface RoleItem {
  id: string
  tenantId: string
  roleName: string
  roleCode?: string | null
  sort?: number
  status?: boolean
}

export interface RoleCreateRequest {
  roleName: string
  roleCode: string
  sort: number
}

export interface UserItem {
  id: string
  accountName: string
  realName: string
  phone?: string | null
  email?: string | null
  deptId?: string | null
  deptName?: string | null
  cancelFlag: boolean
  userType: number
}

export interface UserCreateRequest {
  accountName: string
  realName: string
  password: string
  phone?: string
  email?: string
  deptId?: string | null
}
