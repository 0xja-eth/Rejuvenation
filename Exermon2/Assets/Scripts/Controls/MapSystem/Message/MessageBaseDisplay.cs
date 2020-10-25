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
    public class MessageBaseDisplay : ItemDisplay<DialogMessage> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text message;
		public Image image;
		public GameObject imageFrame;

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public bool setNativeSize = true;
		public float printDeltaTime = 0.05f; // 文本打印间隔时间

        /// <summary>
        /// 内部变量定义
        /// </summary>
        bool stopPrintReq = false; // 停止打印请求（打印到最后一个）

        /// <summary>
        /// 属性
        /// </summary>
        public bool printing { get; protected set; } = false; // 当前是否打印中

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			debugLog("initializeOnce: " + imageFrame);
			debugLog("initializeOnce: " + image?.gameObject);
			if(!imageFrame) imageFrame = image?.gameObject;
			debugLog("initializeOnce: " + imageFrame);
		}

		#endregion

		/// <summary>
		/// 选项数目
		/// </summary>
		/// <returns></returns>
		public int optionCount() {
            return item.options.Count;
        }

        #region 界面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(DialogMessage item) {
            base.drawExactlyItem(item);
            drawMessage(item);
            drawImage(item);
        }

        /// <summary>
        /// 绘制信息
        /// </summary>
        /// <param name="item"></param>
        void drawMessage(DialogMessage item) {
            doRoutine(printMessage(item.message));
        }

		/// <summary>
		/// 获取立绘
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual Sprite getBust(DialogMessage item) {
			return item.bust();
		}

        /// <summary>
        /// 绘制立绘
        /// </summary>
        /// <param name="item"></param>
        virtual protected void drawImage(DialogMessage item) {
			debugLog("drawImage: " + imageFrame);

			if (!imageFrame || !image) return;

			var bust = getBust(item);

			imageFrame.gameObject.SetActive(bust != null);

			image.overrideSprite = bust;
			if (setNativeSize)
				image.SetNativeSize();
		}

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            message.text = "";
			if (imageFrame && image) {
				image.overrideSprite = null;
				imageFrame.gameObject.SetActive(false);
			}
        }

        /// <summary>
        /// 停止打印
        /// </summary>
        public void stopPrint() {
            stopPrintReq = true;
        }

        /// <summary>
        /// 打印信息
        /// </summary>
        /// <returns></returns>
        IEnumerator printMessage(string message) {
            onPrintStart();

            foreach (var c in message) {
                this.message.text += c;
                if (stopPrintReq) {
                    this.message.text = message;
                    break;
                }
                yield return new WaitForSeconds(printDeltaTime);
            }

            onPrintEnd();
        }

        /// <summary>
        /// 打印开始回调
        /// </summary>
        virtual protected void onPrintStart() {
            printing = true;
            message.text = "";
        }

        /// <summary>
        /// 打印结束回调
        /// </summary>
        virtual protected void onPrintEnd() {
            stopPrintReq = printing = false;
        }

        #endregion

    }
}
