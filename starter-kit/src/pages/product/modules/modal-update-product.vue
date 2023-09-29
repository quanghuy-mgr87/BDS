<script setup>
import { parseJwt } from '@/helper/helper'
import { useProductStore } from '@/services/product-services/useProductStore'
import { onMounted, ref } from "vue"

const emit = defineEmits(['refreshData'])

// #region data
const dialog = ref(false)
const productInfo = ref({})
const productStore = useProductStore()
const loading = ref(false)
const form = ref()
const userInfo = ref()

const requireFieldRule = [
  value => {
    if(value) return true
    
    return 'This field is required'
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

// #endregion

onMounted(() => {
  const token = localStorage.getItem('accessToken')

  userInfo.value = parseJwt(token)
})


// #region methods
const openDialog = product => {
  dialog.value = !dialog.value
  productInfo.value = { ...product }
}

const updateProduct = async () => {
  const { valid } = await form.value.validate()

  loading.value = true
  if(valid){
    try {
      productInfo.value.dauChuId = userInfo.value.Id
      if(!productInfo.value.id){
        await productStore.addNewProduct(productInfo.value)
      }
      else{
        await productStore.updateProduct(productInfo.value.id, productInfo.value)
      }
      alert('update success')
      dialog.value=false
      loading.value = false
      emit('refreshData')
    } catch (error) {
      alert('server error')
      loading.value = false
    }
  }
  else{
    loading.value = false
    
    return false
  }
}


defineExpose({
  openDialog,
})

// #endregion
</script>

<template>
  <VRow justify="center">
    <VDialog
      v-model="dialog"
      persistent
      width="1024"
    >
      <VCard>
        <VCardTitle>
          <span class="text-h5">User Profile</span>
        </VCardTitle>
        <VCardText>
          <VContainer>
            <VForm ref="form">
              <VRow>
                <VCol cols="8">
                  <VTextField
                    v-model="productInfo.hostName"
                    :rules="requireFieldRule"
                    label="Host name*"
                    required
                  />
                </VCol>
                <VCol cols="4">
                  <VTextField
                    v-model="productInfo.hostPhoneNumber"
                    :rules="phoneNumberRules"
                    label="Host phone number*"
                    hint="example of helper text only on focus"
                  />
                </VCol>
                <VCol cols="8">
                  <VTextField
                    v-model="productInfo.address"
                    label="Address"
                    required
                  />
                </VCol>
                <VCol cols="4">
                  <AppDateTimePicker
                    v-model="productInfo.build"
                    placeholder="Choose time to build"
                  />
                </VCol>
                <VCol cols="12">
                  <VTextField
                    v-model="productInfo.certificateOfLand1"
                    label="Certificate Of Land 1"
                    required
                  />
                </VCol>
                <VCol cols="12">
                  <VTextField
                    v-model="productInfo.certificateOfLand2"
                    label="Certificate Of Land 2"
                    required
                  />
                </VCol>
                <VCol cols="4">
                  <AppDateTimePicker  
                    v-model="productInfo.batDauBan"
                    :rules="requireFieldRule"
                    placeholder="Choose time to sell*"
                  />
                </VCol>
                <VCol cols="4">
                  <VTextField
                    v-model="productInfo.giaBan"
                    type="number"
                    :rules="requireFieldRule"
                    label="Amount*"
                    prefix="đ"
                  />
                </VCol>
                <VCol cols="4">
                  <VTextField
                    v-model="productInfo.phanTramChiaNV"
                    type="number"
                    :rules="requireFieldRule"
                    label="Commission percentage*"
                    prefix="%"
                  />
                </VCol>
              </VRow>
            </VForm>
          </VContainer>
          <small>*indicates required field</small>
        </VCardText>
        <VCardActions>
          <VSpacer />
          <VBtn
            color="blue-darken-1"
            variant="text"
            @click="dialog = false"
          >
            Close
          </VBtn>
          <VBtn
            color="blue-darken-1"
            variant="text"
            :loading="loading"
            @click="updateProduct"
          >
            Save
          </VBtn>
        </VCardActions>
      </VCard>
    </VDialog>
  </VRow>
</template>
