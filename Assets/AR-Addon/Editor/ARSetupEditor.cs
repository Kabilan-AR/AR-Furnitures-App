using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.ARFoundation;



public class ARSetupEditor : MonoBehaviour
{
    
   
    [MenuItem("AR Setup/Setup AR")]
    public static void SetupAR()
    {
        // Check if AR Foundation is installed
        if (!IsARFoundationInstalled())
        {
            // Install AR Foundation
            InstallARFoundation();
        }

        // Add XR Origin and AR Session to the scene
        AddXROriginAndARSession();

        // Add ARRaycastManager and ARPlaneManager to XR Origin
        AddARRaycastManagerAndARPlaneManager();

        // Add ARTapToPlace script to XR Origin and assign variables
        AddARTapToPlace();
    }

    private static bool IsARFoundationInstalled()
    {
        // Check if AR Foundation packages are installed
        return UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(ARSession).Assembly).name == "com.unity.xr.arfoundation";
    }

    private static void InstallARFoundation()
    {
        // Install AR Foundation package using Unity's package manager CLI
        string command = "unity add com.unity.xr.arfoundation";
        System.Diagnostics.Process.Start("CMD.exe", "/C " + command);
        Debug.Log("Installing AR Foundation package...");
    }

    private static void AddXROriginAndARSession()
    {
        // Check if main camera exists and delete it
       

        // Create XR Origin and AR Session if they don't exist
        if (GameObject.FindObjectOfType<XROrigin>() == null)
        {
            GameObject mainCamera = GameObject.Find("Main Camera");
            if (mainCamera != null)
            {
                Object.DestroyImmediate(mainCamera);
                Debug.Log("Main Camera deleted.");
            }
            GameObject xrOriginGO = new GameObject("XR Origin");
            XROrigin xrOrigin = xrOriginGO.AddComponent<XROrigin>();
            

            GameObject floorOffset = new GameObject("Floor Offset");
            floorOffset.transform.SetParent(xrOriginGO.transform);
            xrOrigin.CameraFloorOffsetObject = floorOffset;
            xrOrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Device;
            

            // Add AR Camera to XR Origin
            GameObject arCameraGO = new GameObject("AR Camera");
            arCameraGO.transform.SetParent(floorOffset.transform);
            Camera arCamera = arCameraGO.AddComponent<Camera>();
            arCamera.clearFlags = CameraClearFlags.SolidColor; 
            arCamera.backgroundColor = Color.black;
            arCameraGO.AddComponent<ARCameraManager>();
            arCameraGO.AddComponent<ARCameraBackground>();
            TrackedPoseDriver poseDriver = arCameraGO.AddComponent<TrackedPoseDriver>();

           
           
            // Map Input Actions to Tracked Pose Driver
            InputAction poseAction = new InputAction("ARPoseAction", InputActionType.PassThrough, "XR"); // Adjust "XR" to your Input System scheme

            // Map custom Input Action to Tracked Pose Driver bindings
            // poseAction.AddBinding("<HandheldARInputDevice>/devicePosition"); // Example custom binding

            poseDriver.positionAction.AddBinding("<XRHMD>/centerEyePosition");
            poseDriver. positionAction.AddBinding("<HandheldARInputDevice>/devicePosition"); // Example custom binding
            poseDriver.rotationAction.AddBinding("<XRHMD>/centerEyeRotation");
            poseDriver.rotationAction.AddBinding("<HandheldARInputDevice>/deviceRotation");
            arCameraGO.tag = "MainCamera";
            xrOrigin.Camera = arCamera;
           

        }

        if (GameObject.FindObjectOfType<ARSession>() == null)
        {
            GameObject arSessionGO = new GameObject("AR Session");
            arSessionGO.AddComponent<ARSession>();
            arSessionGO.AddComponent<ARInputManager>();


        }
    }

    private static void AddARRaycastManagerAndARPlaneManager()
    {
        // Add ARRaycastManager and ARPlaneManager to XR Origin
        GameObject xrOrigin = GameObject.Find("XR Origin");
        if (xrOrigin != null)
        {
            xrOrigin.AddComponent<ARRaycastManager>();
            ARPlaneManager arPlaneManager = xrOrigin.AddComponent<ARPlaneManager>();

            // Load and assign the plane prefab from the package
            GameObject planePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AR-Addon/Prefabs/ARPlane.prefab");
            if (planePrefab != null)
            {
                xrOrigin.GetComponent<ARPlaneManager>().planePrefab = planePrefab;
                Debug.Log("Plane prefab assigned to ARPlaneManager.");
            }
            else
            {
                Debug.LogError("Plane prefab not found in the package.");
            }
        }
        else
        {
            Debug.LogError("XR Origin not found in the scene.");
        }
    }

    private static void AddARTapToPlace()
    {
        // Add ARTapToPlace script to XR Origin
        GameObject xrOrigin = GameObject.Find("XR Origin");
        if (xrOrigin != null)
        {
            ARTapToPlace tapToPlace;
            if (xrOrigin.GetComponent<ARTapToPlace>()!= null)
            {
                tapToPlace = xrOrigin.GetComponent<ARTapToPlace>();
                Debug.Log("ARTapToPlace already added to XR Origin.");
            }
            else
            {
                tapToPlace = xrOrigin.AddComponent<ARTapToPlace>();
                Debug.Log("ARTapToPlace added to XR Origin.");
            }
            

            // Load and assign the marker prefab from the package
            GameObject markerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AR-Addon/Prefabs/Marker.prefab");
            if (markerPrefab != null )
            {
                if (tapToPlace.marker == null)
                {
                    GameObject markerInstance = Instantiate(markerPrefab);
                    markerInstance.name = "Marker";
                    tapToPlace.marker = markerInstance;
                    Debug.Log("Marker prefab assigned to ARTapToPlace.");
                }
                
            }
            else
            {
                Debug.LogError("Marker prefab not found in the package.");
            }

            // Load and assign the SpawnPrefab from the package
            GameObject spawnPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AR-Addon/Prefabs/ReplaceME.prefab");
            if (spawnPrefab != null)
            {
                //GameObject spawnInstance = Instantiate(spawnPrefab);
                tapToPlace.SpawnPrefab = spawnPrefab;
                Debug.Log("SpawnPrefab assigned to ARTapToPlace.");
            }
            else
            {
                Debug.LogError("SpawnPrefab not found in the package.");
            }
        }
        else
        {
            Debug.LogError("XR Origin not found in the scene.");
        }
    }

    [MenuItem("AR Setup/Create and Add AR Prefab")]
    public static void CreateOrUpdatePrefabFromSelected()
    {
        // Get the currently selected GameObject
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject != null)
        {
            // Get the path for saving the prefab
            string prefabPath = "Assets/AR-Addon/Prefabs/" + selectedObject.name + ".prefab";

            // Check if a prefab already exists at the path
            GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (existingPrefab != null)
            {
                // Delete the old prefab from the project
                AssetDatabase.DeleteAsset(prefabPath);
                Debug.Log("Deleted old prefab: " + prefabPath);

                PrefabUtility.SaveAsPrefabAsset(selectedObject, prefabPath);
                Debug.Log("Prefab created from selected GameObject: " + prefabPath);

                GameObject.FindObjectOfType<ARTapToPlace>().SpawnPrefab = existingPrefab;
            }
            else
            {
                // Create a new prefab from the selected GameObject
                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(selectedObject, prefabPath);
                Debug.Log("Prefab created from selected GameObject: " + prefabPath);
                GameObject.FindObjectOfType<ARTapToPlace>().SpawnPrefab = prefab;
            }

            DestroyImmediate(selectedObject);
        }
        else
        {
            Debug.LogWarning("No GameObject selected. Please select a GameObject to create or update a prefab.");
        }
    }
}