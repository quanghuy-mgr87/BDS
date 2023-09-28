export default [
  {
    title: 'Home',
    to: { name: 'index' },
    icon: { icon: 'tabler-smart-home' },
  },
  {
    title: 'Danh sách sản phẩm',
    children: [
      { 
        title: 'Quản lý sản phẩm', 
        to: 'product-product-management',
      },

      // { title: 'Preview', to: { name: 'apps-invoice-preview-id', params: { id: '5036' } } },
      // { title: 'Edit', to: { name: 'apps-invoice-edit-id', params: { id: '5036' } } },
      // { title: 'Add', to: 'apps-invoice-add' },
    ],
  },
]
