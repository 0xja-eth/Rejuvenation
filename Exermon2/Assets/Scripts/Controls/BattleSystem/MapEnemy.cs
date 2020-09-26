
using UnityEngine;

using Core.Data;

using BattleModule.Data;

namespace UI.Common.Controls.BattleSystem {

	using MapSystem;

	/// <summary>
	/// 地图上的行走实体
	/// </summary>
	[RequireComponent(typeof(EnemyDisplay))]
	public abstract class MapEnemy : MapCharacter {

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public bool useCustomParams = false; // 是否使用自定义属性

		public Enemy customEnemy = null; // 自定义敌人数据

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		protected EnemyDisplay display;

		/// <summary>
		/// 类型
		/// </summary>
		public override Type type => Type.Enemy;

		/// <summary>
		/// 敌人ID
		/// </summary>
		public abstract int enemyId { get; }

		/// <summary>
		/// 敌人
		/// </summary>
		Enemy enemy_ = null;
		public Enemy enemy => enemy_ = enemy_ ?? 
			BaseData.poolGet<Enemy>(enemyId);

		/// <summary>
		/// 运行时敌人
		/// </summary>
		public RuntimeEnemy runtimeEnemy => display.getItem();

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			initializeEnemyDisplay();
		}

		/// <summary>
		/// 初始化敌人显示组件
		/// </summary>
		void initializeEnemyDisplay() {
			var runtimeEnemy = new RuntimeEnemy(enemyId);
			if (useCustomParams)
				runtimeEnemy.customEnemy = customEnemy;
			display.setItem(runtimeEnemy);
		}

		#endregion

		#region 移动控制

		/// <summary>
		/// 移动速度
		/// </summary>
		/// <returns></returns>
		public override float moveSpeed() {
			return enemy.speed;
		}

		#endregion

		#region 行动控制

		#endregion

	}
}