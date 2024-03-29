﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

using UI.Common.Controls.AnimationSystem;

namespace Core.UI {

	using Utils;

    /// <summary>
    /// 基本状态行为
    /// </summary>
    public class BaseStateBehaviour : StateMachineBehaviour {
		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		//override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    
		//}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    
		//}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    
		//}

		// OnStateMove is called right after Animator.OnAnimatorMove()
		//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    // Implement code that processes and affects root motion
		//}

		// OnStateIK is called right after Animator.OnAnimatorIK()
		//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    // Implement code that sets up animation IK (inverse kinematics)
		//}

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public float deltaTime = 0.5f;

		/// <summary>
		/// 参数储存
		/// </summary>
		Animator animator;
		protected AnimatorStateInfo stateInfo;
        AnimatorControllerPlayable controller;

		int layerIndex;
        int stateMachinePathHash;

		protected float aniRate = 0;

        protected GameObject gameObject;

		/// <summary>
		/// 拓展
		/// </summary>
		AnimatorExtend animatorExtend;

		/// <summary>
		/// 内部变量
		/// </summary>
		protected bool finished = false;

		#region 初始化

		/// <summary>
		///初始化
		/// </summary>
		/// <param name="go">游戏物体</param>
		protected virtual void setup(GameObject go) {
            gameObject = go;
			animatorExtend = SceneUtils.get<AnimatorExtend>(go);
        }

        #endregion

        /// <summary>
        /// 状态进入
        /// </summary>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            this.animator = animator;
            this.stateInfo = stateInfo;
            this.layerIndex = layerIndex;

            setup(animator.gameObject);
            onStateEnter();
        }

        /// <summary>
        /// 状态进入
        /// </summary>
        protected virtual void onStateEnter() {
			finished = false;
		}

        /// <summary>
        /// 状态更新
        /// </summary>
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            this.animator = animator;
            this.stateInfo = stateInfo;
            this.layerIndex = layerIndex;

            setup(animator.gameObject);
            onStateUpdate();
        }

        /// <summary>
        /// 状态更新
        /// </summary>
        protected virtual void onStateUpdate() {
			aniRate = stateInfo.normalizedTime / (stateInfo.length + deltaTime);

			if (!finished && aniRate >= 1) onStateFinished(); 
		}

		/// <summary>
		/// 状态结束回调
		/// </summary>
		protected virtual void onStateFinished() {
			finished = true;
			animatorExtend?.onStateEnd(stateInfo);
		}

		/// <summary>
		/// 状态结束
		/// </summary>
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateExit(animator, stateInfo, layerIndex);

            this.animator = animator;
            this.stateInfo = stateInfo;
            this.layerIndex = layerIndex;

            setup(animator.gameObject);
            onStateExit();
        }
        /// <summary>
        /// 状态结束
        /// </summary>
        protected virtual void onStateExit() {
			if (!finished) onStateFinished();
		}

	}

}
