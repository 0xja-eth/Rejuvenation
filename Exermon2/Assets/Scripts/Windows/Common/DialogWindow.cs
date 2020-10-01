using Core.UI;
using GameModule.Services;
using MapModule.Data;
using System.Collections.Generic;
using UI.Common.Controls.ItemDisplays;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 通用窗口
/// </summary>
namespace UI.Common.Windows {

    /// <summary>
    /// 对话窗口层
    /// </summary>
    public class DialogWindow : BaseWindow {
        /// <summary>
        /// 内部变量设置
        /// </summary>
        bool isInputing = false;
        bool getNext = false;
        public int offset = 5;
        public int msgLen;
        public int curMsgLen = 0;
        public string curMsg;
        public int msgCnt = 0;
        public bool chosen = false;
        public int dialogSize = 4;
        public int optionSize = 3;
        /// <summary>
        /// 外部系统设置
        /// </summary>
        MessageServices msgServices;


        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text dialogText;
        public Text nameText;
        public Image entityImage;
        public Sprite entitySprite1;
        public Sprite entitySprite2;
        public OptionConDisplay optionConDisplay;

        public BaseMapScene.BaseMapScene scene;

        #region 初始化

        public override void activate() {
            base.activate();
            beginDialog();
        }

        protected override void initializeEvery() {
            base.initializeEvery();
        }

        #endregion

        #region 更新控制

        protected override void update() {
            base.update();

            if (Input.GetKeyDown(KeyCode.U)) {
                nextOrRevealAll();
            }

            if (msgServices.isDialogued) {
                if (getNext) {
                    if (msgServices.getMsgLength() == 0) {
                        msgServices.isDialogued = false;
                        base.deactivate();
                    }
                    else {
                        DialogMessage dMsg = getDialogMessage();
                        optionConDisplay.setItems(dMsg.options);
                        getNext = false;
                        isInputing = true;
                    }
                }
                else if (!getNext && isInputing) {

                    dialogText.text = nextText();

                    if (curMsgLen >= msgLen + offset) {
                        isInputing = false;
                    }
                }
            }
        }
        #endregion

        DialogMessage getDialogMessage() {

            DialogMessage dMsg = msgServices.getMessage();
            dialogText.text = dMsg.message;
            nameText.text = dMsg.name;
            entityImage.sprite = dMsg.sprite;

            curMsg = dMsg.message;
            curMsgLen = 0;
            msgLen = curMsg.Length;
            offset = 1;
            msgCnt++;
            return dMsg;
        }

        void nextOrRevealAll() {
            if (isInputing)
                curMsgLen = msgLen - 1;
            else if (optionConDisplay.itemsCount() == 0)
                getNext = true;
        }

        string nextText() {
            string text = curMsg.Substring(0, Mathf.Clamp(curMsgLen, 0, msgLen));
            curMsgLen += offset;
            return text;
        }

        #region 消息事件

        public void beginDialog() {
            msgServices.isDialogued = true;
            getNext = true;
            msgCnt = 0;
            for (int i = 0; i < dialogSize; i++) {
                DialogMessage dialogMessage = genRandMsg();

                msgServices.addMessage(dialogMessage);
            }
            scene.map1.player?.stop();
            scene.map2.player?.stop();
        }

        DialogMessage genRandMsg() {

            DialogMessage dialogMessage = new DialogMessage();
            int rand = UnityEngine.Random.Range(1, 10);
            dialogMessage.message = new string('哈', UnityEngine.Random.Range(50, 100));
            dialogMessage.name = rand % 2 == 0 ? "墨文" : "墨文？";
            dialogMessage.sprite = rand % 2 == 0 ? entitySprite1 : entitySprite2;

            List<DialogOption> options = new List<DialogOption>();
            for (int i = 0; i < Mathf.Clamp(optionSize, 0, 4); i++) {
                options.Add(genRandOpt());
            }
            dialogMessage.options = options;

            return dialogMessage;
        }

        DialogOption genRandOpt() {
            DialogOption dialogOption = new DialogOption();
            int rand = UnityEngine.Random.Range(1, 10);
            dialogOption.description = new string('嚯', UnityEngine.Random.Range(5, 10));
            dialogOption.action = () =>
            {
                getNext = true;
                Debug.Log(dialogOption.description);
            };

            return dialogOption;
        }


        #endregion
    }
}
