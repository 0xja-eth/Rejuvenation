
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
        public RenderTexture renderTexture;
        public Canvas splitCanvas;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        public MessageServices msgServices;
        [RequireTarget]
        protected Animator animator;

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Map map1, map2;
        public DialogWindow dialogWindow;
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
            else if (Input.GetKeyDown(KeyCode.Y) && !msgServices.isDialogued) {
                dialogWindow.activate();
            }
        }
        #endregion
    }
}