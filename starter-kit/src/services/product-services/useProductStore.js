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
  },
})