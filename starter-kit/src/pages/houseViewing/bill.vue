<script  setup>
import useEmitter from '@/helper/useEmitter'
import { useBillStore } from '@/services/bill/useBillStore'
import { onMounted, ref } from 'vue'
import ModalUpdateBill from './modules/modal-update-bill.vue'

const billStore = useBillStore()
const emitter = useEmitter()
const refModalUpdateBill = ref()
const showConfirmDialog = ref(false)
const billDeleteId = ref(0)

const bills = ref([])

onMounted(() => {
  getAllBill()
})

const getAllBill = async () => {
  try {
    const params = {
      
    }

    bills.value = (await billStore.getAllBill(params)).data
  } catch (error) {
    console.log(error)
    emitter.emit('showAlert', {
      type: 'error',
      content: 'Server error',
    })
  }
}

const deleteBill = async isConfirm => {
  try {  
    if(isConfirm){
      await billStore.deleteBill(billDeleteId.value)
      getAllBill()
      emitter.emit('showAlert', {
        type: 'success',
        content: 'Success!',
      })
    }
  } catch (error) {
    emitter.emit('showAlert', {
      type: 'error',
      content: 'Server error!',
    })
  }
  showConfirmDialog.value = false
}

const openUpdateProductDialog = bill => {
  refModalUpdateBill.value.openDialog(bill)
}

// aaa
</script>

<template>
  <VCard style="padding: 10px">
    <div class="d-flex justify-end mb-3">
      <VBtn @click="openUpdateProductDialog">
        Create bill
      </VBtn>
    </div>
    <VTable
      fixed-header
      height="500px"
    >
      <thead>
        <tr>
          <th class="text-left">
            #
          </th>
          <th class="text-left">
            Customer name
          </th>
          <th class="text-left">
            Customer phone number
          </th>
          <th class="text-left">
            Description
          </th>
          <th class="text-left">
            House info
          </th>
          <th class="text-left">
            Status
          </th>
          <th class="text-left">
            Action
          </th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="(item, index) in bills"
          :key="index"
        >
          <td>{{ index + 1 }}</td>
          <td>{{ item.custumerName }}</td>
          <td>{{ item.custumerPhoneNumber }}</td>
          <td>{{ item.desciption }}</td>
          <td>house test</td>
          <td>status</td>
          <td>
            <div style="display: flex;">
              <VBtn
                style="margin-right: 10px;"
                variant="text"
                size="small"
                @click="openUpdateProductDialog(item)"
              >
                Edit
              </VBtn>
              <VBtn
                variant="text"
                size="small"
                color="#bc2f2f"
                @click="() => {
                  showConfirmDialog = true;
                  billDeleteId = item.id;
                }"
              >
                Delete
              </VBtn>
            </div>
          </td>
        </tr>
      </tbody>
    </VTable>
  </VCard>
  <ConfirmDialog
    :is-dialog-visible="showConfirmDialog"
    confirmation-question="Are you sure to delete this item?"
    confirm-title="Success"
    @confirm="deleteBill"
  />
  <ModalUpdateBill
    ref="refModalUpdateBill"
    @refresh-data="getAllBill"
  />
</template>
