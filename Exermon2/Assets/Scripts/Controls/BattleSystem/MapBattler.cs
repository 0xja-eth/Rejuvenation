using System.Collections.Generic;

using UnityEngine;

using Core.UI.Utils;

using MapModule.Data;

using BattleModule.Data;

namespace UI.Common.Controls.BattleSystem {

	using MapSystem;

	/// <summary>
	/// 地图上的战斗者实体
	/// </summary>
	[RequireComponent(typeof(BattlerDisplay))]
	public abstract class MapBattler : MapCharacter {

		/// <summary>
		/// 字符串常量定义
		/// </summary>
		protected const string SkillStateName = "Skill";
		protected const string SkillAttrName = "skill";

		protected const string TargetStateName = "Target";
		protected const string TargetAttrName = "target";

		protected const string FreezeStateName = "Freeze";
		protected const string FreezeAttrName = "freezing";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public List<SkillProcessor> skillProcessors = new List<SkillProcessor>(); // 技能

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		protected BattlerDisplay display;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		[HideInInspector]
		public RuntimeAction currentAction;
		[HideInInspector]
		public SkillProcessor currentProcessor;

		protected bool freezing = false;

		/// <summary>
		/// 运行时战斗者
		/// </summary>
		public RuntimeBattler runtimeBattler => display.getItem();
		public override RuntimeCharacter runtimeCharacter => runtimeBattler;

		#region 初始化

		/// <summary>
		/// 初始化碰撞函数
		/// </summary>
		protected override void initializeCollFuncs() {
			base.initializeCollFuncs();
			registerOnEnterFunc<SkillProcessor>(onSkillHit);
		}

		/// <summary>
		/// 开始
		/// </summary>
		protected override void start() {
			setupBattlerDisplay();
			base.start();
		}

		/// <summary>
		/// 配置战斗者显示组件
		/// </summary>
		protected abstract void setupBattlerDisplay();

		/// <summary>
		/// 配置更新函数
		/// </summary>
		protected override void setupStateChanges() {
			base.setupStateChanges();

			//runtimeBattler.addStateDict(
			//	RuntimeBattler.BattlerState.Idle, updateIdle);
			//runtimeBattler.addStateDict(
			//	RuntimeBattler.BattlerState.Moving, updateMoving);
			//runtimeBattler.addStateDict(
			//	RuntimeBattler.BattlerState.Using, updateUsing);
			//runtimeBattler.addStateDict(
			//	RuntimeBattler.BattlerState.Hitting, updateHitting);
			//runtimeBattler.addStateDict(
			//	RuntimeBattler.BattlerState.Freezing, updateFreezing);
		}

		#endregion

		#region 更新

		///// <summary>
		///// 更新
		///// </summary>
		//protected override void update() {
		//	base.update();
		//	if (!isValid()) return;
		//	updateBattler();
		//}

		/// <summary>
		/// 更新具体状态
		/// </summary>
		protected virtual void updateIdle() {
			if (isMoving()) changeState(RuntimeBattler.BattlerState.Moving);
			else updateAction();
		}

		/// <summary>
		/// 更新移动
		/// </summary>
		sealed protected override void updateMoving() {
			if (isState(RuntimeBattler.BattlerState.Moving))
				base.updateMoving();
		}

		/// <summary>
		/// 更新使用
		/// </summary>
		protected virtual void updateUsing() {
			updateActing();
		}

		/// <summary>
		/// 更新受击
		/// </summary>
		protected virtual void updateHitting() {
			checkFreezeStart();
		}

		/// <summary>
		/// 更新硬直
		/// </summary>
		protected virtual void updateFreezing() {
			checkFreezeEnd();
		}

		/// <summary>
		/// 开始移动回调
		/// </summary>
		protected override void onMoveStart() {
			base.onMoveStart();
			changeState(RuntimeBattler.BattlerState.Moving);
		}

		/// <summary>
		/// 结束移动回调
		/// </summary>
		protected override void onMoveEnd() {
			base.onMoveEnd();
			changeState(RuntimeBattler.BattlerState.Idle);
		}

		/// <summary>
		/// 检查是否开始硬直
		/// </summary>
		/// <returns></returns>
		void checkFreezeStart() {
			if (!freezing && runtimeBattler.
				isHittingOrFreezing()) onFreezeStart();
		}

		/// <summary>
		/// 检查是否结束硬直
		/// </summary>
		/// <returns></returns>
		void checkFreezeEnd() {
			if (freezing && !runtimeBattler.
				isHittingOrFreezing()) onFreezeEnd();
		}

		/// <summary>
		/// 开始硬直回调
		/// </summary>
		protected virtual void onFreezeStart() {
			animator?.setVar(FreezeAttrName, freezing = true);
		}

