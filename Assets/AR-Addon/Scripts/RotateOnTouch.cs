using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnTouch : MonoBehaviour
{

   public  float RotateSpeed = 2;

   

     public void OnMouseDrag()
    {
        if (Input.touchCount <= 1) {
            
            float x = Input.GetAxis("Mouse X") * RotateSpeed * Mathf.Deg2Rad;
          
            transform.Rotate(Vector3.up, -x);
            
        }
    }
}
