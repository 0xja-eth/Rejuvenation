using System.Collections.Generic;

using UnityEngine;

using MapModule.Data;
using BattleModule.Data;

using GameModule.Services;
using PlayerModule.Services;

namespace UI.MapSystem.Controls {

	/// <summary>
	/// 地图上区域
	/// </summary>
	public class MapRegion : MapBaseRegion {

		/// <summary>
		/// 区域类型
		/// </summary>
		public enum RegionType {
			None
		}

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public RegionType type = RegionType.None;

		/// <summary>
		/// 进入区域的实体列表
		/// </summary>
		List<MapCharacter> entries = new List<MapCharacter>();
		
		#region 内容控制

		/// <summary>
		/// 是否在区域内
		/// </summary>
		/// <returns></returns>
		public bool isEnter(MapCharacter character) {
			return entries.Contains(character);
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 进入回调
		/// </summary>
		public override void onEnter(MapCharacter character) {
			base.onEnter(character);
			if (!entries.Contains(character))
				entries.Add(character);
		}

		/// <summary>
		/// 退出回调
		/// </summary>
		public override void onExit(MapCharacter character) {
			base.onExit(character);
			if (entries.Contains(character))
				entries.Remove(character);
		}

		#endregion

	}
}
