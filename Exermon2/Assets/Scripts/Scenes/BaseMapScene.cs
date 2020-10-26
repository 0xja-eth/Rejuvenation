
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
    using UI.Controls;
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
		const string TravelAttrName = "travel";
		const string SceneExitAttrName = "exit";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public new Camera camera;

		public Map map1, map2;

        public DialogWindow dialogWindow;
        public DialogWindow logWindow;
        public DeadWindow deadWindow = null;

		public Canvas splitCanvas;

        public MainUIDisplay mainUIDisplay;

        /// <summary>
        /// 预制件设置
        /// </summary>
        public GameObject playerPerfab;

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
		public bool autoSave = true; // 进入该场景是否自动保存

		public Vector2 startPos; // 玩家初始位置

		public RenderTexture renderTexture;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		bool loading = true;
		bool traveling = false;

		/// <summary>
		/// 玩家属性
		/// </summary>
		public MapPlayer player => map1.player ?? map2.player;

		/// <summary>
		/// 地图/时空属性
		/// </summary>
		public TimeType timeType {
			get => playerSer.runtimeActor.timeType;
			set { playerSer.runtimeActor.timeType = value; }
		}

		public bool isPresent => timeType == TimeType.Present;
		public bool isPast => timeType == TimeType.Past;

		public Map presentMap => getMap(TimeType.Present);
		public Map pastMap => getMap(TimeType.Past);

		public Map currentMap => getMap(timeType);

		/// <summary>
		/// 外部系统设置
		/// </summary>
		protected MessageService messageSer;

		#region  初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();

            if (!hasPlayer()) return;
			refreshMapActive();
		}

		/// <summary>
		/// 开始
		/// </summary>
		protected override void start() {
			base.start();
			if (!hasPlayer()) return;
			setupPlayer(); setupUI();
		}

		/// <summary>
		/// 玩家启动回调
		/// </summary>
		public virtual void onPlayerStart() { }

        /// <summary>
        /// 设置主界面
        /// </summary>
        void setupUI() {
            mainUIDisplay?.activate();
            mainUIDisplay?.setItem(new UIShowAttributes());
        }

		/// <summary>
		/// 处理通道数据
		/// </summary>
		protected override void processTunnelData(JsonData data) {
			var x = DataLoader.load<float>(data, "x");
			var y = DataLoader.load<float>(data, "y");

			startPos = new Vector2(x, y);
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
        }

		/// <summary>
		/// 更新对话框
		/// </summary>
		void updateDialog() {
            if (messageSer.messageCount() > 0 && !isBusy()) {
                if (messageSer.dialogFlag)
                    dialogWindow.activate();
                else
                    logWindow.activate();
            }
		}

		#endregion

		#region 玩家相关

		/// <summary>
		/// 是否存在玩家
		/// </summary>
		/// <returns></returns>
		public bool hasPlayer() {
			return playerSer.player != null;
		}

		/// <summary>
		/// 初始化玩家
		/// </summary>
		void setupPlayer() {
			setupPlayerData();
			setupMapPlayer();
		}

		/// <summary>
		/// 配置玩家数据
		/// </summary>
		void setupPlayerData() {
			playerSer.player.stage = sceneIndex();
			playerSer.runtimeActor.transfer(
				startPos.x, startPos.y, true);
			if (autoSave) playerSer.savePlayer();
		}

		/// <summary>
		/// 配置地图玩家
		/// </summary>
		void setupMapPlayer() {
			var map = currentMap.transform;
			Instantiate(playerPerfab, map);
		}

		#endregion

		#region Loading相关

		/// <summary>
		/// Loading结束回调
		/// </summary>
		public void onLoadingEnd() {
			loading = false;
		}
		
		/// <summary>
		/// Loading开始回调
		/// </summary>
		public void onLoadingStart() {
			sceneSys.operReady = true;
		}

		#endregion

		#region 状态判断

		/// <summary>
		/// 繁忙
		/// </summary>
		/// <returns></returns>
		public bool isBusy() {
			return loading || traveling || isDialogued();
		}

		/// <summary>
		/// 处于对话框状态
		/// </summary>
		/// <returns></returns>
		public virtual bool isDialogued() {
			return dialogWindow.shown || logWindow.shown;
		}

		#endregion

		#region 地图控制

		/// <summary>
		/// 获取地图
		/// </summary>
		/// <param name="mapType"></param>
		/// <returns></returns>
		public Map getMap(TimeType mapType) {
			if (map1 && map1.type == mapType) return map1;
			if (map2 && map2.type == mapType) return map2;
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
			if (traveling || (!force && timeType == type)) return;

			traveling = true;

			currentMap.travel(getMap(timeType = type));
			playEffect(type);
		}

		/// <summary>
		/// 执行切换效果
		/// </summary>
		/// <param name="type"></param>
		void playEffect(TimeType type) {
			animator.setVar(TravelAttrName);

			// 以镜子为中心进行扭曲
			var center = getPortalScreenPostion(player.transform.position);
			timeTravelEffect.center = center;
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
		/// 刷新地图显示情况
		/// </summary>
		public void refreshMapActive() {
			var map = presentMap;
			if (map) presentMap.active = isPresent;

			map = pastMap;
			if (map) pastMap.active = isPast;
		}

		/// <summary>
		/// 开始时空穿越，应用renderTexture模式
		/// </summary>
		public void onTravelStart() {
			camera.targetTexture = renderTexture;
		}

		/// <summary>
		/// 时空穿越结束，取消renderTexture模式
		/// </summary>
		public void onTravelEnd() {
			camera.targetTexture = null;
			traveling = false;
		}

		#endregion

		#region 场景控制

		/// <summary>
		/// 默认的下一关场景
		/// </summary>
		/// <returns></returns>
		public abstract SceneSystem.Scene nextStage();

		/// <summary>
		/// 下一关
		/// </summary>
		public void changeNextStage() {
			changeStage(nextStage());
		}

		/// <summary>
		/// 重开本关
		/// </summary>
		public void restartStage(bool died) {
			var stage = playerSer.resumeGame();
			changeStage(stage, true);
		}
		public void restartStage() {
			restartStage(false);
		}

		/// <summary>
		/// 切换关卡
		/// </summary>
		public void changeStage(SceneSystem.Scene stage, 
			Vector2? pos, bool reload = false) {

			var flag = false;
			var same = (stage == SceneSystem.Scene.NoneScene || stage == sceneIndex());

			if (flag = (!same || reload))
				changeDifferentStage(stage, pos, reload);
			else if (flag = pos != null)// 同一个场景
				player.transfer(pos.Value, true);

			if (flag) animator.setVar(SceneExitAttrName);
		}
		public void changeStage(SceneSystem.Scene stage, bool reload) {
			changeStage(stage, null, reload);
		}
		public void changeStage(SceneSystem.Scene stage) {
			changeStage(stage, false);
		}

		/// <summary>
		/// 不同关卡的切换
		/// </summary>
		/// <param name="stage"></param>
		/// <param name="pos"></param>
		void changeDifferentStage(SceneSystem.Scene stage, 
			Vector2? pos, bool reload) {
			loading = true;

			var data = makeTunnelData(pos);
			sceneSys.changeScene(stage, data, reload, true);

			doRoutine(sceneSys.startAsync(
				onLoadingProgress, onLoadingCompleted));
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
