﻿
using UnityEngine;

using Core.UI;
using Core.UI.Utils;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 场景进入状态
    /// </summary>
    public class SceneExitState : BaseStateBehaviour {
		
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
		/// 状态结束回调
		/// </summary>
		protected override void onStateFinished() {
			base.onStateFinished();
			mapScene.onLoadingStart();
		}

	}

}
