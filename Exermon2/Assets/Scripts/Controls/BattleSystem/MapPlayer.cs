using System.Collections.Generic;

using UnityEngine;

using MapModule.Data;
using BattleModule.Data;

using GameModule.Services;
using PlayerModule.Services;

namespace UI.BattleSystem.Controls {
    using Core.UI.Utils;
    using MapSystem.Controls;
    using UI.MapSystem;

    /// <summary>
    /// 地图上的玩家实体
    /// </summary>
    public class MapPlayer : MapBattler {

        /// <summary>
        /// 远程攻击蓄力时间
        /// </summary>
        const float LongRangeSkillTime = 0.5f;

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public SkillProcessor normalSkill, longRangeSkill;
        //public List<Material> materials = new List<Material>();
        public Material material;

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

        protected float xDelta => Input.GetAxisRaw("Horizontal");
        protected float yDelta => Input.GetAxisRaw("Vertical");

        Vector2 collCenter => pos + new Vector2(0f, collider.bounds.size.y / 2);//碰撞盒中心

        const float flashCoolTime = 1f;//闪烁冷却时间
        float flashCoolTimeRemain = flashCoolTime;//闪烁冷却计时
        const float dissolveSpeed = 3f;//角色消失/出现    速度
        const float flashDistance = 2f;//闪烁距离
        public float dissolveAnt = 0f;//据色出现/消失参数
        public Vector2 flashPos;//闪烁最终落点

        bool flashIsCooling = false;//闪烁是否冷却
        bool flashBegin = false;//角色是否开始消失
        bool flashEnd = false;//角色是否开始出现

		/// <summary>
		/// 地图
		/// </summary>
        protected override Map map {
            get => scene.currentMap;
        }

        /// <summary>
        /// 外部系统设置
        /// </summary>
        GameService gameSer;
        PlayerService playerSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void start() {
			updateCurrentMap();
            base.start();
        }

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
			updateCurrentMap();

			//测试用，跳过对话
			if (Input.GetKey(KeyCode.T)) {
                MapModule.Services.MessageService.Get().messages.Clear();
            }
        }

        /// <summary>
        /// 更新玩家输入事件
        /// </summary>
        void updateInput() {
            if (!isInputable() && !(!flashBegin && flashEnd)) {
                stop(); return;
            }
            // 返回 True => 有输入  返回 False => 无输入
            if (updateSearching() || updateSkill()) stop();
            else updateMovement();
        }

		#endregion

		#region	地图控制

		/// <summary>
		/// 地图改变回调
		/// </summary>
		protected override void onMapChanged() {
			base.onMapChanged();
			// TODO: 改变形象
		}

		/// <summary>
		/// 更新当前地图
		/// </summary>
		void updateCurrentMap() {
			map = scene.currentMap;
		}

		#endregion

		#region 状态判断

		/// <summary>
		/// 能否移动
		/// </summary>
		/// <returns></returns>
		public bool isMovable() {
            return runtimeBattler.isMoveable();
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
            event_.processTrigger(this, MapEventPage.TriggerType.CollEnter);
        }

        /// <summary>
        /// 事件碰撞持续
        /// </summary>
        /// <param name="player"></param>
        void onEventCollStay(MapEvent event_) {
            event_.processTrigger(this, search ?
                MapEventPage.TriggerType.CollSearch :
                MapEventPage.TriggerType.CollStay);
        }

        /// <summary>
        /// 事件碰撞结束
        /// </summary>
        /// <param name="player"></param>
        void onEventCollExit(MapEvent event_) {
            event_.processTrigger(this, MapEventPage.TriggerType.CollExit);
        }

        #endregion

        #region 移动控制

        /// <summary>
        /// 更新移动
        /// </summary>
        bool updateMovement() {
            var speed = new Vector2(xDelta, yDelta);
            var flag = (speed.x == 0 && speed.y == 0);

            if (flag) stop();
            else moveDirection(RuntimeCharacter.vec2Dir8(speed));

            return !flag;
        }

