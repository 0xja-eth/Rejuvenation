using System.Linq;

using UnityEngine;

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
		/// 内部变量定义
		/// </summary>
		bool boardFlag = false;

		/// <summary>
		/// 初始化碰撞回调
		/// </summary>
		protected override void initializeCollFuncs() {
			base.initializeCollFuncs();

			registerOnEnterFunc<MapPlayer>(tryBoard);
			boardingRegion.registerOnEnterFunc<MapRegion>(tryLand);
		}

		/// <summary>
		/// 尝试自动上船
		/// </summary>
		/// <param name="player"></param>
		void tryBoard(MapPlayer player) {
			if (!boardFlag && addPassenger(player, true))
				boardFlag = true;
		}

		/// <summary>
		/// 尝试自动下船
		/// </summary>
		/// <param name="region"></param>
		void tryLand(MapRegion region) {
			if (boardFlag && landingRegions.Contains(region)
				&& removeAllPassengers()) boardFlag = false;
		}

	}
}
