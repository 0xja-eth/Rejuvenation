using MapModule.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Common.Controls.ItemDisplays;
using UnityEngine.UI;

namespace UI.Common.Controls.ItemDisplays {
    public class OptionDisplay :SelectableItemDisplay<DialogOption>{

        public Text text;


        protected override void drawExactlyItem(DialogOption item) {
            base.drawExactlyItem(item);
            text.text = item.description;
        }
        public override void select() {
            base.select();
            item.action?.Invoke();
        }
    }
}
