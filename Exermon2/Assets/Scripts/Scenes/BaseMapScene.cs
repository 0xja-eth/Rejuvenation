
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

namespace UI.MapSystem {

	using Controls;
	using Windows;

	/// <summary>
	/// 地图场景基类
	/// </summary>
	[RequireComponent(typeof(AnimationExtend), typeof(Animator))]
    public abstract class BaseMapScene : BaseScene {

        /// <summary>
        /// 分屏类型
        /// </summary>
        public enum SplitType {
            PresentSingle, // 现在单屏
            PastSingle, //过去单屏
            Both, // 双屏（左屏为过去，右屏为现在）
            PastMain, // 左屏为主
            PresentMain, // 右屏为主
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Map map1, map2;
		public DialogWindow dialogWindow;
		public RenderTexture renderTexture;
		public Canvas splitCanvas;

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		protected Animator animator;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		bool present = false;
        bool switching = false;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		protected MessageService messageSer;

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
			if (messageSer.messageCount() > 0 && !isBusy())
				dialogWindow.activate();
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

		#region 分屏控制

		/// <summary>
		/// 分屏
		/// </summary>
		public void splitCamera(SplitType type) {
            if (switching)
                return;
            if (type == SplitType.PresentSingle)
                switchToPresent();
            else if (type == SplitType.PastSingle)
                switchToPast();
            animator.SetTrigger(type.ToString());
        }

        /// <summary>
        /// 切换至“现在”
        /// </summary>
        void switchToPresent() {
            present = true;
            switching = true;
            map1.camera.rect = new Rect(0, 0, 1, 1);
            map2.camera.rect = new Rect(0, 0, 1, 1);
            map1.camera.targetTexture = renderTexture;
        }
        /// <summary>
        /// 切换至“过去”
        /// </summary>
        void switchToPast() {
            present = false;
            switching = true;
            map1.camera.rect = new Rect(0, 0, 1, 1);
            map2.camera.rect = new Rect(0, 0, 1, 1);
            map2.camera.targetTexture = renderTexture;
        }

        /// <summary>
        /// 重设相机状态，取消renderTexture模式
        /// </summary>
        public void resetCamera() {
            if (present) {
                //此处设置视口有bug，动画update时会自动恢复，只能在动画中设置视口属性
                map2.camera.rect = new Rect(0, 0, 0, 1);
                map1.camera.rect = new Rect(0, 0, 1, 1);
                map1.camera.targetTexture = null;
            }
            else {
                map1.camera.rect = new Rect(0, 0, 0, 1);
                map2.camera.rect = new Rect(0, 0, 1, 1);
                map2.camera.targetTexture = null;
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
                splitCamera(SplitType.PresentSingle);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                splitCamera(SplitType.PastSingle);
            else if (Input.GetKeyDown(KeyCode.B))
                splitCamera(SplitType.Both);
            //else if (Input.GetKeyDown(KeyCode.L))
            //    splitCamera(SplitType.PastMain);
            else if (Input.GetKeyDown(KeyCode.R))
                splitCamera(SplitType.PresentMain);
            else if (Input.GetKeyDown(KeyCode.Y)) {
				Debug.Log("K " + messageSer.messageCount());
            }
        }

        #endregion
    }
}