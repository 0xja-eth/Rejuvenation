using System.Collections.Generic;

using UnityEngine;

using MapModule.Data;
using BattleModule.Data;

using GameModule.Services;
using PlayerModule.Services;

namespace UI.MapSystem.Controls {

	/// <summary>
	/// 地图上区域基类
	/// </summary>
	public abstract class MapBaseRegion : MapEntity {
		
		/// <summary>
		/// 初始化碰撞函数
		/// </summary>
		protected override void initializeCollFuncs() {
			base.initializeCollFuncs();

			registerOnEnterFunc<MapCharacter>(onEnter);
			registerOnStayFunc<MapCharacter>(onStay);
			registerOnExitFunc<MapCharacter>(onExit);
		}

		#region 回调控制
		
		/// <summary>
		/// 区域进入回调
		/// </summary>
		public virtual void onEnter(MapCharacter character) { }

		/// <summary>
		/// 区域持续回调
		/// </summary>
		public virtual void onStay(MapCharacter character) { }

		/// <summary>
		/// 区域退出回调
		/// </summary>
		public virtual void onExit(MapCharacter character) { }

		#endregion

	}
}
