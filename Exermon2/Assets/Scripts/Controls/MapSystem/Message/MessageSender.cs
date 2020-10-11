using System;
using System.Collections.Generic;

using UnityEngine;

using Core.UI;

using MapModule.Data;

namespace Assets.Scripts.Controls.MapSystem.Message {

	/// <summary>
	/// 消息发送组件
	/// </summary>
	public class MessageSender : GeneralComponent {

		/// <summary>
		/// 外部变量设置
		/// </summary>
		[SerializeField]
		public List<DialogMessageGroup> messages = new List<DialogMessageGroup>();
        
		/// <summary>
		/// 获取消息
		/// </summary>
		/// <returns></returns>
        public List<DialogMessage> getMsgs() {
            if (messages.Count == 0)
                return null;
            List<DialogMessage> msgs = messages[0].group;
            messages.RemoveAt(0);
            return msgs;
        }

		/// <summary>
		/// 测试事件
		/// </summary>
        public void A() {
            debugLog("AAA");
        }
        public void B() {
            debugLog("BBB");
        }
        public void C() {
            debugLog("CCC");
        }
    }
}
