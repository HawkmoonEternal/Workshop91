using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExclamationMarkAnimation : MonoBehaviour
{
    public float scaleFactor;
    public float velocity;
    public float rotVelocity;
    private Vector3 startPos;
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        
        time = 0;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, rotVelocity * Time.deltaTime, 0),Space.World);
        transform.position = startPos + new Vector3(0, Mathf.Sin(time) * scaleFactor, 0);
        time += Time.deltaTime * velocity;
    }
}
