<script setup>
import avatar1 from '@images/avatars/avatar-1.png'
import avatar2 from '@images/avatars/avatar-2.png'
import avatar3 from '@images/avatars/avatar-3.png'
import avatar4 from '@images/avatars/avatar-4.png'
import avatar5 from '@images/avatars/avatar-5.png'
import avatar6 from '@images/avatars/avatar-6.png'
import avatar7 from '@images/avatars/avatar-7.png'
import avatar8 from '@images/avatars/avatar-8.png'

const props = defineProps({
  isDialogVisible: {
    type: Boolean,
    required: true,
  },
})

const emit = defineEmits(['update:isDialogVisible'])

const dialogVisibleUpdate = val => {
  emit('update:isDialogVisible', val)
}

const membersList = [
  {
    avatar: avatar1,
    name: 'User1',
    email: 'user1@gmail.com',
    permission: 'Có thể chỉnh sửa',
  },
  {
    avatar: avatar2,
    name: 'User2',
    email: 'user2@yahoo.com',
    permission: 'Người sở hữu',
  },
  {
    avatar: avatar3,
    name: 'User3',
    email: 'user3@jujpejah.net',
    permission: 'Có thể bình luận',
  },
  {
    avatar: avatar4,
    name: 'User4',
    email: 'user4@nuv.io',
    permission: 'Có thể xem',
  },
  {
    avatar: avatar5,
    name: 'Julian Murphy',
    email: 'lunebame@umdomgu.net',
    permission: 'Can Edit',
  },
  {
    avatar: avatar6,
    name: 'Sophie Gilbert',
    email: 'ha@sugit.gov',
    permission: 'Can View',
  },
  {
    avatar: avatar7,
    name: 'Chris Watkins',
    email: 'zokap@mak.org',
    permission: 'Can Comment',
  },
  {
    avatar: avatar8,
    name: 'Adelaide Nichols',
    email: 'ujinomu@jigo.com',
    permission: 'Can Edit',
  },
]
</script>

<template>
  <VDialog
    :model-value="props.isDialogVisible"
    max-width="800"
    @update:model-value="dialogVisibleUpdate"
  >
    <!-- 👉 Dialog close btn -->
    <DialogCloseBtn @click="$emit('update:isDialogVisible', false)" />

    <VCard class="share-project-dialog pa-5 pa-sm-8">
      <VCardText>
        <h5 class="text-h5 text-center mb-3">
          Chia sẻ dự án
        </h5>
        <p class="text-sm-body-1 text-center mb-6">
          Chia sẻ dự án với các thành viên nhóm
        </p>

        <p class="font-weight-medium mb-1">
          Thêm thành viên
        </p>
        <AppAutocomplete
          :items="membersList"
          item-title="name"
          item-value="name"
          placeholder="Add project members..."
          density="compact"
        >
          <template #item="{ props: listItemProp, item }">
            <VListItem v-bind="listItemProp">
              <template #prepend>
                <VAvatar
                  :image="item.raw.avatar"
                  size="30"
                />
              </template>
            </VListItem>
          </template>
        </AppAutocomplete>

        <h6 class="text-h6 mb-4 mt-8">
          8 thành viên
        </h6>

        <VList class="card-list">
          <VListItem
            v-for="member in membersList"
            :key="member.name"
          >
            <template #prepend>
              <VAvatar :image="member.avatar" />
            </template>

            <VListItemTitle class="text-sm">
              {{ member.name }}
            </VListItemTitle>
            <VListItemSubtitle>
              {{ member.email }}
            </VListItemSubtitle>

            <template #append>
              <VBtn
                variant="plain"
                color="default"
                :icon="$vuetify.display.xs"
              >
                <span class="d-none d-sm-block">{{ member.permission }}</span>
                <VIcon icon="tabler-chevron-down" />

                <VMenu activator="parent">
                  <VList :selected="[member.permission]">
                    <VListItem
                      v-for="(item, index) in ['Owner', 'Can Edit', 'Can Comment', 'Can View']"
                      :key="index"
                      :value="item"
                    >
                      <VListItemTitle>{{ item }}</VListItemTitle>
                    </VListItem>
                  </VList>
                </VMenu>
              </VBtn>
            </template>
          </VListItem>
        </VList>

        <div class="d-flex align-center justify-space-between flex-wrap gap-3 mt-6">
          <h6 class="text-sm font-weight-medium d-flex align-start">
            <VIcon
              icon="tabler-users"
              class="me-2"
            />
            <span>Đến với Sanctuary</span>
          </h6>

          <VBtn class="text-capitalize">
            Sao chép link dự án
          </VBtn>
        </div>
      </VCardText>
    </VCard>
  </VDialog>
</template>

<style lang="scss">
.share-project-dialog {
  .card-list {
    --v-card-list-gap: 1rem;
  }
}
</style>
