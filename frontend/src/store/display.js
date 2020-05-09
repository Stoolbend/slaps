/*
Display Store - Contains the state + actions for playing back slides, and settings for doing so.
*/

const state = {
  slides: [],
  settings: []
};

const getters = {
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
  slides: (state, data) => {
    state.slides = data;
  },
  settings: (state, data) => {
    state.settings = data;
  }
};

const actions = {
  async getSettings(ctx, id = null) {
    let request = await this._vm.$api.getDisplaySettings(id, { token: null });
    if (request.isError) {
      ctx.commit("settings", []);
      return false;
    }
    let result = request.result;
    ctx.commit("settings", result);
    return true;
  },
  async getSlideSet(ctx, id) {
    let request = await this._vm.$api.getSlideSet(id, { token: null });
    if (request.isError) {
      ctx.commit("slides", []);
      return false;
    }
    let result = request.result;
    ctx.commit("slides", result);
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