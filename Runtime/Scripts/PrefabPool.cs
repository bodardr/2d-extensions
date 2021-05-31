using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefab Pool", menuName = "Object Pool/Prefab")]
public class PrefabPool : ScriptableObject, IObjectPool<GameObject>
{
    private Stack<PoolableObject<GameObject>> pool;

    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private int baseInstantiation = 10;

    public PoolableObject<GameObject> Get()
    {
        if (pool == null)
            InstantiatePool();

        if (pool.Count == 0)
        {
            Debug.Log("Pool base exceeded, consider increasing the pool.");
            return new PoolableObject<GameObject>(Instantiate(prefab), this);
        }

        return pool.Pop();
    }

    private void InstantiatePool()
    {
        pool = new Stack<PoolableObject<GameObject>>();
        
        for (int i = 0; i < baseInstantiation; i++)
            pool.Push(new PoolableObject<GameObject>(Instantiate(prefab), this));
    }

    public bool Contains(PoolableObject<GameObject> poolableObject) => pool.Contains(poolableObject);

    public void Dispose(PoolableObject<GameObject> disposedObject)
    {
        pool.Push(disposedObject);
    }
}