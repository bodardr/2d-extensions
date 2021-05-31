public interface IObjectPool<T>
{
    void Dispose(PoolableObject<T> poolableObject);
    bool Contains(PoolableObject<T> poolableObject);
}