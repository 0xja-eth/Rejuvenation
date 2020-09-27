﻿
using UnityEngine;

using BattleModule.Data;

using PlayerModule.Services;

namespace UI.Common.Controls.BattleSystem {

	using MapSystem;

	/// <summary>
	/// 地图上的玩家实体
	/// </summary>
	public class MapPlayer : MapBattler {

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public bool moveable = true;

		/// <summary>
		/// 类型
		/// </summary>
		public override Type type => Type.Player;

		/// <summary>
		/// 属性
		/// </summary>
		public Actor actor => playerSer.actor;

		protected float xDelta => Input.GetAxis("Horizontal");
		protected float yDelta => Input.GetAxis("Vertical");

		/// <summary>
		/// 外部系统设置
		/// </summary>
		PlayerService playerSer;

		#region 初始化

		/// <summary>
		/// 初始化敌人显示组件
		/// </summary>
		protected override void setupBattlerDisplay() {
			display.setItem(playerSer.actor.runtimeActor);
		}

		#endregion

		#region 更新

		/// <summary>
		/// 控制刚体刷新
		/// </summary>
		protected override void fixedUpdate() {
			base.fixedUpdate();
            updateInput();
        }

        /// <summary>
        /// 更新玩家输入事件
        /// </summary>
        void updateInput() {
			updateMovement();
			updateSkill();
        }
		
		/// <summary>
		/// 更新移动
		/// </summary>
		void updateMovement() {
			if (xDelta == 0 && yDelta == 0) stop();
			else {
				var speed = new Vector2(xDelta, yDelta);
				move(speed * moveSpeed());
			}
		}

		/// <summary>
		/// 更新技能使用
		/// </summary>
		void updateSkill() {

		}

		#endregion

		#region 移动控制

		/// <summary>
		/// 移动速度
		/// </summary>
		/// <returns></returns>
		//public override float moveSpeed() {
		//	return actor.speed;
		//}

		/// <summary>
		/// 能否移动
		/// </summary>
		/// <returns></returns>
		public override bool isMoveable() {
            return base.isMoveable() && map.active && moveable;

        }

		#endregion
	}
}