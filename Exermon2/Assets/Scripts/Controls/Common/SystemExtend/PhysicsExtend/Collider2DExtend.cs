
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

using Core.UI;
using Core.UI.Utils;

using GameModule.Services;

using CollFunc = UnityEngine.Events.UnityAction<
	UnityEngine.Collider2D>;
using CollFuncList = System.Collections.Generic.List<
	UnityEngine.Events.UnityAction<UnityEngine.Collider2D>>;

namespace UI.Common.Controls.SystemExtend.PhysicsExtend {

	/// <summary>
	/// 碰撞功能拓展
	/// </summary>
	//[RequireComponent(typeof(Collider2D))]
	public class Collider2DExtend : WorldComponent {

		/// <summary>
		/// 内部控件设置
		/// </summary>
		//[RequireTarget]
		public new Collider2D collider;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			collider = collider ?? get<Collider2D>();
			initializeCollFuncs();
		}
		
		/// <summary>
		/// 初始化碰撞函数
		/// </summary>
		protected virtual void initializeCollFuncs() { }

		#endregion

		#region 碰撞处理

		/// <summary>
		/// 各种碰撞处理函数
		/// </summary>
		CollFuncList onEnterFuncs = new CollFuncList();
		CollFuncList onStayFuncs = new CollFuncList();
		CollFuncList onExitFuncs = new CollFuncList();

		/// <summary>
		/// 注册碰撞处理函数
		/// </summary>
		/// <typeparam name="T">物品类型</typeparam>
		/// <param name="func">绘制函数</param>
		public void registerOnEnterFunc<T>(UnityAction<T> func) {
			registerCollFunc(onEnterFuncs, func);
		}
		public void registerOnStayFunc<T>(UnityAction<T> func) {
			registerCollFunc(onStayFuncs, func);
		}
		public void registerOnExitFunc<T>(UnityAction<T> func) {
			registerCollFunc(onExitFuncs, func);
		}
		void registerCollFunc<T>(
			CollFuncList funcs, UnityAction<T> func) {

			CollFunc func_ = (coll) => {
				var item = SceneUtils.get<T>(coll);
				//debugLog("onColl: " + coll + " => " + item);
				if (item != null) func?.Invoke(item);
			};
			funcs.Add(func_);
		}

		/// <summary>
		/// 碰撞开始
		/// </summary>
		/// <param name="collider"></param>
		void OnTriggerEnter2D(Collider2D collider) {
			onTrigger(collider, onEnterFuncs);
		}

		/// <summary>
		/// 碰撞持续
		/// </summary>
		/// <param name="collider"></param>
		void OnTriggerStay2D(Collider2D collider) {
			onTrigger(collider, onStayFuncs);
		}

		/// <summary>
		/// 碰撞结束
		/// </summary>
		/// <param name="collider"></param>
		void OnTriggerExit2D(Collider2D collider) {
			onTrigger(collider, onExitFuncs);
		}

		/// <summary>
		/// 碰撞开始
		/// </summary>
		/// <param name="collision"></param>
		void OnCollisionEnter2D(Collision2D collision) {
			onTrigger(collision.collider, onEnterFuncs);
		}

		/// <summary>
		/// 碰撞持续
		/// </summary>
		/// <param name="collision"></param>
		void OnCollisionStay2D(Collision2D collision) {
			onTrigger(collision.collider, onStayFuncs);
		}

		/// <summary>
		/// 碰撞结束
		/// </summary>
		/// <param name="collision"></param>
		void OnCollisionExit2D(Collision2D collision) {
			onTrigger(collision.collider, onExitFuncs);
		}

		/// <summary>
		/// 统一碰撞回调处理
		/// </summary>
		/// <param name="collision"></param>
		void onTrigger(Collider2D collision, CollFuncList funcs) {
			foreach (var func in funcs) func.Invoke(collision);
		}

		#endregion
	}
}