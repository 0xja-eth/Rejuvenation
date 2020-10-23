using Core.UI;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace UI.MapSystem.Controls {

	/// <summary>
	/// 镜头挂载脚本，控制镜头状态
	/// </summary>
	[RequireComponent(typeof(Map))]
	public class CameraController : WorldComponent {

		/// <summary>
		/// 外部组件定义
		/// </summary>
		public Collider2D range; // 限制范围

		/// <summary>
		/// 属性
		/// </summary>
		Transform target => map?.player?.transform; // 跟随目标

		new Camera camera => map.camera;
		Transform cTransform => camera.transform; // 摄像机Transform

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public float smoothing = 0.1f; // 镜头运动平滑系数

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget] Map map;

		#region 初始化

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			if (isEnable()) updateCameraPos();
		}

		/// <summary>
		/// 是否有效
		/// </summary>
		/// <returns></returns>
		public bool isEnable() {
			return map.active && camera && target; // && !map.scene.traveling;
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