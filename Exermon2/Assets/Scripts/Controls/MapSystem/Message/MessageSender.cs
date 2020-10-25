using System;
using System.Collections.Generic;

using UnityEngine;

using Core.UI;

using MapModule.Services;

using MapModule.Data;
using Core.Data.Loaders;
using Config;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 消息发送组件
    /// 实际使用还需要继承该类，用于添加触发条件
    /// </summary>
    public class MessageSender : MapEventPage {

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public bool isDialog = true;

        [SerializeField]
        public List<DialogMessage> messages = new List<DialogMessage>();

        /// <summary>
        /// 外部系统设置
        /// </summary>
        protected MessageService messageSer;

        /// <summary>
        /// 内部变量设置
        /// </summary>
        static Dictionary<string, int> bustIdDict = new Dictionary<string, int>();
        string wangzi = "王子";
        string zhizi = "智子";


        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            if (!bustIdDict.ContainsKey(wangzi))
                bustIdDict.Add(wangzi, 1);
            if (!bustIdDict.ContainsKey(zhizi))
                bustIdDict.Add(zhizi, 2);
        }

        #region 事件调用

        /// <summary>
        /// 自定义调用
        /// </summary>
        protected override void invokeCustom() {
            base.invokeCustom();
            messageSer.addMessages(messages);
            if (!isDialog)
                messageSer.DialogFlag = false;
            else
                messageSer.DialogFlag = true;
        }

        #endregion

        /// <summary>
        /// 获取立绘
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Sprite[] busts(string name) {
            if (bustIdDict.ContainsKey(name))
                return AssetLoader.loadAssets<Sprite>(
                    Asset.Type.Bust, bustIdDict[name]);
            return null;
        }

        /// <summary>
        /// 测试事件
        /// </summary>
        public void A() {
            debugLog("AAA");
        }
        public void B() {
            debugLog("BBB");
        }
        public void C() {
            debugLog("CCC");
        }
    }
}
