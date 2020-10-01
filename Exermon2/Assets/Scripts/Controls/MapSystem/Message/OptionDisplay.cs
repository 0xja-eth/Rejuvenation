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

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text text;

        #region 激活和关闭

        /// <summary>
        /// 激活
        /// </summary>
        public override void activate() {
            base.activate();
            actived = false;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        protected override void drawExactlyItem(DialogOption item) {
            base.drawExactlyItem(item);
            text.text = item.description;
        }

        #endregion

        #region 状态控制

        /// <summary>
        /// 选择
        /// </summary>
        public override void select() {
            base.select();
            item.action?.Invoke();
        }

        #endregion
    }
}
