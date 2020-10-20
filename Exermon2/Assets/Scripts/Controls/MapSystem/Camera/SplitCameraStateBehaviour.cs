
using UnityEngine;

using Core.UI;
using Core.UI.Utils;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 基本状态行为
    /// </summary>
    public class SplitCameraStateBehaviour : BaseStateBehaviour {

        /// <summary>
        /// 内部变量
        /// </summary>
        BaseMapScene mapScene;

        #region 初始化
  
		/// <summary>
        /// 初始化
        /// </summary>
        /// <param name="go"></param>
        protected override void setup(GameObject go) {
            base.setup(go);
            mapScene = SceneUtils.get<BaseMapScene>(go);
        }

        #endregion

        /// <summary>
        /// 状态进入
        /// </summary>
        protected override void onStateEnter() {
            base.onStateEnter();
            mapScene.switchSceneMaterial.SetVector("_CenterPos", mapScene.timeTravelEffect.center);
        }

        /// <summary>
        /// 状态更新
        /// </summary>
        protected override void onStateUpdate() {
            base.onStateUpdate();
            if(!finished)
                mapScene.switchSceneMaterial.SetFloat("_Strength", mapScene.timeTravelEffect.switchStrength);
        }

        /// <summary>
        /// 状态离开
        /// </summary>
        protected override void onStateExit() {
            base.onStateExit();
        }

		/// <summary>
		/// 状态结束回调
		/// </summary>
		protected override void onStateFinished() {
			base.onStateFinished();
			mapScene.resetCamera();
		}

	}

}
