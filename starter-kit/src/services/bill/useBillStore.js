import axios from '@axios'
import { defineStore } from 'pinia'

export const useBillStore = defineStore('useBillStore', {
  actions: {
    getAllBill(params){
      return new Promise((resolve, reject) => {
        axios.get('/phieuxemnha/GetAllPhieuXemNha', { ...params })
          .then(res => resolve(res))
          .catch(error => reject(error))
      })
    },
    createBill(params){
      return new Promise((resolve, reject) => {
        axios.post('/phieuxemnha/CreatePhieuXemNha', { ...params })
          .then(res => resolve(res))
          .catch(error => reject(error))
      })
    },
    updateBill(params){
      return new Promise((resolve, reject) => {
        axios.put('/phieuxemnha/UpdatePhieuXemNha', { ...params })
          .then(res => resolve(res))
          .catch(error => reject(error))
      })
    },
    deleteBill(phieuXemNhaId){
      return new Promise((resolve, reject) => {
        axios.delete(`/phieuxemnha/DeletePhieuXemNha/${phieuXemNhaId}`)
          .then(res => resolve(res))
          .catch(error => reject(error))
      })
    },
  },
})