		/// <summary>
		/// 结束硬直回调
		/// </summary>
		protected virtual void onFreezeEnd() {
			animator.setVar(FreezeAttrName, freezing = false);
		}

		#endregion

		#region 碰撞处理

		#endregion

		#region 状态判断

		/// <summary>
		/// 状态判断
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public bool isState(RuntimeBattler.BattlerState state) {
			return runtimeBattler.isState(state);
		}

		/// <summary>
		/// 状态改变
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public void changeState(RuntimeBattler.BattlerState state) {
			runtimeBattler.changeState(state);
		}

		/// <summary>
		/// 是否可用
		/// </summary>
		/// <returns></returns>
		public bool isValid() {
			return runtimeBattler != null;
		}

		/// <summary>
		/// 是否行动中
		/// </summary>
		/// <returns></returns>
		public bool isActing() {
			return currentProcessor && currentProcessor.isUsing;
		}

		/// <summary>
		/// 能否移动
		/// </summary>
		/// <returns></returns>
		public override bool isMoveable() {
			return base.isMoveable() && !isActing();
		}

		/// <summary>
		/// 是否空闲
		/// </summary>
		/// <returns></returns>
		public override bool isIdle() {
			return base.isIdle() && !isActing() && 
				!runtimeBattler.isFreezing();
		}

		#endregion

		#region 移动控制

		/// <summary>
		/// 移动速度
		/// </summary>
		/// <returns></returns>
		public override float moveSpeed() {
            return runtimeBattler.speed;
		}

		#endregion

		#region 动画控制

		/// <summary>
		/// 是否处于某动画状态
		/// </summary>
		/// <returns></returns>
		public bool isPlayingSkillAnimation() {
			return isAnimationState(SkillStateName);
		}
		public bool isPlayingTargetAnimation() {
			return isAnimationState(TargetStateName);
		}

		/// <summary>
		/// 切换并播放动画
		/// </summary>
		/// <param name="animation">动画</param>
		public void playSkillAnimation(AnimationClip animation) {
			playAnimation(SkillStateName, SkillAttrName, animation);
		}
		public void playTargetAnimation(AnimationClip animation) {
			playAnimation(TargetStateName, TargetAttrName, animation);
		}

		#endregion

		#region 技能控制

		/// <summary>
		/// 添加技能处理器
		/// </summary>
		/// <param name="processor"></param>
		public void addSkillProcessor(SkillProcessor processor) {
			if (!skillProcessors.Contains(processor))
				skillProcessors.Add(processor);
		}

		/// <summary>
		/// 获取指定技能的处理器
		/// </summary>
		/// <param name="skill"></param>
		/// <returns></returns>
		public SkillProcessor skillProcessor(Skill skill) {
			return skillProcessors.Find(p => p.skill == skill);
		}
		public SkillProcessor skillProcessor(int skillId) {
			return skillProcessors.Find(p => p.skillId == skillId);
		}

		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="skill"></param>
		void useSkill(Skill skill) {
			currentProcessor = skillProcessor(skill);
			currentProcessor?.use();
		}

		/// <summary>
		/// 技能命中回调
		/// </summary>
		/// <param name="skill"></param>
		void onSkillHit(SkillProcessor skill) {
			if (skill.battler == this) return;
			skill.apply(this);
		}

		#endregion

		#region 行动控制

		/// <summary>
		/// 更新行动
		/// </summary>
		void updateAction() {
			startAction(runtimeBattler.currentAction());
		}

		/// <summary>
		/// 更新技能
		/// </summary>
		void updateActing() {
			checkActionEnd();
		}

		/// <summary>
		/// 检查是否行动结束
		/// </summary>
		/// <returns></returns>
		void checkActionEnd() {
			if (!isPlayingSkillAnimation()
				&& isActing()) onActionEnd();
		}

		/// <summary>
		/// 开始行动
		/// </summary>
		/// <param name="action"></param>
		public void startAction(RuntimeAction action) {
			if ((currentAction = action) == null) return;
			onActionStart();
			processAction();
		}

		/// <summary>
		/// 处理行动
		/// </summary>
		protected virtual void processAction() {
			useSkill(currentAction.skill);
		}

		/// <summary>
		/// 行动开始
		/// </summary>
		protected virtual void onActionStart() {
			runtimeBattler.onActionStart(currentAction);
		}

		/// <summary>
		/// 行动开始
		/// </summary>
		protected virtual void onActionEnd() {
  			runtimeBattler.onActionEnd(currentAction);
			currentProcessor?.onUseEnd();
			currentProcessor = null;
		}

		#endregion

	}
}