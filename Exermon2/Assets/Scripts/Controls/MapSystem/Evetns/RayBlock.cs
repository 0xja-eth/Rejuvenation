
using UnityEngine;
using UnityEngine.Events;

using PlayerModule.Services;
using PlayerModule.Data;

using BattleModule.Data;

namespace UI.MapSystem.Controls {

	using BattleSystem.Controls;

    /// <summary>
    /// 通过开关驱动的实体
    /// </summary>
    public class RayBlock : SwitchEntity {

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public float damage = 0; // 造成伤害
		public float hitting = 0.5f; // 硬直
		public float freezing = 0; // 冻结

		public bool randomSwitch = false; // 随机激光

		public float minDuration = 1f, maxDuration = 2.5f; // 最小最大持续时间
		public float minInterval = 1f, maxInterval = 2f; // 最小最大激光间隔

		/// <summary>
		/// 时间
		/// </summary>
		float timer = 0, curDuration = 0, curInterval = 0;

		/// <summary>
		/// 初始化碰撞回调
		/// </summary>
		protected override void initializeCollFuncs() {
			base.initializeCollFuncs();

			registerOnEnterFunc<MapPlayer>(onPlayerColl);
		}

		/// <summary>
		/// 是否激活
		/// </summary>
		/// <returns></returns>
		public override bool isActive() {
			var flag = base.isActive();
			if (!flag || !randomSwitch) return flag;

			return timer >= curInterval;
		}

		/// <summary>
		/// 随机时长和间隔
		/// </summary>
		void randomDurationAndInterval() {
			curDuration = Random.Range(minDuration, maxDuration);
			curInterval = Random.Range(minInterval, maxInterval);
			timer = curDuration + curInterval;
		}

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update(); updateTime();
		}

		/// <summary>
		/// 更新时间
		/// </summary>
		void updateTime() {
			if (timer <= 0) randomDurationAndInterval();
			else timer -= Time.deltaTime;
		}

		/// <summary>
		/// 与玩家碰撞回调
		/// </summary>
		/// <param name="player"></param>
		public void onPlayerColl(MapPlayer player) {
			if (damage == 0 || !player.runtimeBattler.isTargetEnable())
				return;

			var result = new RuntimeActionResult();
			result.hpDamage = damage;
			result.hitting = hitting;
			result.freezing = freezing;

			player.runtimeBattler.applyResult(result);
		}

	}
}
