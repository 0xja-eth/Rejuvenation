

using BattleModule.Data;

namespace UI.BattleSystem.Controls {
    class MapSeperation : MapPlayer {

        #region 初始化

        /// <summary>
        /// 初始化敌人显示组件
        /// </summary>
        protected override void setupBattlerDisplay() {
            base.setupBattlerDisplay();
            //设置独立的RuntimeActor，分身与player只共享行为，不共享数据
            display.setItem(new RuntimeActor());
        }
        #endregion


        #region 技能控制
        /// <summary>
        /// 受击回调
        /// </summary>
        protected override void onHit() {
            base.onHit();
            onDie();
        }

        /// <summary>
        /// 设置闪烁位置
        /// 由player控制
        /// </summary>
        protected override void setFlashPos() {
        }
        #endregion


        #region 分身
        /// <summary>
        /// 分身不能进行分身
        /// </summary>
        /// <returns></returns>
        protected override bool isSeprateEnable() {
            return false;
        }
        #endregion
    }
}