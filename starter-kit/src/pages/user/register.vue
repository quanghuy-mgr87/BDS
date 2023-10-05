<script setup>
import useEmitter from '@/helper/useEmitter'
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
import { VForm } from 'vuetify/components/VForm'

// #region
const authThemeImg = useGenerateImageVariant(authV2LoginIllustrationLight, authV2LoginIllustrationDark, authV2LoginIllustrationBorderedLight, authV2LoginIllustrationBorderedDark, true)
const authThemeMask = useGenerateImageVariant(authV2MaskLight, authV2MaskDark)
const isPasswordVisible = ref(false)
const isPasswordConfirmVisible = ref(false)
const userStore = useUserStore()
const confirmCode = ref()
const isConfirm = ref(false)
const refVForm = ref()
const router = useRouter()
const emitter = useEmitter()

const user = ref({
  // username: 'admin',
  // email: 'quanghuynguyenba@gmail.com',
  // name: 'Admin',
  // phoneNumber: '0945123123',
  // password: 'Admin@123',
  // passwordConfirm: 'Admin@123',
})

var loading = ref(false)

//#region rules
const nameRules = [
  value => {
    if(value) return true
    
    return 'Name is required'
  },
]

const emailRules = [
  value => {
    if (value) return true
    
    return 'E-mail is required.'
  },
  value => {
    if (/.+@.+\..+/.test(value)) return true

    return 'E-mail must be valid.'
  },
]

const phoneNumberRules = [
  value => {
    if (value) return true

    return 'Phone number is required.'
  },
  value => {
    if (/(03|05|07|08|09|01[2|6|8|9])+([0-9]{8})\b/.test(value)) return true

    return 'Phone number must be valid.'
  },
]

const passwordRules = [
  value => {
    if (value) return true

    return 'Password is required.'
  },
  value => {
    if (/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/.test(value)) return true

    return 'Minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character'
  },
]

const confirmPasswordRules = [
  value => {
    if (user.value.password === value) return true

    return 'Password does not match'
  },
]

// #endregion

// #endregion

// #region methods
const signUp = async () => {
  const { valid } = await refVForm.value.validate()

  if(valid){
    try {
      loading.value = true
      await  userStore.addUser(user.value)
      emitter.emit('showAlert', {
        type: 'success',
        content: 'Success',
      })
      isConfirm.value=true
      loading.value =false
    } catch (error) {
      emitter.emit('showAlert', {
        type: 'error',
        content: 'Server error',
      })
      loading.value =false
    }
  }
  else {
    loading.value =false
    
    return false
  }
}

const onConfirm = async () => {
  const params = {
    confirmCode: confirmCode.value,
  }

  try {
    await userStore.confirmAddUser(params)
    emitter.emit('showAlert', {
      type: 'success',
      content: 'Success',
    })
    router.push('/')
  } catch (error) {
    emitter.emit('showAlert', {
      type: 'error',
      content: 'Server error',
    })
  }
}

// #endregion
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
        <div class="d-flex align-center justify-center w-100">
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
        v-if="!isConfirm"
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
            Adventure starts here 🚀
          </h5>

          <p class="mb-0">
            Make your app management easy and fun!
          </p>
        </VCardText>

        <VCardText>
          <VForm ref="refVForm">
            <VRow>
              <!-- username -->
              <VCol cols="12">
                <AppTextField
                  v-model="user.username"
                  label="Username"
                  type="text"
                  autofocus
                  :rules="nameRules"
                />
              </VCol>
                
              <!-- email -->
              <VCol cols="12">
                <AppTextField
                  v-model="user.email"
                  label="Email"
                  type="email"
                  :rules="emailRules"
                />
              </VCol>

              <!-- name -->
              <VCol cols="12">
                <AppTextField
                  v-model="user.name"
                  label="Full name"
                  type="text"
                />
              </VCol>

              <!-- name -->
              <VCol cols="12">
                <AppTextField
                  v-model="user.phoneNumber"
                  label="Phone number"
                  type="text"
                  :rules="phoneNumberRules"
                />
              </VCol>

              <!-- password -->
              <VCol cols="12">
                <AppTextField
                  v-model="user.password"
                  label="Password"
                  :type="isPasswordVisible ? 'text' : 'password'"
                  :append-inner-icon="isPasswordVisible ? 'tabler-eye-off' : 'tabler-eye'"
                  :rules="passwordRules"
                  @click:append-inner="isPasswordVisible = !isPasswordVisible"
                />
              </VCol>   

              <!-- password -->
              <VCol cols="12">
                <AppTextField
                  v-model="user.passwordConfirm"
                  label="Confirm password"
                  :rules="confirmPasswordRules"
                  :type="isPasswordConfirmVisible ? 'text' : 'password'"
                  :append-inner-icon="isPasswordConfirmVisible ? 'tabler-eye-off' : 'tabler-eye'"
                  @click:append-inner="isPasswordConfirmVisible = !isPasswordConfirmVisible" 
                />
              </VCol>

              <!-- action -->
              <VCol cols="12">
                <VBtn
                  block
                  :loading="loading"
                  @click="signUp"
                >
                  Sign up
                </VBtn>
              </VCol>
              
              <!-- create account -->
              <VCol
                cols="12"
                class="text-center text-base"
              >
                <span>Already have an account?</span>
                <RouterLink
                  class="text-primary ms-2"
                  to="/"
                >
                  Sign in instead
                </RouterLink>
              </VCol>
            </VRow>
          </VForm>
        </VCardText>
      </VCard>
      <VCard v-else>
        <VCardText>
          <VNodeRenderer
            :nodes="themeConfig.app.logo"
            class="mb-6"
          />

          <h5 class="text-h5 mb-1">
            Verification code has been sent to your email 🚀
          </h5>

          <p class="mb-0">
            Please check your email, then enter the verification code to complete registration!
          </p>
        </VCardText>
        <VCardText>
          <VForm
            @submit.prevent
            @submit="onConfirm"
          >
            <VRow>
              <VCol> <VTextField v-model="confirmCode" /></VCol>
              <VCol>
                <VBtn type="submit">
                  Submit
                </VBtn>
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
</route>../../services/user-services/useUserStore