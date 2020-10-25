using Core.Systems;
using Core.UI;
using PlayerModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Scenes {
    public class TitleScene : BaseScene {

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
            sceneSys.pushScene(stage);
        }

        public void continueGame() {
            var stage = playerSer.startGame(true);
            sceneSys.pushScene(stage);
        }
    }
}