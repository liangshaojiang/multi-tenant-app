<script setup lang="ts">
import { computed } from 'vue'
import { RouterView, useRoute } from 'vue-router'

const route = useRoute()
const isLogin = computed(() => route.path === '/login')
</script>

<template>
  <div :class="['app-root', { 'app-root-login': isLogin }]">
    <RouterView />
  </div>
</template>
