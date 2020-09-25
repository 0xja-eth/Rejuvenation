using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using GameModule.Services;

using UI.Common.Controls.ItemDisplays;

using Event = MapModuleData.Data.Event;
using MapModuleData.Data;

namespace UI.Common.Controls.Entities {

	/// <summary>
	/// 地图上的事件
	/// </summary>
	public class EventDisplay : ItemDisplay<Event> {


		#region 界面刷新

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(Event item) {
			base.drawExactlyItem(item);
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
		}

		#endregion
	}
}