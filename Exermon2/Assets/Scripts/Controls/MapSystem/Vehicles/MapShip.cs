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
		/// 初始化碰撞回调
		/// </summary>
		protected override void initializeCollFuncs() {
			base.initializeCollFuncs();

			registerOnEnterFunc<MapPlayer>(tryBoard);
			registerOnEnterFunc<MapRegion>(tryLand);
		}

		/// <summary>
		/// 尝试自动上船
		/// </summary>
		/// <param name="player"></param>
		void tryBoard(MapPlayer player) {
			addPassenger(player);
		}

		/// <summary>
		/// 尝试自动下船
		/// </summary>
		/// <param name="region"></param>
		void tryLand(MapRegion region) {
			if (landingRegions.Contains(region)) 
				removeAllPassengers();
		}

	}
}
