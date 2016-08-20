using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

    public List<Enemy> allCunts = new List<Enemy>();

    private ObjectPool<Enemy> objectPool = null;
    [SerializeField]
    private Enemy CuntPrefab = null;

    void Start()
    {
        objectPool = new ObjectPool<Enemy>(CuntPrefab, 15, transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (Random.value > .95f && Enemy.total < 5)
            PoolWanker(transform.position);
    }

    public Enemy PoolWanker(Vector3 startPos)
    {
        Enemy choomah = objectPool.GetPooledObject(transform);
        choomah.OnPooled(startPos);
        allCunts.Add(choomah);
        return choomah;
    }
}
