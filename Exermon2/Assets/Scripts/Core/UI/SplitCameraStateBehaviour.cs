using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UI.BaseMapScene;

namespace Core.UI {
    /// <summary>
    /// 基本状态行为
    /// </summary>
    public class SplitCameraStateBehaviour : BaseStateBehaviour {
        /// <summary>
        /// 内部变量
        /// </summary>
        BaseMapScene mapScene;
        bool finish = false;

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="go"></param>
        protected override void setup(GameObject go) {
            base.setup(go);
            mapScene = gameObject.GetComponent<BaseMapScene>();
        }
        #endregion

        /// <summary>
        /// 状态进入
        /// </summary>
        protected override void onStatusEnter() {
            base.onStatusEnter();

        }

        /// <summary>
        /// 状态更新
        /// </summary>
        protected override void onStatusUpdate() {
            base.onStatusUpdate();
            if(!finish && stateInfo.normalizedTime > stateInfo.length + 0.5) {
                finish = true;
                mapScene.resetCamera();
            }
        }

        /// <summary>
        /// 状态离开
        /// </summary>
        protected override void onStatusExit() {
            base.onStatusExit();
            finish = false;
        }
    }

}
