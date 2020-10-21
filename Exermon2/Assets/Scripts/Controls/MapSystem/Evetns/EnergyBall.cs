
using UnityEngine;
using UnityEngine.Events;

using PlayerModule.Services;
using PlayerModule.Data;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 开关
    /// </summary>
    public class EnergyBall : MapEventPage {

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public float energy = 10; // 恢复能量值

		/// <summary>
		/// 外部系统设置
		/// </summary>
		PlayerService playerSer;

		/// <summary>
		/// 是否有效
		/// </summary>
		/// <returns></returns>
		public override bool isValid() {
			return base.isValid() && !playerSer.info.getSwitch(
				Info.Switches.EnergyBall1);
		}

		/// <summary>
		/// 执行
		/// </summary>
		protected override void invokeCustom() {
			base.invokeCustom();
			playerSer.info.setSwitch(
				Info.Switches.EnergyBall1, true);
			mapEvent.map.player?.addEnergy(energy);
		}

	}
}
