using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scaleOnTouch : MonoBehaviour
{
    float touchesPrevPosDifference, touchesCurPosDifference;
    [SerializeField]
    float MaxValue, MinValue;

    [Range(0.1f, 0.8f)]
    public float zoomModifier;
    Vector2 firstTouchPrevPos, secondTouchPrevPos;



    float zoomModifierSpeed = 0.0009f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDrag()
    {
        if (Input.touchCount == 2)
        {
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);

            firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
            secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

            touchesPrevPosDifference = (firstTouchPrevPos - secondTouchPrevPos).magnitude;
            touchesCurPosDifference = (firstTouch.position - secondTouch.position).magnitude;
            Debug.LogError(transform.localScale.x);
            zoomModifier = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * zoomModifierSpeed;
           
            var scaleNeedToChange = Vector3.zero;
            if (touchesPrevPosDifference > touchesCurPosDifference)
                scaleNeedToChange = this.transform.localScale -= new Vector3(zoomModifier, zoomModifier, zoomModifier);
            if (touchesPrevPosDifference < touchesCurPosDifference)
                scaleNeedToChange = this.transform.localScale += new Vector3(zoomModifier, zoomModifier, zoomModifier);
            

            scaleNeedToChange.x = Mathf.Clamp(scaleNeedToChange.x, MinValue, MaxValue);
            scaleNeedToChange.y = Mathf.Clamp(scaleNeedToChange.y, MinValue, MaxValue);
            scaleNeedToChange.z = Mathf.Clamp(scaleNeedToChange.z, MinValue, MaxValue);

           

            transform.localScale = scaleNeedToChange;

        }
    }

   

}
