using UnityEngine;
using System.Collections;

public class BloodParticles : MonoBehaviour
{
    public static BloodParticles instance;
    [SerializeField]
    ParticleSystem ps;

    void Awake()
    {
        instance = this;
    }

    public void Blood(Vector3 newPos, int amount)
    {
        transform.position = newPos;
        ps.Emit(amount);
    }
}
