using Core.Services;
using MapModule.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace GameModule.Services {
    public class MessageServices : BaseService<MessageServices> {
        public bool isDialogued = false;

        Queue<DialogMessage> queueMessage = new Queue<DialogMessage>();

        public void addMessage(DialogMessage message) {
            queueMessage.Enqueue(message);
        }

        public DialogMessage getMessage() {
            if (queueMessage.Count == 0)
                return null;
            return queueMessage.Dequeue();
        }

        public int getMsgLength() {
            return queueMessage.Count;
        }
    }
}
