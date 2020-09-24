using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using GameModule.Services;

using Event = MapModuleData.Data.Event;

namespace UI.Common.Controls.Entities.TestScene {

	/// <summary>
	/// 地图上的事件
	/// </summary>
	public class NPC : MapEvent {

		/// <summary>
		/// 初始化事件
		/// </summary>
		protected override void initializeActions() {
			base.initializeActions();
			addTestAction();
		}

		/// <summary>
		/// 测试动作
		/// </summary>
		void addTestAction() {
			var event_ = new Event(Event.TriggerType.CollSearch);
			event_.actions.Add(() => debugLog("Searching."));

			addEvent(event_);
		}

	}
}