
using UnityEngine;

using Core.Systems;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 空间转移
    /// </summary>
    public class TransferPosition : MapEventPage {

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public SceneSystem.Scene targetStage = 
			SceneSystem.Scene.NoneScene; // NoneScene 表示当前场景
		public Vector2 position; // 目标位置

		/// <summary>
		/// 执行
		/// </summary>
		protected override void invokeCustom() {
			base.invokeCustom();
			mapEvent.scene.changeStage(targetStage, position);
		}

	}
}
