
namespace UI.MapSystem.Controls {
	
    /// <summary>
    /// 时空镜
    /// </summary>
    public class TravelTime : MapEventPage {

		/// <summary>
		/// 需要所有复制体和主体都进入门
		/// </summary>
		//public bool needGater = false;

		/// <summary>
		/// 执行
		/// </summary>
		protected override void invokeCustom() {
			base.invokeCustom();
			if (eventPlayer.isMaster()) mapEvent.scene.travel();
		}

	}
}
