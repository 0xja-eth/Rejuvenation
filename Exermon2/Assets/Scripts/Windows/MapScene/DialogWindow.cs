using Core.UI;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using GameModule.Services;

using MapModule.Services;
using MapModule.Data;

namespace UI.MapSystem.Windows {

	using Controls;

	/// <summary>
	/// 对话窗口层
	/// </summary>
	[RequireComponent(typeof(MessageDisplay))]
	public class DialogWindow : BaseWindow {

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		MessageDisplay display;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		MessageService messageSer;
		GameService gameSer;

		/// <summary>
		/// 场景组件
		/// </summary>
		new BaseMapScene scene => base.scene as BaseMapScene;

		#region 初始化

		/// <summary>
		/// 激活
		/// </summary>
		public override void activate() {
            base.activate();
			var msg = messageSer.getMessage();
			display.setItem(msg);
			
			// 用于测试
			//for (int i = 0; i < dialogSize; i++) {
			//	DialogMessage dialogMessage = genRandMsg();

			//	messageSer.addMessage(dialogMessage);
			//}
		}

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();

			updateInput();

			//if (messageSer.isDialogued) {
   //             if (getNext) {
   //                 if (messageSer.messageCount() == 0) {
   //                     messageSer.isDialogued = false;
   //                     base.deactivate();
   //                 }
   //                 else {
   //                     DialogMessage dMsg = getDialogMessage();
   //                     getNext = false;
   //                     isInputing = true;
   //                 }
   //             }
   //             else if (!getNext && isInputing) {

   //                 dialogText.text = nextText();

   //                 if (curMsgLen >= msgLen + offset) {
   //                     isInputing = false;
   //                     //文字未完全展开时选项不激活
   //                     foreach (var item in optionsDisplay.getItemDisplays())
   //                         item.actived = true;
   //                 }
   //             }
   //         }
        }

		/// <summary>
		/// 更新输入
		/// </summary>
		void updateInput() {
			if (Input.GetKeyDown(gameSer.keyboard.nextKey))
				nextOrRevealAll();
		}

		#endregion

		#region 消息事件

        /// <summary>
        /// 下一条消息或者快速展开文字
        /// </summary>
        void nextOrRevealAll() {
            if (display.printing) display.stopPrint();
            else if (display.optionCount() <= 0) deactivate();
        }

        #endregion

        #region 测试
        /// <summary>
        /// 生成随机消息（测试）
        /// </summary>
        DialogMessage genRandMsg() {

            DialogMessage dialogMessage = new DialogMessage();
            //int rand = UnityEngine.Random.Range(1, 10);
            //dialogMessage.message = new string('哈', UnityEngine.Random.Range(50, 100));
            //dialogMessage.name = rand % 2 == 0 ? "墨文" : "墨文？";
            //dialogMessage._bust = rand % 2 == 0 ? entitySprite1 : entitySprite2;

            //List<DialogOption> options = new List<DialogOption>();
            //for (int i = 0; i < Mathf.Clamp(optionSize, 0, 4); i++) {
            //    options.Add(genRandOpt());
            //}
            //dialogMessage.options = options;

            return dialogMessage;
        }

        /// <summary>
        /// 生成随机选项（测试）
        /// </summary>
        DialogOption genRandOpt() {
            DialogOption dialogOption = new DialogOption();

            //dialogOption.description = new string('嚯', UnityEngine.Random.Range(5, 10));
            //dialogOption.actions = () =>
            //{
            //    getNext = true;
            //    Debug.Log(dialogOption.description);
            //};

            return dialogOption;
        }

        #endregion
    }
}
