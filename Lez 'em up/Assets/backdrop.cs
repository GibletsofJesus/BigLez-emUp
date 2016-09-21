using UnityEngine;
using System.Collections;

public class backdrop : MonoBehaviour {

    public MeshRenderer m;
    public Color col;
    [Range(0,1)]
    public float f;
	// Use this for initialization
	void Start () {
	
	}

    float total;
	// Update is called once per frame
	void Update () {
        total += Time.deltaTime*0.1f;

        col = Color.HSVToRGB(total % 1, 1, .5f);

        m.material.SetColor("_TintColor",col);
        m.material.SetTextureOffset("_MainTex", Vector2.one*total*2.5f);
	}
}
