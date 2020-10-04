using Core.UI;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using GameModule.Services;

using MapModule.Services;
using MapModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.MapSystem.Windows {

	/// <summary>
	/// 对话窗口层
	/// </summary>
	[RequireComponent(typeof(OptionConDisplay))]
	public class DialogWindow : BaseWindow {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text dialogText;
		public Text nameText;
		public Image entityImage;
		//public Sprite entitySprite1;
		//public Sprite entitySprite2;
		public OptionConDisplay optionsDisplay;

		/// <summary>
		/// 外部变量设置
		/// </summary>
        public int offset = 5;
        public int msgLen;
        public int curMsgLen = 0;
        public string curMsg;
        public int msgCnt = 0;
        public bool chosen = false;
        public int dialogSize = 4;
        public int optionSize = 3;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		bool isInputing = false;
		bool getNext = false;

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
			messageSer.isDialogued = true;
			getNext = true;
			msgCnt = 0;

			// 用于测试
			for (int i = 0; i < dialogSize; i++) {
				DialogMessage dialogMessage = genRandMsg();

				messageSer.addMessage(dialogMessage);
			}

			scene.map1.player?.stop();
			scene.map2.player?.stop();
		}

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
			
            if (messageSer.isDialogued) {
                if (getNext) {
                    if (messageSer.messageCount() == 0) {
                        messageSer.isDialogued = false;
                        base.deactivate();
                    }
                    else {
                        DialogMessage dMsg = getDialogMessage();
                        optionsDisplay.setItems(dMsg.options);
                        getNext = false;
                        isInputing = true;
                    }
                }
                else if (!getNext && isInputing) {

                    dialogText.text = nextText();

                    if (curMsgLen >= msgLen + offset) {
                        isInputing = false;
                        //文字未完全展开时选项不激活
                        foreach (var item in optionsDisplay.getItemDisplays())
                            item.actived = true;
                    }
                }
            }
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
		/// 从队列获取消息
		/// </summary>
		DialogMessage getDialogMessage() {

            DialogMessage dMsg = messageSer.getMessage();



            dialogText.text = dMsg.message;
            nameText.text = dMsg.name;
            entityImage.sprite = dMsg.bust();

            curMsg = dMsg.message;
            curMsgLen = 0;
            msgLen = curMsg.Length;
            offset = 1;
            msgCnt++;
            return dMsg;
        }

        /// <summary>
        /// 下一条消息或者快速展开文字
        /// </summary>
        void nextOrRevealAll() {
            if (isInputing)
                curMsgLen = msgLen - 1;
            else if (optionsDisplay.itemsCount() == 0)
                getNext = true;
        }

        /// <summary>
        /// 下一段文字
        /// </summary>
        string nextText() {
            string text = curMsg.Substring(0, Mathf.Clamp(curMsgLen, 0, msgLen));
            curMsgLen += offset;
            return text;
        }
        
        /// <summary>
        /// 开始对话
        /// </summary>
        public void beginDialogue() {
            messageSer.isDialogued = true;
            getNext = true;
            msgCnt = 0;
            for (int i = 0; i < dialogSize; i++) {
                DialogMessage dialogMessage = genRandMsg();

                messageSer.addMessage(dialogMessage);
            }
            scene.map1.player?.stop();
            scene.map2.player?.stop();
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
