using System.Linq;

using UnityEngine;

using MapModule.Data;
using BattleModule.Data;

using GameModule.Services;
using PlayerModule.Services;

namespace UI.MapSystem.Controls {

    using BattleSystem.Controls;

    /// <summary>
    /// 水上的柱子
    /// </summary>
    public class WaterColumn : MapEntity {



        /// <summary>
        /// 初始化碰撞回调
        /// </summary>
        protected override void initializeCollFuncs() {
            base.initializeCollFuncs();
            registerOnEnterFunc<MapShip>(onShipColl);
        }

        /// <summary>
        /// 与ship相撞
        /// </summary>
        /// <param name="col"></param>
        void onShipColl(MapShip ship) {
            if (!ship.collider.isTrigger) ship.player.stop();
        }
    }
}
