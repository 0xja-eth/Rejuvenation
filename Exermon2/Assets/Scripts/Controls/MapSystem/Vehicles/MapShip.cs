using System.Linq;

using UnityEngine;

using Core.UI.Utils;

using MapModule.Data;
using BattleModule.Data;

using GameModule.Services;
using PlayerModule.Services;

namespace UI.MapSystem.Controls {

	using BattleSystem.Controls;

    /// <summary>
    /// 地图上的船载具
    /// </summary>
    public class MapShip : MapVehicle {

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public float magnetiteDist = 8; // 万象天引距离
		public float magnetiteEnergy = 1; // 使用万象天引耗能

		/// <summary>
		/// 内部变量定义
		/// </summary>
		WaterColumn targetCol; // 目标柱子

		bool boardFlag = false;

		/// <summary>
		/// 乘客玩家
		/// </summary>
		MapPlayer player => passengers.getSubView(0) as MapPlayer;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		GameService gameSer;

		/// <summary>
		/// 初始化碰撞回调
		/// </summary>
		protected override void initializeCollFuncs() {
			base.initializeCollFuncs();

			registerOnEnterFunc<WaterColumn>(onColumnColl);

			registerOnStayFunc<MapPlayer>(tryBoard);
			boardingRegion.registerOnStayFunc<MapRegion>(tryLand);
		}

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			if (player) updateInput();
		}

		/// <summary>
		/// 更新输入
		/// </summary>
		void updateInput() {
			if (isMagnetite()) useMagnetite();
		}

		#endregion

		#region 磁石柱子控制

		/// <summary>
		/// 是否使用磁石
		/// </summary>
		/// <returns></returns>
		public bool isMagnetite() {
			return player.energy >= magnetiteEnergy && 
				Input.GetKeyDown(gameSer.keyboard.magnetiteKey);
		}

		/// <summary>
		/// 使用磁石
		/// </summary>
		void useMagnetite() {
			player.addEnergy(-magnetiteEnergy);

			var dir = player.direction;
			var vec = RuntimeCharacter.dir82Vec(dir);
			var resList = Physics2D.RaycastAll(pos, vec, magnetiteDist);

			foreach(var res in resList) {
				var obj = res.collider?.gameObject;

				targetCol = SceneUtils.get<WaterColumn>(obj);
				if (targetCol == null) continue;

				moveDirection(dir); break;
			}
		}

		/// <summary>
		/// 与柱子相撞
		/// </summary>
		/// <param name="col"></param>
		void onColumnColl(WaterColumn col) {
			if (col == targetCol) stop();
		}

		#endregion

		#region 乘降操作

		/// <summary>
		/// 是否需要乘降
		/// </summary>
		/// <returns></returns>
		public bool isTake() {
			return Input.GetKeyDown(gameSer.keyboard.takeKey);
		}

		/// <summary>
		/// 尝试自动上船
		/// </summary>
		/// <param name="player"></param>
		void tryBoard(MapPlayer player) {
			if (!isTake() || isFreezing()) return;
			if (!boardFlag && addPassenger(player, true))
				boardFlag = true;
		}

		/// <summary>
		/// 尝试自动下船
		/// </summary>
		/// <param name="region"></param>
		void tryLand(MapRegion region) {
			debugLog("tryLand");
			if (!isTake() || isFreezing()) return;
			if (boardFlag && landingRegions.Contains(region)
				&& removeAllPassengers()) boardFlag = false;
		}

		#endregion

	}
}
