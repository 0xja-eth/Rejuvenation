using System.Collections.Generic;

using UnityEngine;

using MapModule.Data;
using BattleModule.Data;

using GameModule.Services;
using PlayerModule.Services;
using UI.BattleSystem.Controls;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 地图上的河流
    /// </summary>
    public class MapRiver : MapTerrain {
		
		/// <summary>
		/// 外部变量设置
		/// </summary>
		public int emptyLayer = 15;
		public int waterLayer = 4;
		public int iceLayer = 14;

		[SerializeField]
		bool _active = false;
		public bool active {
			get => _active;
			set {
				// 只有 active 改变时候才触发
				if (_active == value) return;
				_active = value; requestRefresh();
			}
		}

		/// <summary>
		/// 状态判断
		/// </summary>
		public bool isWater => active && map.type == TimeType.Past;
		public bool isIce => active && map.type == TimeType.Present;

		#region 更新

		/// <summary>
		/// 更新所属层
		/// </summary>
		protected override void updateLayer() {
			base.updateLayer();
			if (isWater) switchLayer(waterLayer);
			else if (isIce) switchLayer(iceLayer);
			else switchLayer(emptyLayer);
		}

		#endregion

		#region 通行性

		/// <summary>
		/// 能否通过（全局）
		/// </summary>
		public override bool isGlobalPassable() {
			return base.isGlobalPassable() && !active;
		}

		#endregion

		///// <summary>
		///// 敌人能否通行
		///// </summary>
		//protected override bool isBattlerPassable(MapBattler battler) {
		//	return base.isBattlerPassable(battler) && isIce;
		//}

		///// <summary>
		///// 其他实体能否通行
		///// </summary>
		//protected override bool isOthersPassable(MapCharacter character) {
		//	if (!base.isOthersPassable(character)) return false;

		//	var ship = character as MapShip;
		//	if (ship != null) return isShipPassable(ship);

		//	return false;
		//}

		///// <summary>
		///// 船能否通行
		///// </summary>
		///// <param name="ship"></param>
		///// <returns></returns>
		//bool isShipPassable(MapShip ship) {
		//	return isWater;
		//}

		//#endregion

		#region 界面控制

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			if (active) refreshActive();
			else refreshDeactive();
		}

		/// <summary>
		/// 激活
		/// </summary>
		void refreshActive() {
			// TODO: 富文 这里执行水的出现动画/绘制冰

		}

		/// <summary>
		/// 消失
		/// </summary>
		void refreshDeactive() {
			// TODO: 富文 这里执行水的消失动画/清除冰

		}

		#endregion

	}
}
