using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

using Core.Data;
using Core.UI;

using BattleModule.Data;

namespace UI.Common.Controls.MapSystem {

	using BattleSystem;
	using SystemExtend.PhysicsExtend;

	/// <summary>
	/// 技能处理器
	/// </summary>
	public abstract class SkillProcessor : Collider2DExtend {//, ISkillApplication { 

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
		/// 技能
		/// </summary>
		Skill skill_ = null;
		public Skill skill => useCustomParams && customSkill != null ? 
			customSkill : skill_ = skill_ ?? BaseData.poolGet<Skill>(skillId);

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[HideInInspector]
		public MapBattler battler;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		[HideInInspector]
		public RuntimeSkill runtimeSkill = null;

		RuntimeBattler runtimeBattler => battler.runtimeBattler;

		RuntimeAction currentAction => battler.currentAction;

		/// <summary>
		/// 属性
		/// </summary>
		public bool isStarted { get; protected set; } = false;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			initializeRuntimeSkill();
			initializeBattler();
		}

		/// <summary>
		/// 初始化碰撞函数
		/// </summary>
		protected override void initializeCollFuncs() {
			registerOnStayFunc<Tilemap>(t => apply(t));
			registerOnStayFunc<MapEntity>(e => apply(e));
		}

		/// <summary>
		/// 初始化运行时技能
		/// </summary>
		void initializeRuntimeSkill() {
			runtimeSkill = new RuntimeSkill(skill, rate, isUsable);
		}

		/// <summary>
		/// 初始化战斗者
		/// </summary>
		void initializeBattler() {
			battler = findParent<MapBattler>();
			battler?.addSkillProcessor(this);
		}

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateCollider();
		}

		/// <summary>
		/// 更新碰撞体
		/// </summary>
		void updateCollider() {
			if (collider) collider.enabled = isStarted;
		}

		#endregion

		#region 使用

		/// <summary>
		/// 当前状态能否使用
		/// </summary>
		/// <returns></returns>
		public virtual bool isUsable() {
			return !isStarted;
		}

		/// <summary>
		/// 是否已结束
		/// </summary>
		/// <returns></returns>
		public virtual bool isTerminated() {
			return isStarted && !battler.isPlayingSkillAnimation();
		}

		/// <summary>
		/// 使用技能
		/// </summary>
		/// <returns></returns>
		public bool use() {
			if (!isUsable()) return false;
			onUseStart(); onUse();
			return true;
		}

		/// <summary>
		/// 使用前回调
		/// </summary>
		protected virtual void onUseStart() {
			debugLog("On skill start: " + skill);
			isStarted = true;
		}

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
			battler.playSkillAnimation(skill.startAnimation());
		}

		/// <summary>
		/// 使用后回调
		/// </summary>
		public virtual void onUseEnd() {
			debugLog("On skill end: " + skill);
			if (collider) collider.enabled = true;
			isStarted = false;
		}

		#endregion

		#region 作用

		/// <summary>
		/// 是否有效
		/// </summary>
		/// <returns></returns>
		public virtual bool isApplyValid() {
			return isStarted;
		}

		/// <summary>
		/// 是否为技能目标
		/// </summary>
		protected virtual bool isTarget(MapBattler battler) {
			return this.battler.opponents().Contains(battler);
		}

		/// <summary>
		/// 作用到TileMap
		/// </summary>
		public virtual bool apply(Tilemap map) { return true; }

		/// <summary>
		/// 作用到MapEntity
		/// </summary>
		public virtual bool apply(MapEntity entity) {
			if (!entity.isApplyable() || !isApplyValid())
				return false;

			var battler = entity as MapBattler;
			if (battler != null) return applyBattler(battler);

			return applyEntity(entity);
		}

		/// <summary>
		/// 作用到MapEntity（实际）
		/// </summary>
		protected virtual bool applyEntity(MapEntity entity) { return true; }

		/// <summary>
		/// 作用到MapBattler
		/// </summary>
		/// <param name="battler"></param>
		/// <returns></returns>
		protected virtual bool applyBattler(MapBattler battler) {
			if (!isTarget(battler)) return false;

			applyRuntimeBattler(battler.runtimeBattler);
			applyMapBattler(battler);

			return true;
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
			battler.playTargetAnimation(skill.targetAnimation());
		}

		#endregion
	}
}