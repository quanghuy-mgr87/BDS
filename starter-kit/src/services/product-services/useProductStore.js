import axios from '@axios'
import { defineStore } from 'pinia'

export const useProductStore = defineStore('useProductStore', {
  actions: {
    getAll(params){
      return new Promise((resolve, reject) => {
        axios.get('/product/GetAll', { ...params })
          .then(res => resolve(res))
          .catch(error => reject(error))
      })
    },
    addNewProduct(params){
      return new Promise((resolve, reject) => {
        axios.post('/product/AddNewProduct', { ...params })
          .then(res => resolve(res))
          .catch(error => reject(error))
      })
    },
    updateProduct(id, params){
      return new Promise((resolve, reject) => {
        axios.put(`/product/UpdateProduct/${id}`, { ...params })
          .then(res => resolve(res))
          .catch(error => reject(error))
      })
    },
    deleteProduct(id){
      return new Promise((resolve, reject) => {
        axios.put(`/product/DeleteProduct/${id}`)
          .then(res => resolve(res))
          .catch(error => reject(error))
      })
    },
  },
})