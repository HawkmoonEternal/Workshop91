using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_along : MonoBehaviour
{
    public GameObject follow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = follow.transform.position;
        transform.rotation = follow.transform.rotation;
    }
}
