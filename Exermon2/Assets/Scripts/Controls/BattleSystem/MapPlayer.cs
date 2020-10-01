
using UnityEngine;

using BattleModule.Data;

using PlayerModule.Services;

using Event = MapModule.Data.Event;

namespace UI.Common.Controls.BattleSystem {
    using GameModule.Services;
    using MapSystem;

	/// <summary>
	/// 地图上的玩家实体
	/// </summary>
	public class MapPlayer : MapBattler {

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public bool inputable = true;

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
        MessageServices msgServices;

		#region 初始化

		/// <summary>
		/// 初始化碰撞函数
		/// </summary>
		protected override void initializeCollFuncs() {
			base.initializeCollFuncs();
			registerOnEnterFunc<MapEvent>(onEventCollEnter);
			registerOnStayFunc<MapEvent>(onEventCollStay);
			registerOnExitFunc<MapEvent>(onEventCollExit);
		}

		/// <summary>
		/// 初始化敌人显示组件
		/// </summary>
		protected override void setupBattlerDisplay() {
			display.setItem(playerSer.actor.runtimeActor);
		}

        protected override void initializeEvery() {
            base.initializeEvery();
            msgServices = MessageServices.Get();
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
			if (!isInputable()) return;
			updateMovement(); updateSkill();
        }

		#endregion

		#region 事件处理

		/// <summary>
		/// 事件碰撞开始
		/// </summary>
		/// <param name="player"></param>
		void onEventCollEnter(MapEvent event_) {
			event_.processTrigger(this, Event.TriggerType.CollEnter);
		}

		/// <summary>
		/// 事件碰撞持续
		/// </summary>
		/// <param name="player"></param>
		void onEventCollStay(MapEvent event_) {
			event_.processTrigger(this, event_.isSearching ?
				Event.TriggerType.CollSearch : Event.TriggerType.CollStay);
		}

		/// <summary>
		/// 事件碰撞结束
		/// </summary>
		/// <param name="player"></param>
		void onEventCollExit(MapEvent event_) {
			event_.processTrigger(this, Event.TriggerType.CollExit);
		}

		#endregion

		#region 移动控制

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
		/// 能否输入
		/// </summary>
		/// <returns></returns>
		public bool isInputable() {
            return map.active && inputable && !msgServices.isDialogued;
        }

		#endregion

		#region 技能控制

		/// <summary>
		/// 更新技能使用
		/// </summary>
		void updateSkill() {
			// TODO: 人物技能
		}

		#endregion
	}
}