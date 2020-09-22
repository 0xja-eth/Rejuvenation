
namespace Core.UI {

    /// <summary>
    /// 批量刷新组件
    /// </summary>
    public class RefreshHelper : GeneralComponent {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public GeneralComponent[] views;
        
        #region 界面控制

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void refresh() {
            base.refresh();
            foreach (var view in views)
                refreshView(view);
        }

        /// <summary>
        /// 刷新单个视窗
        /// </summary>
        /// <param name="view">视窗</param>
        protected virtual void refreshView(GeneralComponent view) {
            view.requestRefresh(true);
        }

        #endregion
    }
}