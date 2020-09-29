using UnityEngine;

using UI.Common.Controls.AnimationSystem;
using UI.Common.Controls.ItemDisplays;

using BattleModule.Data;

namespace UI.Common.Controls.BattleSystem {

	/// <summary>
	/// 战斗者状态显示
	/// </summary>
	public class BattlerDisplay : ItemDisplay<RuntimeBattler> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public GameObject hpDisplay;
		public AnimationExtend hpBar;

		/// <summary>
		/// 碰撞的玩家
		/// </summary>

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
		}

		#endregion

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateHP();
		}

		/// <summary>
		/// 更新HP
		/// </summary>
		void updateHP() {
			if (!isNullItem(item)) drawHP(item);
		}

		#endregion

		#region 界面刷新

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(RuntimeBattler item) {
			base.drawExactlyItem(item);
			drawHP(item);
		}

		/// <summary>
		/// 绘制HP
		/// </summary>
		void drawHP(RuntimeBattler item) {
			if (hpDisplay) hpDisplay.SetActive(item.isDead());
			if (hpBar) {
				var scale = new Vector3(item.hpRate(), 1, 1);
				hpBar.scaleTo(scale, play: true);
			}
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
			if (hpDisplay) hpDisplay.SetActive(false);
		}

		#endregion
	}
}