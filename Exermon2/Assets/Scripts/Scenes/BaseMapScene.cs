
using System;
using System.Collections.Generic;

using LitJson;

using UnityEngine;

using Core.UI;
using Core.UI.Utils;

using GameModule.Services;
using MapModule.Services;

using MapModule.Data;

using UI.Common.Controls.AnimationSystem;
using UI.BattleSystem.Controls;
namespace UI.MapSystem {

	using Controls;
	using Windows;

	/// <summary>
	/// 地图场景基类
	/// </summary>
	[RequireComponent(typeof(Animator))]
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
        /// 外部组件设置
        /// </summary>
        public Map map1, map2;
        public MapPlayer player;

        public DialogWindow dialogWindow;

		public Canvas splitCanvas;

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[HideInInspector]
		public Map curMap;
		[RequireTarget]
		protected TimeTravelEffect timeTravelEffect;
		[RequireTarget]
		protected Animator animator;

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public RenderTexture renderTexture;
		public Material switchSceneMaterial;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		bool present = true;
        bool switching = false;
        ThroughType splitType;

		/// <summary>
		/// 属性
		/// </summary>
		float switchStrength => timeTravelEffect.switchStrength;

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
            curMap = map1;
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
            if (switching) {
                //debugLog("switch strength:" + switchStrength);
                switchSceneMaterial.SetFloat("_Strength", switchStrength);
            }
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

		#endregion

		#region 时空穿越控制

        public void travel() {
            if (present)
                TravelThrough(ThroughType.PastSingle);
            else
                TravelThrough(ThroughType.PresentSingle);
        }

        /// <summary>
        /// 时空穿越
        /// </summary>
        public void TravelThrough(ThroughType type) {
            if (switching)
                return;
            splitType = type;
            if (type == ThroughType.PresentSingle) {
                if (curMap == map1)
                    return;
                switchToPresent();
            }
            else if (type == ThroughType.PastSingle) {
                if (curMap == map2)
                    return;
                switchToPast();
            }
            switchScene();
            animator.SetTrigger(type.ToString());
        }

        /// <summary>
        /// 地图场景切换
        /// </summary>
        void switchScene() {
            //以镜子为中心进行扭曲
            var center = getPortalScreenPostion(player.transform.position);
            switchSceneMaterial.SetVector("_CenterPos", center);
            switching = true;
            //TODO状态切换
            if (present) {
                player.transform.position -= map2.transform.position - map1.transform.position;
            }
            else {
                player.transform.position += map2.transform.position - map1.transform.position;
            }
        }

        /// <summary>
        /// 将当前扭曲位置转化为屏幕坐标百分比
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        Vector2 getPortalScreenPostion(Vector3 position) {
            var screenPos = map1.camera.WorldToScreenPoint(position);
            return new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        }

        /// <summary>
        /// 切换至“现在”
        /// </summary>
        void switchToPresent() {
            present = true;
            map2.camera.targetTexture = renderTexture;
        }
        /// <summary>
        /// 切换至“过去”
        /// </summary>
        void switchToPast() {
            present = false;
            map1.camera.targetTexture = renderTexture;
        }

        /// <summary>
        /// 重设相机状态，取消renderTexture模式
        /// </summary>
        public void resetCamera() {
            //debugLog("reset switch:");
            if (present) {
                map1.camera.targetTexture = null;
                curMap = map1;
            }
            else {
                map2.camera.targetTexture = null;
                curMap = map2;
            }

            switching = false;
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