import Vue from "vue";
import App from "./App.vue";
import VueRouter from "vue-router";
import Vuex from "vuex";
import router from "./router";
import store from "./store";

// Core plugins
Vue.use(VueRouter);
Vue.use(Vuex);

// Bootstrap-Vue
import { BootstrapVue, BootstrapVueIcons } from 'bootstrap-vue'
import './assets/style_master.scss'
Vue.use(BootstrapVue)
Vue.use(BootstrapVueIcons)

// Custom plugins
import Api from "./plugins/api";
Vue.use(Api);
import DisplayApi from "./plugins/displayApi";
Vue.use(DisplayApi);
import Utils from "./plugins/utils";
Vue.use(Utils);

Vue.config.productionTip = false;
new Vue({
  render: h => h(App),
  router,
  store,
}).$mount("#app");
