using UnityEngine;

using UI.Common.Controls.AnimationSystem;
using UI.Common.Controls.ItemDisplays;

using MapModule.Data;
using BattleModule.Data;
using UnityEngine.UI;
using UI.MapSystem;
using Core.UI.Utils;
using Core.Systems;

namespace UI.Controls {

    /// <summary>
    /// 战斗者状态显示
    /// </summary>
    //[RequireComponent(typeof(SpriteRenderer))]
    public class MainUIDisplay : ItemDisplay<UIShowAttributes> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image hpBar = null;
        public Image powerBar = null;
        public Text keyNumber;

        /// <summary>
        /// 场景组件
        /// </summary>
        BaseMapScene scene => SceneUtils.getCurrentScene() as BaseMapScene;

        /// <summary>
        /// 外部系统
        /// </summary>
        SceneSystem sceneSys;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();

            if (isNullItem(item)) return;
            updateHPChange();
            updatePower();
            updateKey();
        }

        /// <summary>
        /// 更新HP改变
        /// </summary>
        void updateHPChange() {
            if (hpBar && item.hpRate != hpBar.fillAmount)
                drawHP(item);
        }

        /// <summary>
        /// 更新能量
        /// </summary>
        void updatePower() {
            //item.runtimeActor.addEnergy(Random.Range(-10, 10));
            if (powerBar && item.powerRate != powerBar.fillAmount)
                drawPower(item);
        }

        /// <summary>
        /// 更新钥匙
        /// </summary>
        void updateKey() {
            if (keyNumber && item.keyNumber.ToString() != keyNumber.text)
                drawKey(item);
        }

        #endregion

        #region 数据操作

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool isNullItem(UIShowAttributes item) {
            return base.isNullItem(item) || item.runtimeActor == null;
        }

        #endregion

        #region 界面刷新

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(UIShowAttributes item) {
            base.drawExactlyItem(item);
            drawHP(item);
            drawPower(item);
            drawKey(item);
        }

        /// <summary>
        /// 绘制HP
        /// </summary>
        void drawHP(UIShowAttributes item) {
            if (hpBar) {
                hpBar.fillAmount = item.hpRate;
                debugLog("draw hp:" + item.hpRate);
            }
        }

        /// <summary>
        /// 绘制能量
        /// </summary>
        /// <param name="item"></param>
        void drawPower(UIShowAttributes item) {
            if (powerBar) {
                powerBar.fillAmount = item.powerRate;
                debugLog("draw power:" + item.powerRate);
            }
        }

        /// <summary>
        /// 绘制钥匙
        /// </summary>
        /// <param name="item"></param>
        void drawKey(UIShowAttributes item) {
            if (keyNumber)
                keyNumber.text = item.keyNumber.ToString();
            debugLog("draw Key:" + item.keyNumber);

        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            if (keyNumber) keyNumber.text = "";
        }

        #endregion

        #region 事件回调
        /// <summary>
        /// 返回主菜单
        /// </summary>
        public void onExit() {
            sceneSys.gotoScene(SceneSystem.Scene.TitleScene);
        }

        /// <summary>
        /// 重玩
        /// </summary>
        public void onRetry() {
            scene.restartStage(false);
        }
        #endregion
    }
}