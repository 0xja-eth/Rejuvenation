
using UnityEngine;

namespace Core.Data {

	/// <summary>
	/// 配置数据
	/// </summary>
	public class ConfigureData : BaseData {

		/// <summary>
		/// 设置项
		/// </summary>
		//[AutoConvert]
		//public string rememberPassword { get; set; } = null; // 记住密码
		//[AutoConvert]
		//public string rememberUsername { get; set; } = null; // 记住账号
		//[AutoConvert]
		//public bool autoLogin { get; set; } = false; // 自动登录
		[AutoConvert]
		public KeyboardConfig keyboard { get; set; } = new KeyboardConfig(); // 按键设置

		/// <summary>
		/// 是否需要ID
		/// </summary>
		protected override bool idEnable() { return false; }
	}

	/// <summary>
	/// 按键设置
	/// </summary>
	public class KeyboardConfig : BaseData {

		/// <summary>
		/// 按键项
		/// </summary>
		[AutoConvert]
		public KeyCode attackKey { get; set; } = KeyCode.J; // 攻击键
		[AutoConvert]
		public KeyCode rushKey { get; set; } = KeyCode.Space; // 冲刺键
		[AutoConvert]
		public KeyCode switchKey { get; set; } = KeyCode.LeftShift; // 切换人物键
		[AutoConvert]
		public KeyCode searchKey { get; set; } = KeyCode.E; // 搜索键

		/// <summary>
		/// 是否需要ID
		/// </summary>
		protected override bool idEnable() { return false; }
	}
}