using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.BattleSystem.Controls;
using UI.MapSystem.Controls;

namespace Assets.Scripts.Controls.BattleSystem {
    class BulletHitEntity : MapEntity{

        /// <summary>
        /// 玩家
        /// </summary>
        public MapBattler battler;

        protected override void initializeEvery() {
            base.initializeEvery();
            if (!battler) battler = findParent<MapBattler>();
        }

        /// <summary>
        /// 初始化碰撞函数
        /// </summary>
        protected override void initializeCollFuncs() {
            base.initializeCollFuncs();
            registerOnEnterFunc<BulletProcessor>(passToPlayer);
        }


        void passToPlayer(BulletProcessor bullet) {
            debugLog("hithithit!" + bullet);
            if (bullet == null) return;
            bullet.skillProcessor.apply(battler);
        }

    }
}
