
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
		
		#region 界面刷新

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(RuntimeBattler item) {
			base.drawExactlyItem(item);

		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();

		}

		#endregion
	}
}