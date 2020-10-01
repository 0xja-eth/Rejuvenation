
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
using UnityEngine.UI;
using GameModule.Services;
using static GameModule.Services.MessageServices;

namespace UI.BaseMapScene {

    /// <summary>
    /// 地图场景基类
    /// </summary>
    [RequireComponent(typeof(AnimationExtend))]
    public abstract class BaseMapScene : BaseScene {

        /// <summary>
        /// 分屏类型
        /// </summary>
        public enum SplitType {
            Single, // 单屏
            Both, // 双屏（左屏为过去，右屏为现在）
            LeftMain, // 左屏为主
            RightMain // 右屏为主
        }


        /// <summary>
        /// 外部系统设置
        /// </summary>
        public MessageServices msgServices;

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Map map1, map2;
        public DialogWindow dialogWindow;


        /// <summary>
        /// 内部组件设置
        /// </summary>
        [RequireTarget]
        protected new AnimationExtend animation;

        #region 初始化
        protected override void initializeOthers() {
            base.initializeOthers();
        }
        #endregion

        #region 更新控制

        protected override void update() {
            base.update();
            if (Input.GetKeyDown(KeyCode.Y) && !msgServices.isDialogued) {
                dialogWindow.activate();
            }
        }
        #endregion

        #region 分屏控制

        /// <summary>
        /// 分屏
        /// </summary>
        public void splitCamera(SplitType type) {
            // TODO: 添加分屏动画代码
        }

        #endregion


    }
}