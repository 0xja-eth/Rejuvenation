using System.Collections.Generic;

using UnityEngine;

using Core.Data;

using BattleModule.Data;

namespace UI.Common.Controls.BattleSystem {

	using MapSystem;

	/// <summary>
	/// 地图上的敌人实体
	/// </summary>
	public abstract class MapEnemy : MapBattler {

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public bool useCustomParams = false; // 是否使用自定义属性

		public Enemy customEnemy = null; // 自定义敌人数据
		
		/// <summary>
		/// 类型
		/// </summary>
		public override Type type => Type.Enemy;

		/// <summary>
		/// 敌人ID
		/// </summary>
		public virtual int enemyId => 0;

		/// <summary>
		/// 敌人
		/// </summary>
		Enemy enemy_ = null;
		public Enemy enemy => enemy_ = enemy_ ?? 
			BaseData.poolGet<Enemy>(enemyId);

		/// <summary>
		/// 运行时敌人
		/// </summary>
		public RuntimeEnemy runtimeEnemy => runtimeBattler as RuntimeEnemy;

		#region 初始化

		/// <summary>
		/// 初始化敌人显示组件
		/// </summary>
		protected override void setupBattlerDisplay() {
			var runtimeEnemy = new RuntimeEnemy(enemyId);
			if (useCustomParams)
				runtimeEnemy.customEnemy = customEnemy;
			display.setItem(runtimeEnemy);
		}

		#endregion

	}
}