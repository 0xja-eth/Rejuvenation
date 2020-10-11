using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using GameModule.Services;

using Event = MapModule.Data.Event;
using MapModule.Services;
using MapModule.Data;

namespace UI.MapSystem.Controls.Events {

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
            //MessageService msgSer = MessageService.Get();

            //event_.actions.Add(() => {
            //    debugLog("Searching.");
            //    if (SceneUtils.getCurrentScene<BaseMapScene>().isDialogued())
            //        return;
            //    List<DialogMessage> msgs = msgSender.getMsgs();
            //    if (msgs == null) return;
            //    foreach(DialogMessage msg in msgs) {
            //        msgSer.addMessage(msg);
            //    }
                    
            //});
            //addEvent(event_);
		}

	}
}