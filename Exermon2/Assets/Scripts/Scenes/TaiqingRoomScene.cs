
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
    public class TaiqingRoomScene : BaseMapScene {

        /// <summary>
        /// 外部变量
        /// </summary>

        /// <summary>
        /// 外部系统定义
        /// </summary>
        PlayerService playerSer;
        GameService gameSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOthers() {
            base.initializeOthers();
            playerSer.createPlayer("TestPlayer");
            playerSer.actor.runtimeActor.direction = MapModule.Data.RuntimeCharacter.Direction.Up;
        }

        /// <summary>
        /// 场景索引
        /// </summary>
        /// <returns></returns>
        public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.TaiqingRoomScene;
        }

        #endregion

        #region 事件控制

        /// <summary>
        /// 退出回调
        /// </summary>
        public override void onTerminated() {
            base.onTerminated();
            gameSer.switchCheckPoint();
        }

        #endregion
    }
}