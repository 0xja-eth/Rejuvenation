
using System;
using System.Collections.Generic;

using LitJson;

using UnityEngine;

using Core.Systems;

using Core.UI;
using Core.UI.Utils;

using UI.Common.Windows;
using UI.Common.Controls.AnimationSystem;
using UI.Common.Controls.MapSystem;

namespace UI.BaseMapScene {

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
        public RenderTexture renderTexture;
        public Canvas splitCanvas;

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		protected new AnimationExtend animation;
        [RequireTarget]
        protected Animator animator;

        bool present = false;
        bool switching = false;

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
            //map1.camera.targetTexture
            animator.SetTrigger(type.ToString());
		}

        void switchToPresent() {
            present = true;
            switching = true;
            map1.camera.rect = new Rect(0, 0, 1, 1);
            map2.camera.rect = new Rect(0, 0, 1, 1);
            map1.camera.targetTexture = renderTexture;
        }

        void switchToPast() {
            present = false;
            switching = true;
            map1.camera.rect = new Rect(0, 0, 1, 1);
            map2.camera.rect = new Rect(0, 0, 1, 1);
            map2.camera.targetTexture = renderTexture;
        }

        public void resetCamera() {
            if (present) {
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
        protected override void update() {
            base.update();
            if (Input.GetKeyDown(KeyCode.Alpha1))
                splitCamera(SplitType.PresentSingle);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                splitCamera(SplitType.PastSingle);
            else if (Input.GetKeyDown(KeyCode.B))
                splitCamera(SplitType.Both);
            else if (Input.GetKeyDown(KeyCode.L))
                splitCamera(SplitType.PastMain);
            else if (Input.GetKeyDown(KeyCode.R))
                splitCamera(SplitType.PresentMain);
        }
        #endregion
    }
}