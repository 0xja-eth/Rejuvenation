using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UI.Common.Controls.ItemDisplays;

using MapModule.Data;

namespace UI.MapSystem.Controls {

    using Windows;

    /// <summary>
    /// 对话框显示
    /// </summary>
    [RequireComponent(typeof(DialogWindow))]
    public class MessageDisplay : MessageBaseDisplay {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public new Text name = null;
        public GameObject nameFrame = null;
        public Image bust = null;

        #region 界面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(DialogMessage item) {
            base.drawExactlyItem(item);
            drawName(item);
            drawBust(item);
        }

        /// <summary>
        /// 绘制名称
        /// </summary>
        /// <param name="item"></param>
        void drawName(DialogMessage item) {
            nameFrame?.SetActive(!string.IsNullOrEmpty(item.name));
            name.text = item.name;
        }

        /// <summary>
        /// 绘制立绘
        /// </summary>
        /// <param name="item"></param>
        void drawBust(DialogMessage item) {

            if (MessageSender.busts(item.name) != null)
                bust.overrideSprite = MessageSender.busts(item.name)[0];
            else
                bust.overrideSprite = null;

            bust.gameObject.SetActive(bust.overrideSprite != null);
            //this.bust.gameObject.SetActive(true);
            //this.bust.SetNativeSize();
        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            name.text = "";
            nameFrame?.SetActive(false);
            bust.gameObject.SetActive(false);
            bust.overrideSprite = null;
            bust.SetNativeSize();
        }

        #endregion

    }
}