		/// <summary>
		/// 同步角色
		/// </summary>
		/// <param name="player"></param>
		public void syncPlayer(MapPlayer player) {
			//transform.localPosition = player.transform.localPosition;
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

            stop();
            var key = gameSer.keyboard.attackKey;
            attack = Input.GetKeyUp(key);
            attacking = Input.GetKey(key);

            if (attack) useSkill();
            if (attacking) {
                attackTime += Time.deltaTime;
                debugLog(attackTime);
            }

            updateSkillFlash();

            var keyflash = gameSer.keyboard.rushKey;
            bool flash = Input.GetKeyDown(keyflash);

            if (flash && !flashIsCooling)
                useSkillFlash();

            return attack || attacking || (flashBegin && !flashEnd);
        }

        /// <summary>
        /// 更新闪烁技能使用
        /// </summary>
        void updateSkillFlash() {
            //闪烁冷却倒计时
            if (flashIsCooling) {
                flashCoolTimeRemain -= Time.deltaTime;
            }
            //闪烁冷却完成
            if (flashCoolTimeRemain <= 0f) {
                flashCoolTimeRemain = flashCoolTime;
                flashIsCooling = false;
                debugLog("cool end");
            }
            //闪烁开始，角色逐渐消失
            if (flashBegin) {
                dissolveAnt += Time.deltaTime * dissolveSpeed;
                dissolveAnt = Mathf.Clamp01(dissolveAnt);
                material.SetFloat("_DissolveAmount", dissolveAnt);
            }
            //角色完全消失，位置改变
            if (flashBegin && dissolveAnt >= 1f) {
                transform.position = flashPos;
                flashBegin = false;
                flashEnd = true;
            }
            //角色逐渐出现
            if (!flashBegin && flashEnd) {
                dissolveAnt -= Time.deltaTime * dissolveSpeed;
                dissolveAnt = Mathf.Clamp01(dissolveAnt);
                material.SetFloat("_DissolveAmount", dissolveAnt);
            }
            //闪烁结束
            if (flashEnd && dissolveAnt <= 0f) {
                flashEnd = false;
                //debugLog("tutorialFlash x:" + gameSer.tutorialFlash);
                //debugLog("wall x:" + gameSer.tutorialFlashPosX);
                //debugLog("flash x:" + flashPos.x);
                //新手教程
                if (gameSer.tutorialFlash) {
                    if (flashPos.x > gameSer.tutorialFlashPosX) {
                        gameSer.tutorialFlashPosX = -1;
                        gameSer.tutorialFlash = false;
                    }
                    else
                        gameSer.onTutorialFlashFail();
                }
            }
        }

        /// <summary>
        /// 使用闪烁技能
        /// </summary>
        /// <param name="skill"></param>
        void useSkillFlash() {

            Vector2 flashVec = RuntimeCharacter.dir82Vec(direction);//闪烁方向
            Vector2 dropPos = collCenter + flashVec * flashDistance;//落点
            Vector2 colliderSize = new Vector2(collider.bounds.size.x - 0.02f, collider.bounds.size.y - 0.02f);//微调碰撞盒
            Collider2D collTemp = Physics2D.OverlapCapsule(dropPos,
                colliderSize, CapsuleDirection2D.Horizontal, 0f, 1 << 11);//落点碰撞判断

            float flashDistStep = 0.05f;
            float flashDistRes = 0.0f;

            if (collTemp) {
                //寻找最远落点
                while (flashDistStep <= flashDistance) {
                    collTemp = Physics2D.OverlapCapsule(collCenter + flashVec * flashDistStep, 
                        colliderSize, CapsuleDirection2D.Horizontal, 0f, 1 << 11);
                    debugLog(collTemp?.name);
                    if (!collTemp)
                        flashDistRes = flashDistStep;
                    flashDistStep += 0.05f;
                }
                Vector2 offset = flashVec * flashDistRes;
                flashPos = pos + offset;
            }
            //直接闪烁到最远距离
            else {
                flashPos = pos + flashVec * flashDistance;
            }
            flashIsCooling = true;
            flashBegin = true;
        }


        ///// <summary>
        ///// 材质切换
        ///// </summary>
        ///// <param name="index"></param>
        //void switchMaterial(int index) {
        //    material = materials[index];
        //}


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
