using Core.UI;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 镜头挂载脚本，控制镜头状态
    /// </summary>
    public class CameraController : WorldComponent {

		/// <summary>
		/// 外部组件定义
		/// </summary>
		public Transform target; // 跟随目标
        public Collider2D range; // 限制范围

		public new Camera camera;

		/// <summary>
		/// 摄像机Transform
		/// </summary>
		Transform cTransform => camera.transform;
		
		/// <summary>
		/// 外部变量设置
		/// </summary>
		public float smoothing = 0.1f; // 镜头运动平滑系数

        #region 初始化

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
            float visibleHeight, visibleWidth;

            var type = camera.orthographic;

			if (type) {
                visibleHeight = camera.orthographicSize;
            } else {
                float distance = Mathf.Abs(cTransform.position.z);
                // 计算摄像机可视区域宽度与高度的一半
                visibleHeight = distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            }
            visibleWidth = visibleHeight * camera.aspect;

			Vector3 minRange = range.bounds.min;
            Vector3 maxRange = range.bounds.max;

            // 线性插值
            var pos = Vector3.Lerp(target.position, cTransform.position, smoothing);
            // 限制可移动范围
            float x = Mathf.Clamp(pos.x, minRange.x + visibleWidth, maxRange.x - visibleWidth);
            float y = Mathf.Clamp(pos.y, minRange.y + visibleHeight, maxRange.y - visibleHeight);

			cTransform.position = new Vector3(x, y, cTransform.position.z);
            debugLog(cTransform.position);
            debugLog(camera.transform.position);
        }

        #endregion

    }
}