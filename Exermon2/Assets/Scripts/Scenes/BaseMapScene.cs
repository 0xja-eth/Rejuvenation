﻿
using System;
using System.Collections.Generic;

using LitJson;

using UnityEngine;

using Core.UI;
using Core.UI.Utils;

using Core.Data.Loaders;

using Core.Systems;

using GameModule.Services;
using MapModule.Services;
using PlayerModule.Services;

using MapModule.Data;

using UI.Common.Controls.AnimationSystem;
using UI.BattleSystem.Controls;
namespace UI.MapSystem {

	using Controls;
    using System.Collections;
    using Windows;

	/// <summary>
	/// 地图场景基类
	/// </summary>
	[RequireComponent(typeof(AnimatorExtend))]
	[RequireComponent(typeof(AnimationExtend))]
	[RequireComponent(typeof(TimeTravelEffect))]
	public abstract class BaseMapScene : BaseScene {

        /// <summary>
        /// 时空穿越类型
        /// </summary>
        public enum ThroughType {
            PresentSingle, // 现在单屏
            PastSingle, //过去单屏
            //Both, // 双屏（左屏为过去，右屏为现在）
            //PastMain, // 左屏为主
            //PresentMain, // 右屏为主
        }

		/// <summary>
		/// 常量定义
		/// </summary>
		public const string StrengthAttrName = "_Strength";
		public const string CenterPosAttrName = "_CenterPos";

		const string SceneLoadingAttrName = "SceneLoading";
		const string SceneEnterAttrName = "SceneEnter";
		const string SceneExitAttrName = "SceneExit";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public new Camera camera;

		public Map map1, map2;
        public MapPlayer player;

        public DialogWindow dialogWindow;

		public Canvas splitCanvas;
		
		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		[HideInInspector]
		public TimeTravelEffect timeTravelEffect;
		[RequireTarget]
		protected AnimatorExtend animator;

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public RenderTexture renderTexture;
		public Material switchSceneMaterial;

		/// <summary>
		/// 内部变量定义
		/// </summary>
        bool switching = false;

		/// <summary>
		/// 特效属性
		/// </summary>
		float switchStrength => timeTravelEffect.switchStrength;

		/// <summary>
		/// 地图/时空属性
		/// </summary>
		public TimeType timeType {
			get => playerSer.actor.runtimeActor.timeType;
			set { playerSer.actor.runtimeActor.timeType = value; }
		}

		public bool isPresent => timeType == TimeType.Present;
		public bool isPast => timeType == TimeType.Past;

		public Map presentMap => getMap(TimeType.Present);
		public Map pastMap => getMap(TimeType.Past);

		public Map currentMap => getMap(timeType);

		/// <summary>
		/// 外部系统设置
		/// </summary>
		protected PlayerService playerSer;
		protected MessageService messageSer;

		#region  初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			playerSer.player.stage = sceneIndex();
		}

		/// <summary>
		/// 开始
		/// </summary>
		protected override void start() {
			travel(timeType, true); base.start();
			doRoutine(loading());
        }

		/// <summary>
		/// 处理通道数据
		/// </summary>
		protected override void processTunnelData(JsonData data) {
			var x = DataLoader.load<float>(data, "x");
			var y = DataLoader.load<float>(data, "y");

			player.transfer(x, y, true);
		}

		/// <summary>
		/// 加载动画
		/// </summary>
		/// <returns></returns>
		IEnumerator loading() {
            animator.setVar(SceneLoadingAttrName);
            yield return new WaitForSeconds(1.2f);
            animator.setVar(SceneEnterAttrName);
        }
        
		#endregion

        #region 更新

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
			base.update();
			updateDialog();

