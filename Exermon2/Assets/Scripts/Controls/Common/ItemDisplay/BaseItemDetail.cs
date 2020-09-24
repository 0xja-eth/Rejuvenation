using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using ItemModule.Data;

using Core.UI.Utils;

namespace UI.Common.Controls.ItemDisplays {

	/// <summary>
	/// 基本物品显示详情
	/// </summary>
	[RequireComponent(typeof(BaseItemDisplay))]
	public class BaseItemDetail : ItemDetailDisplay<BaseItem> {

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		BaseItemDisplay itemDisplay;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			initializeDrawFuncs();
		}

		/// <summary>
		/// 初始化绘制函数
		/// </summary>
		protected virtual void initializeDrawFuncs() { }

		#endregion

		#region 绘制函数控制

		/// <summary>
		/// 注册物品类型
		/// </summary>
		/// <typeparam name="T">物品类型</typeparam>
		/// <param name="func">绘制函数</param>
		public virtual void registerItemType<T>(
			UnityAction<T> func) where T : BaseItem {
			itemDisplay.registerItemType(func);
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 物品改变回调
		/// </summary>
		protected override void onItemChanged() {
			base.onItemChanged();
			itemDisplay.setItem(item);
		}

		#endregion
	}
}