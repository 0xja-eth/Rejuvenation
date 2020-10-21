using Core.UI;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 时间旅行效果（变量控制）
    /// </summary>
    public class TimeTravelEffect : GeneralComponent {

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public float switchStrength = 0;
        public Vector2 center = new Vector2(0.5f, 0.5f);

		public Material material;

		/// <summary>
		/// 设置材质变量
		/// </summary>
		/// <typeparam name="T">类型</typeparam>
		/// <param name="name">变量名</param>
		/// <param name="val">值</param>
		//public void setVar<T>(string name, T val) {
		//	debugLog("setVar: " + name + "(" + typeof(T) + "): " + val);
		//	if (typeof(T) == typeof(ComputeBuffer))
		//		material.SetBuffer(name, (ComputeBuffer)(object)val);
		//	else if (typeof(T) == typeof(int))
		//		material.SetInt(name, (int)(object)val);
		//	else if (typeof(T) == typeof(double) || typeof(T) == typeof(float))
		//		material.SetFloat(name, (float)(object)val);
		//}

	}
}