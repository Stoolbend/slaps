import axios from "axios";
import { cloneDeep } from "lodash-es";

const apiHost = "http://localhost:5000/api";
//let apiHost = window.location.hostname + "/api";

const defaultOptions = {
  Accept: "application/json",
  "Content-Type": "application/json"
};

function isNullOrWhitespace(value) {
  return !value || !value.trim();
}

function handleToken(axiosOptions, token) {
  if (!isNullOrWhitespace(token)) axiosOptions.Authorize = "Bearer " + token;
  return axiosOptions;
}

export default {
  install: (Vue) => {
    Vue.prototype.$api = {
      async getNewToken(username, password) {
        return !(username + password);
      },
      async renewToken(token) {
        return !(token);
      },
      async getDisplaySettings(id = null, { token }) {
        let uri = "";
        if (isNullOrWhitespace(id))
          uri = encodeURI(apiHost + "/Settings/Display");
        else
          uri = encodeURI(apiHost + "/Settings/Display/" + id);
        let options = cloneDeep(defaultOptions);
        options = handleToken(options, token);
        try {
          let result = await axios.get(uri, options);
          return { result, isError: false };
        } catch (error) {
          return { error, isError: true };
        }
      },
      async getSlideSets({ token }) {
        let uri = encodeURI(apiHost + "/SlideSets");
        let options = cloneDeep(defaultOptions);
        options = handleToken(options, token);
        try {
          let result = await axios.get(uri, options);
          return { result, isError: false };
        } catch (error) {
          return { error, isError: true };
        }
      },
      async getSlideSet(id, { token }) {
        if (isNullOrWhitespace(id)) throw new Error("getSlideSet - Missing parameter: id");
        let uri = encodeURI(apiHost + "/SlideSets/" + id);
        let options = cloneDeep(defaultOptions);
        options = handleToken(options, token);
        try {
          let result = await axios.get(uri, options);
          return { result, isError: false };
        } catch (error) {
          return { error, isError: true };
        }
      },
      async getSlideContent(id, { token }) {
        if (isNullOrWhitespace(id)) throw new Error("getSlideContent - Missing parameter: id");
        let uri = encodeURI(apiHost + "/Slides" + id + "/Content");
        let options = cloneDeep(defaultOptions);
        options = handleToken(options, token);
        try {
          let result = await axios.get(uri, options);
          return { result, isError: false };
        } catch (error) {
          return { error, isError: true };
        }
      }
    }
  }
}