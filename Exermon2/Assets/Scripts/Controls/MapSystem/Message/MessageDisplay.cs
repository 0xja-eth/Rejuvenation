using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UI.Common.Controls.ItemDisplays;

using MapModule.Data;

namespace UI.MapSystem.Controls {

	/// <summary>
	/// 对话框显示
	/// </summary>
	[RequireComponent(typeof(OptionContainer))]
	public class MessageDisplay : ItemDisplay<DialogMessage>{
		
		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text dialogText;
		public Text nameText;
		public Image bust;

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		OptionContainer optionsDisplay;

		/// <summary>
		/// 物品改变回调
		/// </summary>
		protected override void onItemChanged() {
			base.onItemChanged();
			optionsDisplay.setItems(item.options);
		}

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(DialogMessage item) {
			base.drawExactlyItem(item);
			drawMessage(item); drawBust(item);
		}

		/// <summary>
		/// 绘制信息
		/// </summary>
		/// <param name="item"></param>
		void drawMessage(DialogMessage item) {
			dialogText.text = item.message;
			nameText.text = item.name;
		}

		/// <summary>
		/// 绘制立绘
		/// </summary>
		/// <param name="item"></param>
		void drawBust(DialogMessage item) {
			var bust = item.bust();

			this.bust.gameObject.SetActive(bust != null);
			this.bust.overrideSprite = bust;
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
			dialogText.text = nameText.text = "";
			bust.gameObject.SetActive(false);
			bust.overrideSprite = null;
		}

	}
}
