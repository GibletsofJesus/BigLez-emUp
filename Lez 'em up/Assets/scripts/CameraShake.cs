using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{

    public static CameraShake instance;
    // How long the object should shake for.
    public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    void OnEnable()
    {
        instance = this;
        originalPos = new Vector3(0, 0, -10);
    }

    public void Shake()
    {
        shakeDuration += .1f;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            Vector3 standardPosition;
            Vector3 newPos = -(Player.instance.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)) / 3;
            newPos = Player.instance.transform.position + (newPos / 1.5f);
            standardPosition =  new Vector3(newPos.x, newPos.y, -10);

            Camera.main.transform.localPosition = (Vector3.up * shakeAmount) + standardPosition + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;

            if (shakeDuration <= 0)
            {
                Camera.main.transform.position = standardPosition;
            }
        }
    }
}
