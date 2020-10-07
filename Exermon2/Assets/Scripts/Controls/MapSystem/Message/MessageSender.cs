using Core.UI;
using MapModule.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Controls.MapSystem.Message {
    [Serializable]
    public class MessageSender : BaseComponent {
        public List<DialogMsgs> msgsList = new List<DialogMsgs>();
        
        public List<DialogMessage> getMsgs() {
            if (msgsList.Count == 0)
                return null;
            List<DialogMessage> msgs = msgsList[0].msgs;
            msgsList.RemoveAt(0);
            return msgs;
        }
    }
}
