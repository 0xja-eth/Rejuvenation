
using UnityEngine;
using UnityEngine.Events;

using Core.UI;

namespace UI.MapSystem.Controls {

	using BattleSystem.Controls;

	/// <summary>
	/// 地图上的事件
	/// </summary>
	public class MapEventPage : GeneralComponent {

		/// <summary>
		/// 触发类型
		/// </summary>
		public enum TriggerType {

			Never, // 不会触发

			CollEnter, // 碰撞开始
			CollStay, // 碰撞盒内持续
			CollSearch, // 碰撞盒内按下搜索键
			CollExit, // 碰撞结束

			Always, // 总是执行
		}

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public TriggerType triggerType = TriggerType.Never; // 触发类型
		public UnityEvent actions = new UnityEvent(); // 事件

		/// <summary>
		/// 显示的图片
		/// </summary>
		[SerializeField]
		Sprite _picture = null;
		public Sprite picture {
			get => _picture;
			set {
				if (_picture == value) return;
				_picture = value; requestRefresh();
			}
		} 

		public bool invokeOnce = false; // 只触发一次

		/// <summary>
		/// 内部组件定义
		/// </summary>
		protected MapEvent mapEvent;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		protected bool isInvoked = false;

		/// <summary>
		/// 玩家
		/// </summary>
		public MapPlayer mapPlayer => mapEvent?.map?.player;
		public MapPlayer eventPlayer => mapEvent?.eventPlayer;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			initializeEvent();
		}

		/// <summary>
		/// 初始化事件
		/// </summary>
		void initializeEvent() {
			mapEvent = findParent<MapEvent>();
			mapEvent?.addEventPage(this);
		}

		#endregion

		#region 事件调用

		/// <summary>
		/// 是否有效
		/// </summary>
		/// <returns></returns>
		public virtual bool isValid() {
			return triggerType != TriggerType.Never 
				&& (!isInvoked || !invokeOnce);
		}

		/// <summary>
		/// 是否为当前页
		/// </summary>
		/// <returns></returns>
		public bool isCurrent() {
			return mapEvent.currentPage() == this;
		}
		
		/// <summary>
		/// 调用
		/// </summary>
		public void invoke() {
			isInvoked = true;

			invokeActions();
			invokeCustom();
		}

		/// <summary>
		/// 调用预设动作
		/// </summary>
		void invokeActions() {
			actions?.Invoke();
		}

		/// <summary>
		/// 自定义调用
		/// </summary>
		protected virtual void invokeCustom() { }

		#endregion

		#region 刷新

		/// <summary>
		/// 刷新事件页
		/// </summary>
		void refreshEventPage() {
			if (isCurrent()) mapEvent.processor.requestRefresh(true);
		}

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			refreshEventPage();
		}

		#endregion
	}
}