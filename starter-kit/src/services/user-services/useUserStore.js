import axios from '@axios'
import { defineStore } from 'pinia'

export const useUserStore = defineStore('useUserStore', {
  actions: {
    addUser(params){
      return new Promise((resolve, reject) => {
        axios.post('/auth/register', { ...params })
          .then(res => resolve(res))
          .catch(error => reject(error))
      })
      
    },
    confirmAddUser(params){
      return new Promise((resolve, reject) => {
        axios.post('auth/confirm-create-account', { ...params })
          .then(res => resolve(res))
          .catch(error => reject(error))
      })
    },
    login(params){
      return new Promise((resolve, reject) => {
        axios.post('/auth/login', { ...params })
          .then(res => resolve(res))
          .catch(error => reject(error))
      })
    },
  },
})