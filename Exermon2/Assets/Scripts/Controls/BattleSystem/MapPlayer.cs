using System.Collections.Generic;

using UnityEngine;

using Core.UI.Utils;

using MapModule.Data;
using BattleModule.Data;

using GameModule.Services;
using PlayerModule.Services;

namespace UI.BattleSystem.Controls {

    using MapSystem.Controls;

    /// <summary>
    /// 地图上的玩家实体
    /// </summary>
    public class MapPlayer : MapBattler {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public SkillProcessor normalSkill, longRangeSkill;
        //public List<Material> materials = new List<Material>();
        public Material material;
        public GameObject seperationPrefab;//分身预制件

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public int presentCharacterId = 1;
		public int pastCharacterId = 2;

		public bool inputable = true;

		public int maxSeperation = 1; // 最大分身数

		/// <summary>
		/// 类型
		/// </summary>
		public override Type type => Type.Player;

        /// <summary>
        /// 属性
        /// </summary>
        public Actor actor => playerSer.actor;
        public RuntimeActor runtimeActor => runtimeBattler as RuntimeActor;

        protected float xDelta => Input.GetAxisRaw("Horizontal");
        protected float yDelta => Input.GetAxisRaw("Vertical");

        Vector2 collCenter => pos + new Vector2(0f, collider.bounds.size.y / 2);//碰撞盒中心

        public bool flashUsable { get; set; } = true;//是否能进行闪烁
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
        /// 分身
        /// </summary>
        List<MapSeperation> mapSeperations = null;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        protected GameService gameSer;
        protected PlayerService playerSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void start() {
            base.start();
			scene?.onPlayerStart();
		}

        /// <summary>
        /// 初始化碰撞函数
        /// </summary>
        protected override void initializeCollFuncs() {
            base.initializeCollFuncs();
            registerOnEnterFunc<MapEvent>(onEventCollEnter);
            registerOnStayFunc<MapEvent>(onEventCollStay);
            registerOnExitFunc<MapEvent>(onEventCollExit);

			// TODO: 代码优化
			//registerOnStayFunc<RayBlock>((block) => block.onPlayerColl(this));
		}

        /// <summary>
        /// 初始化敌人显示组件
        /// </summary>
        protected override void setupBattlerDisplay() {
            display.setItem(playerSer.actor.runtimeActor);
        }

        #endregion

        #region 更新

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();

            //测试用，跳过对话
            if (Input.GetKey(KeyCode.T)) {
                MapModule.Services.MessageService.Get().messages.Clear();
            }
		}

		/// <summary>
		/// Idle状态更新
		/// </summary>
		protected override void updateIdle() {
			base.updateIdle();
			updateInput();
		}

		/// <summary>
		/// 移动状态更新
		/// </summary>
		protected override void updateMoving() {
			base.updateMoving();
			updateInput();
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
			switchCharacter();
		}

