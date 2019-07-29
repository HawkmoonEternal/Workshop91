using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
    private bool touchActive = true;
    public float maxTimeForSingleTouch = 0.25f;
    public float maxDistanceForSingleTouch = 50f;
    public float scaleSpeed = 10;
    public float rotationSpeed = 1;
    private Transform playerTransform;
    private Vector2 singleTouchStartPos;
    private float singleTouchStartTime;
    public Transform clampXMin;
    public Transform clampXMax;
    public Transform clampZMin;
    public Transform clampZMax;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GetComponent<Transform>();
        Screen.autorotateToPortrait = true;
        Screen.autorotateToLandscapeLeft = true;
        
        Screen.autorotateToPortraitUpsideDown = true;

        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!touchActive) return;
        //One Finger gestures for select and rotate
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                singleTouchStartPos = touch.position;
                singleTouchStartTime = Time.time;

            }

            Vector2 rot = touch.deltaPosition * rotationSpeed * Time.deltaTime;
            playerTransform.Rotate(new Vector3(0, rot.x, 0));

            //Check if Gesture was a tap
            if (touch.phase == TouchPhase.Ended)
            {

                if (Time.time - singleTouchStartTime < maxTimeForSingleTouch && Vector2.Distance(touch.position, singleTouchStartPos) < maxDistanceForSingleTouch)
                {
                    Debug.Log("SingleTouch");
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        string name = hit.transform.name;
                        Debug.Log("hit smth");

                        if (name.StartsWith("Car"))
                        {
                            Debug.Log("Hit Car");
                            gameObject.GetComponent<MyGUI>().openContentPanel(5);

                        }
                        else if (name.StartsWith("Radio"))
                        {
                            gameObject.GetComponent<MyGUI>().openContentPanel(7);
                        }


                    }

                }
            }
        }

        //Zoom Gesture for moving forwards/backward
        else if (Input.touchCount == 2)
        {
            singleTouchStartTime = -1;
            Debug.Log("Two Fingers");
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            // Get the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;

            float deltaMagnitude = touchDeltaMag - prevTouchDeltaMag;
            this.transform.position += playerTransform.forward * deltaMagnitude * scaleSpeed * Time.deltaTime;
            //simple clamp
            if (transform.position.x < clampXMin.position.x + 1) transform.position = new Vector3(clampXMin.position.x + 1, transform.position.y, transform.position.z);
            if (transform.position.x > clampXMax.position.x - 1) transform.position = new Vector3(clampXMax.position.x - 1, transform.position.y, transform.position.z);
            if (transform.position.z < clampZMin.position.z + 1) transform.position = new Vector3(transform.position.x, transform.position.y, clampZMin.position.z + 1);
            if (transform.position.z > clampZMax.position.z - 1) transform.position = new Vector3(transform.position.x, transform.position.y, clampZMax.position.z - 1);

        }
        else if (Input.touchCount == 4) {
            GameData.updateCash(10000);
            GameData.updateXP(10000);
            Debug.Log("Four Fingers");
            gameObject.GetComponent<MyGUI>().setCashText();
            gameObject.GetComponent<MyGUI>().setXPText();
            gameObject.GetComponent<MyGUI>().setLevelText();
        }
    }

    public void enableTouchControl() {
        touchActive = true;
    }
    public void disableTouchControl()
    {
        touchActive = false;
    }
}
