using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Core.UI;

namespace UI.Common.Controls.ItemDisplays {

    /// <summary>
    /// 可选择物品容器接口
    /// </summary>
    public interface ISelectableContainerDisplay {
        
        /// <summary>
        /// 启动视窗
        /// </summary>
        void activate(int index = 0);

        /// <summary>
        /// 选择项
        /// </summary>
        void select(int index, bool force = false);

        /// <summary>
        /// 选中项
        /// </summary>
        void check(int index, bool force = false);
    }

    /// <summary>
    /// 可选择物品容器接口
    /// </summary>
    public interface ISelectableContainerDisplay<T> : ISelectableContainerDisplay, 
		IContainerDisplay<SelectableItemDisplay<T>, T> where T : class {

        /// <summary>
        /// 选择项
        /// </summary>
        void select(T item, bool force = false);
        
        /// <summary>
        /// 选中项
        /// </summary>
        void check(T item, bool force = false);
    }

    /// <summary>
    /// 物品容器显示
    /// </summary>
    public class SelectableContainerDisplay<T> :
		ContainerDisplay<SelectableItemDisplay<T>, T>, ISelectableContainerDisplay<T> where T : class {

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public int maxCheck = 0; // 最大选中数

		#region 变量设置

		/// <summary>
		/// 外部变量设置
		/// </summary>
		[SerializeField]
		bool _actived = true; // 是否可用
		public virtual bool actived {
			get { return _actived; }
			set {
				_actived = value;
				requestRefresh();
			}
		}
		[SerializeField]
		bool _selectable = true; // 能否选择
		public virtual bool selectable {
			get { return _selectable; }
			set {
				_selectable = value;
				requestRefresh();
			}
		}
		[SerializeField]
		bool _deselectable = true; // 能否取消选择
		public virtual bool deselectable {
			get { return _deselectable; }
			set {
				_deselectable = value;
				requestRefresh();
			}
		}
		[SerializeField]
		bool _checkable = true; // 能否选中
		public virtual bool checkable {
			get { return _checkable; }
			set {
				_checkable = value;
				requestRefresh();
			}
		}
		[SerializeField]
		bool _highlightable = true; // 能否高亮
		public virtual bool highlightable {
			get { return _highlightable; }
			set {
				_highlightable = value;
				requestRefresh();
			}
		}

		#endregion

		/// <summary>
		/// 回调函数集
		/// </summary>
        List<UnityAction> onSelectChangedCallbacks = new List<UnityAction>();
        List<UnityAction> onCheckChangedCallbacks = new List<UnityAction>();
        List<UnityAction<int>> onClickedCallbacks = new List<UnityAction<int>>();

        /// <summary>
        /// 内部变量声明
        /// </summary>
        protected List<int> checkedIndices = new List<int>(); // 已选中索引

        protected int selectedIndex = -1, lastIndex = -1; // 选择的索引, 上次索引
		
        #region 启动/结束控制

        /// <summary>
        /// 启动视窗
        /// </summary>
        public override void activate(int index) {
            base.activate();
            select(index);
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
		/// <param name="type">回调类型（0：物品变更，1：选择变更，2：选中变更）</param>
		public override void addCallback(UnityAction cb, int type = 0) {
            if (cb == null) return;
            switch (type) {
                case 1: onSelectChangedCallbacks.Add(cb); break;
				case 2: onCheckChangedCallbacks.Add(cb); break;
				default: base.addCallback(cb, type); break;
			}
		}

		/// <summary>
		/// 添加点击回调函数
		/// </summary>
		/// <param name="cb"></param>
		public void addClickedCallback(UnityAction<int> cb) {
			if (cb == null) return;
			onClickedCallbacks.Add(cb);
		}

		/// <summary>
		/// 物品变更回调
		/// </summary>
		protected override void onItemsChanged() {
			base.onItemsChanged();
			processForceCheckItems();
		}

		#endregion

		#region 数据控制

		#region 物品控制

		#region 数据判断

		/// <summary>
		/// 空物品是否有效
		/// </summary>
		/// <returns>空物品是否有效</returns>
		protected virtual bool isEmptyEnabled() {
            return includeEmpty();
        }

        /// <summary>
        /// 物品是否有效
        /// </summary>
        /// <param name="item">物品</param>
        /// <returns>返回物品是否有效</returns>
        public virtual bool isEnabled(T item) {
            if (item == null) return isEmptyEnabled();
            return true;
        }

        /// <summary>
        /// 当前物品是否有效
        /// </summary>
        public bool isCurrentEnabled() {
            return isEnabled(selectedItem());
        }

		#endregion

		#region 数据操作

		/// <summary>
		/// 设置物品集
		/// </summary>
		/// <param name="items">物品集</param>
		public override void setItems(T[] items) {
			deselect(); clearChecks();
			base.setItems(items);
		}

		/// <summary>
		/// 清空物品
		/// </summary>
		public override void clearItems() {
			deselect(); clearChecks();
			base.clearItems();
		}

		/// <summary>
		/// 准备转移
		/// </summary>
		/// <param name="item">物品</param>
		protected virtual T prepareTransfer(T item) {
			removeItem(item);
			return item;
		}

		/// <summary>
		/// 接受转移
		/// </summary>
		/// <param name="container">容器</param>
		/// <param name="item">物品</param>
		protected virtual void acceptTransfer(SelectableContainerDisplay<T> container, T item) {
			addItem(item);
		}

		/// <summary>
		/// 转移物品
		/// </summary>
		/// <param name="container">容器</param>
		/// <param name="item">物品</param>
		public void transferItem(SelectableContainerDisplay<T> container, T item) {
			if (!containsItem(item)) return;
			container.acceptTransfer(this, prepareTransfer(item));
		}
		public void transferItem<T1>(DroppableContainerDisplay<T1, T> container, T item) where T1 : class {
			if (!containsItem(item)) return;
			container.acceptTransfer(this, prepareTransfer(item));
		}
		public void transferItem<T1>(ISlotItemDisplay<T1, T> slotItem, T item) where T1 : class {
			if (!containsItem(item)) return;
			slotItem.setEquip(this, prepareTransfer(item));
		}

		#endregion

		#endregion

		#region 索引操作

		/// <summary>
		/// 获取循环索引
		/// </summary>
		/// <param name="i">索引</param>
		/// <returns>循环索引</returns>
		protected int getLoopedIndex(int i) {
			var cnt = itemDisplaysCount();
			if (cnt == 0) return -1;
			return (i % cnt + cnt) % cnt;
		}

		/// <summary>
		/// 获取限制索引
		/// </summary>
		/// <param name="i">索引</param>
		/// <returns>限制索引</returns>
		protected int getClampedIndex(int i) {
			var cnt = itemDisplaysCount();
			return Mathf.Clamp(i, 0, cnt - 1);
		}

		#endregion

		#region 选择控制

		/// <summary>
		/// 获取选择索引
		/// </summary>
		/// <returns>选择索引</returns>
		public int getSelectedIndex() {
            return selectedIndex;
        }

		/// <summary>
		/// 是否有选择
		/// </summary>
		/// <returns></returns>
		public bool isSelected() {
			return selectedIndex >= 0;
		}

        /// <summary>
        /// 获取选择项
        /// </summary>
        /// <returns>选择项</returns>
        public SelectableItemDisplay<T> selectedItemDisplay() {
            if (selectedIndex == -1) return null;
            return subViews[selectedIndex];
        }

        /// <summary>
        /// 获取选择物品
        /// </summary>
        /// <returns>物品</returns>
        public T selectedItem() {
            if (selectedIndex == -1) return null;
            return selectedItemDisplay().getItem();
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="index">索引</param>
        public virtual void select(T item, bool force = false) {
            select(items.IndexOf(item), force);
        }
        public virtual void select(int index, bool force = false) {
			debugLog("select: " + name + ": " + index);

            index = getLoopedIndex(index);
            if (index >= 0) {
                var item = subViews[index];
                if (!force && !item.isSelectable()) return;
            }

            lastIndex = selectedIndex = index;
            onSelectChanged();
        }

        /// <summary>
        /// 取消选择
        /// </summary>
        public virtual void deselect() {
            //if (selectedIndex < 0) return;
            debugLog("deselect: " + name);

            selectedIndex = -1;
            onSelectChanged();
        }

        /// <summary>
        /// 选择上次
        /// </summary>
        public virtual void selectLast(int default_ = 0, bool force = false) {
            debugLog("selectLast: " + name + ": " + selectedIndex + ", " + lastIndex);

            if (lastIndex >= 0) select(lastIndex, force);
            else if (default_ >= 0) select(default_, force);
            else deselect();
        }

        /// <summary>
        /// 选择改变事件回调
        /// </summary>
        protected virtual void onSelectChanged() {
            requestRefresh();
            updateItemHelp();
            callbackSelectChange();
        }

        /// <summary>
        /// 处理选择改变回调
        /// </summary>
        void callbackSelectChange() {
            foreach (var cb in onSelectChangedCallbacks) cb?.Invoke();
        }

        #endregion

        #region 选中控制

        /// <summary>
        /// 处理强制选中项
        /// </summary>
        void processForceCheckItems() {
            for (int i = 0; i < itemDisplaysCount(); i++)
                if (subViews[i].isForceChecked()) check(i);
        }

        /// <summary>
        /// 最大选中数量
        /// </summary>
        /// <returns>最大选中数量</returns>
        public virtual int maxCheckCount() {
            return maxCheck;
        }

        /// <summary>
        /// 获取选中索引数组
        /// </summary>
        /// <returns>选中索引数组</returns>
        public int[] getCheckedIndices() {
            return checkedIndices.ToArray();
        }

        /// <summary>
        /// 是否选中某项
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>是否选中</returns>
        public bool isChecked(int index) {
            return checkedIndices.Contains(index);
        }

        /// <summary>
        /// 获取选中项
        /// </summary>
        /// <returns>选中项数组</returns>
        public SelectableItemDisplay<T>[] getCheckedItemDisplays() {
            var cnt = checkedIndices.Count;
            var items = new SelectableItemDisplay<T>[cnt];
            for (int i = 0; i < cnt; ++i) {
                var index = checkedIndices[i];
                items[i] = subViews[index];
            }
            return items;
        }

        /// <summary>
        /// 获取选中物品数组
        /// </summary>
        /// <returns>选中项数组</returns>
        public T[] getCheckedItems() {
            var cnt = checkedIndices.Count;
            var items = new T[cnt];
            for (int i = 0; i < cnt; ++i) {
                var index = checkedIndices[i];
                items[i] = this.items[index];
            }
            return items;
        }

        /// <summary>
        /// 选中
        /// </summary>
        /// <param name="index">索引</param>
        public virtual void check(T item, bool force = false) {
            check(items.IndexOf(item), force);
        }
        public virtual void check(int index, bool force = false) {
            //Debug.Log("check: " + index);

            index = getLoopedIndex(index);

            var item = subViews[index];
            if (!force && !item.isCheckable()) return;
            if (isChecked(index)) return;

            var cnt = checkedIndices.Count;
            //Debug.Log("cnt: " + cnt + ", max: " + maxCheckCount());
            if (maxCheckCount() > 0 && cnt >= maxCheckCount())
                if (!removeFirstCheckIndex())
                    return; // 如果不能移除，则不继续执行
            checkedIndices.Add(index);
            onCheckChanged();
        }

        /// <summary>
        /// 取消第一个选中项
        /// </summary>
        /// <returns>能否执行操作</returns>
        bool removeFirstCheckIndex() {
            for (int i = 0; i < checkedIndices.Count; ++i) {
                var index = checkedIndices[i];
                if (subViews[index].isUncheckable()) {
                    checkedIndices.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 取消选中
        /// </summary>
        /// <param name="index">索引</param>
        public virtual void uncheck(T item, bool force = false) {
            uncheck(items.IndexOf(item), force);
        }
        public virtual void uncheck(int index, bool force = false) {
            //Debug.Log("uncheck: " + index);

            index = getLoopedIndex(index);

            var item = subViews[index];
            if (!force && !item.isUncheckable()) return;
            if (!isChecked(index)) return;

            checkedIndices.Remove(index);
            onCheckChanged();
        }

        /// <summary>
        /// 反转选中
        /// </summary>
        /// <param name="index">索引</param>
        public virtual void toggle(T item, bool force = false) {
            toggle(items.IndexOf(item), force);
        }
        public void toggle(int index, bool force = false) {
            //Debug.Log("toggle: " + index);
            //Debug.Log("Checked: " + string.Join(",", checkedIndices));

            index = getLoopedIndex(index);
            if (isChecked(index)) uncheck(index, force);
            else check(index, force);
        }

        /// <summary>
        /// 清空选中
        /// </summary>
        public void clearChecks() {
            checkedIndices.Clear();
            onCheckChanged();
        }

        /// <summary>
        /// 选中改变事件回调
        /// </summary>
        protected virtual void onCheckChanged() {
            Debug.Log("onCheckChanged: " + name + ": " + 
                string.Join(",", checkedIndices));
            requestRefresh();
            callbackCheckChange();
        }

        /// <summary>
        /// 处理物品改变回调
        /// </summary>
        void callbackCheckChange() {
            foreach (var cb in onCheckChangedCallbacks) cb?.Invoke();
        }

		#endregion

		#region 点击控制

		/// <summary>
		/// 点击回调
		/// </summary>
		/// <param name="index"></param>
		public virtual void onClick(int index) {
			Debug.Log("onClick: " + name + ": " + index);
			callbackClicked(index);
		}

		/// <summary>
		/// 处理点击发送回调
		/// </summary>
		void callbackClicked(int index) {
			foreach (var cb in onClickedCallbacks) cb?.Invoke(index);
		}

		#endregion

		#endregion

		#region 界面控制

        /// <summary>
        /// 绘制数目
        /// </summary>
        protected override void drawCount() {
            if (countText == null) return;
            countText.text = string.Format(countTextFormat(), 
				itemsCount(), selectedIndex + 1);
        }

        #region 物品显示项绘制
		
		/// <summary>
		/// 配置子项
		/// </summary>
		protected override void configureSubItem(SelectableItemDisplay<T> sub, int index) {
            sub.configure(this, index);
        }

		#endregion

		#region 物品帮助绘制

		/// <summary>
		/// 绘制物品帮助
		/// </summary>
		public void updateItemHelp() {
			var item = selectedItem();
			if (item == null) drawEmptyHelp();
			else drawExactlyItemHelp(item);
		}

		/// <summary>
		/// 绘制空的帮助
		/// </summary>
		protected virtual void drawEmptyHelp() {
			itemDetail?.clearItem();
		}

		/// <summary>
		/// 绘制实际物品帮助
		/// </summary>
		/// <param name="item">物品</param>
		protected virtual void drawExactlyItemHelp(T item) {
			itemDetail?.activate(item, selectedIndex);
		}

		/// <summary>
		/// 清除物品帮助
		/// </summary>
		protected virtual void clearItemHelp() {
			itemDetail?.requestClear();
		}

		#endregion

		/// <summary>
		/// 清除视窗
		/// </summary>
		protected override void clear() {
			base.clear();
			clearItemHelp();
		}

		#endregion

	}
}