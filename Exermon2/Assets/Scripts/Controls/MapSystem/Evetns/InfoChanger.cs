
using System;

using UnityEngine;
using UnityEngine.Events;

using PlayerModule.Services;
using PlayerModule.Data;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 改变游戏信息
    /// </summary>
    public class InfoChanger : MapEventPage {

		/// <summary>
		/// 开关更改
		/// </summary>
		[Serializable]
		public class SwitchChange {

			/// <summary>
			/// 变量
			/// </summary>
			public Info.Switches switch_ = Info.Switches.None;
			public bool val = true;

			/// <summary>
			/// 调用
			/// </summary>
			public void invoke() {
				var playerSer = PlayerService.Get();
				playerSer.info.setSwitch(switch_, val);
			}
		}

		/// <summary>
		/// 变量更改
		/// </summary>
		[Serializable]
		public class VariableChange {

			/// <summary>
			/// 变量
			/// </summary>
			public Info.Variables variable = Info.Variables.None;
			public float val = 0;

			/// <summary>
			/// 调用
			/// </summary>
			public void invoke() {
				var playerSer = PlayerService.Get();
				playerSer.info.setVariable(variable, val);
			}
		}

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public SwitchChange[] switches;
		public VariableChange[] variables;

		/// <summary>
		/// 执行
		/// </summary>
		protected override void invokeCustom() {
			base.invokeCustom();
			foreach (var s in switches) s.invoke();
			foreach (var v in variables) v.invoke();
		}

	}
}
