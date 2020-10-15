using System;
using System.Collections.Generic;

using UnityEngine;

using GameModule.Services;

namespace UI.MapSystem.Controls {

    using BattleSystem.Controls;

	/// <summary>
	/// 地图上的事件
	/// </summary>
	[RequireComponent(typeof(EventPageProcessor))]
	public class MapEvent : MapEntity {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public List<MapEventPage> eventPages = new List<MapEventPage>();

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		protected EventPageProcessor processor;
		
		/// <summary>
		/// 外部系统
		/// </summary>
		protected GameService gameSer;

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			currentPage_ = null;
			processor.setItem(currentPage());
		}

		#endregion

		#region 事件页

		/// <summary>
		/// 添加事件页
		/// </summary>
		/// <param name="page"></param>
		public void addEventPage(MapEventPage page) {
			if (!eventPages.Contains(page)) eventPages.Add(page);
		}

		#endregion

		#region 事件控制

		/// <summary>
		/// 当前事件
		/// </summary>
		/// <returns></returns>
		MapEventPage currentPage_ = null;
		public MapEventPage currentPage() {
			if (currentPage_ == null) 
				foreach (var event_ in eventPages)
					if (event_.isValid()) currentPage_ = event_;

			return currentPage_;
		}

		/// <summary>
		/// 添加事件
		/// </summary>
		/// <param name="action">事件</param>
		/// <param name="cond">条件</param>
		public void addEvent(MapEventPage event_) {
			eventPages.Add(event_);
		}

		/// <summary>
		/// 移除事件
		/// </summary>
		/// <param name="action">事件</param>
		/// <param name="cond">条件</param>
		public void removeEvent(MapEventPage event_) {
			eventPages.Remove(event_);
		}

		/// <summary>
		/// 处理触发
		/// </summary>
		/// <param name="player">触发相关的玩家</param>
		/// <param name="type">触发类型</param>
		/// <returns></returns>
		public void processTrigger(MapPlayer player, MapEventPage.TriggerType type) {
			processor.processTrigger(player, type);
		}
		
		#endregion

	}
}