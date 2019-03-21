using UnityEngine;
using System.Collections;

public class TempScript : ExpiryScript {

    // Update is called once per frame
    public Vector3 target;
    public float speed = 1;

    void Update()
    {
        if (target != null)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }
    }
}
