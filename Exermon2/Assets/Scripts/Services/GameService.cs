using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;

using Core.Data;
using Core.Services;
using Core.Systems;

using Core.UI.Utils;

using PlayerModule.Services;

/// <summary>
/// 基本系统
/// </summary>
namespace GameModule.Services {
    
    /// <summary>
    /// Exermon控制类
    /// </summary>
    /// <remarks>
    /// 控制整个游戏进程（游戏内流程）
    /// </remarks>
    public class GameService : BaseService<GameService> {

        /// <summary>
        /// 游戏配置（设置）
        /// </summary>
        public ConfigureData configure { get; protected set; } = new ConfigureData();

        /// <summary>
        /// 外部系统
        /// </summary>
        SceneSystem sceneSys;
        StorageSystem storageSys;

		PlayerService playerSer;

   //     /// <summary>
   //     /// 初始化外部系统
   //     /// </summary>
   //     protected override void initializeSystems() {
   //         base.initializeSystems();
   //         sceneSys = SceneSystem.get();
   //         storageSys = StorageSystem.get();
			//playerSer = PlayerService.get();
   //     }

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOthers() {
			base.initializeOthers();
			gameSys.reconnectedCallback = onReconnected;
		}

		#region 配置数据快捷获取

		/// <summary>
		/// 按键设定
		/// </summary>
		public KeyboardConfig keyboard => configure.keyboard;

		#endregion

		#region 流程控制

		/// <summary>
		/// 开始游戏（根据用户是否创建角色自动分配实际执行的操作）
		/// </summary>
		public void startGame() {
            storageSys.save();
        }

        /// <summary>
        /// 新游戏（未有角色的玩家）
        /// </summary>
        public void newGame() {
			// TODO: 新游戏
        }

        /// <summary>
        /// 继续游戏（已经有角色的玩家）
        /// </summary>
        public void loadGame() {
			// TODO: 读取游戏
        }

        /// <summary>
        /// 游戏登出
        /// </summary>
        public void logoutGame() {
			// TODO: 退出游戏
			playerSer.clearPlayer();
        }

        /// <summary>
        /// 返回主菜单
        /// </summary>
        public void backToMenu() {
			beforeQuit();
			sceneSys.gotoScene(SceneSystem.Scene.TitleScene);
        }

        /// <summary>
        /// 结束游戏
        /// </summary>
        public void exitGame() {
            gameSys.terminate(beforeQuit);
        }

        /// <summary>
        /// 结束前操作
        /// </summary>
        void beforeQuit() {
            if (playerSer.isPlaying()) logoutGame();
        }

        /// <summary>
        /// 关卡切换
        /// </summary>
        public void switchCheckPoint() {
            sceneSys.pushScene(SceneSystem.Scene.SwitchScene);
        }

        #endregion

        #region 回调控制

        /// <summary>
        /// 重连回调
        /// </summary>
        void onReconnected() {
            //Debug.Log("onReconnected: " + playerSer.isPlaying());
            //if (playerSer.isLogined()) playerSer.reconnect();
        }

        bool _tutorialRobotDie = false;
        /// <summary>
        /// 新手教程死亡回调
        /// 自动重置
        /// </summary>
        public bool tutorialRobotDie {
            get {
                var res = _tutorialRobotDie;
                _tutorialRobotDie = false;
                return res; }
        }
        /// <summary>
        /// 新手教程机器人死亡回调
        /// </summary>
        public void onTutorialRobotDie() {
            _tutorialRobotDie = true;
        }

        /// <summary>
        /// 新手教程闪烁教程状态判定
        /// </summary>
        public bool tutorialFlash { get; set; } = false;
        public float tutorialFlashPosX { get; set; } = -1;
        bool _tutorialFlashFail = false;
        /// <summary>
        /// 新手教程死亡回调
        /// 自动重置
        /// </summary>
        public bool tutorialFlashFail {
            get {
                var res = _tutorialFlashFail;
                _tutorialFlashFail = false;
                return res;
            }
        }
        public void onTutorialFlashFail() {
            _tutorialFlashFail = true;
        }

        #endregion
    }

}