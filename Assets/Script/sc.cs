using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class sc : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector3 PanelPosision;
    public int SlowSpeed;

    // Start is called before the first frame update
    void Start()
    {
        PanelPosision = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnDrag(PointerEventData data)
    {
        
        //if(GameManager.GetInstance().tpoi == false)
        //{
        //    if (x >= 0 && x <= 3.5)
        //    {
        //        x = (data.pressPosition.y - data.position.y) / SlowSpeed;
        //        transform.position = new Vector3(0, PanelPosision.y, 90) - new Vector3(0, x, 0);

        //    }
        //    else if (x < 0)
        //    {
        //        x = 0;
        //    }
        //    else if (x > 3.5)
        //    {
        //        x = 3.5f;
        //    }
        //}
       
    }
    public void OnEndDrag(PointerEventData data)
    {
        //if (x < 0)
        //{
        //    x = 0;
        //}
        //if( x >3.5)
        //{
        //    x = 3.5f;
        //}
    }
}
