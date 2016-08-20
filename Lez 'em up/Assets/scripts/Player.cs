using UnityEngine;
using System.Collections;

public class Player : Actor
{
    public static Player instance;

    public enum PlayerCharacter
    {
        Sassy,
        Lez,
        Nolezy,
    }

    public PlayerCharacter character;
    public float deadzone, fireRate;
    [SerializeField]
    Sprite[] charSprites;
    [SerializeField]
    Transform[] shootTransforms;
    [SerializeField]
    Projectile.ProjData bulletStats;
    [SerializeField]
    ParticleSystem[] bulletCasings;
    [SerializeField]
    ParticleSystem muzzleFlash;
    [SerializeField]
    AudioClip shootSound;
    [SerializeField]
    GameObject weaponSprite;
    float shootCooldown,abilityCooldown;
    
    public override void Start()
    {
        base.Start();
        instance = this;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            gunNess = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            gunNess = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            gunNess = 3;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            gunNess = 4;

        if (Input.GetKeyDown(KeyCode.Alpha5))
            character = PlayerCharacter.Lez;
        if (Input.GetKeyDown(KeyCode.Alpha6))
            character = PlayerCharacter.Sassy;
        if (Input.GetKeyDown(KeyCode.Alpha7))
            character = PlayerCharacter.Nolezy;

        sr[0].sprite = charSprites[(int)character];
        for (int i = 1; i < sr.Length; i++)
        {
            sr[i].sprite = charSprites[(int)character +3];
        }

        Movement();

        if (Input.GetButton("Fire1") && shootCooldown <= 0 && !backshot)
        {
            Shoot(false);
        }
        if (Input.GetButton("Fire2") && abilityCooldown <= 0)
        {
            if (!backshot)
                StartCoroutine(Ability());
        }
        if (shootCooldown > 0)
            shootCooldown -= Time.deltaTime;
        if (abilityCooldown > 0)
            abilityCooldown -= Time.deltaTime;
    }

    public int gunNess;
    float angleOfFire = 30;
    bool backshot;
    IEnumerator Ability()
    {
        abilityCooldown = 1f / fireRate;

        backshot = true;

        float lerpy = 0;
        while (lerpy < 1)
        {
            lerpy += Time.deltaTime * 5;
            Vector3 pos = Input.mousePosition;
            pos.z = transform.position.z - Camera.main.transform.position.z;
            pos = Camera.main.ScreenToWorldPoint(pos);
            float z = Mathf.Atan2(pos.y - transform.position.y, pos.x - transform.position.x) * Mathf.Rad2Deg;
            z += Mathf.Lerp(0,180,lerpy);
            weaponSprite.transform.rotation = Quaternion.Euler(0, 0, (transform.localScale.x > 0 ? z : -z) + (transform.localScale.x > 0 ? 0 : 180));
            yield return new WaitForEndOfFrame();
        }

        Shoot(true);

        lerpy = 0;
        while (lerpy < 1)
        {
            lerpy += Time.deltaTime * 5;
            Vector3 pos = Input.mousePosition;
            pos.z = transform.position.z - Camera.main.transform.position.z;
            pos = Camera.main.ScreenToWorldPoint(pos);
            float z = Mathf.Atan2(pos.y - transform.position.y, pos.x - transform.position.x) * Mathf.Rad2Deg;
            z += Mathf.Lerp(180, 0, lerpy);
            weaponSprite.transform.rotation = Quaternion.Euler(0, 0, (transform.localScale.x > 0 ? z : -z) + (transform.localScale.x > 0 ? 0 : 180));
            yield return new WaitForEndOfFrame();
        }
        backshot = false;
    }

    void Shoot(bool reverse)
    {
        angleOfFire = gunNess * 7.5f;
        foreach (Transform t in shootTransforms)
        {
            for (float i = 0; i < gunNess; i++)
            {
                //Work at bullet rotation
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float z = Mathf.Atan2(pos.y - transform.position.y, pos.x - transform.position.x) * Mathf.Rad2Deg;

                if (reverse)
                    z += 180;

                Vector3 shootDir;
                if (gunNess > 1)
                    shootDir = new Vector3(0, 0, z + ((angleOfFire / 2) - (angleOfFire / (gunNess - 1) * i)));
                else
                    shootDir = new Vector3(0, 0, z);

                Projectile p = ProjectilePooler.instance.PoolProj(bulletStats, shootDir, t.position);
            }
            if (CameraShake.instance.shakeDuration < 0.125f)
                CameraShake.instance.shakeDuration = 0.125f;

            SoundManager.instance.playSound(shootSound, 1, Random.Range(1, 1.3f));
            CameraShake.instance.shakeAmount = 0.2f * gunNess;
            bulletCasings[transform.localScale.x > 0 ? 1 : 0].startSize = .6f + (gunNess * 0.4f);
            bulletCasings[transform.localScale.x > 0 ? 1 : 0].Emit(1);
            muzzleFlash.Emit(1);
            //SoundManager.instance.playSound(shootSound, 1, Random.Range(.95f, 1.5f));
            //SoundManager.instance.playSound(shootSound, 1, Random.Range(2, 1.75f));

        }
        shootCooldown = 1f / fireRate;
    }

    public override void Movement()
    {
        base.Movement();
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > deadzone)
        {
            rigBod.AddForce(Vector2.right * moveSpeed * Input.GetAxis("Horizontal"));
        }
        if (Mathf.Abs(Input.GetAxis("Vertical")) > deadzone)
        {
            rigBod.AddForce(Vector2.up * moveSpeed * Input.GetAxis("Vertical"));
        }

        if (!backshot)
        {
            Vector3 pos = Input.mousePosition;
            pos.z = transform.position.z - Camera.main.transform.position.z;
            pos = Camera.main.ScreenToWorldPoint(pos);
            transform.localScale = new Vector3(pos.x > transform.position.x ? 1 : -1, 1, 1);

            float z = Mathf.Atan2(pos.y - transform.position.y, pos.x - transform.position.x) * Mathf.Rad2Deg;
            //(transform.localScale.x > 0 ? 0 : 180)
            weaponSprite.transform.rotation = Quaternion.Euler(0, 0, (transform.localScale.x > 0 ? z : -z) + (transform.localScale.x > 0 ? 0 : 180));
        }
    }
    
}
