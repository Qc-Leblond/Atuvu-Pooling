namespace Atuvu.Pooling
{
    public interface IPoolable
    {
        void OnPop();
        void OnRelease();
    }
}
