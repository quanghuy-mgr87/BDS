<script setup>
import { useProductStore } from '@/services/product-services/useProductStore'
import moment from "moment"
import { onMounted, ref } from 'vue'
import ModalUpdateProduct from './modules/modal-update-product.vue'

// #region data
const productStore = useProductStore()
const products = ref([])
const refUpdateProduct = ref()

// #endregion

onMounted(() => {
  getAllProduct()
})

const getAllProduct = async () => {
  const params = {}

  products.value = (await productStore.getAll(params)).data
}

const openUpdateProductDialog = product => {
  refUpdateProduct.value.openDialog(product)
} 
</script>

<template>
  <VCard style="padding: 10px;">
    <div class="d-flex justify-end mb-3">
      <VBtn @click="openUpdateProductDialog">
        Create new product
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
            Host name
          </th>
          <th class="text-left">
            Host phone number
          </th>
          <th class="text-left">
            Build
          </th>
          <th class="text-left">
            Certificate of land 1
          </th>
          <th class="text-left">
            Certificate of land 2
          </th>
          <th class="text-left">
            Thao tác
          </th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="(item, index) in products"
          :key="index"
        >
          <td>{{ index + 1 }}</td>
          <td>{{ item.hostName }}</td>
          <td>{{ item.hostPhoneNumber }}</td>
          <td>{{ moment(item.build).format('DD/MM/YYYY') }}</td>
          <td>{{ item.certificateOfLand1 }}</td>
          <td>{{ item.certificateOfLand2 }}</td>
          <td>
            <div style="display: flex; justify-content: space-between;">
              <VBtn
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
              >
                Delete
              </VBtn>
            </div>
          </td>
        </tr>
      </tbody>
    </VTable>
    <ModalUpdateProduct
      ref="refUpdateProduct"
      @refresh-data="getAllProduct"
    />
  </VCard>
</template>
