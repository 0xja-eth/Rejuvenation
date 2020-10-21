using System.Collections.Generic;

using UnityEngine;

using Core.UI.Utils;

using GameModule.Services;

using UI.Common.Controls.ItemDisplays;

namespace UI.MapSystem.Controls {

	using BattleSystem.Controls;

	/// <summary>
	/// 事件页处理类
	/// </summary>
	public class EventPageProcessor : ItemDisplay<MapEventPage> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public SpriteRenderer sprite;

		/// <summary>
		/// 碰撞的玩家
		/// </summary>
		[HideInInspector]
		public MapPlayer eventPlayer;

		/// <summary>
		/// 外部系统
		/// </summary>
		protected GameService gameSer;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			sprite = sprite ?? get<SpriteRenderer>();
		}

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
            processTrigger(MapEventPage.TriggerType.Always);
		}

		#endregion

		#region 事件控制
			
		#region 执行事件

		/// <summary>
		/// 处理触发
		/// </summary>
		/// <param name="player">触发相关的玩家</param>
		/// <param name="type">触发类型</param>
		/// <returns></returns>
		public void processTrigger(MapPlayer player, MapEventPage.TriggerType type) {
			eventPlayer = player; processTrigger(type);
		}
		public void processTrigger(MapEventPage.TriggerType type) {
			if (judgeTrigger(type)) processAction();
		}

		/// <summary>
		/// 判断是否触发
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		bool judgeTrigger(MapEventPage.TriggerType type) {
			return item?.triggerType == type;
		}

		/// <summary>
		/// 处理事件
		/// </summary>
		void processAction() {
			item?.invoke();
		}

		#endregion

		#endregion

		#region 界面刷新

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(MapEventPage item) {
			base.drawExactlyItem(item);
			if (sprite) sprite.sprite = item.picture;
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
			if (sprite) sprite.sprite = null;
		}

		#endregion
	}
}