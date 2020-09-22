
namespace Core.Data {

    /// <summary>
    /// 类型数据
    /// </summary>
    public class TypeData : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string name { get; set; } = null; // 名称
        [AutoConvert]
        public string description { get; set; } = null; // 描述
    }
}