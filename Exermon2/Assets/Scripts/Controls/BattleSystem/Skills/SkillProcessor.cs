using System.Collections.Generic;

using UnityEngine;

using Core.Data;
using Core.UI;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.AnimationSystem;

using BattleModule.Data;

namespace UI.Common.Controls.MapSystem {

	using BattleSystem;

	/// <summary>
	/// 技能处理器
	/// </summary>
	public abstract class SkillProcessor : WorldComponent { // ItemDisplay<Skill> {

		/// <summary>
		/// 字符串常量定义
		/// </summary>
		const string SkillStateName = "Skill";
		const string SkillAttrName = "skill";

		const string TargetStateName = "Target";
		const string TargetAttrName = "target";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public new Collider2D collider;

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public float rate = 10; // 权重

		public bool useCustomParams = false; // 是否使用自定义属性

		public Skill customSkill = null; // 自定义技能数据

		/// <summary>
		/// 技能ID
		/// </summary>
		public virtual int skillId => 0;

		/// <summary>
		/// 敌人
		/// </summary>
		Skill skill_ = null;
		public Skill skill => useCustomParams && customSkill != null ? 
			customSkill : skill_ = skill_ ?? BaseData.poolGet<Skill>(skillId);

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[HideInInspector]
		public MapBattler battler;
		protected AnimatorExtend animator;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		RuntimeAction currentAction => battler.currentAction;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			initializeBattler();
		}

		/// <summary>
		/// 初始化战斗者
		/// </summary>
		void initializeBattler() {
			battler = findParent<MapBattler>();
			animator = battler.animator;

			battler.addSkillProcessor(this);
		}

		#endregion

		#region 使用

		/// <summary>
		/// 使用中
		/// </summary>
		/// <returns></returns>
		public virtual bool isUsing() {
			return false;
		}

		/// <summary>
		/// 当前状态能否使用
		/// </summary>
		/// <returns></returns>
		public virtual bool isUsable() {
			return !isUsing();
		}

		/// <summary>
		/// 使用技能
		/// </summary>
		/// <returns></returns>
		public bool use() {
			if (!isUsable()) return false;
			beforeUse(); onUse(); afterUse();
			return true;
		}

		/// <summary>
		/// 使用前回调
		/// </summary>
		protected virtual void beforeUse() { }

		/// <summary>
		/// 使用技能
		/// </summary>
		/// <returns></returns>
		protected virtual void onUse() {
			playUseAnimation();
		}

		/// <summary>
		/// 播放使用动画
		/// </summary>
		void playUseAnimation() {
			var animation = skill.startAnimation();
			animator?.changeAni(SkillStateName, animation);
			animator?.setVar(SkillAttrName);
		}

		/// <summary>
		/// 使用后回调
		/// </summary>
		protected virtual void afterUse() { }

		#endregion

		#region 作用

		/// <summary>
		/// 作用到指定Battler
		/// </summary>
		/// <param name="battler"></param>
		public virtual void apply(MapBattler battler) {
			applyRuntimeBattler(battler.runtimeBattler);
			applyMapBattler(battler);
		}

		/// <summary>
		/// 应用到运行时数据
		/// </summary>
		/// <param name="battler"></param>
		void applyRuntimeBattler(RuntimeBattler battler) {
			var res = currentAction.makeResult(battler);
			battler.applyResult(res);
		}

		/// <summary>
		/// 应用到运行时数据
		/// </summary>
		/// <param name="battler"></param>
		protected virtual void applyMapBattler(MapBattler battler) {

			var animation = skill.targetAnimation();
			var animator = battler.animator;

			animator?.changeAni(TargetStateName, animation);
			animator?.setVar(TargetAttrName);
		}

		#endregion
	}
}