using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour {
    	
	void Update ()
    {
        Vector3 newPos =  -(Player.instance.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition))/3;
        newPos = Player.instance.transform.position + (newPos / 1.5f);
        transform.position = new Vector3(newPos.x, newPos.y, -10);
    }
}
