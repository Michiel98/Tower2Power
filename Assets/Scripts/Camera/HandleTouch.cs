using UnityEngine;

public class HandleTouch : MonoBehaviour
{
    Camera cam;
    MoveCamera moveCamera;

    bool zooming = false;
    Vector3 firstPanOrTapPosition;
    Vector3 secondPanOrTapPosition;
    Vector3[] firstZoomAndRotatePositions = new Vector3[2];
    Vector3[] secondZoomAndRotatePositions = new Vector3[2];

    void Start()
    {
        cam = Camera.main;
        moveCamera = cam.GetComponent<MoveCamera>();
        firstZoomAndRotatePositions = new Vector3[2];
        secondZoomAndRotatePositions = new Vector3[2];
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            PanOrTap(touch);
        }

        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);
            ZoomAndRotate(touch1, touch2);
        }

        else zooming = false;
    }

    public void PanOrTap(Touch touch)
    {
        if (touch.phase == TouchPhase.Began) firstPanOrTapPosition = cam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, transform.position.y));
        
        if (touch.phase == TouchPhase.Moved)
        {
            secondPanOrTapPosition = cam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, transform.position.y));
            moveCamera.Pan(firstPanOrTapPosition, secondPanOrTapPosition);
            firstPanOrTapPosition = cam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, transform.position.y));
        }

        if (touch.phase == TouchPhase.Ended) moveCamera.Tap(firstPanOrTapPosition); // tap on object in scene
    }

    public void ZoomAndRotate(Touch touch1, Touch touch2)
    {
        if (!(touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved) && !zooming)
        {
            zooming = true;
            firstZoomAndRotatePositions[0] = cam.ScreenToWorldPoint(new Vector3(touch1.position.x, touch1.position.y, transform.position.y));
            firstZoomAndRotatePositions[1] = cam.ScreenToWorldPoint(new Vector3(touch2.position.x, touch2.position.y, transform.position.y));
        }

        if (zooming)
        {
            secondZoomAndRotatePositions[0] = cam.ScreenToWorldPoint(new Vector3(touch1.position.x, touch1.position.y, transform.position.y));
            secondZoomAndRotatePositions[1] = cam.ScreenToWorldPoint(new Vector3(touch2.position.x, touch2.position.y, transform.position.y));

            moveCamera.ZoomAndRotate(firstZoomAndRotatePositions, secondZoomAndRotatePositions);

            firstZoomAndRotatePositions[0] = cam.ScreenToWorldPoint(new Vector3(touch1.position.x, touch1.position.y, transform.position.y));
            firstZoomAndRotatePositions[1] = cam.ScreenToWorldPoint(new Vector3(touch2.position.x, touch2.position.y, transform.position.y));
        }
    }
}