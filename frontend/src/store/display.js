/*
Display Store - Contains the state + actions for playing back slides, and settings for doing so.
*/

const state = {
  id: null,
  slides: [],
  settings: []
};

const getters = {
  id: (state) => {
    return state.id;
  },
  slides: (state) => {
    return state.slides;
  },
  settings: (state) => {
    return state.settings;
  },
  setting: (state) => (key) => {
    if (!state.settings || state.settings.length < 1) 
      return null;
    return state.settings[key];
  }
};

const mutations = {
  id: (state, data) => {
    state.id = data;
  },
  slide: (state, data) => {
    for (let i = 0; i < state.slides.length; i++) {
      if (state.slides[i].id == data.id) {
        state.slides[i] = data;
        return;
      }
      else
        continue;
    }
  },
  slides: (state, data) => {
    state.slides = data;
  },
  settings: (state, data) => {
    state.settings = data;
  }
};

const actions = {
  async getId(ctx) {
    let id = localStorage.getItem("displayId");
    if (id === null || id === undefined || id === "") {
      let request = await this._vm.$displayApi.registerDisplay({ token: null });
      if (request.isError) {
        ctx.commit("id", null);
        return false;
      }
      id = request.result.data.id;
      localStorage.setItem("displayId", id);
    }
    ctx.commit("id", id);
    return true;
  },
  async getSettings(ctx) {
    let request = await this._vm.$displayApi.getDisplaySettings(null, { token: null });
    if (request.isError) {
      ctx.commit("settings", []);
      return false;
    }
    let result = request.result;
    if (result.status === 204)
      ctx.commit("settings", []);
    else
      ctx.commit("settings", result.data);
    return true;
  },
  async getSlideSet(ctx) {
    let request = await this._vm.$displayApi.getDisplaySlideSet(ctx.getters["id"], { token: null });
    if (request.isError) {
      ctx.commit("slides", []);
      return false;
    }
    let result = request.result;
    if (result.status === 204)
      ctx.commit("slides", []);
    else
      if (result.data !== null && result.data.slides)
        ctx.commit("slides", result.data.slides);
      else
        ctx.commit("slides", []);
    return true;
  },
  async getSlidesContent(ctx) {
    let slides = ctx.getters["slides"];
    for (let i = 0; i < slides.length; i++) {
      let request = await this._vm.$displayApi.getSlideContent(slides[i].id, { token: null });
      if (request.isError) {
        continue;
      }
      ctx.commit("slide", request.result.data);
    }
    return true;
  }
};

export default {
  namespaced: true,
  state,
  getters,
  mutations, 
  actions
};