namespace SdtdServerKit.Functions
{
    public interface IFunction
    {
        /// <summary>
        /// 是否启用功能
        /// </summary>
        public bool IsEnabled { get; }

        /// <summary>
        /// 功能名称
        /// </summary>
        public string FunctionName { get; }

        /// <summary>
        /// 加载配置
        /// </summary>
        internal void LoadSettings();
    }
}