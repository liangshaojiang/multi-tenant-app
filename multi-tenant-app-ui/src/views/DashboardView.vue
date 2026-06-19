<script setup lang="ts">
import { ElMessage } from 'element-plus'
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import http from '../http'
import { useAuthStore } from '../stores/auth'
import type { ApiResponse, DepartmentCreateRequest, DepartmentItem, DeptTreeNode, RoleCreateRequest, RoleItem, UserCreateRequest, UserItem } from '../types'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const activeTab = computed(() => route.query.tab as string || 'dashboard')

const departments = ref<DepartmentItem[]>([])
const deptTree = ref<DeptTreeNode[]>([])
const roles = ref<RoleItem[]>([])
const users = ref<UserItem[]>([])
const loading = ref(false)

const showDeptDialog = ref(false)
const deptForm = ref<DepartmentCreateRequest>({ deptName: '', deptType: 3, parentId: null, taxNumber: '', secondTaxNumber: '', secondDeptName: '', sort: 0 })

const showRoleDialog = ref(false)
const roleForm = ref<RoleCreateRequest>({ roleName: '', roleCode: '', sort: 0 })

const showUserDialog = ref(false)
const userForm = ref<UserCreateRequest>({ accountName: '', realName: '', password: '', phone: '', email: '', deptId: null })

async function loadData() {
  loading.value = true
  try {
    const [deptResponse, treeResponse, roleResponse, userResponse] = await Promise.all([
      http.get<ApiResponse<DepartmentItem[]>>('/api/departments'),
      http.get<ApiResponse<DeptTreeNode[]>>('/api/departments/tree'),
      http.get<ApiResponse<RoleItem[]>>('/api/roles'),
      http.get<ApiResponse<UserItem[]>>('/api/auth/users'),
    ])
    departments.value = deptResponse.data.data
    deptTree.value = treeResponse.data.data
    roles.value = roleResponse.data.data
    users.value = userResponse.data.data
  } catch (error: any) {
    ElMessage.error(error?.response?.data?.message || '加载数据失败')
  } finally {
    loading.value = false
  }
}

function logout() {
  authStore.clear()
  router.push('/login')
}

async function switchCompany(companyId: string) {
  loading.value = true
  try {
    await authStore.switchCompany(companyId)
    ElMessage.success('切换成功')
    await loadData()
  } catch (error: any) {
    ElMessage.error(error?.response?.data?.message || '切换失败')
  } finally {
    loading.value = false
  }
}

function switchTab(tab: string) {
  router.push({ query: { tab } })
}

function openDeptDialog() {
  deptForm.value = { deptName: '', deptType: 3, parentId: null, taxNumber: '', secondTaxNumber: '', secondDeptName: '', sort: 0 }
  showDeptDialog.value = true
}

async function submitDept() {
  try {
    await http.post('/api/departments', deptForm.value)
    ElMessage.success('创建成功')
    showDeptDialog.value = false
    await loadData()
  } catch (error: any) {
    ElMessage.error(error?.response?.data?.message || '创建失败')
  }
}

function openRoleDialog() {
  roleForm.value = { roleName: '', roleCode: '', sort: 0 }
  showRoleDialog.value = true
}

async function submitRole() {
  try {
    await http.post('/api/roles', roleForm.value)
    ElMessage.success('创建成功')
    showRoleDialog.value = false
    await loadData()
  } catch (error: any) {
    ElMessage.error(error?.response?.data?.message || '创建失败')
  }
}

function openUserDialog() {
  userForm.value = { accountName: '', realName: '', password: '123456', phone: '', email: '', deptId: null }
  showUserDialog.value = true
}

async function submitUser() {
  try {
    await http.post('/api/auth/users', userForm.value)
    ElMessage.success('创建成功')
    showUserDialog.value = false
    await loadData()
  } catch (error: any) {
    ElMessage.error(error?.response?.data?.message || '创建失败')
  }
}

onMounted(() => {
  loadData()
})
</script>

