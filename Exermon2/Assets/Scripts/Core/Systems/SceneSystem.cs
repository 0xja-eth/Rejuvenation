﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;

using Core.Data.Loaders;

using Core.UI.Utils;

using GameModule.Services;

namespace Core.Systems {

    /// <summary>
    /// 用于控制场景切换的类
    /// </summary>
    /// <remarks>
    /// 用于控制全局的场景切换，即将原本的 SceneUtils 的功能分出来了，并将层次提升到 System 层
    /// </remarks>
    public class SceneSystem : BaseSystem<SceneSystem> {

        /// <summary>
        /// 游戏场景数据
        /// </summary>
        public enum Scene {

            NoneScene = -1,

			TitleScene = 0,
			TestScene,

			TutorialScene,
			TaiqingScene,
			TaiqingRoom1Scene,
			TaiqingRoom2Scene,
			FusangCorridorScene,
			FusangCopyScene,
			FinalScene
		}

		/// <summary>
		/// 场景栈
		/// </summary>
		Stack<Scene> sceneStack = new Stack<Scene>();

        /// <summary>
        /// 通道数据
        /// </summary>
        public JsonData tunnelData { get; private set; } = null;

        /// <summary>
        /// 异步场景切换操作对象
        /// </summary>
        public AsyncOperation asyncOper { get; private set; }
        public bool operReady { get; set; }

        /// <summary>
        /// 外部系统
        /// </summary>
        GameSystem gameSys;
        GameService gameSer;

        #region 初始化

        ///// <summary>
        ///// 初始化外部系统
        ///// </summary>
        //protected override void initializeSystems() {
        //    base.initializeSystems();
        //    gameSys = GameSystem.get();
        //    gameSer = GameService.get();
        //}

        #endregion

        #region 场景管理

        /// <summary>
        /// 当前场景名称
        /// </summary>
        /// <returns>场景名称</returns>
        public Scene currentScene() {
            return sceneStack.Count > 0 ? sceneStack.Peek() : Scene.NoneScene;
        }

        /// <summary>
        /// 真实当前场景名称
        /// </summary>
        /// <returns>场景名称</returns>
        public Scene realCurrentScene() {
            return (Scene)SceneManager.GetActiveScene().buildIndex;
        }

        /// <summary>
        /// 是否出现场景分歧
        /// </summary>
        /// <returns>是否场景分歧</returns>
        public bool differentScene() {
            return currentScene() != realCurrentScene();
        }

        /// <summary>
        /// 返回上一场景（如果上一场景为空则退出游戏）
        /// </summary>
        /// <returns>当前场景名称</returns>
        public void popScene(JsonData data, bool reload = false, bool async = false) {
            sceneStack.Pop(); loadScene(reload, data, async);
        }
        public void popScene(object data, bool reload = false, bool async = false) {
			var json = DataLoader.convert(data.GetType(), data);
			popScene(json, reload, async);
        }
        public void popScene(bool reload = false, bool async = false) {
            popScene((JsonData)null, reload, async);
        }

        /// <summary>
        /// 添加场景（往当前追加场景）
        /// </summary>
        /// <param name="scene">场景名称</param>
        public void pushScene(Scene scene, JsonData data, 
			bool reload = false, bool async = false) {
            sceneStack.Push(scene); loadScene(reload, data, async);
        }
        public void pushScene(Scene scene, object data,
			bool reload = false, bool async = false) {
			var json = DataLoader.convert(data.GetType(), data);
			pushScene(scene, json, reload, async);
        }
        public void pushScene(Scene scene, bool reload = false, bool async = false) {
            pushScene(scene, (JsonData)null, reload, async);
        }

        /// <summary>
        /// 切换场景（替换掉当前场景）
        /// </summary>
        /// <param name="scene">场景名称</param>
        public void changeScene(Scene scene, JsonData data, bool reload = false, bool async = false) {
            if (sceneStack.Count > 0) sceneStack.Pop();
            pushScene(scene, data, reload, async);
        }
        public void changeScene(Scene scene, object data, bool reload = false, bool async = false) {
			var json = DataLoader.convert(data.GetType(), data);
			changeScene(scene, json, reload, async);
        }
        public void changeScene(Scene scene, bool reload = false, bool async = false) {
            changeScene(scene, (JsonData)null, reload, async);
        }

        /// <summary>
        /// 转到场景（前面的场景将被清空）
        /// </summary>
        /// <param name="scene">场景名称</param>
        public void gotoScene(Scene scene, JsonData data, bool reload = false, bool async = false) {
            clearScene(); pushScene(scene, data, reload, async);
        }
        public void gotoScene(Scene scene, object data, bool reload = false, bool async = false) {
			var json = DataLoader.convert(data.GetType(), data);
			gotoScene(scene, json, reload, async);
        }
        public void gotoScene(Scene scene, bool reload = false, bool async = false) {
            gotoScene(scene, (JsonData)null, reload, async);
        }

        /// <summary>
        /// 清除场景
        /// </summary>
        public void clearScene() {
            sceneStack.Clear();
        }

        /// <summary>
        /// 读取/重新读取当前场景（如果当前场景为空则退出游戏）
        /// </summary>
        /// <param name="reload">是否重载</param>
        /// <param name="async">是否异步操作</param>
        public void loadScene(bool reload = false, JsonData data = null, bool async = false) {
            tunnelData = data;
            var scene = currentScene();
            Debug.Log("loadScene: " + scene + " (real: " + realCurrentScene() + 
                "), tunnel: "+ tunnelData?.ToJson());
            if (scene == Scene.NoneScene) gameSer.exitGame();
            // 如果需要重载（强制LoadScene）或者场景分歧
            else if (reload || differentScene()) {
                SceneUtils.clearSceneObjects();
                if (async) {
                    asyncOper = SceneManager.LoadSceneAsync((int)scene);
                    asyncOper.allowSceneActivation = false;
                } else
                    SceneManager.LoadScene((int)scene);
            }
        }

		/// <summary>
		/// 清除通道数据
		/// </summary>
		public void clearTunnelData() {
			tunnelData = null;
		}

        #endregion

        #region 异步管理

        /// <summary>
        /// 获取异步任务
        /// </summary>
        /// <returns>返回异步任务</returns>
        /// <param name="onProgress">进度回调函数</param>
        /// <param name="onCompleted">完成回调函数</param>
        public IEnumerator startAsync(UnityAction<float> onProgress, UnityAction onCompleted = null) {
            Debug.Log("startAsync");
            float progress;
            while (!asyncOper.isDone) {
                progress = getProgress();
                Debug.Log("onProgress: " + progress);
                if (operReady && progress >= 0.9) {
                    if (onCompleted == null)
                        onProgress.Invoke(1);
                    else
                        onCompleted.Invoke();
                    activeAsyncOper();
                } else
                    onProgress.Invoke(progress);
                yield return null;
            }
            clearAsyncOper();
        }

        /// <summary>
        /// 清除异步任务
        /// </summary>
        public void clearAsyncOper() {
            Debug.Log("clearAsyncOper");
            asyncOper = null;
            operReady = false;
        }

        /// <summary>
        /// 获取进度
        /// </summary>
        /// <returns></returns>
        public float getProgress() {
            var progress = asyncOper.progress;
            return progress < 0.9f ? progress : 1;
        }

        /// <summary>
        /// 激活异步任务
        /// </summary>
        public void activeAsyncOper() {
            asyncOper.allowSceneActivation = true;
        }

        #endregion

    }

}