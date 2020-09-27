using Core.UI;
using UnityEngine;

namespace UI.Common.Controls {
    /// <summary>
    /// 镜头分屏控制器
    /// </summary>
    public class CameraController : BaseComponent {
        /// <summary>
        /// 外部变量
        /// </summary>
        public CameraFollow leftCamera;
        public CameraFollow rightCemera;
        public GameObject interval;

        protected override void initializeOnce() {
            base.initializeOnce();
            onSplitCameras();
        }

        #region 回调
        /// <summary>
        /// 分屏回调
        /// </summary>
        /// <param name="ratio"></param>
        void onSplitCameras(float ratio = 0.5f) {
            var width = 1 * ratio;
            Rect rect1 = new Rect(new Vector2(0, 0), new Vector2(width, 1));
            Rect rect2 = new Rect(new Vector2(width, 0), new Vector2(width, 1));
            leftCamera?.onSplitCamera(rect1);
            rightCemera?.onSplitCamera(rect2);

            interval?.SetActive(true);
        }
        #endregion
    }
}
