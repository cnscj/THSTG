namespace ASGame
{
    public interface IAssetLoader 
    {
        /// <summary>
        /// 开始资源加载
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        AssetLoadHandler StartLoad(string path);


        /// <summary>
        /// 停止资源加载
        /// </summary>
        /// <param name="handler"></param>
        void StopLoad(AssetLoadHandler handler);

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="pat"></param>
        void UnLoad(string pat);
    }

}
