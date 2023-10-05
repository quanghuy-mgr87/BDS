import { roleEnum } from '@/helper/roleEnum'

const userInfo = JSON.parse(localStorage.getItem('userInfo'))

export default [
  {
    title: 'Home',
    to: { name: 'index' },
    icon: { icon: 'tabler-smart-home' },
  },
  {
    title: 'Product management',
    roleId: [roleEnum.ADMIN],
    children: [
      { 
        title: 'Product list', 
        to: 'product-product-management',
      },
      { 
        title: 'Sold products list', 
        to: 'soldProducts-sold-product-list',
      },

      // { title: 'Preview', to: { name: 'apps-invoice-preview-id', params: { id: '5036' } } },
      // { title: 'Edit', to: { name: 'apps-invoice-edit-id', params: { id: '5036' } } },
      // { title: 'Add', to: 'apps-invoice-add' },
    ],
  },
  {
    title: 'House viewing bill',
    roleId: [roleEnum.ADMIN, roleEnum.OWNER, roleEnum.MANAGER, roleEnum.MOD, roleEnum.STAFF],
    children: [
      { 
        title: 'Bill list', 
        to: 'houseViewing-bill',
      },
    ],
  },
].filter(x=> (x.roleId && x.roleId.findIndex(y => y == userInfo.RoleId) !== -1) || !x.roleId)
