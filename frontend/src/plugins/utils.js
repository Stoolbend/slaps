export default {
  install: (Vue) => {
    Vue.prototype.$utils = {
      isNullOrWhitespace(value) {
        return !value || !value.trim();
      },
    }
  }
}