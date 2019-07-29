using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam_rotate : MonoBehaviour
{
    public GameObject center;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        transform.RotateAround(center.transform.position,new Vector3(0,1,0), 1);
    }
}
