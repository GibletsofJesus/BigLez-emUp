using UnityEngine;
using System.Collections.Generic;

//Interface that gives access to pooldata functionality
public interface IPoolable<T> where T : MonoBehaviour, IPoolable<T>
{
    PoolData<T> poolData { get; set; }
}

//Class that holds functionality for adding and removing from pool
public class PoolData<T> where T : MonoBehaviour, IPoolable<T>
{ 
    public ObjectPool<T> myPool;

    public void TakePool(ObjectPool<T> _pool)
    {
        myPool = _pool;
    }

    public void ReturnPool(T _poolObject)
    {
        myPool.ReturnPooledObject(_poolObject);
    }
}

//Generic pool class
public class ObjectPool<T> where T : MonoBehaviour, IPoolable<T>
{
    protected List<T> pooledObjects = new List<T>();
    protected T poolObject = null;

    //Constructor calls initialise function
    public ObjectPool(T _poolObject, int _startSize,Transform t)
    {
        InitialisePool(_poolObject, _startSize,t);
    }

    //Sets up pool
    private void InitialisePool(T _poolObject, int _startSize,Transform t)
    {
        poolObject = _poolObject;
        for (int i = 0; i < _startSize; ++i)
        {
            T _obj = Object.Instantiate(poolObject);
            _obj.poolData = new PoolData<T>();
            _obj.gameObject.SetActive(false);
            pooledObjects.Add(_obj);
            _obj.transform.SetParent(t);
        }
    }

    //Get a pooled object from the list
    public T GetPooledObject(Transform parent)
    {
        T _obj;
        //Create a new element
        if (pooledObjects.Count == 0)
        {
            _obj = Object.Instantiate(poolObject);
            _obj.poolData = new PoolData<T>();
            _obj.gameObject.SetActive(false);
            _obj.transform.parent = parent;
        }
        //Use last element of pooled object list
        else
        {
            _obj = pooledObjects[pooledObjects.Count - 1];
            pooledObjects.RemoveAt(pooledObjects.Count - 1);
        }
        //Tell the object what pool it came from so it can return
        _obj.poolData.TakePool(this);
        return _obj;
    }

    //Return a pooled object to the list
    public void ReturnPooledObject(T _obj)
    {
        pooledObjects.Add(_obj);
    }
}