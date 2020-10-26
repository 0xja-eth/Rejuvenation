using System;
using System.Collections.Generic;

using UnityEngine;

using Core.UI;

using MapModule.Services;

using MapModule.Data;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 消息发送组件
    /// 实际使用还需要继承该类，用于添加触发条件
    /// </summary>
    public class MessageSender : MapEventPage {

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public bool isDialog = true;

        [SerializeField]
        public List<DialogMessage> messages = new List<DialogMessage>();

        /// <summary>
        /// 外部系统设置
        /// </summary>
        protected MessageService messageSer;

        /// <summary>
        /// 内部变量设置
        /// </summary>
        Dictionary<string, int> bustIdDict = new Dictionary<string, int>();
		
        #region 事件调用

        /// <summary>
        /// 自定义调用
        /// </summary>
        protected override void invokeCustom() {
            base.invokeCustom();
            messageSer.addMessages(messages);
			messageSer.dialogFlag = isDialog;
        }

        #endregion

    }
}
