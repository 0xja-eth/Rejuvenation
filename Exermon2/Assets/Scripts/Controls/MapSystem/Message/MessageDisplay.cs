using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Config;
using Core.Data.Loaders;

using MapModule.Data;
using PlayerModule.Services;

namespace UI.MapSystem.Controls {

	using Windows;

	/// <summary>
	/// 对话框显示
	/// </summary>
	[RequireComponent(typeof(DialogWindow))]
    public class MessageDisplay : MessageBaseDisplay {

		/// <summary>
		/// 常量定义
		/// </summary>
		const string WangZi = "王子";
		const string Zhizi = "智子";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public new Text name;
        public GameObject nameFrame;

		public OptionContainer optionContainer;

        /// <summary>
        /// 内部组件设置
        /// </summary>
        [RequireTarget]
        [HideInInspector]
        public DialogWindow window;

		/// <summary>
		/// 内部变量设置
		/// </summary>
		Dictionary<string, int> bustIdDict = new Dictionary<string, int>();

		/// <summary>
		/// 外部系统设置
		/// </summary>
		PlayerService playerSer;

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			bustIdDict.Add(WangZi, 0); // 0 表示跟随主角
			bustIdDict.Add(Zhizi, 3);
		}

		#region 数据操作

		/// <summary>
		/// 物品改变回调
		/// </summary>
		protected override void onItemChanged() {
            base.onItemChanged();
            if (printing) stopPrint();
            optionContainer?.setItems(item.options);
        }

        /// <summary>
        /// 物品清除回调
        /// </summary>
        protected override void onItemClear() {
            base.onItemClear();
            if (printing) stopPrint();
            optionContainer?.clearItems();
        }

		/// <summary>
		/// 获取立绘
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Sprite[] busts(string name) {
			if (bustIdDict.ContainsKey(name)) {
				var bid = bustIdDict[name];
				if (bid == 0) bid = playerSer.actor.characterId;
				return AssetLoader.loadAssets<Sprite>(Asset.Type.Bust, bid);
			}
			return null;
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
			if(nameFrame) nameFrame.SetActive(!string.IsNullOrEmpty(item.name));
            name.text = item.name;
		}

		/// <summary>
		/// 获取立绘
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected override Sprite getBust(DialogMessage item) {
			if (busts(item.name) != null) return busts(item.name)[0];

			return base.getBust(item);
		}

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            name.text = "";
			if (nameFrame) nameFrame.SetActive(false);
        }

        /// <summary>
        /// 打印开始回调
        /// </summary>
        protected override void onPrintStart() {
            base.onPrintStart();
            optionContainer?.deactivate();
        }

        /// <summary>
        /// 打印结束回调
        /// </summary>
        protected override void onPrintEnd() {
            base.onPrintEnd();
            optionContainer?.activate();
        }

        #endregion

    }
}
