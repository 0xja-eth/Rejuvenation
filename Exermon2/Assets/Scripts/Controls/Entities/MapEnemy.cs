
using UnityEngine;

using Core.Data;

using Core.UI;
using Core.UI.Utils;

using BattleModule.Data;

namespace UI.Common.Controls.Entities {

	using Common.Controls.AnimationSystem;

	/// <summary>
	/// 地图上的行走实体
	/// </summary>
	public abstract class MapEnemy : MapCharacter {

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


	}
}