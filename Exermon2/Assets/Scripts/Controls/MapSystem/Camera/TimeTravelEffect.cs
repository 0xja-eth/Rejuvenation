using Core.UI;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 时间旅行效果（变量控制）
    /// </summary>
    public class TimeTravelEffect : GeneralComponent {

		/// <summary>
		/// 外部组件定义
		/// </summary>
		public float switchStrength = 0;

        [HideInInspector]
        public Vector2 center = new Vector2(0.5f, 0.5f);
    }
}