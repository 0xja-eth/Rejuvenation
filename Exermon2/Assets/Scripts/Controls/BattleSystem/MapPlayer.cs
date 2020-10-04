using System.Collections.Generic;

using UnityEngine;

using MapModule.Data;
using BattleModule.Data;

using GameModule.Services;
using PlayerModule.Services;

using Event = MapModule.Data.Event;

namespace UI.BattleSystem.Controls {

    using MapSystem.Controls;

	/// <summary>
	/// 地图上的玩家实体
	/// </summary>
	public class MapPlayer : MapBattler {

		/// <summary>
		/// 远程攻击蓄力时间
		/// </summary>
		const float LongRangeSkillTime = 1;

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public SkillProcessor normalSkill, longRangeSkill;

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

		protected float xDelta => limitVal(Input.GetAxis("Horizontal"));
		protected float yDelta => limitVal(Input.GetAxis("Vertical"));

		/// <summary>
		/// 外部系统设置
		/// </summary>
		GameService gameSer;
		PlayerService playerSer;

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
			actor.characterId = 1;
			display.setItem(playerSer.actor.runtimeActor);
		}

        #endregion

        #region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateInput();
		}

        /// <summary>
        /// 更新玩家输入事件
        /// </summary>
        void updateInput() {
			if (!isInputable()) {
				stop(); return;
			}
			// 返回 True => 有输入  返回 False => 无输入
			if (updateSearching() || updateSkill()) stop();
			else updateMovement();
		}
		
		#endregion

		#region 输入控制变量

		/// <summary>
		/// 搜索相关
		/// </summary>
		bool search = false, searching = false;

		/// <summary>
		/// 攻击相关
		/// </summary>
		float attackTime = 0;
		bool attack = false, attacking = false;

		/// <summary>
		/// 能否输入
		/// </summary>
		/// <returns></returns>
		public bool isInputable() {
            return map.isActive() && inputable;
        }

		#endregion

		#region 事件处理

		/// <summary>
		/// 能否搜索
		/// </summary>
		/// <returns></returns>
		bool isSearchable() {
			return runtimeBattler.isIdle() || runtimeBattler.isMoving();
		}

		/// <summary>
		/// 更新搜索状态
		/// </summary>
		bool updateSearching() {
			if (!isSearchable()) return false;

			var key = gameSer.keyboard.searchKey;
			search = Input.GetKeyDown(key);
			searching = Input.GetKey(key);

			return search || searching;
		}

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
			event_.processTrigger(this, search ?
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
		/// 开关变量值
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		float limitVal(float val) {
			return val > 0.5f ? 1 : (val < -0.5f ? -1 : 0);
		}

		/// <summary>
		/// 更新移动
		/// </summary>
		bool updateMovement() {
			var speed = new Vector2(xDelta, yDelta);
			var flag = speed.x == 0 && speed.y == 0;

			if (flag) stop();
			else moveDirection(RuntimeCharacter.vec2Dir8(speed));

			return !flag;
		}

		#endregion

		#region 技能控制

		/// <summary>
		/// 对手
		/// </summary>
		/// <returns></returns>
		public override List<MapBattler> opponents() {
			return map.battlers(Type.Enemy);
		}

		/// <summary>
		/// 队友
		/// </summary>
		/// <returns></returns>
		public override List<MapBattler> friends() {
			return map.battlers(Type.Player);
		}

		/// <summary>
		/// 能否使用技能
		/// </summary>
		/// <returns></returns>
		bool isSkillUsable() {
			return runtimeBattler.isIdle() || runtimeBattler.isMoving();
		}

		/// <summary>
		/// 更新技能使用
		/// </summary>
		bool updateSkill() {
			if (!isSkillUsable()) return false;

			var key = gameSer.keyboard.attackKey;
			attack = Input.GetKeyUp(key);
			attacking = Input.GetKey(key);

			if (attack) useSkill();
			if (attacking) attackTime += Time.deltaTime;

			return attack || attacking;
		}

		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="skill"></param>
		void useSkill(SkillProcessor skill) {
			debugLog("useSkill: " + skill + ", time: " + attackTime);
			runtimeBattler.addAction(skill.skill);
			attackTime = 0;
		}
		void useSkill() {
			useSkill(attackTime >= LongRangeSkillTime ?
				longRangeSkill : normalSkill);
		}

		#endregion
	}
}
