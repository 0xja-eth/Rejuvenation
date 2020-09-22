using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Core.UI;

namespace UI.Common.Controls.ItemDisplays {

	/// <summary>
	/// 物品容器接口
	/// </summary>
	public interface IContainerDisplay<T> where T : class {

		/// <summary>
		/// 配置
		/// </summary>
		/// <param name="items">物品集</param>
		void configure(T[] items);
		void configure(List<T> items);

		/// <summary>
		/// 设置物品集
		/// </summary>
		/// <param name="items">物品集</param>
		void setItems(T[] items);
		void setItems(List<T> items);

		/// <summary>
		/// 是否包含物品
		/// </summary>
		/// <param name="item">物品</param>
		/// <returns>是否包含</returns>
		bool containsItem(T item);

		/// <summary>
		/// 获取物品集
		/// </summary>
		/// <returns>物品集</returns>
		T[] getItems();

		/// <summary>
		/// 获取物品
		/// </summary>
		/// <returns>物品集</returns>
		T getItem(int index);

		/// <summary>
		/// 获取物品显示项集
		/// </summary>
		/// <returns>物品显示项</returns>
		IItemDisplay<T>[] getItemDisplays();

		/// <summary>
		/// 获取物品显示项
		/// </summary>
		/// <returns>物品显示项</returns>
		IItemDisplay<T> getItemDisplay(int index);

		/// <summary>
		/// 根据物品获取物品显示项
		/// </summary>
		/// <returns>物品显示项</returns>
		IItemDisplay<T> getItemDisplay(T item);
	}

	/// <summary>
	/// 物品容器接口
	/// </summary>
	public interface IContainerDisplay<D, T> : IContainerDisplay<T>
		where D : IItemDisplay<T> where T : class {

		/// <summary>
		/// 获取物品显示项集
		/// </summary>
		/// <returns>物品显示项</returns>
		new D[] getItemDisplays();

		/// <summary>
		/// 获取物品显示项
		/// </summary>
		/// <returns>物品显示项</returns>
		new D getItemDisplay(int index);

		/// <summary>
		/// 根据物品获取物品显示项
		/// </summary>
		/// <returns>物品显示项</returns>
		new D getItemDisplay(T item);
	}

	/// <summary>
	/// 物品容器显示
	/// </summary>
	public class ContainerDisplay<D, T> :
		GroupComponent<D>, IContainerDisplay<D, T>
		where D : ItemDisplay<T> where T : class {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public RectTransform mask; // 内容的上层蒙版（仅CanvasComponent使用）
		public Text countText; // 个数文本

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public int defaultCapacity = 0; // 默认容量

		public string defaultCountTextFormat = "总数目：{0}";

		/// <summary>
		/// 回调函数集
		/// </summary>
		public List<UnityAction> onItemsChangedCallbacks = new List<UnityAction>();

		/// <summary>
		/// 内部变量声明
		/// </summary>
		protected List<T> items = new List<T>(); // 物品列表

		#region 初始化

		/// <summary>
		/// 初次初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			initializeMask();
			configureDetail();
			configureItemDisplays();
		}

		/// <summary>
		/// 初始化上层蒙版
		/// </summary>
		void initializeMask() {
			mask = mask ?? container.parent as RectTransform;
		}

		/// <summary>
		/// 配置物品帮助组件
		/// </summary>
		void configureDetail() {
			itemDetail?.configure(this);
		}

		/// <summary>
		/// 配置初始的物品显示项
		/// </summary>
		void configureItemDisplays() {
			for (int i = 0; i < itemDisplaysCount(); ++i)
				createSubView(null, i);
		}

		/// <summary>
		/// 配置
		/// </summary>
		/// <param name="items">物品集</param>
		public void configure(T[] items) {
			base.configure(); setItems(items);
		}
		public void configure(List<T> items) {
			base.configure(); setItems(items);
		}

		#endregion

		#region 启动控制

		/// <summary>
		/// 启动视窗
		/// </summary>
		/// <param name="index"></param>
		public virtual void activate(int index = 0) {
			base.activate();
		}

