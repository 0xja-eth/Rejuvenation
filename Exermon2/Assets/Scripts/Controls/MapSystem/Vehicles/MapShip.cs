using System.Linq;

using UnityEngine;

using Core.UI.Utils;

using MapModule.Data;
using BattleModule.Data;

using GameModule.Services;
using PlayerModule.Services;

namespace UI.MapSystem.Controls {

    using BattleSystem.Controls;

    /// <summary>
    /// 地图上的船载具
    /// </summary>
    public class MapShip : MapVehicle {

		/// <summary>
		/// 常量定义
		/// </summary>
		const int GroundLayer = 13;

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public float magnetiteDist = 8; // 万象天引距离
        public int magnetiteEnergy = 1; // 使用万象天引耗能

        /// <summary>
        /// 内部变量定义
        /// </summary>
        WaterColumn targetCol; // 目标柱子

        bool boardFlag = false;

        /// <summary>
        /// 乘客玩家
        /// </summary>
        public MapPlayer player => passengers.getSubView(0) as MapPlayer;
        RuntimeActor runtimeActor => player?.runtimeActor;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        GameService gameSer;

        #region 初始化

        /// <summary>
        /// 初始化碰撞回调
        /// </summary>
        protected override void initializeCollFuncs() {
            base.initializeCollFuncs();
            registerOnEnterFunc<MapPlayer>(onColliderEnterWithPlayer);
            registerOnEnterFunc<MapRegion>(onColliderEnterWithGround);
            registerOnExitFunc<MapPlayer>(onColliderExitWithPlayer);
            registerOnExitFunc<MapRegion>(onColliderExitWithGround);


            registerOnStayFunc<MapPlayer>(tryBoard);
            boardingRegion.registerOnStayFunc<MapRegion>(tryLand);
        }

        #endregion

        #region 更新

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            if (player) updateInput();
        }

        /// <summary>
        /// 更新输入
        /// </summary>
        void updateInput() {
            if (isMagnetite()) useMagnetite();
        }

        #endregion

        #region 磁石柱子控制

        /// <summary>
        /// 是否使用磁石
        /// </summary>
        /// <returns></returns>
        public bool isMagnetite() {
            return runtimeActor.energy >= magnetiteEnergy &&
                Input.GetKeyDown(gameSer.keyboard.magnetiteKey);
        }

        /// <summary>
        /// 使用磁石
        /// </summary>
        void useMagnetite() {
            var dir = player.direction;
            var vec = RuntimeCharacter.dir82Vec(dir);
            //var resList = Physics2D.RaycastAll(pos, vec, magnetiteDist);

            Collider2D collider2d = Physics2D.OverlapBox(pos, collider.bounds.size, 0f, 1 << 8);
            for (int i = 1; i < magnetiteDist; i++) {
                collider2d = Physics2D.OverlapBox(pos + vec * i, collider.bounds.size, 0f, 1 << 8);
                debugLog(collider2d?.name);
                debugLog(collider?.bounds.size);
                var obj = collider2d?.gameObject;
                targetCol = SceneUtils.get<WaterColumn>(obj);
                if (targetCol == null) continue;
                runtimeActor.addEnergy(-magnetiteEnergy);
                moveDirection(dir); break;
            }
            //foreach(var res in resList) {
            //	var obj = res.collider?.gameObject;

            //	targetCol = SceneUtils.get<WaterColumn>(obj);
            //	if (targetCol == null) continue;

            //	moveDirection(dir); break;
            //}
        }

        /// <summary>
        /// 与柱子相撞
        /// </summary>
        /// <param name="col"></param>
        void onColumnColl(WaterColumn col) {
            if (col == targetCol) stop();
        }

        #endregion

        #region 乘降操作

        /// <summary>
        /// 是否需要乘降
        /// </summary>
        /// <returns></returns>
        public bool isTake() {
            return Input.GetKeyDown(gameSer.keyboard.takeKey);
        }

        /// <summary>
        /// 尝试自动上船
        /// </summary>
        /// <param name="player"></param>
        void tryBoard(MapPlayer player) {
            if (!isTake() || isFreezing()) return;
            if (!boardFlag && addPassenger(player, true))
                boardFlag = true;
        }

        /// <summary>
        /// 尝试自动下船
        /// </summary>
        /// <param name="region"></param>
        public void tryLand(MapRegion region) {
            debugLog("tryLand");
            if (!isTake() || isFreezing()) return;
            if (boardFlag && landingRegions.Contains(region)
                && removeAllPassengers()) boardFlag = false;
        }

        #endregion

        #region 按键提示

        /// <summary>
        /// 人进入船碰撞体
        /// </summary>
        void onColliderEnterWithPlayer(MapPlayer player) {
			if (!player) return;
			player.keyTip.SetActive(true);
			player.keyTipVehicle = true;
		}

        /// <summary>
        /// 船进入地面碰撞体
        /// </summary>
        /// <param name="region"></param>
        void onColliderEnterWithGround(MapRegion region) {
			if (player && region.gameObject.layer == GroundLayer) {
				player.keyTip.SetActive(true);
				player.keyTipVehicle = true;
			}
        }

        /// <summary>
        /// 人离开船碰撞体
        /// </summary>
        /// <param name="player"></param>
        void onColliderExitWithPlayer(MapPlayer player) {
			if (!player) return;
			player?.keyTip.SetActive(false);
			player.keyTipVehicle = false;
		}

		/// <summary>
		/// 船离开地面碰撞体
		/// </summary>
		/// <param name="region"></param>
		void onColliderExitWithGround(MapRegion region) {
            if (player && region.gameObject.layer == GroundLayer) {
				player.keyTip.SetActive(false);
				player.keyTipVehicle = false;
			}
		}

        #endregion
    }
}
