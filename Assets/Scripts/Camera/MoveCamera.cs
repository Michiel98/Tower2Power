using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] Transform playerTransform;
    [SerializeField] InteractScript interactor;

    [Header("UI")]
    [SerializeField] Button followButton;
    [SerializeField] Button navigateButton;

    bool followingPlayer;
    bool isFinished;
    int cameraFollowOffset = 20;

    void Start()
    {
        followButton.gameObject.SetActive(false);
        navigateButton.gameObject.SetActive(true);
        followButton.onClick.AddListener(delegate { Follow(); });
        navigateButton.onClick.AddListener(delegate { Navigate(); });

        followingPlayer = true;
        isFinished = true;
    }

    void LateUpdate()
    {
        if (followingPlayer && isFinished) // match camera position/rotation to player position/rotation
        {
            transform.position = playerTransform.position - playerTransform.forward * cameraFollowOffset + new Vector3(0,cameraFollowOffset,0);
            transform.rotation = Quaternion.LookRotation((playerTransform.position - transform.position).normalized, Vector3.up);
        }
    }

    void Follow()
    {
        if (!playerTransform) return;
        isFinished = false;
        StartCoroutine(MoveToPosition(playerTransform.position - playerTransform.forward * cameraFollowOffset + new Vector3(0,cameraFollowOffset,0), Quaternion.LookRotation((playerTransform.position - (playerTransform.position - playerTransform.forward * cameraFollowOffset + new Vector3(0,cameraFollowOffset,0))).normalized, Vector3.up)));
        followButton.gameObject.SetActive(false); // hide follow button since we are now in 'following mode'
        navigateButton.gameObject.SetActive(true); // hide follow button since we are now in 'following mode'
        followingPlayer = true; // we are following the player
    }

    void Navigate()
    {
        if (!playerTransform) return;
        isFinished = false;
        StartCoroutine(MoveToPosition(playerTransform.position + new Vector3(0, cameraFollowOffset, 0), Quaternion.Euler(90,0,0)));
        followButton.gameObject.SetActive(true); // hide follow button since we are now in 'following mode'
        navigateButton.gameObject.SetActive(false); // hide follow button since we are now in 'following mode'
        followingPlayer = false; // we are following the player
    }

    // player has tapped on object in scene, give hit object to player interaction handles
    public void Tap(Vector3 tapPosition)
    {
      if (followingPlayer && isFinished)
      {
        if (Physics.Raycast(transform.position, tapPosition - transform.position, out RaycastHit hit)) interactor.Interact(hit.transform.gameObject);
      }
    }

    public void Pan(Vector3 firstPanPosition, Vector3 secondPanPosition)
    {
        if (!followingPlayer && isFinished)
        {
          transform.position = transform.position + firstPanPosition - secondPanPosition;
        }
    }

    public void ZoomAndRotate(Vector3[] firstZoomAndRotatePositions, Vector3[] secondZoomAndRotatePositions)
    {
      if (!followingPlayer && isFinished)
      {
        float zoom = 1f - Vector3.Distance(firstZoomAndRotatePositions[0], firstZoomAndRotatePositions[1]) / Vector3.Distance(secondZoomAndRotatePositions[0], secondZoomAndRotatePositions[1]);
        float angle = Vector3.SignedAngle(firstZoomAndRotatePositions[0] - firstZoomAndRotatePositions[1], secondZoomAndRotatePositions[0] - secondZoomAndRotatePositions[1], Vector3.up);
        Vector3 focalPoint = (firstZoomAndRotatePositions[0] + firstZoomAndRotatePositions[1] + secondZoomAndRotatePositions[0] + secondZoomAndRotatePositions[1]) / 4;
        transform.position = Vector3.LerpUnclamped(transform.position, focalPoint, zoom);
        transform.rotation = transform.rotation * Quaternion.Euler(0, 0, angle);
      }
    }

    public IEnumerator MoveToPosition(Vector3 targetPosition, Quaternion targetRotation)
    {
        float progress = 0;
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float duration = 2.5f * Mathf.Max(Vector3.Distance(startPosition, targetPosition) / 155f, Quaternion.Angle(startRotation, targetRotation) / 180f);

        while (progress < 1)
        {
            progress = (Time.time - startTime) / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0, 1, progress));
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, Mathf.SmoothStep(0, 1, progress));
            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;
        isFinished = true;
    }
}
