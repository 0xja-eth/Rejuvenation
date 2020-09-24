using Core.UI;
using UnityEngine;

namespace UI.Common.Controls {
    /// <summary>
    /// 镜头挂载脚本，控制镜头状态
    /// </summary>
    public class CameraFollow : BaseComponent {

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public Transform target;
        /// <summary>
        /// 地图范围
        /// </summary>
        public Collider range;
        /// <summary>
        /// 镜头运动平滑系数
        /// </summary>
        public float smoothing = 0.1f;

        public Camera thisCamera;

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            if (thisCamera == null)
                thisCamera = GetComponent<Camera>();
        }
        #endregion

        #region 更新
        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateCameraPos();
        }
        /// <summary>
        /// 更新镜头位置
        /// </summary>
        void updateCameraPos() {
            float visibleHeight;
            float visibleWidth;
            var type = thisCamera.orthographic;
            if (type) {
                visibleHeight = thisCamera.orthographicSize;
            }
            else {
                float distance = Mathf.Abs(transform.position.z);
                // 计算摄像机可视区域宽度与高度的一半
                visibleHeight = distance * Mathf.Tan(thisCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            }
            visibleWidth = visibleHeight * thisCamera.aspect;

            Vector3 minRange = range.bounds.min;
            Vector3 maxRange = range.bounds.max;

            // 线性插值
            var pos = Vector3.Lerp(target.position, transform.position, smoothing);
            // 限制可移动范围
            float x = Mathf.Clamp(pos.x, minRange.x + visibleWidth, maxRange.x - visibleWidth);
            float y = Mathf.Clamp(pos.y, minRange.y + visibleHeight, maxRange.y - visibleHeight);

            transform.position = new Vector3(x, y, transform.position.z);
        }

        #endregion

        #region 回调
        /// <summary>
        /// 分屏回调，设置镜头Viewport
        /// </summary>
        /// <param name="rect"></param>
        public void onSplitCamera(Rect rect) {
            thisCamera.rect = rect;
        }

        #endregion

    }
}