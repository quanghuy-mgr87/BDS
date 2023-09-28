<script setup>
import { useUserStore } from '@/services/user-services/useUserStore'
import { useGenerateImageVariant } from '@core/composable/useGenerateImageVariant'
import authV2LoginIllustrationBorderedDark from '@images/pages/auth-v2-login-illustration-bordered-dark.png'
import authV2LoginIllustrationBorderedLight from '@images/pages/auth-v2-login-illustration-bordered-light.png'
import authV2LoginIllustrationDark from '@images/pages/auth-v2-login-illustration-dark.png'
import authV2LoginIllustrationLight from '@images/pages/auth-v2-login-illustration-light.png'
import authV2MaskDark from '@images/pages/misc-mask-dark.png'
import authV2MaskLight from '@images/pages/misc-mask-light.png'
import { VNodeRenderer } from '@layouts/components/VNodeRenderer'
import { themeConfig } from '@themeConfig'
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { VForm } from 'vuetify/components/VForm'

const authThemeImg = useGenerateImageVariant(authV2LoginIllustrationLight, authV2LoginIllustrationDark, authV2LoginIllustrationBorderedLight, authV2LoginIllustrationBorderedDark, true)
const authThemeMask = useGenerateImageVariant(authV2MaskLight, authV2MaskDark)
const userStore = useUserStore()
const isPasswordVisible = ref(false)
const rememberMe = ref(false)
const refVForm = ref()
const router=useRouter()

// #region rules
const usernameRules = [
  value => {
    if (value) return true

    return 'Username is requred.'
  },
]

const passwordRules = [
  value => {
    if (value) return true

    return 'Password is requred.'
  },
  value => {
    if (/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/.test(value)) return true

    return 'Minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character'
  },
]

//#endregion

const user = ref({
  userName: '',
  password: '',
})

const loading = ref(false)

const onLogin = async () => {
  const { valid } = await refVForm.value.validate()
  if(valid){
    loading.value = true

    const res = (await userStore.login(user.value)).data

    if(res.status === 200){
      alert(res.message)
      if(!localStorage.getItem('accessToken')){
        localStorage.setItem('accessToken', res.data.accessToken)
        localStorage.setItem('refreshToken', res.data.refreshToken)
      }
      router.push({ path: '/' })
    }
    else{
      alert(res.message)
    }

    loading.value = false
  }
  else {
    loading.value = false
    
    return false
  }
}
</script>

<template>
  <VRow
    no-gutters
    class="auth-wrapper bg-surface"
  >
    <VCol
      lg="8"
      class="d-none d-lg-flex"
    >
      <div class="position-relative bg-background rounded-lg w-100 ma-8 me-0">
        <div class="d-flex align-center justify-center w-100 h-100">
          <VImg
            max-width="505"
            :src="authThemeImg"
            class="auth-illustration mt-16 mb-2"
          />
        </div>

        <VImg
          :src="authThemeMask"
          class="auth-footer-mask"
        />
      </div>
    </VCol>

    <VCol
      cols="12"
      lg="4"
      class="auth-card-v2 d-flex align-center justify-center"
    >
      <VCard
        flat
        :max-width="500"
        class="mt-12 mt-sm-0 pa-4"
      >
        <VCardText>
          <VNodeRenderer
            :nodes="themeConfig.app.logo"
            class="mb-6"
          />

          <h5 class="text-h5 mb-1">
            Chào mừng bạn đến với <span class="text-capitalize"> {{ themeConfig.app.title }} </span>! 👋🏻
          </h5>

          <p class="mb-0">
            Vui lòng đăng nhập để có được trải nghiệm tốt nhất
          </p>
        </VCardText>

        <VCardText>
          <VForm ref="refVForm">
            <VRow>
              <!-- tai khoan -->
              <VCol cols="12">
                <AppTextField
                  v-model="user.username"
                  label="Tài khoản"
                  type="text"
                  :rules="usernameRules"
                  autofocus
                />
              </VCol>

              <!-- password -->
              <VCol cols="12">
                <AppTextField
                  v-model="user.password"
                  label="Mật khẩu"
                  :rules="passwordRules"
                  :type="isPasswordVisible ? 'text' : 'password'"
                  :append-inner-icon="isPasswordVisible ? 'tabler-eye-off' : 'tabler-eye'"
                  @click:append-inner="isPasswordVisible = !isPasswordVisible"
                />

                <div class="d-flex align-center flex-wrap justify-space-between mt-2 mb-4">
                  <VCheckbox
                    v-model="rememberMe"
                    label="Lưu mật khẩu"
                  />
                  <a
                    class="text-primary ms-2 mb-1"
                    href="#"
                  >
                    Quên mật khẩu?
                  </a>
                </div>

                <VBtn
                  block
                  :loading="loading"
                  @click="onLogin"
                >
                  Đăng nhập
                </VBtn>
              </VCol>

              <!-- create account -->
              <VCol
                cols="12"
                class="text-center"
              >
                <a
                  class="text-primary ms-2"
                  style="cursor: pointer;"
                  @click="() => {router.push({ path: '/register' })}"
                >
                  Đăng ký tài khoản 
                </a>
              </VCol>

              <VCol
                cols="12"
                class="d-flex align-center"
              />

              <!-- auth providers -->
              <VCol
                cols="12"
                class="text-center"
              >
                <!-- <AuthProvider /> -->
              </VCol>
            </VRow>
          </VForm>
        </VCardText>
      </VCard>
    </VCol>
  </VRow>
</template>

<style lang="scss">
@use "@core/scss/template/pages/page-auth.scss";
</style>

<route lang="yaml">
meta:
  layout: blank
</route>
../services/user-services/useUserStore