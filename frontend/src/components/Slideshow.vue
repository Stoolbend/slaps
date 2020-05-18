<template>
  <transition 
    name="fade"
    mode="in-out">
    <div
      v-html="currentSlide.content"
      :key="currentSlide.id"
      class="slide-content" />
  </transition>
</template>
<script>
export default {
  props: {},
  data () {
    return {
      index: 0,
      displayTimer: null
    }
  },
  computed: {
    slides () {
      return this.$store.getters["display/slides"];
    },
    currentSlide () {
      if (this.slides && this.slides.length > 0) {
        return this.slides[this.index];
      } else return "There are no slides in the deck.";
    },
    nextIndex () {
      if (this.slides && this.slides.length > 0 && this.index < this.slides.length - 1) {
        return this.index + 1;
      } else return 0;
    }
  },
  methods: {
    startDisplayTimer () {
      this.displayTimer = setInterval(() => {
        this.index = this.nextIndex;
      }, this.currentSlide.displaySeconds * 1000);
    }
  },
  mounted () {
    this.startDisplayTimer();
  },
  beforeDestroy () {
    clearInterval(this.displayTimer);
  },
  watch: {
    slides () {
      // Slide set has changed. Reset state
      // Or should we? The timer just 
    }
  }
}
</script>
<style scoped>
.slide-content {
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