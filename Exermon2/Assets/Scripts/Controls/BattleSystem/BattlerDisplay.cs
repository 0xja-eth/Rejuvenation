using UnityEngine;

using UI.Common.Controls.AnimationSystem;
using UI.Common.Controls.ItemDisplays;

using MapModule.Data;
using BattleModule.Data;

namespace UI.BattleSystem.Controls {

	/// <summary>
	/// 战斗者状态显示
	/// </summary>
	//[RequireComponent(typeof(SpriteRenderer))]
	public class BattlerDisplay : ItemDisplay<RuntimeBattler> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public SpriteRenderer sprite;

		public GameObject hpDisplay;
		public AnimationExtend hpBar;

		public TextMesh stateText;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		int lastDir = 0;

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
			debugLog("Direction: " + item.direction);

			if (isNullItem(item)) return;
			updateHPChange(); updateCharacter();
			updateState();
		}

		/// <summary>
		/// 更新HP改变
		/// </summary>
		void updateHPChange() {
			var deltaHP = item.deltaHP;
			if (deltaHP == null) return;

			drawHP(item, true);
		}

		/// <summary>
		/// 更新行走图
		/// </summary>
		void updateCharacter() {
			drawCharacter(item);
		}

		/// <summary>
		/// 更新状态
		/// </summary>
		void updateState() {
			drawState(item);
		}

		#endregion

		#region 数据操作

		/// <summary>
		/// 是否为空
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool isNullItem(RuntimeBattler item) {
			return base.isNullItem(item) || item.battler == null;
		}

		#endregion

		#region 界面刷新

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(RuntimeBattler item) {
			base.drawExactlyItem(item);
			drawHP(item); drawCharacter(item);
			drawState(item);
		}

		/// <summary>
		/// 绘制HP
		/// </summary>
		void drawHP(RuntimeBattler item, bool ani = false) {
			if (hpDisplay)
				hpDisplay.SetActive(!item.isDead());
			if (hpBar) {
				var scale = new Vector3(item.hpRate(), 1, 1);
				if (ani) hpBar.scaleTo(scale, play: true);
				else hpBar.transform.localScale = scale;
			}
		}

		/// <summary>
		/// 绘制角色
		/// </summary>
		/// <param name="item"></param>
		void drawCharacter(RuntimeBattler item) {
			var dir = item.direction;

			if (RuntimeCharacter.isDir4(dir))
				lastDir = RuntimeCharacter.dir2Index(dir);

			if (RuntimeCharacter.isRightDir(dir))
				sprite.flipX = true;
			else if (RuntimeCharacter.isLeftDir(dir))
				sprite.flipX = false;

			sprite.sprite = item.battler.
				getSprite(lastDir, item.pattern);
		}

		/// <summary>
		/// 绘制状态
		/// </summary>
		/// <param name="item"></param>
		void drawState(RuntimeBattler item) {
			if (stateText) stateText.text = ((RuntimeBattler.State)item.state).ToString();
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
			if (hpDisplay) hpDisplay.SetActive(false);
			sprite.sprite = null;

			if (stateText) stateText.text = "";
		}

		#endregion
	}
}