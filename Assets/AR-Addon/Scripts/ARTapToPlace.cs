using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ARTapToPlace : MonoBehaviour
{
    private Camera mainCamera;

    private ARRaycastManager _arRaycastManager;
    private ARPlaneManager _arPlaneManager;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private bool isPlaced = false;

    
    public GameObject marker;
    [HideInInspector]
    public GameObject PlacedObject;
    
    public GameObject SpawnPrefab;

    void Start()
    {
        mainCamera = Camera.main;
        _arRaycastManager = GetComponent<ARRaycastManager>();
        _arPlaneManager = GetComponent<ARPlaneManager>();
        marker.SetActive(false);
    }

    void Update()
    {
        if (isPlaced) return;

        HandleTapToPlace();
    }

    void HandleTapToPlace()
    {
        Vector3 screenCenter = mainCamera.ViewportToScreenPoint(new Vector3(0.5f,0.35f,0));
       

        if (_arRaycastManager.Raycast(new Vector2(screenCenter.x,screenCenter.y), hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            marker.SetActive(true);
            marker.transform.position = hitPose.position;
            marker.transform.rotation = hitPose.rotation;
            if (PlacedObject != null)
            {
                PlacedObject.transform.position = hitPose.position;
                PlacedObject.transform.rotation = hitPose.rotation;

            }
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (PlacedObject == null)
                {
                    PlaceObject(hitPose);
                }
                else
                {
                    MoveObject(hitPose);
                }
            }
            
        }
        else
        {
            marker.SetActive(false);
        }
    }

    void PlaceObject(Pose hitPose)
    {
        PlacedObject = Instantiate(SpawnPrefab, hitPose.position, hitPose.rotation);
        marker.SetActive(false);
        PlaneVisibility(false);
        isPlaced = true;
    }

    void MoveObject(Pose hitPose)
    {
        PlacedObject.transform.position = hitPose.position;
        PlacedObject.transform.rotation = hitPose.rotation;
        marker.SetActive(false);
        PlaneVisibility(false);
        isPlaced = true;
    }

    public void PlaneVisibility(bool value)
    {
        foreach (var plane in _arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
        _arPlaneManager.enabled = value;
    }

    public void ResetToOriginal()
    {
       
        marker.SetActive(true);
        PlaneVisibility(true);
        isPlaced = false;
    }

    public void CallURL()
    {
        Application.OpenURL("https://www.amazon.in/dp/B0CHYR4TDS/?coliid=I3OYE9K7N241YR&colid=3GE3D6LHYGYFI&ref_=list_c_wl_lv_ov_lig_dp_it_im&th=1");
    }
}
