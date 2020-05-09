<template>
  <div class="d-flex justify-content-center align-items-center">
    <div class="splash-screen d-flex flex-column justify-content-center">
      <b-img class="logo" src="../assets/slaps_logo.png" fluid />
      <div
        v-if="error" 
        class="status alert-danger border border-danger rounded p-2 d-flex flex-row justify-content-center align-items-center">
        <div class="mr-3">
          <b-icon 
            icon="exclamation-triangle-fill"
            style="font-size: 150%;"/>
        </div>
        <div class="status-text">
          <slot />
        </div>
      </div>
      <div 
        v-else
        class="status border border-primary bg-primary rounded text-light p-2 d-flex flex-row justify-content-center align-items-center">
        <div class="mr-3">
          <b-spinner v-model="spinner" />
        </div>
        <div class="status-text">
          <slot />
        </div>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  name: "SplashScreen",
  props: {
    spinner: {
      type: Boolean,
      default: false
    },
    error: {
      type: Boolean, 
      default: false
    }
  }
}
</script>
<style lang="scss" scoped>
.splash-screen {
  max-width: 75%;
}
.status-text {
  font-size: 1.5rem;
}
.status {
  margin-top: 3rem;
}
@media (min-width: 1200px) { 
  .splash-screen {
    max-width: 50%;
  }
  .status-text {
    font-size: 2rem;
  }
}
</style>