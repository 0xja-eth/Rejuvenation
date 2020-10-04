using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using GameModule.Services;

using Event = MapModule.Data.Event;

namespace UI.MapSystem.Controls {

	using BattleSystem.Controls;

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
		/// 事件列表
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

		#region 事件控制

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

		/// <summary>
		/// 处理触发
		/// </summary>
		/// <param name="player">触发相关的玩家</param>
		/// <param name="type">触发类型</param>
		/// <returns></returns>
		public void processTrigger(MapPlayer player, Event.TriggerType type) {
			processor.processTrigger(player, type);
		}
		
		#endregion

	}
}