		#endregion

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateDisplays();
		}

		/// <summary>
		/// 更新显示
		/// </summary>
		void updateDisplays() {
			for (int i = 0; i < itemDisplaysCount(); ++i) {
				var itemDisplay = subViews[i];
				if (itemDisplay.isRequestDestroy())
					removeItem(itemDisplay.getItem());
			}
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 添加回调函数
		/// </summary>
		/// <param name="cb">回调函数</param>
		/// <param name="type">回调类型（0：物品变更）</param>
		public virtual void addCallback(UnityAction cb, int type = 0) {
			if (cb == null) return;
			switch (type) {
				case 0: onItemsChangedCallbacks.Add(cb); break;
			}
		}

		/// <summary>
		/// 物品变更回调
		/// </summary>
		protected virtual void onItemsChanged() {
			filterItems();
			requestRefresh();
			refreshItemDisplays();
			callbackItemsChange();
		}

		/// <summary>
		/// 处理物品改变回调
		/// </summary>
		void callbackItemsChange() {
			foreach (var cb in onItemsChangedCallbacks) cb?.Invoke();
		}

		#endregion

		#region 调试

		/// <summary>
		/// 显示所有的物品
		/// </summary>
		public void debugShowItems() {
			debugLog(itemsCount() + ": " + string.Join(", ", items));
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 数目文本格式
		/// </summary>
		/// <returns></returns>
		protected virtual string countTextFormat() {
			return defaultCountTextFormat;
		}

		#region 物品控制

		/// <summary>
		/// 获取物品帮助组件
		/// </summary>
		/// <returns>帮助组件</returns>
		protected virtual IItemDetailDisplay<T> itemDetail => null;

		#region 数量相关

		/// <summary>
		/// 获取容量
		/// </summary>
		/// <returns>容量</returns>
		public virtual int capacity() {
			return defaultCapacity;
		}

		/// <summary>
		/// 获取物品数量
		/// </summary>
		/// <returns>数量</returns>
		public int itemsCount() {
			return items.Count;
		}

		/// <summary>
		/// 物品显示项数量
		/// </summary>
		/// <returns></returns>
		public int itemDisplaysCount() {
			return Math.Max(maxItemDisplaysCount(), subViews.Count);
		}

		/// <summary>
		/// 物品显示项最大数量
		/// </summary>
		/// <returns></returns>
		public int maxItemDisplaysCount() {
			var capacity = this.capacity();
			return capacity > 0 ? capacity : itemsCount();
		}

		#endregion

		#region 数据判断

		/// <summary>
		/// 是否包含空物品
		/// </summary>
		/// <returns>返回容器是否包含空物品</returns>
		protected virtual bool includeEmpty() {
			return false;
		}

		/// <summary>
		/// 是否包含物品
		/// </summary>
		/// <param name="item">物品</param>
		/// <returns>返回指定物品能否包含在容器中</returns>
		protected virtual bool isIncluded(T item) {
			if (item == null) return includeEmpty();
			return true;
		}

		#endregion

		#region 数据操作

		/// <summary>
		/// 设置物品集
		/// </summary>
		/// <param name="items">物品集</param>
		public virtual void setItems(T[] items) {
			//clearItems();
			this.items = items == null ?
				new List<T>() : new List<T>(items);
			onItemsChanged();
		}
		public void setItems(List<T> items) {
			setItems(items?.ToArray());
		}

		/// <summary>
		/// 是否包含物品
		/// </summary>
		/// <param name="item">物品</param>
		/// <returns>是否包含</returns>
		public bool containsItem(T item) {
			return items.Contains(item);
		}

		/// <summary>
		/// 增加物品
		/// </summary>
		/// <param name="item">物品</param>
		public virtual void addItem(T item) {
			items.Add(item);
			onItemsChanged();
		}

		/// <summary>
		/// 移除物品
		/// </summary>
		/// <param name="item">物品</param>
		public virtual void removeItem(T item) {
			items.Remove(item);
			onItemsChanged();
		}

		/// <summary>
		/// 清空物品
		/// </summary>
		public virtual void clearItems() {
			items.Clear();
			onItemsChanged();
		}

		/// <summary>
		/// 过滤物品列表
		/// </summary>
		void filterItems() {
			items = items.FindAll(isIncluded);
		}

		/// <summary>
		/// 刷新物品（重新设置）
		/// </summary>
		public virtual void refreshItems() {
			setItems(items);
		}

		#endregion

		#region 数据获取

		/// <summary>
		/// 获取物品集
		/// </summary>
		/// <returns>物品集</returns>
		public T[] getItems() {
			return items.ToArray();
		}

		/// <summary>
		/// 获取物品
		/// </summary>
		/// <param name="index">索引</param>
		/// <returns></returns>
		public T getItem(int index) {
			return items[index];
		}

		/// <summary>
		/// 获取物品显示项数组
		/// </summary>
		/// <returns>物品显示项数组</returns>
		public D[] getItemDisplays() { return getSubViews(); }

		/// <summary>
		/// 接口实现
		/// </summary>
		/// <returns></returns>
		IItemDisplay<T>[] IContainerDisplay<T>.getItemDisplays() {
			var displays = getItemDisplays();
			var res = new ItemDisplay<T>[displays.Length];
			for (int i = 0; i < displays.Length; ++i)
				res[i] = displays[i];

			return res;
		}

		/// <summary>
		/// 获取物品显示项
		/// </summary>
		/// <returns>物品显示项数组</returns>
		public D getItemDisplay(int index) { return getSubView(index); }
		IItemDisplay<T> IContainerDisplay<T>.getItemDisplay(int index) {
			return getItemDisplay(index);
		}

		/// <summary>
		/// 获取物品对应的物品显示项
		/// </summary>
		/// <param name="item">物品</param>
		/// <returns>物品显示项</returns>
		public D getItemDisplay(T item) {
			return subViews.Find((item_) => item_.getItem() == item);
		}
		IItemDisplay<T> IContainerDisplay<T>.getItemDisplay(T item) {
			return getItemDisplay(item);
		}

		#endregion

		#endregion

		#endregion

		#region 界面控制

		#region 滚动（仅对Canvas组件可用）

		/// <summary>
		/// 滚动到指定位置
		/// </summary>
		/// <param name="x">x位置</param>
		/// <param name="y">y位置</param>
		public void scrollTo(float x, float y) {
			if (rectContainer == null) return;
			rectContainer.anchoredPosition = new Vector2(x, y);
		}
		/// <param name="rt">RectTransform</param>
		public void scrollTo(RectTransform rt) {
			if (rectContainer == null) return;
			rectContainer.anchoredPosition = -rt.anchoredPosition;
		}
		/// <param name="index">项索引</param>
		public void scrollTo(int index) {
			if (rectContainer == null) return;
			if (index < 0 || index > subViewsCount()) return;
			var subView = subViews[index];
			var rt = subView.transform as RectTransform;
			if (rt) scrollTo(rt);
		}

		/// <summary>
		/// 指定项是否可视
		/// </summary>
		/// <param name="rt">RectTransform</param>
		/// <returns>返回指定项是否可视</returns>
		public bool isItemVisible(RectTransform rt) {
			if (mask == null) return true;
			if (rectContainer == null) return false;
			//Debug.Log("isItemVisible: " + name + ": " + rt);
			var rSize = rt.rect.size;
			var mSize = mask.rect.size;
			var rPos = rt.anchoredPosition;
			var cPos = rectContainer.anchoredPosition;
			//Debug.Log(name + ": cPos: " + cPos + ", rPos: " + rPos + 
			//    " , mSize: " + mSize + " , rSize: " + rSize);
			return (cPos.x + rPos.x >= 0 && cPos.x + rPos.x + rSize.x <= mSize.x &&
				cPos.y + rPos.y - rSize.y >= -mSize.y && cPos.y + rPos.y <= 0);
		}
		/// <param name="index">项索引</param>
		public bool isItemVisible(int index) {
			if (rectContainer == null) return false;
			if (index < 0 || index > subViewsCount()) return false;
			var subView = subViews[index];
			var rt = subView.transform as RectTransform;
			return isItemVisible(rt);
		}

		#endregion
		
		/// <summary>
		/// 绘制数目
		/// </summary>
		protected virtual void drawCount() {
			if (countText == null) return;
			countText.text = string.Format(countTextFormat(), itemsCount());
		}

		/// <summary>
		/// 清除数目
		/// </summary>
		void clearCount() {
			if (countText == null) return;
			countText.text = "";
		}

		#region 物品显示项绘制

		/// <summary>
		/// 创建物品显示组件
		/// </summary>
		void refreshItemDisplays() {
			/*
            if (gameObject.name == "Choices") {
                Debug.Log("Choices: itemsCount: " + itemsCount() + "\n" +
                    "itemDisplaysCount: " + itemDisplaysCount() + "\n" +
                    "maxItemDisplaysCount: " + maxItemDisplaysCount());
                Debug.Log("Choices: items: " + string.Join(",", items));
                Debug.Log("Choices: subViews: " + string.Join(",", subViews));
            }
            Debug.Log(name + ": refreshItemDisplays");
            Debug.Log(name + ": subViews: " + string.Join(",", subViews));
            Debug.Log(name + ": items: " + string.Join(",", items));

            Debug.Log(name + ": maxItemDisplaysCount: " + maxItemDisplaysCount());
            Debug.Log(name + ": itemDisplaysCount: " + itemDisplaysCount());
            Debug.Log(name + ": itemsCount: " + itemsCount());
            */

			createSubViews();
			destroyRedundantSubViews();
		}

		/// <summary>
		/// 创建子视图
		/// </summary>
		protected virtual void createSubViews() {
			for (int i = 0; i < maxItemDisplaysCount(); ++i) {
				T item = (i < itemsCount() ? items[i] : null);
				createSubView(item, i);
			}
		}

		/// <summary>
		/// 移除冗余子视图
		/// </summary>
		protected virtual void destroyRedundantSubViews() {
			for (int i = itemDisplaysCount() - 1; i >= maxItemDisplaysCount(); --i)
				destroySubView(i);
		}

		/// <summary>
		/// 创建物品显示组件
		/// </summary>
		/// <param name="item">物品</param>
		protected virtual void createSubView(T item, int index) {
			createSubView(index).activate(item);
		}

		/// <summary>
		/// ItemDisplay 创建回调
		/// </summary>
		protected override void onSubViewCreated(D sub, int index) {
			configureSubItem(sub, index);
			base.onSubViewCreated(sub, index);
		}

		/// <summary>
		/// 配置子项
		/// </summary>
		protected virtual void configureSubItem(D sub, int index) {
			sub.configure();
		}

		#endregion

		/// <summary>
		/// 刷新视窗
		/// </summary>
		protected override void refresh() {
			base.refresh();
			drawCount();
		}

		/// <summary>
		/// 清除视窗
		/// </summary>
		protected override void clear() {
			base.clear();
			clearCount();
			clearItems();
		}

		#endregion

	}

	/// <summary>
	/// 物品容器显示
	/// </summary>
	public class ContainerDisplay<T> : ContainerDisplay<ItemDisplay<T>, T>, 
		IContainerDisplay<T> where T : class { }
}