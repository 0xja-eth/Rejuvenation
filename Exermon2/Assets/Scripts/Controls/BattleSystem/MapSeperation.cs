

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

		/// <summary>
		/// 是否主体
		/// </summary>
		/// <returns></returns>
		public override bool isMaster() {
			return false;
		}

		#endregion

		#region 技能控制

		/// <summary>
		/// 受击回调
		/// </summary>
		protected override void onHit() {
            base.onHit();
        }

        /// <summary>
        /// 设置闪烁位置
        /// 由player控制
        /// </summary>
        protected override void setFlashPos() { }

        #endregion
		
    }
}