<template>
  <div class="layout-shell">
    <aside class="layout-sidebar">
      <div class="sidebar-logo">
        <span class="logo-mark">J</span>
        <div>
          <strong>JVS Style Admin</strong>
          <p>Vue 3 + Element Plus</p>
        </div>
      </div>

      <nav class="menu-list">
        <a class="menu-item" :class="{ active: activeTab === 'dashboard' }" @click="switchTab('dashboard')">工作台</a>
        <a class="menu-item" :class="{ active: activeTab === 'users' }" @click="switchTab('users')">用户中心</a>
        <a class="menu-item" :class="{ active: activeTab === 'departments' }" @click="switchTab('departments')">组织架构</a>
        <a class="menu-item" :class="{ active: activeTab === 'roles' }" @click="switchTab('roles')">角色权限</a>
      </nav>
    </aside>

    <main class="layout-main">
      <header class="topbar">
        <div>
          <h2>{{ activeTab === 'dashboard' ? '控制台' : activeTab === 'users' ? '用户中心' : activeTab === 'departments' ? '组织架构' : '角色权限' }}</h2>
          <p>{{ authStore.companyName }} · {{ authStore.realName }}</p>
        </div>

        <div class="topbar-actions">
          <el-select :model-value="authStore.companyId" placeholder="切换公司" style="width: 260px" @change="switchCompany">
            <el-option-group
              v-for="group in authStore.companyGroups"
              :key="group.tenantId"
              :label="group.tenantName + (group.deploymentType === 'on_premise' ? ' (内网)' : '')"
            >
              <el-option
                v-for="company in group.companies"
                :key="company.companyId"
                :label="company.companyName"
                :value="company.companyId"
              />
            </el-option-group>
          </el-select>
          <el-button @click="logout">退出</el-button>
        </div>
      </header>

      <section v-if="activeTab === 'dashboard'" class="stats-grid">
        <div class="stat-card">
          <span>当前公司</span>
          <strong>{{ authStore.companyName }}</strong>
        </div>
        <div class="stat-card">
          <span>所属租户</span>
          <strong>{{ authStore.tenantName }}</strong>
        </div>
        <div class="stat-card">
          <span>部门数量</span>
          <strong>{{ departments.length }}</strong>
        </div>
        <div class="stat-card">
          <span>角色数量</span>
          <strong>{{ roles.length }}</strong>
        </div>
      </section>

      <section v-if="activeTab === 'dashboard'" class="content-grid">
        <el-card class="content-card" shadow="never">
          <template #header>
            <div class="card-header">
              <span>部门列表</span>
              <el-button type="primary" size="small" @click="openDeptDialog">新建部门</el-button>
            </div>
          </template>
          <el-table :data="departments" v-loading="loading" stripe>
            <el-table-column prop="deptName" label="部门名称" />
            <el-table-column prop="tenantId" label="租户ID" width="240" />
            <el-table-column prop="parentId" label="上级部门" width="240" />
          </el-table>
        </el-card>

        <el-card class="content-card" shadow="never">
          <template #header>
            <div class="card-header">
              <span>角色列表</span>
              <el-button type="primary" size="small" @click="openRoleDialog">新建角色</el-button>
            </div>
          </template>
          <el-table :data="roles" v-loading="loading" stripe>
            <el-table-column prop="roleName" label="角色名称" />
            <el-table-column prop="roleCode" label="角色编码" />
            <el-table-column prop="tenantId" label="租户ID" width="240" />
          </el-table>
        </el-card>
      </section>

      <section v-if="activeTab === 'users'">
        <el-card shadow="never">
          <template #header>
            <div class="card-header">
              <span>用户列表</span>
              <el-button type="primary" size="small" @click="openUserDialog">新建用户</el-button>
            </div>
          </template>
          <el-table :data="users" v-loading="loading" stripe>
            <el-table-column prop="accountName" label="账号" />
            <el-table-column prop="realName" label="姓名" />
            <el-table-column prop="deptName" label="部门" />
            <el-table-column prop="phone" label="手机号" />
            <el-table-column prop="email" label="邮箱" />
            <el-table-column label="状态" width="100">
              <template #default="{ row }">
                <el-tag :type="row.cancelFlag ? 'danger' : 'success'">{{ row.cancelFlag ? '已停用' : '正常' }}</el-tag>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </section>

      <section v-if="activeTab === 'departments'">
        <el-card shadow="never">
          <template #header>
            <div class="card-header">
              <span>组织架构（公司 {{ companyCount }} / 5）</span>
              <el-button type="primary" size="small" @click="openDeptDialog">新建节点</el-button>
            </div>
          </template>
          <el-table :data="deptTree" v-loading="loading" stripe row-key="id" default-expand-all>
            <el-table-column prop="deptName" label="名称" />
            <el-table-column label="类型" width="100">
              <template #default="{ row }">
                <el-tag :type="row.deptType === 1 ? 'warning' : row.deptType === 2 ? 'primary' : 'info'">
                  {{ row.deptType === 1 ? '集团' : row.deptType === 2 ? '公司' : '部门' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="taxNumber" label="税号" />
            <el-table-column prop="secondTaxNumber" label="第二税号" />
            <el-table-column prop="secondDeptName" label="曾用名" />
            <el-table-column prop="sort" label="排序" width="80" />
          </el-table>
        </el-card>
      </section>

      <section v-if="activeTab === 'roles'">
        <el-card shadow="never">
          <template #header>
            <div class="card-header">
              <span>角色权限</span>
              <el-button type="primary" size="small" @click="openRoleDialog">新建角色</el-button>
            </div>
          </template>
          <el-table :data="roles" v-loading="loading" stripe>
            <el-table-column prop="roleName" label="角色名称" />
            <el-table-column prop="roleCode" label="角色编码" />
            <el-table-column prop="sort" label="排序" width="100" />
            <el-table-column label="状态" width="100">
              <template #default="{ row }">
                <el-tag :type="row.status ? 'success' : 'info'">{{ row.status ? '启用' : '停用' }}</el-tag>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </section>
    </main>

    <el-dialog v-model="showDeptDialog" title="新建组织节点" width="480px">
      <el-form :model="deptForm" label-width="90px">
        <el-form-item label="节点类型">
          <el-select v-model="deptForm.deptType" style="width: 100%">
            <el-option :value="1" label="集团根" />
            <el-option :value="2" label="公司" />
            <el-option :value="3" label="部门" />
          </el-select>
        </el-form-item>
        <el-form-item label="名称">
          <el-input v-model="deptForm.deptName" placeholder="请输入名称" />
        </el-form-item>
        <el-form-item label="上级节点">
          <el-select v-model="deptForm.parentId" placeholder="可留空表示根节点" clearable style="width: 100%">
            <el-option v-for="dept in departments" :key="dept.id" :label="dept.deptName" :value="dept.id" />
          </el-select>
        </el-form-item>
        <template v-if="deptForm.deptType === 2">
          <el-form-item label="税号">
            <el-input v-model="deptForm.taxNumber" placeholder="统一社会信用代码" />
          </el-form-item>
          <el-form-item label="第二税号">
            <el-input v-model="deptForm.secondTaxNumber" placeholder="备用税号，可留空" />
          </el-form-item>
          <el-form-item label="曾用名">
            <el-input v-model="deptForm.secondDeptName" placeholder="公司历史名称，防改名对账" />
          </el-form-item>
        </template>
        <el-form-item label="排序">
          <el-input-number v-model="deptForm.sort" :min="0" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showDeptDialog = false">取消</el-button>
        <el-button type="primary" @click="submitDept">确定</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="showRoleDialog" title="新建角色" width="480px">
      <el-form :model="roleForm" label-width="80px">
        <el-form-item label="角色名称">
          <el-input v-model="roleForm.roleName" placeholder="请输入角色名称" />
        </el-form-item>
        <el-form-item label="角色编码">
          <el-input v-model="roleForm.roleCode" placeholder="请输入角色编码" />
        </el-form-item>
        <el-form-item label="排序">
          <el-input-number v-model="roleForm.sort" :min="0" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showRoleDialog = false">取消</el-button>
        <el-button type="primary" @click="submitRole">确定</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="showUserDialog" title="新建用户" width="480px">
      <el-form :model="userForm" label-width="80px">
        <el-form-item label="账号">
          <el-input v-model="userForm.accountName" placeholder="请输入账号" />
        </el-form-item>
        <el-form-item label="姓名">
          <el-input v-model="userForm.realName" placeholder="请输入姓名" />
        </el-form-item>
        <el-form-item label="密码">
          <el-input v-model="userForm.password" type="password" placeholder="请输入密码" />
        </el-form-item>
        <el-form-item label="手机号">
          <el-input v-model="userForm.phone" placeholder="请输入手机号" />
        </el-form-item>
        <el-form-item label="邮箱">
          <el-input v-model="userForm.email" placeholder="请输入邮箱" />
        </el-form-item>
        <el-form-item label="部门">
          <el-select v-model="userForm.deptId" placeholder="可留空" clearable style="width: 100%">
            <el-option v-for="dept in departments" :key="dept.id" :label="dept.deptName" :value="dept.id" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showUserDialog = false">取消</el-button>
        <el-button type="primary" @click="submitUser">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
</style>
