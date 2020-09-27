using System.Collections.Generic;

using UnityEngine;

using Core.Data;

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
		RuntimeAction currentAction;
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
			setupSkillProcessors();
		}

		/// <summary>
		/// 配置战斗者显示组件
		/// </summary>
		protected abstract void setupBattlerDisplay();

		/// <summary>
		/// 配置技能处理器
		/// </summary>
		void setupSkillProcessors() {
			skillProcessors.AddRange(gets<SkillProcessor>());
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
			return currentProcessor && currentProcessor.isRunning();
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
		/// 行动开始
		/// </summary>
		protected virtual void onActionStart() {
			runtimeBattler.onActionStart(currentAction);
		}

		/// <summary>
		/// 处理行动
		/// </summary>
		protected virtual void processAction() {
			useSkill(currentAction.skill);
		}

		#endregion
	}
}