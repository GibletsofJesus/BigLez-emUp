using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour
{
    public float moveSpeed,MaxHP;
    public Rigidbody2D rigBod;
    float HP;
    public SpriteRenderer[] sr;
    public Color[] standardCol;
    public Animator anim;

    public float footstepAmp;

    public virtual void Start()
    {
        HP = MaxHP;
        standardCol = new Color[sr.Length];
        for (int i = 0; i < sr.Length; i++)
        {
            standardCol[i] = sr[i].color;
            standardCol[i] = Color.white;
        }
    }

    void Update()
    {

    }

    public virtual IEnumerator TakeDamage(float damage)
    {
        HP -= damage;
        if (HP <= 0)
            Death();
        int i = 0;
        foreach (SpriteRenderer s in sr)
        {
            s.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);

        foreach (SpriteRenderer s in sr)
        {
            s.color = standardCol[i];
            i++;
        }
    }

    public virtual void Death()
    {
        gameObject.SetActive(false);
    }

    public virtual void Movement()
    {
        if (anim)
        {
            anim.SetBool("walking", (rigBod.velocity.magnitude / 10) > 0.1f ? true : false);
            anim.SetFloat("speed", 1  +rigBod.velocity.magnitude/10);
        }
    }

    [SerializeField]
    AudioClip[] footstepSounds;

    public void Footstep()
    {
        SoundManager.instance.playSound(footstepSounds[Random.Range(0, footstepSounds.Length)],0.25f*footstepAmp);
    }
}
