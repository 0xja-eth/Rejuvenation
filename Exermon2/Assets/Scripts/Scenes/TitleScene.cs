using Core.Systems;
using Core.UI;
using PlayerModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.MapSystem.Windows;

namespace Assets.Scripts.Scenes {
    public class TitleScene : BaseScene {

        public IllustrationWindow illustrationWindow = null;


        SceneSystem sceneSys;
        PlayerService playerSer;

        /// <summary>
        /// 场景索引
        /// </summary>
        /// <returns></returns>
        public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.TitleScene;
        }

        public void beginGame() {
            var stage = playerSer.startGame(false);
            debugLog("start " + stage);
            setupStartIllustration();
        }

        public void continueGame() {
            var stage = playerSer.startGame(true);
            sceneSys.pushScene(stage);
        }


        #region 流程控制
        /// <summary>
        /// 开始游戏
        /// </summary>
        public void startGame() {
            sceneSys.pushScene(playerSer.player.stage);
        }

        /// <summary>
        /// 开始背景插画
        /// </summary>
        void setupStartIllustration() {
            if (playerSer.player.firstStart) {
                illustrationWindow?.activate();
            }
            //test
            else {
                //playerSer.player.firstStart = false;
            }
        }
        #endregion
    }
}