			updateForTest();
            updateSwitchStrength();
        }

		/// <summary>
		/// 更新对话框
		/// </summary>
		void updateDialog() {
			if (messageSer.messageCount() > 0 && !isBusy())
				dialogWindow.activate();
		}

        /// <summary>
        /// 更新镜头扭曲强度
        /// </summary>
        void updateSwitchStrength() {
            if (switching)
				switchSceneMaterial.SetFloat(
					StrengthAttrName, switchStrength);
        }

		#endregion

		#region 状态判断

		/// <summary>
		/// 繁忙
		/// </summary>
		/// <returns></returns>
		public bool isBusy() {
			return isDialogued();
		}

		/// <summary>
		/// 处于对话框状态
		/// </summary>
		/// <returns></returns>
		public bool isDialogued() {
			return dialogWindow.shown;
		}

		#endregion

		#region 地图控制

		/// <summary>
		/// 获取地图
		/// </summary>
		/// <param name="mapType"></param>
		/// <returns></returns>
		public Map getMap(TimeType mapType) {
			if (map1.type == mapType) return map1;
			if (map2.type == mapType) return map2;
			return null;
		}

		#endregion

		#region 时空穿越控制

		/// <summary>
		/// 穿越时空
		/// </summary>
        public void travel() {
            travel(isPresent ? TimeType.Past : TimeType.Present);
        }

        /// <summary>
        /// 时空穿越
        /// </summary>
        public void travel(TimeType type, bool force = false) {
			if (switching || (!force && timeType == type)) return;

			timeType = type;
			camera.targetTexture = renderTexture;

			playEffect(type);
		}

		/// <summary>
		/// 执行切换效果
		/// </summary>
		/// <param name="type"></param>
		void playEffect(TimeType type) {
			switching = true;
			animator.setVar(type.ToString());

			// 以镜子为中心进行扭曲
			var center = getPortalScreenPostion(player.transform.position);
			switchSceneMaterial.SetVector(CenterPosAttrName, center);

			// 坐标切换已在MapEntity中实现
            player.clearSeperation();
		}

		/// <summary>
		/// 将当前扭曲位置转化为屏幕坐标百分比
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		Vector2 getPortalScreenPostion(Vector3 position) {
            var screenPos = camera.WorldToScreenPoint(position);
            return new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        }
		
        /// <summary>
        /// 重设相机状态，取消renderTexture模式
        /// </summary>
        public void resetCamera() {
			camera.targetTexture = null;
            switching = false;
        }

		#endregion

		#region 场景控制

		/// <summary>
		/// 默认的下一关场景
		/// </summary>
		/// <returns></returns>
		public abstract SceneSystem.Scene nextScene();

		/// <summary>
		/// 下一关
		/// </summary>
		public void nextStage() {
			changeStage(nextScene());
		}

		/// <summary>
		/// 切换关卡
		/// </summary>
		public void changeStage(SceneSystem.Scene stage, Vector2? pos) {
			animator.setVar(SceneExitAttrName);

			if (stage != SceneSystem.Scene.NoneScene && 
				stage != sceneIndex()) {
				var data = makeTunnelData(pos);
				sceneSys.changeScene(stage, data, true);

				doRoutine(sceneSys.startAsync(
					onLoadingProgress, onLoadingCompleted));
				sceneSys.operReady = true;
			} else
				player.transfer(pos.Value, true);
		}
		public void changeStage(SceneSystem.Scene stage) {
			changeStage(stage, null);
		}

		/// <summary>
		/// 组装通讯数据
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		JsonData makeTunnelData(Vector2? pos) {
			if (pos == null) return null;
			var res = new JsonData();
			res["x"] = pos?.x; res["y"] = pos?.y;
			return res;
		}

		/// <summary>
		/// 读取进度改变
		/// </summary>
		/// <param name="progress"></param>
		protected virtual void onLoadingProgress(float progress) {
			animator.setVar(SceneLoadingAttrName);
		}

		/// <summary>
		/// 读取完成
		/// </summary>
		/// <param name="progress"></param>
		protected virtual void onLoadingCompleted() {
		}

		#endregion

		#region 测试

		/// <summary>
		/// 测试
		/// </summary>
		void updateForTest() {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                //TravelThrough(ThroughType.PresentSingle);
                travel();
            //else if (Input.GetKeyDown(KeyCode.Alpha2))
            //    //TravelThrough(ThroughType.PastSingle);
            //    travelToPast();
            //else if (Input.GetKeyDown(KeyCode.B))
            //    splitCamera(SplitType.Both);
            //else if (Input.GetKeyDown(KeyCode.L))
            //    splitCamera(SplitType.PastMain);
            //else if (Input.GetKeyDown(KeyCode.R))
            //    splitCamera(SplitType.PresentMain);
            else if (Input.GetKeyDown(KeyCode.Y)) {
                Debug.Log("K " + messageSer.messageCount());
            }
        }

        #endregion
    }
}
