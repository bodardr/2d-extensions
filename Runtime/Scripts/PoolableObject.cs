using System;
using UnityEngine;

public struct PoolableObject<T> : IDisposable
{
    private IObjectPool<T> pool;
    public T Content { get; }

    public void Dispose()
    {
        if (pool.Contains(this))
            Debug.Log("Object already disposed.");
        
        pool.Dispose(this);
    }

    public PoolableObject(T content, IObjectPool<T> pool)
    {
        Content = content;
        this.pool = pool;
    }
}