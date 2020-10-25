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
        /// 外部控件
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
            next();
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public override void deactivate() {
            base.deactivate();
            var titleScene = scene as TitleScene;
            if (titleScene)
                titleScene.startGame();

            var finaleScene = scene as FinalScene;
            if (finaleScene)
				sceneSys.pushScene(SceneSystem.Scene.TitleScene);
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
        /// 刷新
        /// </summary>
        protected void next() {
            debugLog("next refresh:" + illustrationMessages.Count);
            if (illustrationMessages.Count != 0) {
                var msg = illustrationMessages[0];
                display.setItem(msg);
                illustrationMessages.RemoveAt(0);
                requestRefresh();
            }
            else // 结束
                deactivate();
        }

        #endregion

        #region 消息事件

        /// <summary>
        /// 下一条消息或者快速展开文字
        /// </summary>
        void nextOrRevealAll() {
            debugLog("next:" + display.printing);
            if (display.printing) display.stopPrint();
            else next();
        }

        #endregion

    }
}
