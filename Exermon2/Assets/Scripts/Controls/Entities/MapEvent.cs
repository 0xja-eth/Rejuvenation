using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using Core.Data;

namespace UI.Common.Controls.Entities {

	/// <summary>
	/// 地图上的事件
	/// </summary>
	public class MapEvent : MapEntity {

		/// <summary>
		/// 行动字典
		/// </summary>
		protected List<Event> events;

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

		#region 碰撞检测

		/// <summary>
		/// 碰撞
		/// </summary>
		/// <param name="collision"></param>
		private void OnTriggerEnter2D(Collider2D collision) {
			
		}

		#endregion

		#region 事件控制

		/// <summary>
		/// 添加事件
		/// </summary>
		/// <param name="action">事件</param>
		/// <param name="cond">条件</param>
		public void addEvent(Event event_) {
			events.Add(event_);
		}

		#region 执行事件

		/// <summary>
		/// 处理事件
		/// </summary>
		public void processAction() {
			foreach(var event_ in events) {
			}
		}

		#endregion

		#endregion

	}
}