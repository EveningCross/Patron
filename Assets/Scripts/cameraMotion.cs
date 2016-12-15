using UnityEngine;
using System.Collections;

public class cameraMotion : MonoBehaviour {
    public float camera_acceleration = 0.01F;
    private float x = 0;
    private float y = 0;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            //Vector3 vec = Camera.main.ScreenPointToVector(Input.mousePosition);
            // print(vec.x + " " + vec.y);
            Camera camera = GetComponent<Camera>();
            Vector3 p = camera.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, camera.nearClipPlane));
            print(Mathf.Floor(p.x) + " " + Mathf.Floor(p.y));
        }

        if (Input.GetKey("up") && y < 10)
        {
            y += camera_acceleration;
        }

        if (Input.GetKey("down") && y > 0.0)
        {
            y -= camera_acceleration;
        }

        if (Input.GetKey("right") && x < 10)
        {
            x += camera_acceleration;
        }

        if (Input.GetKey("left") && x > 0.0)
        {
            x -= camera_acceleration;
        }
        
        transform.position = new Vector3(Mathf.Clamp(this.x + x, 0.1F , 9.9F ), Mathf.Clamp(this.y + y, 0.1F, 9.9F ), -10);
    }
    
}
