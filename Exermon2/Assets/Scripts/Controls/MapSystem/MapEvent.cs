using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using GameModule.Services;

using Event = MapModule.Data.Event;

namespace UI.Common.Controls.MapSystem {

	/// <summary>
	/// 地图上的事件
	/// </summary>
	[RequireComponent(typeof(EventProcessor))]
	public class MapEvent : MapEntity {

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		protected EventProcessor processor;

		/// <summary>
		/// 行动字典
		/// </summary>
		protected List<Event> events = new List<Event>();

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
			initializeActions();
		}

		/// <summary>
		/// 初始化事件行动
		/// </summary>
		protected virtual void initializeActions() { }

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			currentEvent_ = null;
			processor.setItem(currentEvent());
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

		////#endregion

		#region 事件控制

		///// <summary>
		///// 是否处于搜索状态
		///// </summary>
		//public bool isSearching => Input.GetKeyDown(gameSer.keyboard.searchKey);

		/// <summary>
		/// 当前事件
		/// </summary>
		/// <returns></returns>
		Event currentEvent_ = null;
		public Event currentEvent() {
			if (currentEvent_ == null) 
				foreach (var event_ in events)
					if (event_.isValid()) currentEvent_ = event_;

			return currentEvent_;
		}

		/// <summary>
		/// 添加事件
		/// </summary>
		/// <param name="action">事件</param>
		/// <param name="cond">条件</param>
		public void addEvent(Event event_) {
			events.Add(event_);
		}

		/// <summary>
		/// 添加事件
		/// </summary>
		/// <param name="action">事件</param>
		/// <param name="cond">条件</param>
		public void removeEvent(Event event_) {
			events.Remove(event_);
		}

		//#region 执行事件

		///// <summary>
		///// 处理触发
		///// </summary>
		///// <param name="player">触发相关的玩家</param>
		///// <param name="type">触发类型</param>
		///// <returns></returns>
		//public void processTrigger(MapPlayer player, Event.TriggerType type) {
		//	eventPlayer = player; processTrigger(type);
		//}
		//public void processTrigger(Event.TriggerType type) {
		//	if (judgeTrigger(type)) processAction();
		//}

		///// <summary>
		///// 判断是否触发
		///// </summary>
		///// <param name="type"></param>
		///// <returns></returns>
		//bool judgeTrigger(Event.TriggerType type) {
		//	return currentEvent()?.triggerType == type;
		//}

		///// <summary>
		///// 处理事件
		///// </summary>
		//void processAction() {
		//	currentEvent()?.process();
		//}

		//#endregion

		#endregion

	}
}