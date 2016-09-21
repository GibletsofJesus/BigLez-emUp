using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Spectrum : MonoBehaviour {

    [SerializeField]
    AudioSource audioSrc;
    [SerializeField]
    int sampleSize;
    [SerializeField]
    GameObject cloneMe;
    List<GameObject> cubes = new List<GameObject>();
    public float amplitude;
    public int dampening;

    [SerializeField]
    FFTWindow windowType;
    [Header("UI")]
    [SerializeField]
    Slider seekerBar;
    [SerializeField]
    Text trackText;

    [Header("Texture stuff")]
    public float MaxColourHeight;
    public Color BaseCol, TopCol;

    // Use this for initialization
    void Start()
    {
        makeCubes();
    }

    void makeCubes()
    {
        if (cubes.Count > 0)
        {
            foreach(GameObject g in cubes)
            {
                Destroy(g);
            }
            cubes.Clear();
        }

        for (int i = 0; i < sampleSize / dampening; i++)
        {
            GameObject newCube = Instantiate(cloneMe, new Vector3((i * 1.1f) - (1.1f * ((float)sampleSize / (dampening * 2)))+transform.position.x, 0, 0), transform.rotation) as GameObject;
            newCube.transform.parent = transform;
            newCube.name = "Cube " + i;
            newCube.GetComponentInChildren<Renderer>().material.mainTexture = CreateGradientTexture(TopCol, BaseCol);
            cubes.Add(newCube);
        }
    }

    public Gradient gr;

    Texture2D CreateGradientTexture(Color ColourA,Color ColourB)
    {
        Texture2D tex = new Texture2D(1, 512, TextureFormat.RGBAFloat, false, true);
        for (int i = 0; i < tex.height; i++)
        {
            //tex.SetPixel(0, i, Color.Lerp(ColourA, ColourB, (float)i / (float)tex.height));
            tex.SetPixel(0, i, gr.Evaluate((float)i / (float)tex.height));
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        return tex;
    }

    public void Seek(float f)
    {
        audioSrc.time = f * audioSrc.clip.length;
    }

    // Update is called once per frame
    void Update()
    {
        if (cubes.Count != sampleSize / dampening && Mathf.IsPowerOfTwo(sampleSize / dampening))
        {
            makeCubes();
        }
        if (Mathf.IsPowerOfTwo(sampleSize) && sampleSize > 63)
        {
            float[] f = new float[sampleSize];
            audioSrc.GetSpectrumData(f, 0, windowType);

            for (int i = 0; i < cubes.Count; i++)
            {
                float currentHeight = cubes[i].transform.localScale.y;

                float newHeight = 0f;
                for (int d = 0; d < dampening; d++)
                {
                    newHeight = f[i * dampening] * amplitude*100;
                    newHeight = Mathf.Lerp(newHeight, f[(i * dampening) + d] * amplitude*100, .5f);
                }
                float outputHeight = Mathf.Lerp(currentHeight, 1+Mathf.Sqrt(newHeight), Time.deltaTime * 50f);
                cubes[i].transform.localScale = new Vector3(1, outputHeight, 1);
                cubes[i].GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(1, outputHeight / MaxColourHeight);
                cubes[i].GetComponentInChildren<Renderer>().material.mainTextureOffset = new Vector2(1, 1-(outputHeight/MaxColourHeight));
                
            }
        }
        trackText.text = audioSrc.clip.name;
        seekerBar.value = audioSrc.time / audioSrc.clip.length;
    }
}
