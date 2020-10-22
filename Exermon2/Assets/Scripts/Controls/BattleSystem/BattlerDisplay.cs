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

		public float aniRate = 0;

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

			if (isNullItem(item)) return;

			updateHPChange();
			updateAnimation();
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
		void updateAnimation() {
			drawAnimation(item);
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
			drawHP(item); drawAnimation(item);
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
		/// 绘制动画
		/// </summary>
		/// <param name="item"></param>
		void drawAnimation(RuntimeBattler item) {
			var dir = item.direction;

			if (RuntimeCharacter.isDir4(dir))
				lastDir = RuntimeCharacter.dir2Index(dir);

			switch ((RuntimeBattler.State)item.state) {
				case RuntimeBattler.State.Idle:
				case RuntimeBattler.State.Moving:
					drawCharacter(item); break;
				case RuntimeBattler.State.Using:
					drawAttackAction(item); break;
				default:
					drawCharacter(item); break;
			}
		}

		/// <summary>
		/// 绘制角色
		/// </summary>
		/// <param name="item"></param>
		void drawCharacter(RuntimeBattler item) {
			aniRate = 0;
			sprite.flipX = false;
			sprite.sprite = item.battler.
				getSprite(lastDir, item.pattern);
		}

		/// <summary>
		/// 绘制攻击动画
		/// </summary>
		/// <param name="item"></param>
		void drawAttackAction(RuntimeBattler item) {
			sprite.flipX = RuntimeCharacter.isLeftDir(item.direction);
			sprite.sprite = item.battler.
				getAttackAni(lastDir, aniRate);
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