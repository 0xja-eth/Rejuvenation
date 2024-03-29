﻿using System.Collections.Generic;

using UnityEngine;

using Core.UI.Utils;

using MapModule.Data;

using BattleModule.Data;

namespace UI.BattleSystem.Controls {

	using MapSystem.Controls;
	using UnityEditor;

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

		protected const string FreezeStateName = "Frozen";
		protected const string FreezeAttrName = "freezing";

		protected const string DeadStateName = "Dead";
		protected const string DeadAttrName = "dead";

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

		//protected bool freezing = false;

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
		}

		/// <summary>
		/// 开始
		/// </summary>
		protected override void start() {
			setupBattlerDisplay();
			setupSkills();
			base.start();
		}

		/// <summary>
		/// 配置战斗者显示组件
		/// </summary>
		protected abstract void setupBattlerDisplay();

		/// <summary>
		/// 配置技能
		/// </summary>
		void setupSkills() {
			foreach(var processor in skillProcessors)
				runtimeBattler.addSkill(processor.runtimeSkill);
		}

		/// <summary>
		/// 配置更新函数
		/// </summary>
		protected override void configureStateChanges() {
			base.configureStateChanges();

            runtimeBattler?.addStateDict(
                RuntimeBattler.State.Idle, updateIdle);
			runtimeBattler?.addStateDict(
				RuntimeBattler.State.Moving, updateMoving);
			runtimeBattler?.addStateDict(
                RuntimeBattler.State.Using, updateUsing);

            runtimeBattler?.addStateEnter(
                RuntimeBattler.State.Hitting, onHit);

            // TODO: 需要注意 Using 转 Hitting 时候的技能取消

            runtimeBattler?.addStateExit(
                RuntimeBattler.State.Freezing, onFreezeEnd);

            runtimeBattler?.addStateEnter(
                RuntimeBattler.State.Dead, onDie);
		}

		#endregion

		#region 释放资源

		/// <summary>
		/// 销毁回调
		/// </summary>
		protected override void onDestroy() {
			base.onDestroy();

			runtimeBattler?.removeStateDict(
				RuntimeBattler.State.Idle, updateIdle);
			runtimeBattler?.removeStateDict(
				RuntimeBattler.State.Moving, updateMoving);
			runtimeBattler?.removeStateDict(
				RuntimeBattler.State.Using, updateUsing);

			runtimeBattler?.removeStateEnter(
				RuntimeBattler.State.Hitting, onHit);

			// TODO: 需要注意 Using 转 Hitting 时候的技能取消

			runtimeBattler?.removeStateExit(
				RuntimeBattler.State.Freezing, onFreezeEnd);

			runtimeBattler?.removeStateEnter(
				RuntimeBattler.State.Dead, onDie);
		}

		#endregion

		#region 更新

		/// <summary>
		/// 更新具体状态
		/// </summary>
		protected virtual void updateIdle() {
			updateAction();
		}

		/// <summary>
		/// 移动状态更新
		/// </summary>
		protected virtual void updateMoving() { }

		/// <summary>
		/// 更新使用
		/// </summary>
		protected virtual void updateUsing() {
			updateActing();
		}

		/// <summary>
		/// 受击回调
		/// </summary>
		protected virtual void onHit() {
			// 取消技能
			
			onFreezeStart();
		}

		/// <summary>
		/// 开始硬直回调
		/// </summary>
		protected virtual void onFreezeStart() {
			animator?.setVar(FreezeAttrName, true);
			stop();
		}

		/// <summary>
		/// 结束硬直回调
		/// </summary>
		protected virtual void onFreezeEnd() {
			animator?.setVar(FreezeAttrName, false);
		}

		/// <summary>
		/// 死亡回调
		/// </summary>
		protected virtual void onDie() {
			animator?.setVar(FreezeAttrName, false);
			animator?.setVar(DeadAttrName, true);
			stop();
		}

		#endregion

		#region 移动控制

		/// <summary>
		/// 移动速度
		/// </summary>
		/// <returns></returns>
		public override float moveSpeed() {
            return vehicle ? 0 : runtimeBattler.moveSpeed();
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
		/// 对手
		/// </summary>
		/// <returns></returns>
		public abstract List<MapBattler> opponents();

		/// <summary>
		/// 队友
		/// </summary>
		/// <returns></returns>
		public abstract List<MapBattler> friends();

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
            debugLog(name + " Use skill: " + skill);
            currentProcessor = skillProcessor(skill);
			currentProcessor?.use();
		}

		///// <summary>
		///// 技能命中回调
		///// </summary>
		///// <param name="skill"></param>
		//protected override void apply(ISkillApplication skill) {
		//	skill.applyBattler(this);
		//}

		///// <summary>
		///// 能否被击中
		///// </summary>
		///// <returns></returns>
		//public override bool isApplyable() {
		//	return runtimeBattler.isHitting() || runtimeBattler.isDead();
		//}

		///// <summary>
		///// 是否有效的技能
		///// </summary>
		///// <param name="skill"></param>
		///// <returns></returns>
		//protected override bool isValidSkill(ISkillApplication skill) {
		//	if (!base.isValidSkill(skill)) return false;
		//	var skillProcessor = skill as SkillProcessor;
		//	if (skillProcessor == null) return true;
		//	return skillProcessor.battler != this;
		//}

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
			if (currentProcessor == null || 
				currentProcessor.isTerminated()) onActionEnd();
		}

		/// <summary>
		/// 开始行动
		/// </summary>
		/// <param name="action"></param>
		public void startAction(RuntimeAction action) {
			if (action == null) return;
			currentAction = action;

			onActionStart();
			useSkill(currentAction.skill);

			//processAction();
		}

		/// <summary>
		/// 技能发动
		/// </summary>
		public void onSkillUse() {
			currentProcessor?.onUse();
        }

		/// <summary>
		/// 行动开始
		/// </summary>
		protected virtual void onActionStart() {
			runtimeBattler.onActionStart(currentAction);
		}

		/// <summary>
		/// 行动结束
		/// </summary>
		protected virtual void onActionEnd() {
			runtimeBattler.onActionEnd(currentAction);
			currentProcessor?.onUseEnd();

			//currentAction = null;
			//currentProcessor = null;
		}

        #endregion

        #region 音效播放
		protected void playDeathAudio() {
			Debug.Log("play death audio dante");
			AudioSource audio = gameObject.GetComponent<AudioSource>();
            if (audio == null)
                return;
            AudioClip clip = (AudioClip)Resources.Load("Audios/Sword-Swing");
			audio.clip = clip;
			audio.volume = 1.0f;
			audio.Play();
		}

		protected void playSkillAudio() {
			Debug.Log("play skill audio dante");
			AudioSource audio = gameObject.GetComponent<AudioSource>();
            if (audio == null)
                return;
			AudioClip clip = (AudioClip)Resources.Load("Audios/Sword-Swing");
            audio.clip = clip;
			audio.volume = 1.0f;
			audio.Play();
        }

        protected void playHitAudio()
        {
			Debug.Log("play hit audio dante");
            AudioSource audio = gameObject.GetComponent<AudioSource>();
            if (audio == null)
                return;
            AudioClip clip = (AudioClip)Resources.Load("Audios/Sword-Hit");
			audio.clip = clip;
			audio.volume = 1.0f;
			audio.Play();
        }

        #endregion

    }
}