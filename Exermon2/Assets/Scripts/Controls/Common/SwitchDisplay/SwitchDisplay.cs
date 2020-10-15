using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Core.Data.Loaders;
using GameModule.Data;
using Core.Systems;
using Core.UI;
using UI.Common.Controls.ItemDisplays;

namespace UI.Common.Controls {

    /// <summary>
    /// 休息据点显示
    /// </summary>
    [RequireComponent(typeof(BaseWindow))]
    public class SwitchDisplay :
        ItemDetailDisplay<GameSwitchData>, IPointerClickHandler {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string TipFormat = "{0}：{1}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image background;
        public Text text;
        public Sprite[] textures;

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public float lastTime = 10; // 持续时间（秒）

        /// <summary>
        /// 内部变量定义
        /// </summary>
        float sumTime = 0;
        SceneSystem sceneSystem;
        [RequireTarget]
        BaseWindow window;

        /// <summary>
        /// 外部系统设置
        /// </summary>

        #region 初始化

        #endregion

        #region 开启/结束控制

        /// <summary>
        /// 开启视窗
        /// 但实际不调用该函数，使用时应调用：activate(T item, int index = -1)；
        /// </summary>
        public override void activate() {
            base.activate();
            sumTime = 0;
            window?.show();
        }

        /// <summary>
        /// 结束视窗
        /// </summary>
        public override void deactivate() {
            //base.deactivate();
            window?.hide();
            switchScene();
        }

        #endregion

        #region 场景切换
        /// <summary>
        /// 场景切换
        /// </summary>
        void switchScene() {
            //sceneSystem.popScene();
            sceneSystem.pushScene(SceneSystem.Scene.TestScene);
        }
        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateTerminate();
        }

        /// <summary>
        /// 更新结束
        /// </summary>
        void updateTerminate() {
            if ((sumTime += Time.deltaTime) >= lastTime)
                deactivate();
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="item">题目</param>
        protected override void drawExactlyItem(GameSwitchData item) {
            base.drawExactlyItem(item);
            drawBaseInfo(item);
        }

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="item"></param>
        void drawBaseInfo(GameSwitchData item) {
            background.overrideSprite = generateRandomBackground();
            text.text = string.Format(TipFormat, item.name, item.description);
        }

        /// <summary>
        /// 生成随机背景
        /// </summary>
        /// <returns></returns>
        Sprite generateRandomBackground() {
            var index = Random.Range(0, textures.Length);
            return textures[index];
        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            text.text = "";
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 点击回调
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData) {
            //if (!window.isBusy()) deactivate();
        }

        #endregion

    }
}
