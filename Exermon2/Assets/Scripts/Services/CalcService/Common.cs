using System;

using Core.Services;

/// <summary>
/// 游戏模块服务
/// </summary>
namespace GameModule.Services {

	/// <summary>
	/// 计算服务
	/// </summary>
	public partial class CalcService : BaseService<CalcService> {

		/// <summary>
		/// 公用计算函数
		/// </summary>
		public class Common {

			/// <summary>
			/// sigmoid函数
			/// </summary>
			/// <param name="x"></param>
			/// <returns></returns>
			public static double sigmoid(double x) {
				return 1 / (1 + Math.Exp(-x));
			}
		}
	}

}
