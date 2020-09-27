using System.Collections.Generic;

using UnityEngine;

using Core.UI.Utils;

using BattleModule.Data;

namespace UI.Common.Controls.BattleSystem {

	using MapSystem;

	/// <summary>
	/// 地图上的战斗者实体
	/// </summary>
	[RequireComponent(typeof(BattlerDisplay))]
	public abstract class MapBattler : MapCharacter {

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
		SkillProcessor currentProcessor;

		/// <summary>
		/// 运行时敌人
		/// </summary>
		public RuntimeBattler runtimeBattler => display.getItem();

		#region 初始化

		/// <summary>
		/// 开始
		/// </summary>
		protected override void start() {
			base.start();
			setupBattlerDisplay();
		}

		/// <summary>
		/// 配置战斗者显示组件
		/// </summary>
		protected abstract void setupBattlerDisplay();

		/// <summary>
		/// 初始化碰撞函数
		/// </summary>
		protected override void initializeCollFuncs() {
			base.initializeCollFuncs();
			registerOnEnterFunc<SkillProcessor>(onSkillHit);
		}

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			if (isValid()) updateAction();
		}

		#endregion

		#region 碰撞处理

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

		#region 状态判断

		/// <summary>
		/// 能否移动
		/// </summary>
		/// <returns></returns>
		public override bool isMoveable() {
			return base.isMoveable() && !isActing();
		}
		
		#endregion

		#region 行动控制

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
			return currentProcessor && currentProcessor.isUsing();
		}

		/// <summary>
		/// 更新行动
		/// </summary>
		void updateAction() {
			if (isActing()) return;
			startAction(runtimeBattler.currentAction());
		}

		/// <summary>
		/// 开始行动
		/// </summary>
		/// <param name="action"></param>
		public void startAction(RuntimeAction action) {
			currentAction = action;
			if (currentAction == null) return;
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
			runtimeBattler.onActionStart(currentAction);
		}

		#endregion

	}
}