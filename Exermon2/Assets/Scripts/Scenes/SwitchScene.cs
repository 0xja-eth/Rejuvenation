
using System;
using System.Collections.Generic;

using Core.Systems;
using Core.UI;
using UI.Common.Controls;

namespace UI.SwitchScene {

    /// <summary>
    /// 切换场景
    /// </summary>
    public class SwitchScene : BaseScene {

        /// <summary>
        /// 外部控件
        /// </summary>
        public SwitchDisplay switchDisplay;

        #region 初始化
        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
            var testData = new GameModule.Data.GameSwitchData();
            testData.name = "关卡1";
            testData.description = "Loading...";
            switchDisplay.activate(testData);
        }

        /// <summary>
        /// 场景索引
        /// </summary>
        /// <returns></returns>
        public override SceneSystem.Scene sceneIndex() {
			return SceneSystem.Scene.NoneScene; //.SwitchScene;
        }

        #endregion
    }
}