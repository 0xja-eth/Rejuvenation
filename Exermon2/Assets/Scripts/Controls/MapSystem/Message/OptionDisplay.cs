using System;

using UnityEngine.UI;

using MapModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.MapSystem.Controls {

	/// <summary>
	/// 选项显示
	/// </summary>
	public class OptionDisplay : SelectableItemDisplay<DialogOption>{

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text text;

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        protected override void drawExactlyItem(DialogOption item) {
            base.drawExactlyItem(item);
            text.text = item.description;
        }

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
			text.text = "";
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 点击回调
		/// </summary>
		public override void onClick() {
			base.onClick();
			item?.invoke();
		}

		#endregion
	}
}
