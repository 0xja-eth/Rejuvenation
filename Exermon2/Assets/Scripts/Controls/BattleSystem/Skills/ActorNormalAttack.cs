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
	[RequireComponent(typeof(Collider2D))]
	public abstract class ActorNormalAttack : SkillProcessor {// ItemDisplay<Skill> {

		[RequireTarget]
		protected new Collider2D collider;


	}
}