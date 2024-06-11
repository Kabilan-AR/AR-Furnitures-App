using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class TapToPlaceAR : MonoBehaviour
{
    private Camera MainCamera;
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    public TextMeshProUGUI text;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public GameObject Marker;
    public GameObject[] spawnPrefab;
    private GameObject placedObject;

    private bool isPlaced=false;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(AppManager.itemNumber);
        MainCamera = Camera.main;
        raycastManager=GetComponent<ARRaycastManager>();
        planeManager=GetComponent<ARPlaneManager>();
        text.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlaced)
        {
            text.enabled = false;
            return;
        }
        HandletoPlace();
    }
    void HandletoPlace()
    {
        Vector3 screenpoint = MainCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.35f, 0));
        if (raycastManager.Raycast(new Vector2(screenpoint.x, screenpoint.y), hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPos = hits[0].pose;
            Marker.SetActive(true);
            Marker.transform.position = hitPos.position;
            Marker.transform.rotation = hitPos.rotation;
            if(placedObject!=null)
            {
                moveObject(hitPos);

            }
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (placedObject == null)
                {
                    createPrefab(hitPos);
                }
                else
                {
                    moveObject(hitPos);
                    planeVisiblity(false);
                }
            }
        }
    }

    void createPrefab(Pose hitPos)
    {
        placedObject = Instantiate(spawnPrefab[AppManager.itemNumber],hitPos.position,hitPos.rotation);
        Marker.SetActive(false);
        isPlaced = true;
        planeVisiblity(false);
        text.enabled = false;
    }
    void moveObject(Pose hitPos)
    {
        placedObject.transform.position = hitPos.position;
        placedObject.transform.rotation=hitPos.rotation;
        Marker.SetActive(false);
        isPlaced=true;
    }
    public void planeVisiblity(bool value)
    {
        foreach(var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
        planeManager.enabled = value;
    }

    public void resetPos()
    {
        Marker.SetActive(true);
        isPlaced = false;
        planeVisiblity(true);
        //Maybe Deleted
        Destroy(placedObject);
        text.enabled = true;
    }
    public void backButton()
    {
        SceneManager.LoadScene(0);
    }
    public void CallURL()
    {
        Application.OpenURL("https://www.amazon.in/dp/B0CHYR4TDS/?coliid=I3OYE9K7N241YR&colid=3GE3D6LHYGYFI&ref_=list_c_wl_lv_ov_lig_dp_it_im&th=1");
    }
   

}
