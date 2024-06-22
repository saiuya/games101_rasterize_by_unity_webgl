<script setup lang="ts">
// This starter template is using Vue 3 <script setup> SFCs
// Check out https://vuejs.org/api/sfc-script-setup.html#script-setup
import Greet from "./components/Greet.vue";
import { ref, onMounted } from "vue";


let unityWindow = ref<any>(null);
let unityInput = ref<any>("unity的输入内容");

function colorChange(): void {
  unityWindow.value.contentWindow.sendUnityEvent(
    "QuadFather",
    "changeTriangleColor",
  );
}


/**
 *  监听index.html发送来的数据
 */
onMounted(() => {
  window.addEventListener(
    "message",
    (res: {
      data: {
        status: string;
        data: string | null;
      };
    }) => {
      if (res.data.status == "loading") {
        console.log("unity场景加载完成");
      }
      if (res.data.status == "input") {
        unityInput.value = res.data.data!!;
      }
    },
    false,
  );
});



</script>

<template>
  <div class="container">
    <h1>Welcome to WebGL</h1>
    <button @click="colorChange">改变Unity物体颜色</button>
    <div>{{ unityInput }}</div>
    <iframe
      ref="unityWindow"
      src="unity/index.html"
      style="position: absolute; width: 100%; height: 100%"
    ></iframe>

    <Greet />
  </div>
</template>

<style scoped></style>
