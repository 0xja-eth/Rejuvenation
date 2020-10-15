using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using GameModule.Services;

using Event = MapModule.Data.Event;

namespace UI.BattleSystem.Controls.Enemies {

	/// <summary>
	/// 地图上的事件
	/// </summary>
	public class RobotA : MapEnemy {

        /// <summary>
        /// 外部系统
        /// </summary>
        GameService gameService;

		/// <summary>
		/// 敌人ID
		/// </summary>
		public override int enemyId => 1;

        #region 更新

        /// <summary>
        /// 死亡回调
        /// </summary>
        protected override void onDie() {
            base.onDie();
            gameService.onTutorialRobotDie();
        }
        #endregion

    }
}