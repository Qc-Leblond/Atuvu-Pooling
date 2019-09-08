namespace Atuvu.Pooling
{
    sealed class EditorPlayerBridge : PlayerBridgeBase
    {
        protected override void RequestPoolList()
        {
            PoolListReceived(new PoolListReturnEvent(PoolManager.poolsInternal));
        }

        protected override void OnInitialize()
        {
            PoolProfiler.editorPoolInitialized += PoolInitializedReceived;
            PoolProfiler.editorPoolResized += PoolResizeReceived;
        }

        protected override void OnDestroy()
        {
            PoolProfiler.editorPoolInitialized -= PoolInitializedReceived;
            PoolProfiler.editorPoolResized -= PoolResizeReceived;
        }
    }
}