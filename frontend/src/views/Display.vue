<template>
  <div class="display-container">
    <transition name="fade">
      <template v-if="loading.state || error.state">
        <SplashScreen 
          class="slide"
          :error="error.state">
          {{message}}
        </SplashScreen>
      </template>
      <template v-else>
        <Slideshow />
      </template>
    </transition>
  </div>
</template>
<script>
import Slideshow from "../components/Slideshow";
import SplashScreen from "../components/SplashScreen";
export default {
  name: "Display",
  components: { Slideshow, SplashScreen },
  data() {
    return {
      loading: {
        state: true,
        message: "Preparing display system..."
      },
      error: {
        state: false,
        message: "An unhandled error has occurred."
      }
    };
  },
  computed: {
    settings() {
      return this.$store.getters["display/settings"];
    },
    message() {
      if (this.error.state) return this.error.message;
      else return this.loading.message;
    }
  },
  async mounted() {
    this.loading.state = true;
    if (!this.$utils.isNullOrWhitespace(this.$route.params.id))
      this.displayId = this.$route.params.id;

    // Load id / register with server
    this.loading.message = "Registering with server...";
    if (!await this.$store.dispatch("display/getId"))
    {
      this.error.state = true;
      this.error.message = "Error registering with server. Please make sure the server is running and accessible.";
      return;
    }

    // Get display settings from API
    this.loading.message = "Getting display settings...";
    if (!await this.$store.dispatch("display/getSettings"))
    {
      this.error.state = true;
      this.error.message = "Error getting display settings. Please make sure the server is running and accessible.";
      return;
    }

    // Get slide set from API
    this.loading.message = "Getting slides...";
    if (!await this.$store.dispatch("display/getSlideSet"))
    {
      this.error.state = true;
      this.error.message = "Error getting slide set. Please make sure the server is running and accessible.";
      return;
    }

    // Pre-load content for slides
    this.loading.message = "Pre-loading slide content...";
    if (!await this.$store.dispatch("display/getSlidesContent"))
    {
      this.error.state = true;
      this.error.message = "Error getting slide content. Please make sure the server is running and accessible.";
      return;
    }

    this.loading.state = false;
    this.loading.message = "Startup complete!";
  }
};
</script>
<style scoped>
.display-container {
  height: 100vh;
  background-color: #000;
}
.slide {
  width: 100%;
  height: 100%;
}

/* Transitions */
.fade-enter-active, .fade-leave-active {
  transition: opacity .5s;
}
.fade-enter, .fade-leave-to {
  opacity: 0;
}
</style>