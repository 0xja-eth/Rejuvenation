﻿using System;
using System.Collections.Generic;

using Core.Services;

namespace MapModule.Services {

	using Data;

	/// <summary>
	/// 消息服务类，进行对话的后台处理
	/// </summary>
	public class MessageService : BaseService<MessageService> {
		
		/// <summary>
		/// 消息队列
		/// </summary>
        Queue<DialogMessage> messages = new Queue<DialogMessage>();

		/// <summary>
		/// 添加消息
		/// </summary>
		/// <param name="message"></param>
        public void addMessage(DialogMessage message) {
            messages.Enqueue(message);
        }

		/// <summary>
		/// 获取消息
		/// </summary>
		/// <returns></returns>
        public DialogMessage getMessage() {
            if (messages.Count == 0) return null;
            return messages.Dequeue();
        }

		/// <summary>
		/// 获取消息数
		/// </summary>
		/// <returns></returns>
        public int messageCount() {
            return messages.Count;
        }
    }
}