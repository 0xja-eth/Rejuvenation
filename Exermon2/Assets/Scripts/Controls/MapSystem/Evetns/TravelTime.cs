
namespace UI.MapSystem.Controls {
	
    /// <summary>
    /// 时空镜
    /// </summary>
    public class TravelTime : MapEventPage {

		/// <summary>
		/// 执行
		/// </summary>
		protected override void invokeCustom() {
			base.invokeCustom();
			mapEvent.scene.travel();
		}

	}
}
