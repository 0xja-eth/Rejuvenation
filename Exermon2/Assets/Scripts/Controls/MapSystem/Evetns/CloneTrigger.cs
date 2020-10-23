
using UnityEngine;
using UnityEngine.Events;

using PlayerModule.Services;
using PlayerModule.Data;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 复制触发器
    /// </summary>
    public class CloneTrigger : MapEventPage {

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public Transform targetPosition; // 目标位置

		/// <summary>
		/// 外部系统设置
		/// </summary>
		PlayerService playerSer;

		/// <summary>
		/// 执行
		/// </summary>
		protected override void invokeCustom() {
			base.invokeCustom();
			mapPlayer.addSeperation(targetPosition);
		}

	}
}
