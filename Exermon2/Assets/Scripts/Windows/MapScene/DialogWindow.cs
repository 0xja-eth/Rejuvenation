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
	[RequireComponent(typeof(MessageBaseDisplay))]
	public class DialogWindow : BaseWindow {

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
        MessageBaseDisplay display;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		MessageService messageSer;
		GameService gameSer;

		/// <summary>
		/// 场景组件
		/// </summary>
		new BaseMapScene scene => base.scene as BaseMapScene;
		
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

		#region 内容绘制

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			var msg = messageSer.getMessage();
			display.setItem(msg);
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
		
    }
}
