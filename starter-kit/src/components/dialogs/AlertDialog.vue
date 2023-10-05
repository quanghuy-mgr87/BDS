<script setup>
defineProps({
  alertType: {
    type: String,
    require: false,
    default: undefined,
  },
  alertContent: {
    type: String,
    require: false,
    default: undefined,
  },
})
</script>

<template>
  <VAlert
    style="z-index: 20000;"
    border="start"
    :type="alertType"
    :text="alertContent"
  />
</template>