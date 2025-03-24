using Zenject;

namespace Core.Factory
{
    public interface IPoolableFactory<out TValue> : IFactory<TValue>
    {
        
    }
}