namespace Converter.ApiServer.Apies
{
    /// <summary>
    /// The interface allows you to consolidate and register various APIs
    /// </summary>
    public interface IApi
    {
        /// <summary>
        /// Registration of the implementation api
        /// </summary>
        /// <param name="app"></param>
        void Register(WebApplication app);
    }
}
