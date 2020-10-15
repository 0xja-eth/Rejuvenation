using System.Collections.Generic;

using UnityEngine;

using MapModule.Data;

namespace UI.MapSystem.Controls {

	using BattleSystem.Controls;

	/// <summary>
	/// 地图上的载具实体
	/// </summary>
	[RequireComponent(typeof(MapCharacterGroup))]
	public class MapVehicle : MapCharacter {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public MapRegion boardingRegion; // 上车区域
		public MapRegion[] landingRegions; // 下车区域（为空则任意位置均可下车）

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public int capacity = 1; // 载具最大容量
		public bool hidePassengers = false; // 隐藏乘客

		public Vector2 offset = new Vector2(); // 座位偏移

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		protected MapCharacterGroup passengers; // “乘客”集

		/// <summary>
		/// 内部变量定义
		/// </summary>
		Vector2? landPoint = null; // 着陆点

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			boardingRegion = boardingRegion ?? get<MapRegion>();
		}

		#endregion

		#region 乘客控制

		/// <summary>
		/// 是否存在指定乘客
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		public bool hasPassenger(MapCharacter character) {
			return passengers.hasSubView(character);
		}

		/// <summary>
		/// 添加乘客
		/// </summary>
		public bool addPassenger(MapCharacter character, bool force = false) {
			return (force || isBoardingValid(character)) &&
				_addPassenger(character);
		}
		bool _addPassenger(MapCharacter character) {
			if (passengers.addSubView(character)) {
				onBoard(character); return true;
			}
			return false;
		}

		/// <summary>
		/// 移除乘客
		/// </summary>
		/// <param name="entity"></param>
		public bool removePassenger(MapCharacter character) {
			return isLandingValid() && _removePassenger(character);
		}
		bool _removePassenger(MapCharacter character) {
			if (passengers.removeSubView(character)) {
				onLand(character); return true;
			}
			return false;
		}

		/// <summary>
		/// 移除所有乘客
		/// </summary>
		public bool removeAllPassengers() {
			if (!isLandingValid()) return false;

			var flag = true;
			foreach (var character in passengers.getSubViews())
				flag = flag && _removePassenger(character);

			return flag;
		}

		/// <summary>
		/// 能否上车
		/// </summary>
		/// <returns></returns>
		public virtual bool isBoardingValid(MapCharacter character) {
			return !isFull() && isInBoardingRegions(character);
		}

		/// <summary>
		/// 容量是否已满
		/// </summary>
		/// <returns></returns>
		public bool isFull() {
			return capacity > 0 && passengers.subViewsCount() >= capacity;
		}

		/// <summary>
		/// 是否在上车区域中
		/// </summary>
		/// <returns></returns>
		bool isInBoardingRegions(MapCharacter character) {
			return boardingRegion && boardingRegion.isEnter(character);
		}

		/// <summary>
		/// 能否下车
		/// </summary>
		/// <returns></returns>
		public virtual bool isLandingValid() {
			return isInLandingRegions();
		}

		/// <summary>
		/// 是否在下车区域中
		/// </summary>
		/// <returns></returns>
		bool isInLandingRegions() {
			return (landPoint = findLandPoint()) != null;
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 上车回调
		/// </summary>
		/// <param name="character"></param>
		protected virtual void onBoard(MapCharacter character) {
			if (hidePassengers) character.deactivate();

			character.vehicle = this;
			character.transform.localPosition = offset;
			character.stop();
		}

		/// <summary>
		/// 下车回调
		/// </summary>
		/// <param name="character"></param>
		protected virtual void onLand(MapCharacter character) {
			if (hidePassengers) character.activate();

			character.vehicle = null;
			character.pos = (Vector2)landPoint;

			landPoint = null;
		}

		/// <summary>
		/// 寻找着陆点
		/// </summary>
		protected virtual Vector2? findLandPoint() {
			if (landingRegions.Length <= 0) return pos;

			foreach (var region in landingRegions) {
				var pt = region.collider.ClosestPoint(pos);
				if (collider.OverlapPoint(pt)) return pt;
			}

			return null;
		}

		#endregion

	}
}
