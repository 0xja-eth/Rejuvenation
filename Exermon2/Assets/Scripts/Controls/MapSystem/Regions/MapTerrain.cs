using System.Collections.Generic;

using UnityEngine;

using MapModule.Data;
using BattleModule.Data;

using GameModule.Services;
using PlayerModule.Services;

namespace UI.MapSystem.Controls {

	using BattleSystem.Controls;

	/// <summary>
	/// 地图上的地形
	/// </summary>
	public class MapTerrain : MapBaseRegion {

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public bool passable = true;
		
		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
		}

		/// <summary>
		/// 更新通行性
		/// </summary>
		void updatePassable() {
			collider.isTrigger = !isGlobalPassable();
		}

		#endregion

		#region 通行性

		/// <summary>
		/// 能否通行（全局）
		/// </summary>
		public virtual bool isGlobalPassable() {
			return passable;
		}

		/// <summary>
		/// 能否通行（对于某Character）
		/// </summary>
		public bool isPassable(MapCharacter character) {
			if (!isGlobalPassable()) return false;

			var battler = character as MapBattler;
			if (battler != null) { // 如果是 Battler
				if (!isBattlerPassable(battler)) return false;

				var enemy = character as MapEnemy;
				if (enemy != null) return isEnemyPassable(enemy);

				var player = character as MapPlayer;
				if (player != null) return isPlayerPassable(player);
			}

			return isOthersPassable(character);
		}

		/// <summary>
		/// 战斗者能否通行
		/// </summary>
		/// <param name="battler"></param>
		/// <returns></returns>
		protected virtual bool isBattlerPassable(MapBattler battler) {
			return true;
		}

		/// <summary>
		/// 敌人能否通行
		/// </summary>
		/// <param name="enemy"></param>
		/// <returns></returns>
		protected virtual bool isEnemyPassable(MapEnemy enemy) {
			return true;
		}

		/// <summary>
		/// 玩家能否通行
		/// </summary>
		/// <param name="enemy"></param>
		/// <returns></returns>
		protected virtual bool isPlayerPassable(MapPlayer player) {
			return true;
		}

		/// <summary>
		/// 其他行走实体能否通行
		/// </summary>
		/// <param name="enemy"></param>
		/// <returns></returns>
		protected virtual bool isOthersPassable(MapCharacter character) {
			return true;
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 区域进入回调
		/// </summary>
		public override void onEnter(MapCharacter character) {
			if (!isPassable(character)) character.stop();
		}

		/// <summary>
		/// 区域持续回调
		/// </summary>
		public override void onStay(MapCharacter character) {
			if (!isPassable(character)) character.stop();
		}

		#endregion

	}
}
