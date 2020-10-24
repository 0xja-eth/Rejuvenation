using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UI.Common.Controls.ItemDisplays;

using MapModule.Data;

namespace UI.MapSystem.Controls {

	using Windows;

	/// <summary>
	/// 对话框显示
	/// </summary>
	[RequireComponent(typeof(DialogWindow))]
	public class MessageDisplay : MessageBaseDisplay{

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public new Text name = null; 
		public GameObject nameFrame = null; 
        public OptionContainer optionContainer = null;

        /// <summary>
        /// 内部组件设置
        /// </summary>
        [RequireTarget]
        [HideInInspector]
        public DialogWindow window;

        #region 数据操作

        /// <summary>
        /// 物品改变回调
        /// </summary>
        protected override void onItemChanged() {
            base.onItemChanged();
            if (printing) stopPrint();
            optionContainer.setItems(item.options);
        }

        /// <summary>
        /// 物品清除回调
        /// </summary>
        protected override void onItemClear() {
            base.onItemClear();
            if (printing) stopPrint();
            optionContainer.clearItems();
        }

        #endregion


        #region 界面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(DialogMessage item) {
            base.drawExactlyItem(item);
            drawName(item);
            drawImage(item);
        }

		/// <summary>
		/// 绘制名称
		/// </summary>
		/// <param name="item"></param>
		void drawName(DialogMessage item) {
			nameFrame?.SetActive(!string.IsNullOrEmpty(item.name));
            name.text = item.name;
		}

        /// <summary>
        /// 绘制立绘
        /// </summary>
        /// <param name="item"></param>
        override protected void drawImage(DialogMessage item) {
            var bust = item.bust();

            image.gameObject.SetActive(bust != null);
            image.overrideSprite = MessageSender.busts(item.name)[0];
            image.gameObject.SetActive(true);
        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            name.text = "";
            nameFrame?.SetActive(false);
        }

        /// <summary>
        /// 打印开始回调
        /// </summary>
        protected override void onPrintStart() {
            base.onPrintStart();
            optionContainer.deactivate();
        }

        /// <summary>
        /// 打印结束回调
        /// </summary>
        protected override void onPrintEnd() {
            base.onPrintEnd();
            optionContainer.activate();
        }

        #endregion

    }
}
