using Core.UI;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using GameModule.Services;

using MapModule.Services;
using MapModule.Data;

namespace UI.MapSystem.Windows {
    using Assets.Scripts.Scenes;
    using Controls;
    using Core.Systems;

    /// <summary>
    /// 对话窗口层
    /// </summary>
    [RequireComponent(typeof(MessageBaseDisplay))]
    public class IllustrationWindow : BaseWindow {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public GameObject foreground; // 前景，用于最后窗口隐藏后覆盖在最上面的背景

		/// <summary>
		/// 外部变量设置
		/// </summary>
        public List<DialogMessage> illustrationMessages;

        /// <summary>
        /// 内部组件设置
        /// </summary>
        [RequireTarget]
        MessageBaseDisplay display;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        GameService gameSer;
		SceneSystem sceneSys;

        #region 流程控制

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
            refreshMessage();
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public override void deactivate() {
            base.deactivate();
        }

		/// <summary>
		/// 窗口完全开启回调
		/// </summary>
		protected override void onWindowShown() {
			base.onWindowShown();
			if (foreground) foreground.SetActive(true);
		}

		/// <summary>
		/// 窗口完全隐藏回调
		/// </summary>
		protected override void onWindowHidden() {
			base.onWindowHidden();

			if (hasMessages()) refreshMessage();
			else processSceneChange();
		}

		/// <summary>
		/// 处理场景切换
		/// </summary>
		void processSceneChange() {
			// 开始游戏
			var titleScene = scene as TitleScene;
			if (titleScene) {
				titleScene.startGame(); return;
			}

			// 游戏结束
			var finaleScene = scene as FinalScene;
			if (finaleScene)
				sceneSys.gotoScene(SceneSystem.Scene.TitleScene);
		}

		#endregion

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
            base.update();
            updateInput();
        }

        /// <summary>
        /// 更新输入
        /// </summary>
        void updateInput() {
            if (Input.GetKeyDown(gameSer.keyboard.nextKey))
                nextOrRevealAll();
        }

        #endregion

        #region 内容绘制

		/// <summary>
		/// 是否还有消息
		/// </summary>
		/// <returns></returns>
		public bool hasMessages() {
			return illustrationMessages.Count > 0;
		}

		/// <summary>
		/// 显示新消息
		/// </summary>
		public void refreshMessage() {
			var msg = illustrationMessages[0];
			illustrationMessages.RemoveAt(0);
			display.setItem(msg);
			activate();
		}

		#endregion

		#region 消息事件

		/// <summary>
		/// 下一条消息或者快速展开文字
		/// </summary>
		void nextOrRevealAll() {
            if (display.printing) display.stopPrint();
            else deactivate();
        }

        #endregion

    }
}
