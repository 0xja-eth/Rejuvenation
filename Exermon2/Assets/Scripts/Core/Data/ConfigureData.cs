
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
		/// 动作按键
		/// </summary>
		[AutoConvert]
		public KeyCode attack1Key { get; set; } = KeyCode.J; // 近战攻击键
		[AutoConvert]
		public KeyCode attack2Key { get; set; } = KeyCode.K; // 远程攻击键
		[AutoConvert]
		public KeyCode rushKey { get; set; } = KeyCode.Space; // 冲刺键
		[AutoConvert]
		public KeyCode searchKey { get; set; } = KeyCode.E; // 搜索键

		/// <summary>
		/// 载具按键
		/// </summary>
		[AutoConvert]
		public KeyCode takeKey { get; set; } = KeyCode.E; // 乘降键

		/// <summary>
		/// 道具按键
		/// </summary>
		[AutoConvert]
		public KeyCode magnetiteKey { get; set; } = KeyCode.L; // 使用引力键

		/// <summary>
		/// 对话框按键
		/// </summary>
		[AutoConvert]
		public KeyCode nextKey { get; set; } = KeyCode.Return; // 下一步

		/// <summary>
		/// 是否需要ID
		/// </summary>
		protected override bool idEnable() { return false; }
	}
}