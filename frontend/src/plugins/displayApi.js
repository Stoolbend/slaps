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
    Vue.prototype.$displayApi = {
      async registerDisplay({ token }) {
        let uri = encodeURI(apiHost + "/Displays/Register");
        let options = cloneDeep(defaultOptions);
        options = handleToken(options, token);
        try {
          let result = await axios.get(uri, options);
          return { result, isError: false };
        } catch (error) {
          return { error, isError: true };
        }
      },
      async getDisplaySettings(id = null, { token }) {
        let uri = "";
        if (isNullOrWhitespace(id))
          uri = encodeURI(apiHost + "/Displays/Settings");
        else
          uri = encodeURI(apiHost + "/Displays/Settings/" + id);
        let options = cloneDeep(defaultOptions);
        options = handleToken(options, token);
        try {
          let result = await axios.get(uri, options);
          return { result, isError: false };
        } catch (error) {
          return { error, isError: true };
        }
      },
      async getDisplaySlideSet(id, { token }) {
        if (isNullOrWhitespace(id)) throw new Error("getDisplaySlideSet - Missing parameter: id");
        let uri = encodeURI(apiHost + "/Displays/" + id + "/SlideSet");
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
        let uri = encodeURI(apiHost + "/Slides/" + id + "/Content");
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