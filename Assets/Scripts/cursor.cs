using UnityEngine;
using System.Collections;

public class cursor : MonoBehaviour {

    public void setPosition(int x, int y)
    {
        transform.position = new Vector3(x + .5F, y + .5F, 0);
    }
}
