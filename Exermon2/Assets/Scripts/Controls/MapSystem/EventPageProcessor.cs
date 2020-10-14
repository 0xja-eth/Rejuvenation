using System.Collections.Generic;

using UnityEngine;

using Core.UI.Utils;

using GameModule.Services;

using UI.Common.Controls.ItemDisplays;

using Event = MapModule.Data.Event;

namespace UI.MapSystem.Controls {

	using BattleSystem.Controls;

	/// <summary>
	/// 地图上的事件
	/// </summary>
	[RequireComponent(typeof(Collider2D))]
	public class EventPageProcessor : ItemDisplay<MapEventPage> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public SpriteRenderer sprite;

		/// <summary>
		/// 碰撞的玩家
		/// </summary>
		[HideInInspector]
		public MapPlayer eventPlayer;

		/// <summary>
		/// 外部系统
		/// </summary>
		protected GameService gameSer;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			sprite = sprite ?? get<SpriteRenderer>();
		}

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			processTrigger(MapEventPage.TriggerType.Always);
		}

		#endregion

		//#region 碰撞检测

		///// <summary>
		///// 碰撞开始
		///// </summary>
		///// <param name="collision"></param>
		//private void OnTriggerEnter2D(Collider2D collision) {
		//	var player = SceneUtils.get<MapPlayer>(collision);
		//	if (player != null) onPlayerCollEnter(player);
		//}

		///// <summary>
		///// 碰撞持续
		///// </summary>
		///// <param name="collision"></param>
		//private void OnTriggerStay2D(Collider2D collision) {
		//	var player = SceneUtils.get<MapPlayer>(collision);
		//	if (player != null) onPlayerCollStay(player);
		//}

		///// <summary>
		///// 碰撞结束
		///// </summary>
		///// <param name="collision"></param>
		//private void OnTriggerExit2D(Collider2D collision) {
		//	var player = SceneUtils.get<MapPlayer>(collision);
		//	if (player != null) onPlayerCollExit(player);
		//}

		///// <summary>
		///// 玩家碰撞开始
		///// </summary>
		///// <param name="player"></param>
		//protected virtual void onPlayerCollEnter(MapPlayer player) {
		//	processTrigger(player, Event.TriggerType.CollEnter);
		//}

		///// <summary>
		///// 玩家碰撞持续
		///// </summary>
		///// <param name="player"></param>
		//protected virtual void onPlayerCollStay(MapPlayer player) {
		//	processTrigger(player, isSearching ?
		//		Event.TriggerType.CollSearch : Event.TriggerType.CollStay);
		//}

		///// <summary>
		///// 玩家碰撞结束
		///// </summary>
		///// <param name="player"></param>
		//protected virtual void onPlayerCollExit(MapPlayer player) {
		//	processTrigger(player, Event.TriggerType.CollExit);
		//}

		//#endregion

		#region 事件控制
			
		#region 执行事件

		/// <summary>
		/// 处理触发
		/// </summary>
		/// <param name="player">触发相关的玩家</param>
		/// <param name="type">触发类型</param>
		/// <returns></returns>
		public void processTrigger(MapPlayer player, MapEventPage.TriggerType type) {
			eventPlayer = player; processTrigger(type);
		}
		public void processTrigger(MapEventPage.TriggerType type) {
			if (judgeTrigger(type)) processAction();
		}

		/// <summary>
		/// 判断是否触发
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		bool judgeTrigger(MapEventPage.TriggerType type) {
			return item?.triggerType == type;
		}

		/// <summary>
		/// 处理事件
		/// </summary>
		void processAction() {
			item?.invoke();
		}

		#endregion

		#endregion

		#region 界面刷新

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(MapEventPage item) {
			base.drawExactlyItem(item);
			if (sprite) sprite.sprite = item.picture;
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
			if (sprite) sprite.sprite = null;
		}

		#endregion
	}
}