		/// <summary>
		/// 切换行走图
		/// </summary>
		void switchCharacter() {
			if (runtimeBattler == null) return;
			var cid = scene.isPresent ? presentCharacterId : pastCharacterId;
			runtimeBattler.battler.characterId = cid;
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

		/// <summary>
		/// 是否主体
		/// </summary>
		/// <returns></returns>
		public virtual bool isMaster() {
			return true;
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
		bool attack = false, attacking = false;

		/// <summary>
		/// 能否输入
		/// </summary>
		/// <returns></returns>
		public bool isInputable() {
            return map && map.isActive() && inputable;
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

			//stop();

			return updateAttack() || updateFlash();
        }

		/// <summary>
		/// 更新攻击输入
		/// </summary>
		bool updateAttack() {
			attacking = Input.GetKey(gameSer.keyboard.attack1Key) || 
				Input.GetKey(gameSer.keyboard.attack2Key);

			if (attack = Input.GetKeyUp(gameSer.keyboard.attack1Key))
				useSkill(normalSkill);
			else if (attack = Input.GetKeyUp(gameSer.keyboard.attack2Key))
				useSkill(longRangeSkill);

			return attack || attacking;
		}

		/// <summary>
		/// 更新闪烁输入
		/// </summary>
		/// <returns></returns>
		bool updateFlash() {
			updateFlashEffect();

			if (Input.GetKeyDown(gameSer.keyboard.rushKey) 
				&& !flashIsCooling) useSkillFlash();

			return flashBegin && !flashEnd;
		}

		/// <summary>
		/// 更新闪烁技能使用
		/// </summary>
		void updateFlashEffect() {
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
        /// 计算闪烁位置
        /// </summary>
        public float calFlashDistance() {
            Vector2 flashVec = RuntimeCharacter.dir82Vec(direction);//闪烁方向
            Vector2 dropPos = collCenter + flashVec * flashDistance;//落点
            Vector2 colliderSize = new Vector2(collider.bounds.size.x - 0.02f, collider.bounds.size.y - 0.02f);//微调碰撞盒
            Collider2D collHard = Physics2D.OverlapCapsule(dropPos,
                colliderSize, CapsuleDirection2D.Horizontal, 0f, (1 << 11) | (1 << 4) | (1 << 15));//落点碰撞判断

            RaycastHit2D raycastHit2D = Physics2D.Raycast(pos, flashVec, flashDistance, (1 << 11) | (1 << 4) | (1 << 15));

            Collider2D collSoft = Physics2D.OverlapCapsule(dropPos,
                colliderSize, CapsuleDirection2D.Horizontal, 0f, (1 << 8));//落点碰撞判断


            float flashDistStep = 0.05f;
            float flashDistRes = 0.0f;

            if (raycastHit2D || collSoft) {
                //寻找最远落点
                while (flashDistStep <= flashDistance) {
                    collHard = Physics2D.OverlapCapsule(collCenter + flashVec * flashDistStep,
                        colliderSize, CapsuleDirection2D.Horizontal, 0f, (1 << 11) | (1 << 4) | (1 << 15));


                    collSoft = Physics2D.OverlapCapsule(collCenter + flashVec * flashDistStep,
                        colliderSize, CapsuleDirection2D.Horizontal, 0f, (1 << 8));//落点碰撞判断

                    debugLog(collHard?.name);
                    debugLog(collSoft?.name);
                    if (collHard)
                        break;
                    if (!collSoft || collSoft.isTrigger)
                        flashDistRes = flashDistStep;
                    flashDistStep += 0.05f;
                }
                return flashDistRes;
            }
            //直接闪烁到最远距离
            else {
                return flashDistance;
            }
        }

        /// <summary>
        /// 将闪烁距离应用到闪烁
        /// </summary>
        /// <param name="distance"></param>
        public void applyFlashDistance(float distance) {
            Vector2 flashVec = RuntimeCharacter.dir82Vec(direction);//闪烁方向
            flashPos = pos + flashVec * distance;
        }

        /// <summary>
        /// 设置闪烁位置
        /// </summary>
        virtual protected void setFlashPos() {
            if (mapSeperations == null || mapSeperations.Count == 0)
                applyFlashDistance(calFlashDistance());
            else {
                List<float> disList = new List<float>();
                disList.Add(calFlashDistance());
                foreach (var sepearation in mapSeperations) {
                    disList.Add(sepearation.calFlashDistance());
                }
                disList.Sort();
                var minDis = disList[0];
                foreach (var sepearation in mapSeperations) {
                    sepearation.applyFlashDistance(minDis);
                }
                applyFlashDistance(minDis);
            }
        }

        /// <summary>
        /// 使用闪烁技能
        /// </summary>
        /// <param name="skill"></param>
        void useSkillFlash() {
            setFlashPos();

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
            runtimeBattler.addAction(skill.skill);
        }

        #endregion

        #region 能量控制

        /// <summary>
        /// 能量
        /// </summary>
        public float energy => runtimeActor.energy;

        /// <summary>
        /// 添加能量
        /// </summary>
        /// <param name="value"></param>
        public void addEnergy(float value) {
            runtimeActor.addEnergy(value);
        }

		#endregion

		#region 分身

		/// <summary>
		/// 受击回调
		/// </summary>
		protected override void onDie() {
			base.onDie();
			if (seperationsCount() > 0)
				foreach (var sep in mapSeperations)
					sep.onDie();
		}

		/// <summary>
		/// 重载更改地图函数（分身也要一起转移）
		/// </summary>
		public override void changeMap(Map map, bool origin = false) {
			base.changeMap(map, origin);
			if (seperationsCount() > 0)
				changeMapForSeperations(map, origin);
		}

		/// <summary>
		/// 更改分身的地图
		/// </summary>
		void changeMapForSeperations(Map map, bool origin = false) {
			foreach (var sep in mapSeperations)
				sep.changeMap(map, origin);
		}

		/// <summary>
		/// 分身数目
		/// </summary>
		/// <returns></returns>
		public int seperationsCount() {
			return mapSeperations == null ? 0 : mapSeperations.Count;
		}

		/// <summary>
		/// 是否能进行分身
		/// </summary>
		/// <returns></returns>
		protected virtual bool isSeprateEnable() {
			return seperationsCount() < maxSeperation;
		}

		/// <summary>
		/// 添加分身
		/// </summary>
		public void addSeperation(Vector3 position) {
			if (!isSeprateEnable()) return;

			if (mapSeperations == null)
				mapSeperations = new List<MapSeperation>();

			var obj = Instantiate(seperationPrefab,
				position, transform.rotation, transform.parent);

			var mapSeperation = SceneUtils.get<MapSeperation>(obj);
			mapSeperations.Add(mapSeperation);
		}
		public void addSeperation(Transform transform) {
			addSeperation(transform.position);
		}
		public void addSeperation() {
			addSeperation(transform);
		}

		/// <summary>
		/// 清除分身
		/// </summary>
		public void clearSeperations() {
			if (mapSeperations == null) return;

			foreach (var seperation in mapSeperations) 
                seperation.destroy(true);

            mapSeperations.Clear();
        }
        #endregion
    }
}
