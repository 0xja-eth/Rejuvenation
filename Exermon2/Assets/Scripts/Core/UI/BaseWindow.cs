
using UnityEngine;

using UI.Common.Controls.AnimationSystem;

namespace Core.UI {

	using Utils;

    /// <summary>
    /// 窗口基类
    /// </summary>
    /// <remarks>
    /// 游戏所有窗口的基类，实际上几乎是一个带有打开动画和关闭动画的视图
    /// 一般用于控制管理一个窗口下的组件，也用于进行不同窗口间的交互
    /// </remarks>
	//[RequireComponent(typeof(AnimatorExtend))]
    public class BaseWindow : CanvasComponent {

        /// <summary>
        /// 外部组件设置
        /// </summary>
		public AnimatorExtend animator;
        public GameObject background; // 窗口背景

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public string showingState = "Show"; // 显示动画状态名称
        public string hidingState = "Hide"; // 隐藏动画状态名称
		public string hiddenState = "Hidden"; // 隐藏状态名称

		public string shownAttr = "shown"; // 显示状态属性

		/// <summary>
		/// 内部组件设置
		/// </summary>
		protected BaseScene scene => SceneUtils.getCurrentScene();

		/// <summary>
		/// 显示状态
		/// </summary>
		//public override bool shown {
  //          get {
		//		if (animator == null) return base.shown;
		//		return base.shown; && !animator.isState(hiddenState);
  //          }
  //      }

        /// <summary>
        /// 动画过渡
        /// </summary>
        bool isShowing = false, isHiding = false;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
			animator = animator ?? SceneUtils.get<AnimatorExtend>(transform);

			initializeStates();
		}

        /// <summary>
        /// 初始化状态
        /// </summary>
        protected virtual void initializeStates() {
            animator.addEndEvent(showingState, onWindowShown);
			animator.addEndEvent(hidingState, onWindowHidden);
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update(); updateBackground();
        }

        /// <summary>
        /// 更新窗口背景
        /// </summary>
        void updateBackground() {
            if (background != null) background.SetActive(isBackgroundVisible());
        }
        
        #endregion

        #region 启动/结束控制

        /// <summary>
        /// 显示窗口（视窗）
        /// </summary>
        public override void show() {
			base.show();

			isHiding = false; isShowing = true;
			if (!animator) onWindowShown();
            else animator.setVar(shownAttr, true);
        }

        /// <summary>
        /// 隐藏窗口（视窗）
        /// </summary>
        public override void hide() {
			isHiding = true; isShowing = false;
			if (!animator) onWindowHidden();
            else animator.setVar(shownAttr, false);
		}

		/// <summary>
		/// 窗口完全显示回调
		/// </summary>
		protected virtual void onWindowShown() {
			debugLog("onWindowShown: " + name + ": " + isShowing);
            if (isShowing) isShowing = false;
        }

        /// <summary>
        /// 窗口完全隐藏回调
        /// </summary>
        protected virtual void onWindowHidden() {
            debugLog("onWindowHidden: " + name + ": " + isHiding);
            if (isHiding) {
                base.hide();
                isHiding = false;
                updateBackground();
            }
        }

        /// <summary>
        /// 打开/关闭窗口
        /// </summary>
        public void toggleWindow() {
            if (shown) activate(); else deactivate();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 判断是否处于可视状态
        /// </summary>
        /// <returns>是否可视状态</returns>
        protected virtual bool isBackgroundVisible() {
            return shown;
        }

		/// <summary>
		/// 忙碌中（Showing/Hiding)
		/// </summary>
		/// <returns></returns>
		public bool isBusy() {
			return isShowing || isHiding;
		}

		/// <summary>
		/// 获取场景
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T getScene<T>() where T: BaseScene {
			return scene as T;
		}

        #endregion

    }
}