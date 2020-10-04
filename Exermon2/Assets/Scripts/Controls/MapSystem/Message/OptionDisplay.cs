using System;

using UnityEngine.UI;

using MapModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.MapSystem.Controls {

	using Windows;

	/// <summary>
	/// 选项显示
	/// </summary>
	public class OptionDisplay : SelectableItemDisplay<DialogOption>{

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text text;

		#region 数据控制

		/// <summary>
		/// 对话框窗口
		/// </summary>
		DialogWindow dialogWindow => (container as OptionContainer).messageDisplay.window;

		#endregion

		#region 界面控制

		/// <summary>
		/// 绘制确切物品
		/// </summary>
		protected override void drawExactlyItem(DialogOption item) {
            base.drawExactlyItem(item);
            text.text = item.text;
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
			dialogWindow.deactivate();
			item?.invoke();
		}

		#endregion
	}
}
