
using System;
using System.Collections.Generic;

using LitJson;

using UnityEngine;

using Core.Systems;

using PlayerModule.Services;
using GameModule.Services;

namespace UI.MapSystem {
    using Controls;
    /// <summary>
    /// 测试地图场景
    /// </summary>
    public class TutorialScene : BaseMapScene {

        /// <summary>
        /// 外部变量
        /// </summary>
        /// 需手动触发的事件
        public MapEvent attackEndTrigger;
        public MapEvent flashFailTrigger;
        public Collider2D flashWall;

		/// <summary>
		/// 外部系统定义
		/// </summary>
		protected GameService gameSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOthers() {
            base.initializeOthers();
		}

		/// <summary>
		/// 开始
		/// </summary>
		protected override void start() {
			//var stage = playerSer.startGame(true);
			//changeStage(stage);
			base.start();
		}

		/// <summary>
		/// 场景索引
		/// </summary>
		/// <returns></returns>
		public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.TutorialScene;
        }

		/// <summary>
		/// 下一关
		/// </summary>
		/// <returns></returns>
		public override SceneSystem.Scene nextStage() {
			return SceneSystem.Scene.TaiqingScene;
		}

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
            base.update();
            if (gameSer.tutorialRobotDie)
                onAttackEnd();
            if (gameSer.tutorialFlashFail)
                onFlashFail();
        }
        #endregion

        #region 事件控制

        /// <summary>
        /// 战斗结束回调
        /// </summary>
        void onAttackEnd() {
            debugLog("attack end");
            attackEndTrigger.processTrigger(player, MapEventPage.TriggerType.Initiactive);
        }

        /// <summary>
        /// 闪烁教程开始回调
        /// </summary>
        public void onFlashStart() {
            //debugLog("flash wall x: " + flashWall.bounds.max.x);
            gameSer.tutorialFlash = true;
            gameSer.tutorialFlashPosX = flashWall.bounds.max.x;
        }

        /// <summary>
        /// 闪烁失败回调
        /// </summary>
        void onFlashFail() {
            debugLog("flash on fail");
            flashFailTrigger.processTrigger(player, MapEventPage.TriggerType.Initiactive);
        }

        #endregion
    }
}