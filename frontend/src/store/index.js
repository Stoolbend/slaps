import Vue from "vue";
import Vuex from "vuex";

Vue.use(Vuex);

import display from "./display";

export default new Vuex.Store({
  modules: {
    display
  }
});
