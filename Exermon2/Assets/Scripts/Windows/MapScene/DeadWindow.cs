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
    public class DeadWindow : BaseWindow {

        /// <summary>
        /// 外部变量
        /// </summary>
        public float showTime = 2.0f;
        bool hiding = false;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        GameService gameSer;

        /// <summary>
        /// 场景组件
        /// </summary>
        new BaseMapScene scene => base.scene as BaseMapScene;

        /// <summary>
        /// 内部变量
        /// </summary>
        float time = 0;

        #region 流程控制
        /// <summary>
        /// 开始
        /// </summary>
        public override void activate() {
            base.activate();
            time = 0;
            hiding = false;
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public override void deactivate() {
            //time = 0;
            scene.restartStage(true);
            base.deactivate();
        }
        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateTime();
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        void updateTime() {
            if (!hiding && (time += Time.deltaTime) > showTime) {
                debugLog("dead end:"); hiding = true; deactivate(); }
        }

        #endregion

    }
}
