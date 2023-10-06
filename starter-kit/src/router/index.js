import { roleEnum } from '@/helper/roleEnum'
import { hasRoles } from '@/helper/roleServices'
import { setupLayouts } from 'virtual:generated-layouts'
import { createRouter, createWebHistory } from 'vue-router'
import routes from '~pages'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: to => {
        if(localStorage.getItem('accessToken')){
          return { name: 'index' }
        }
        
        return { name: 'login', query: to.query }
      },
    },
    {
      path: '/register',
      component: () => import('@/pages/user/register.vue'),
    },
    {
      path: '/product/product-management',
      redirect: to => {
        if(localStorage.getItem('accessToken') && hasRoles([roleEnum.ADMIN, roleEnum.MANAGER, roleEnum.OWNER])) {
          return { name: 'product-product-management', query: to.query }
        }
        
        return { name: 'not-authorized', query: to.query }
        
      },
    },
    {
      path: '/soldProducts/sold-product-list',
      redirect: to => {
        if(localStorage.getItem('accessToken') && hasRoles([roleEnum.ADMIN, roleEnum.MANAGER, roleEnum.OWNER])) {
          return { name: 'soldProducts-sold-product-list', query: to.query }
        }
        
        return { name: 'not-authorized', query: to.query }
        
      },
    },
    {
      path: '/houseViewing/bill',
      redirect: to => {
        if(localStorage.getItem('accessToken') && hasRoles([roleEnum.ADMIN, roleEnum.OWNER, roleEnum.MANAGER, roleEnum.MOD, roleEnum.STAFF])) {
          return { name: 'houseViewing-bill', query: to.query }
        }
        
        return { name: 'not-authorized', query: to.query }
        
      },
    },
    ...setupLayouts(routes),
  ],
})


// Docs: https://router.vuejs.org/guide/advanced/navigation-guards.html#global-before-guards
// router.beforeEach(to => {
//   const isLoggedIn = isUserLoggedIn()

//   /*
  
//     ℹ️ Commented code is legacy code
  
//     if (!canNavigate(to)) {
//       // Redirect to login if not logged in
//       // ℹ️ Only add `to` query param if `to` route is not index route
//       if (!isLoggedIn)
//         return next({ name: 'login', query: { to: to.name !== 'index' ? to.fullPath : undefined } })
  
//       // If logged in => not authorized
//       return next({ name: 'not-authorized' })
//     }
  
//     // Redirect if logged in
//     if (to.meta.redirectIfLoggedIn && isLoggedIn)
//       next('/')
  
//     return next()
  
//     */
//   if (canNavigate(to)) {
//     if (to.meta.redirectIfLoggedIn && isLoggedIn)
//       return '/'
//   }
//   else {
//     if (isLoggedIn)
//       return { name: 'not-authorized' }
//     else
//       return { name: 'login', query: { to: to.name !== 'index' ? to.fullPath : undefined } }
//   }
// })
export default router
