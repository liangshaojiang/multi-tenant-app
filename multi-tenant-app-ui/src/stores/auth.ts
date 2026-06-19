import { defineStore } from 'pinia'
import http from '../http'
import type {
  ApiResponse,
  CurrentUserInfo,
  LoginRequest,
  LoginResponse,
  TenantCompanyGroup,
} from './types'

interface AuthState {
  token: string
  userId: string
  accountName: string
  realName: string
  tenantId: string
  tenantName: string
  companyId: string
  companyName: string
  companyGroups: TenantCompanyGroup[]
}

const storageKeys = {
  token: 'access_token',
  refresh: 'refresh_token',
  auth: 'auth_state',
}

function emptyState(): AuthState {
  return {
    token: '',
    userId: '',
    accountName: '',
    realName: '',
    tenantId: '',
    tenantName: '',
    companyId: '',
    companyName: '',
    companyGroups: [],
  }
}

function readAuthState(): AuthState {
  const raw = localStorage.getItem(storageKeys.auth)
  if (!raw) {
    return emptyState()
  }

  try {
    return JSON.parse(raw) as AuthState
  } catch {
    return emptyState()
  }
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => readAuthState(),
  getters: {
    isAuthenticated: (state) => Boolean(state.token),
    allCompanies: (state) => {
      return state.companyGroups.flatMap(g =>
        g.companies.map(c => ({
          ...c,
          tenantId: g.tenantId,
          tenantName: g.tenantName,
          deploymentType: g.deploymentType,
          gatewayUrl: g.gatewayUrl,
        }))
      )
    },
  },
  actions: {
    applyAuth(data: LoginResponse) {
      this.token = data.accessToken
      this.userId = data.userId
      this.accountName = data.accountName
      this.realName = data.realName
      this.tenantId = data.tenantId
      this.tenantName = data.tenantName
      this.companyId = data.companyId
      this.companyName = data.companyName
      this.companyGroups = data.companyGroups
      this.persist()
    },
    persist() {
      localStorage.setItem(storageKeys.token, this.token)
      if (this.refreshToken) {
        localStorage.setItem(storageKeys.refresh, this.refreshToken)
      }
      localStorage.setItem(storageKeys.auth, JSON.stringify({
        token: this.token,
        userId: this.userId,
        accountName: this.accountName,
        realName: this.realName,
        tenantId: this.tenantId,
        tenantName: this.tenantName,
        companyId: this.companyId,
        companyName: this.companyName,
        companyGroups: this.companyGroups,
      }))
    },
    clear() {
      Object.assign(this, emptyState())
      localStorage.removeItem(storageKeys.token)
      localStorage.removeItem(storageKeys.auth)
    },
    async login(payload: LoginRequest) {
      const response = await http.post<ApiResponse<LoginResponse>>('/api/auth/login', payload)
      const data = response.data.data
      this.applyAuth(data)
      if (data.refreshToken) {
        localStorage.setItem(storageKeys.refresh, data.refreshToken)
      }
      return data
    },
    async refreshMe() {
      const response = await http.get<ApiResponse<CurrentUserInfo>>('/api/auth/me')
      return response.data.data
    },
    async switchCompany(companyId: string) {
      const response = await http.post<ApiResponse<LoginResponse>>('/api/auth/switch-company', { companyId })
      const data = response.data.data
      this.applyAuth(data)
      return data
    },
  },
})
