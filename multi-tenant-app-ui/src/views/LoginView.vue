<script setup lang="ts">
import { ElMessage } from 'element-plus'
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const loading = ref(false)

const form = reactive({
  accountName: 'admin',
  password: '123456',
})

async function handleLogin() {
  loading.value = true
  try {
    const result = await authStore.login({
      accountName: form.accountName,
      password: form.password,
    })

    ElMessage.success(`欢迎回来，${result.realName}`)
    await router.push('/dashboard')
  } catch (error: any) {
    const message = error?.response?.data?.message || '登录失败'
    ElMessage.error(message)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="login-page">
    <div class="login-panel">
      <div class="brand-block">
        <div class="brand-mark">J</div>
        <div>
          <h1>Multi Tenant App</h1>
          <p>统一用户中心 · 多租户后台</p>
        </div>
      </div>

      <el-form class="login-form" label-position="top">
        <el-form-item label="账号">
          <el-input v-model="form.accountName" placeholder="请输入账号" />
        </el-form-item>

        <el-form-item label="密码">
          <el-input v-model="form.password" type="password" show-password placeholder="请输入密码" @keyup.enter="handleLogin" />
        </el-form-item>

        <el-button class="submit-button" type="primary" :loading="loading" @click="handleLogin">
          登录系统
        </el-button>
      </el-form>

      <div class="hint-block">
        <p>测试账号</p>
        <p>admin / 123456（跨租户：云端+内网）</p>
        <p>tenant1_admin / 123456</p>
        <p>user1 / 123456</p>
      </div>
    </div>
  </div>
</template>
