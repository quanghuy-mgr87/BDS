<script setup>
import useEmitter from '@/helper/useEmitter'
import { useBillStore } from '@/services/bill/useBillStore'
import { useProductStore } from '@/services/product-services/useProductStore'
import { ref } from 'vue'

const emit = defineEmits(['refreshData'])

// #region data
const dialog = ref(false)
const products = ref([])
const houseViewingBill = ref({})
const loading = ref(false)
const form = ref()
const emitter = useEmitter()
const billStore = useBillStore()
const productStore = useProductStore()
const userInfo = JSON.parse(localStorage.getItem('userInfo'))

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

// #region methods
onMounted(() => {
  getAllProduct()
})

const getAllProduct = async () => {
  const params = {}

  products.value = (await productStore.getAll(params)).data.data
  products.value.forEach(x=>{
    x.title =`${x.hostName} (${x.hostPhoneNumber}) - ${x.address}`
  })
}

const openDialog = bill => {
  dialog.value = !dialog.value
  houseViewingBill.value = { ...bill }
}

const updateBill = async () => {
  try {
    loading.value = true
    houseViewingBill.value.nhanVienId = userInfo.Id
    houseViewingBill.value.custumerId = userInfo.Id
    if(!houseViewingBill.value.id){
      await billStore.createBill(houseViewingBill.value)
    }
    else{
      houseViewingBill.value.phieuXemNhaId = houseViewingBill.value.id
      await billStore.updateBill(houseViewingBill.value)
    }
    emitter.emit('showAlert', {
      type: 'success',
      content: 'Success!',
    })
    emit('refreshData')
    dialog.value = false
    loading.value = false
  } catch (error) {
    emitter.emit('showAlert', {
      type: 'error',
      content: 'Server error!',
    })
    loading.value = false
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
          <span class="text-h5">HOUSE VIEWING BILL</span>
        </VCardTitle>
        <VCardText>
          <VContainer>
            <VForm ref="form">
              <VRow>
                <VCol cols="8">
                  <VTextField
                    v-model="houseViewingBill.custumerName"
                    :rules="requireFieldRule"
                    label="Customer name*"
                    required
                  />
                </VCol>
                <VCol cols="4">
                  <VTextField
                    v-model="houseViewingBill.custumerPhoneNumber"
                    :rules="phoneNumberRules"
                    label="Customer phone number*"
                    hint="example of helper text only on focus"
                  />
                </VCol>
                <VCol cols="12">
                  <VTextarea
                    v-model="houseViewingBill.desciption"
                    label="Description"
                    required
                  />
                </VCol>
                <VCol cols="12">
                  <VFileInput
                    v-model="houseViewingBill.custumerIdImg1"
                    label="Image 1"
                    required
                  />
                </VCol>
                <VCol cols="12">
                  <VFileInput
                    v-model="houseViewingBill.custumerIdImg2"
                    label="Image 2"
                    required
                  />
                </VCol>
                <VCol cols="12">
                  <VSelect
                    v-model="houseViewingBill.nhaId"
                    :items="products"
                    item-value="id"
                    item-title="title"
                    label="Select house"
                  />
                </VCol>
                <VCol cols="4">
                  <VSwitch
                    v-model="houseViewingBill.banThanhCong"
                    label="Is sold?"
                    :color="houseViewingBill.banThanhCong ? 'success' : ''"
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
            @click="updateBill"
          >
            Save
          </VBtn>
        </VCardActions>
      </VCard>
    </VDialog>
  </VRow>
</template>
