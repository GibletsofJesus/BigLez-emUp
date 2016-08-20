using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour, IPoolable<Projectile>
{
    public PoolData<Projectile> poolData { get; set; }
    public float maxLifetime;
    float currentLifeTime;

    [System.Serializable]
    public class ProjData
    {
        public float speed = 5;
        public float damage = 50;
    }
    public ProjData ProjectileData;

    void Awake()
    {

    }

    void Update()
    {
        transform.Translate(Vector3.right * ProjectileData.speed);

        currentLifeTime += Time.deltaTime;
        if (currentLifeTime > maxLifetime)
            ReturnPool();
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag != "Player" && col.tag != "bullet")
        {
            if (col.GetComponent<Actor>())
            {
                BloodParticles.instance.Blood(transform.position, Random.Range(2, 4));
                col.GetComponent<Actor>().StartCoroutine(col.GetComponent<Actor>().TakeDamage(ProjectileData.damage));
            }
            ReturnPool();
        }
    }

    public void OnPooled(ProjData data, Vector3 eulerRotation,Vector3 startPos)
    {
        //set everything up
        transform.position = startPos;
        transform.rotation = Quaternion.Euler(eulerRotation);
        ProjectileData = data;
        gameObject.SetActive(true);
    }

    public void ReturnPool()
    {
        currentLifeTime = 0;
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }
}
