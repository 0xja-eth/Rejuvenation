
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
    /// 太清房间地图场景
    /// </summary>
    public class FusangCopyScene : BaseMapScene {

		/// <summary>
		/// 外部变量
		/// </summary>
		public Vector2 copyStartPos;

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
            playerSer.createPlayer("TestPlayer");
            playerSer.actor.runtimeActor.direction = MapModule.Data.RuntimeCharacter.Direction.Up;
            playerSer.actor.runtimeActor.addEnergy(100);
        }

		/// <summary>
		/// 开始
		/// </summary>
		protected override void start() {
			base.start();
        }

		/// <summary>
		/// 玩家启动回调
		/// </summary>
		public override void onPlayerStart() {
			base.onPlayerStart();
			player.addSeperation(copyStartPos);
		}

		/// <summary>
		/// 场景索引
		/// </summary>
		/// <returns></returns>
		public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.FusangCopyScene;
		}

		/// <summary>
		/// 下一关
		/// </summary>
		/// <returns></returns>
		public override SceneSystem.Scene nextStage() {
			return SceneSystem.Scene.FinalScene;
		}

		#endregion
		
    }
}