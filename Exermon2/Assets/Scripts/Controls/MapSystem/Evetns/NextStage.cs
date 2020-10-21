
namespace UI.MapSystem.Controls {

    /// <summary>
    /// 通关事件
    /// </summary>
    public class NextStage : MapEventPage {

		/// <summary>
		/// 执行
		/// </summary>
		protected override void invokeCustom() {
			base.invokeCustom();
			mapEvent.scene.changeNextStage();
		}

	}
}
