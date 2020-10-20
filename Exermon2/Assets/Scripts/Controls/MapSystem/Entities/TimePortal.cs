using System.Linq;

using UnityEngine;

using MapModule.Data;
using BattleModule.Data;

using GameModule.Services;
using PlayerModule.Services;

namespace UI.MapSystem.Controls {

	using BattleSystem.Controls;

    /// <summary>
    /// 时空镜
    /// </summary>
    public class TimePortal : MapEventPage {

		/// <summary>
		/// 执行
		/// </summary>
		protected override void invokeCustom() {
			base.invokeCustom();
			mapEvent.scene.travel();
		}

	}
}
