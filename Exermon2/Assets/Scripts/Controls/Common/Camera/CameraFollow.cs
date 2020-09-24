using Core.UI;
using UnityEngine;

namespace UI.Common.Controls {
    public class CameraFollow : BaseComponent {

        public Transform target;
        public Collider range;
        public float smoothing = 0.1f;

        private Camera thisCamera;

        protected override void initializeOnce() {
            base.initializeOnce();
            thisCamera = GetComponent<Camera>();
        }

        // Update is called once per frame
        void FixedUpdate() {
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
            var pos = Vector3.Lerp(target.position, transform.position, smoothing);
            // 限制可移动范围
            float x = Mathf.Clamp(pos.x, minRange.x + visibleWidth, maxRange.x - visibleWidth);
            float y = Mathf.Clamp(pos.y, minRange.y + visibleHeight, maxRange.y - visibleHeight);

            transform.position = new Vector3(x, y, transform.position.z);
        }
    }
}