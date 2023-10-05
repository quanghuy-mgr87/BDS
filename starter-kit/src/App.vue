<script setup>
import useEmitter from '@/helper/useEmitter'
import ScrollToTop from '@core/components/ScrollToTop.vue'
import { useThemeConfig } from '@core/composable/useThemeConfig'
import { hexToRgb } from '@layouts/utils'
import { onMounted } from 'vue'
import { useTheme } from 'vuetify'

const emitter = useEmitter()
const alert = ref({})
const isShowAlert=ref(false)

const {
  syncInitialLoaderTheme,
  syncVuetifyThemeWithTheme: syncConfigThemeWithVuetifyTheme,
  isAppRtl,
  handleSkinChanges,
} = useThemeConfig()

const { global } = useTheme()

// ℹ️ Sync current theme with initial loader theme
syncInitialLoaderTheme()
syncConfigThemeWithVuetifyTheme()
handleSkinChanges()

onMounted(()=>{
  emitter.on('showAlert', showAlert)
})

const showAlert = alertInfo => {
  isShowAlert.value = true
  alert.value = { ...alertInfo }
  setTimeout(() => {
    isShowAlert.value = false
  }, 1500)
}
</script>

<template>
  <VLocaleProvider :rtl="isAppRtl">
    <!-- ℹ️ This is required to set the background color of active nav link based on currently active global theme's primary -->
    <AlertDialog
      v-if="isShowAlert"
      :alert-type="alert.type"
      :alert-content="alert.content"
    />
    <VApp :style="`--v-global-theme-primary: ${hexToRgb(global.current.value.colors.primary)};`">
      <RouterView />
      <ScrollToTop />
    </VApp>
  </VLocaleProvider>
</template>
