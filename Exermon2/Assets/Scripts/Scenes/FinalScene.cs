
using System;
using System.Collections.Generic;

using LitJson;

using UnityEngine;

using Core.Systems;

using PlayerModule.Services;
using GameModule.Services;

namespace UI.MapSystem {
    using Controls;
    using UI.MapSystem.Windows;

    /// <summary>
    /// 最终场景
    /// </summary>
    public class FinalScene : BaseMapScene {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public IllustrationWindow illustrationWindow = null;

        /// <summary>
        /// 外部系统定义
        /// </summary>
        GameService gameSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOthers() {
            base.initializeOthers();
			//playerSer.createPlayer("TestPlayer");
			//playerSer.actor.runtimeActor.direction = MapModule.Data.RuntimeCharacter.Direction.Up;
			//playerSer.actor.runtimeActor.addEnergy(100);
		}

		/// <summary>
		/// 场景索引
		/// </summary>
		/// <returns></returns>
		public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.FinalScene;
		}

		/// <summary>
		/// 下一关
		/// </summary>
		/// <returns></returns>
		public override SceneSystem.Scene nextStage() {
			return SceneSystem.Scene.NoneScene;
		}
        
        /// <summary>
        /// 游戏结束
        /// </summary>
        protected override void onGameWin() {
			illustrationWindow?.activate();
        }

		#endregion
		
    }
}