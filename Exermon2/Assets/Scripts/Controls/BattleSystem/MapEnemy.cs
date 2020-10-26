using System.Collections.Generic;

using UnityEngine;

using Core.Data;

using Core.UI.Utils;

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
        MapPlayer player => map?.player;

        #region 初始化

        /// <summary>
        /// 配置更新函数
        /// </summary>
        protected override void configureStateChanges() {
            base.configureStateChanges();

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

        #region 释放资源

        /// <summary>
        /// 销毁回调
        /// </summary>
        protected override void onDestroy() {
            base.onDestroy();

            runtimeBattler?.removeStateDict(
                RuntimeBattler.State.Moving, updateMoveTime);
        }

        #endregion

        #region 更新

        /// <summary>
        /// 更新
        /// </summary>
        protected override void updateIdle() {
            base.updateIdle();
            if (map.isActive()) updateEnemyBehaviour();
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
            if (moveTime > 0 && (moveTime -= Time.deltaTime) <= 0) stop();
        }

        #endregion

        #region 敌人行为控制

        /// <summary>
        /// 是否警戒状态
        /// </summary>
        /// <returns></returns>
        public bool isCritical() {
            var closerPlayer = closerOpponent();
            if (!closerPlayer) return false;

            var dist = (pos - closerPlayer.pos).magnitude;
            if (dist > enemy.criticalRange) return false;

            Vector2 targetDirection = closerPlayer.transform.position - transform.position;
            Vector2 towardDirection = RuntimeCharacter.dir82Vec(direction);
            float angle = Vector2.Angle(targetDirection, towardDirection);

            if (angle > deteAngle) return false;

            Ray2D ray2D = new Ray2D(pos, closerPlayer.pos);
            Debug.DrawLine(pos, closerPlayer.pos, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(pos, closerPlayer.pos - pos, 100, (1 << 11 | 1 << 10));

            var mapPlayer = SceneUtils.get<MapPlayer>(hit.collider);

            return hit && mapPlayer;
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

            var closerPlayer = closerOpponent();
            var dist = (pos - closerPlayer.pos).magnitude;
            var curr = currentProcessor;
            if (curr && dist <= curr.range) {
                runtimeCharacter.direction = d;
                return;
            }

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
            var delta = closerOpponent().pos - pos;
            return RuntimeCharacter.vec2Dir8(delta);
        }

        /// <summary>
        /// 最近敌人
        /// </summary>
        /// <returns></returns>
        MapBattler closerOpponent() {
            float minDis = 100f;
            MapBattler closerOp = null;
            foreach (var opponent in opponents()) {
                if((opponent.pos - pos).magnitude < minDis) {
                    minDis = (opponent.pos - pos).magnitude;
                    closerOp = opponent;
                }
            }
            return closerOp;
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
            debugLog("AAA" + map.battlers(Type.Player).Count);
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