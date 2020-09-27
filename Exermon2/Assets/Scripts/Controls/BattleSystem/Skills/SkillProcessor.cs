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
	/// 地图上的事件
	/// </summary>
	[RequireComponent(typeof(MapBattler))]
	[RequireComponent(typeof(AnimatorExtend))]
	public abstract class SkillProcessor : WorldComponent {// ItemDisplay<Skill> {

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public float rate = 10; // 权重

		public bool useCustomParams = false; // 是否使用自定义属性

		public Skill customSkill = null; // 自定义技能数据

		/// <summary>
		/// 敌人ID
		/// </summary>
		public abstract int skillId { get; }

		/// <summary>
		/// 敌人
		/// </summary>
		Skill skill_ = null;
		public Skill skill => useCustomParams && customSkill != null ? 
			customSkill : skill_ = skill_ ?? BaseData.poolGet<Skill>(skillId);

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		protected MapBattler battler;
		[RequireTarget]
		protected AnimatorExtend animator;

		#region 执行&使用

		/// <summary>
		/// 执行中
		/// </summary>
		/// <returns></returns>
		public virtual bool isRunning() {
			return false;
		}

		/// <summary>
		/// 当前状态能否使用
		/// </summary>
		/// <returns></returns>
		public virtual bool isUsable() {
			return true;
		}

		/// <summary>
		/// 使用技能
		/// </summary>
		/// <returns></returns>
		public void use() {
			beforeUse(); onUse(); afterUse();
		}

		/// <summary>
		/// 使用前回调
		/// </summary>
		protected virtual void beforeUse() { }

		/// <summary>
		/// 使用技能
		/// </summary>
		/// <returns></returns>
		protected abstract void onUse();

		/// <summary>
		/// 使用后回调
		/// </summary>
		protected virtual void afterUse() { }

		#endregion
	}
}