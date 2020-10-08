using System.Collections.Generic;

using UnityEngine;

using Core.Data;

using MapModule.Data;
using BattleModule.Data;

namespace UI.BattleSystem.Controls {

	using MapSystem.Controls;

	/// <summary>
	/// 地图上的敌人实体
	/// </summary>
	public abstract class MapEnemy : MapBattler {

		/// <summary>
		/// 常量定义
		/// </summary>
		const float DeltaMoveTime = 0.5f;
        const float deteAngle = 90.0f;

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public bool useCustomParams = false; // 是否使用自定义属性

		public Enemy customEnemy = null; // 自定义敌人数据

		/// <summary>
		/// 内部变量定义
		/// </summary>
		float moveTime = 0; // 一次移动的时间

		/// <summary>
		/// 类型
		/// </summary>
		public override Type type => Type.Enemy;

		/// <summary>
		/// 敌人ID
		/// </summary>
		public virtual int enemyId => 0;

		/// <summary>
		/// 敌人
		/// </summary>
		//Enemy enemy_ = null;
		public Enemy enemy => runtimeEnemy?.enemy();//enemy_ = enemy_ ?? 
			//BaseData.poolGet<Enemy>(enemyId);

		/// <summary>
		/// 运行时敌人
		/// </summary>
		public RuntimeEnemy runtimeEnemy => runtimeBattler as RuntimeEnemy;

		/// <summary>
		/// 玩家（敌人目标）
		/// </summary>
		MapPlayer player => map.player;

		#region 初始化
		/// <summary>
		/// 配置更新函数
		/// </summary>
		protected override void setupStateChanges() {
			base.setupStateChanges();

			runtimeBattler?.addStateDict(
				RuntimeBattler.State.Moving, updateMoveTime);
		}


		/// <summary>
		/// 初始化敌人显示组件
		/// </summary>
		protected override void setupBattlerDisplay() {
			RuntimeEnemy enemy;
			if (useCustomParams)
				enemy = new RuntimeEnemy(enemyId, customEnemy);
			else
				enemy = new RuntimeEnemy(enemyId);

			display.setItem(enemy);
		}

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void updateIdle() {
			base.updateIdle();
            updateEnemyBehaviour();
		}

		/// <summary>
		/// 更新敌人行动
		/// </summary>
		void updateEnemyBehaviour() {
			processBehaviour((runtimeEnemy.isCritical = isCritical()) ?
				enemy.criticalBehaviour : enemy.generalBehaviour);
		}

		/// <summary>
		/// 更新移动时间
		/// </summary>
		void updateMoveTime() {
			if (moveTime > 0 && 
				(moveTime -= Time.deltaTime) <= 0) stop();
		}

		#endregion

		#region 敌人行为控制

		/// <summary>
		/// 是否警戒状态
		/// </summary>
		/// <returns></returns>
		public bool isCritical() {
			var dist = (pos - player.pos).magnitude;

            Vector2 targetDirection = player.transform.position - transform.position;
            Vector2 towardDirection = RuntimeCharacter.dir82Vec(direction);
            float angle = Vector2.Angle(targetDirection, towardDirection);
            bool canSeePlayer = false;
            Ray2D ray2D = new Ray2D(pos, player.pos);
            Debug.DrawLine(pos, player.pos, Color.red, 1);
            RaycastHit2D hit = Physics2D.Raycast(pos, player.pos - pos , 100, (1 << 11 | 1 << 10));
            if (hit) {
                Debug.Log(hit.collider.name);
                if(hit.collider.name == "Player") {
                    canSeePlayer = true;
                }
            }

            return dist <= enemy.criticalRange && angle <= deteAngle && canSeePlayer;
		}

		/// <summary>
		/// 处理行为
		/// </summary>
		/// <param name="type"></param>
		public void processBehaviour(Enemy.BehaviourType type) {
			RuntimeCharacter.Direction d; // = RuntimeCharacter.Direction.None;
			moveTime = DeltaMoveTime;
			switch (type) {
				case Enemy.BehaviourType.Random:
					d = getRandomDirection(); break;
				case Enemy.BehaviourType.Close:
					d = getCloseDirection(); break;
				case Enemy.BehaviourType.Far:
					d = getFarDirection(); break;
				case Enemy.BehaviourType.Custom:
					customBehaviour(); return;
				default: stop(); return;
			}
			debugLog("Move " + d);
			moveDirection(d);
		}

		/// <summary>
		/// 获取随机方向
		/// </summary>
		/// <returns></returns>
		RuntimeCharacter.Direction getRandomDirection() {
			return (RuntimeCharacter.Direction)Random.Range(
				1, RuntimeCharacter.DirectionCount + 1);
		}

		/// <summary>
		/// 获取靠近角色方向
		/// </summary>
		/// <returns></returns>
		RuntimeCharacter.Direction getCloseDirection() {
			var delta = player.pos - pos;
			return RuntimeCharacter.vec2Dir8(delta);
		}

		/// <summary>
		/// 获取远离角色方向
		/// </summary>
		/// <returns></returns>
		RuntimeCharacter.Direction getFarDirection() {
			var delta = pos - player.pos;
			return RuntimeCharacter.vec2Dir8(delta);
		}

		/// <summary>
		/// 处理自定义行为
		/// </summary>
		protected virtual void customBehaviour() { }

		#endregion

		#region 技能控制

		/// <summary>
		/// 对手
		/// </summary>
		/// <returns></returns>
		public override List<MapBattler> opponents() {
			return map.battlers(Type.Player);
		}

		/// <summary>
		/// 队友
		/// </summary>
		/// <returns></returns>
		public override List<MapBattler> friends() {
			return map.battlers(Type.Enemy);
		}

		#endregion

	}
}