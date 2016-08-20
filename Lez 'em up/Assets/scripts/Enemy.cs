using UnityEngine;
using System.Collections;

public class Enemy : Actor, IPoolable<Enemy>
{
    public static int total;
    public PoolData<Enemy> poolData { get; set; }

    [SerializeField]
    GameObject corpse;
    [SerializeField]
    AudioClip[] deathSound;

    void Update()
    {
        Movement();
    }

    public void OnPooled(Vector3 startPos)
    {
        total++;
        //set everything up
        rigBod.velocity = Vector2.zero;
        transform.position = startPos;

        base.Start();
        int i = 0;
        foreach (SpriteRenderer s in sr)
        {
            s.color =standardCol[i];
            i++;
        }

        gameObject.SetActive(true);
    }

    public override void Death()
    {
        total--;
        SoundManager.instance.playSound(deathSound[Random.Range(0,deathSound.Length)],1,Random.Range(0.9f,1.1f));
        BloodParticles.instance.Blood(transform.position, Random.Range(16, 22));
        Instantiate(corpse, transform.position + Vector3.down * 0.4375f, transform.rotation);
        frameHolder.instance.holdFrame(0.1f);
        ReturnPool();
        base.Death();
    }

    public void ReturnPool()
    {
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }

    public override void Movement()
    {
        base.Movement();

        Vector3 target = Player.instance.transform.position;

        if (target.x > transform.position.x)
        {
            if (rigBod.velocity.x < moveSpeed)
            rigBod.AddForce(Vector2.right * 5);
        }
        if (target.x < transform.position.x)
        {
            if (rigBod.velocity.x > -moveSpeed)
                rigBod.AddForce(Vector2.left * 5);
        }

            transform.localScale = new Vector3(target.x > transform.position.x ? 1 : -1, 1, 1);
        if (target.y > transform.position.y)
        {
            if (rigBod.velocity.y < moveSpeed)
                rigBod.AddForce(Vector2.up * 5);
        }
        if (target.y < transform.position.y)
        {
            if (rigBod.velocity.y > -moveSpeed)
                rigBod.AddForce(Vector2.down * 5);
        }

    }
}
