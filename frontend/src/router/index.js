import VueRouter from "vue-router";

// Views
import Display from "../views/Display";

export default new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes: [
    {
      path: "/",
      component: Display
    },
    {
      path: "/Display",
      name: "Display",
      component: Display
    },
    {
      path: "/Display/:id",
      component: Display
    }
  ]
});