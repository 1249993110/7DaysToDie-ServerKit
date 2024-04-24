namespace SdtdServerKit.FunctionSettings
{
    public interface ISettings
    {
        /// <summary>
        /// 是否启用功能
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}