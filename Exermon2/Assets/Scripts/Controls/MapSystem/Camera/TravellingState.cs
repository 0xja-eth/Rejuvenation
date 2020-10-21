
using UnityEngine;

using Core.UI;
using Core.UI.Utils;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 基本状态行为
    /// </summary>
    public class TravellingState : BaseStateBehaviour {

		/// <summary>
		/// 常量定义
		/// </summary>
		public const string StrengthAttrName = "_Strength";
		public const string CenterPosAttrName = "_CenterPos";

		/// <summary>
		/// 内部变量
		/// </summary>
		BaseMapScene mapScene;
		TimeTravelEffect effect;

		bool mapFlag = false; // 标志是否调用了 updateMap

        #region 初始化
  
		/// <summary>
        /// 初始化
        /// </summary>
        /// <param name="go"></param>
        protected override void setup(GameObject go) {
            base.setup(go);
            mapScene = SceneUtils.get<BaseMapScene>(go);
			effect = mapScene.timeTravelEffect;
        }

        #endregion

        /// <summary>
        /// 状态进入
        /// </summary>
        protected override void onStateEnter() {
            base.onStateEnter();
			mapFlag = false;
			mapScene.onTravelStart();
			effect.material.SetVector(
				CenterPosAttrName, effect.center);
        }

        /// <summary>
        /// 状态更新
        /// </summary>
        protected override void onStateUpdate() {
            base.onStateUpdate();
            if (!finished)
				effect.material.SetFloat(
					StrengthAttrName, effect.switchStrength);

			if (!mapFlag && aniRate >= 0.5) {
				mapScene.refreshMapActive();
				mapFlag = true;
			}
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
			mapScene.onTravelEnd();
		}

	}

}
