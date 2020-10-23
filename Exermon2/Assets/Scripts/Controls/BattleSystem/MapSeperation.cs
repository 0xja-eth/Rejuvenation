

using BattleModule.Data;

namespace UI.BattleSystem.Controls {

	/// <summary>
	/// 分身
	/// </summary>
	public class MapSeperation : MapPlayer {

        #region 初始化

        /// <summary>
        /// 初始化敌人显示组件
        /// </summary>
        protected override void setupBattlerDisplay() {
            base.setupBattlerDisplay();
            display.setItem(new RuntimeSeperation());
        }

        #endregion

        #region 技能控制

        /// <summary>
        /// 受击回调
        /// </summary>
        protected override void onHit() {
            base.onHit();
            //onDie();
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