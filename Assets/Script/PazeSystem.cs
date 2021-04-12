using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class PazeEvent : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private List<Vector3> vec3list = new List<Vector3>();
    private Vector3 PanelPosision;
    public int SlowSpeed;
    private Vector3 vec3;
    public int Paze = 2;
    float x;
    // Start is called before the first frame update
    void LerpVector()
    {
        for(int i = 0; i<10;i++)
        {

        }
    }
    void Start()
    {
        PanelPosision = transform.position;
        transform.position= new Vector3( 0,0, 90);
    }

    public void OnDrag(PointerEventData data)
    {
        x = (data.pressPosition.x- data.position.x) / SlowSpeed;
        transform.position = new Vector3(PanelPosision.x,0, 90) - new Vector3(x, 0,0);

    }
    public void OnEndDrag(PointerEventData data)
    {
        if (x > 2f)
        {
            if (Paze == 2)
                Paze--;
            else if (Paze == 3)
                Paze--;
        }
        else if (x < -2f)
        {
            if (Paze == 1)
                Paze++;
            else if (Paze == 2)
                Paze++;
        }
    
        if(Paze == 1)
        {
            transform.position=new Vector3(-5.6f,0, 90);
        }
        else if (Paze == 2)
        {
            transform.position = new Vector3(0, 0, 90);
        }
        else if (Paze == 3)
        {
            transform.position = new Vector3(5.6f, 0, 90);
        }
        PanelPosision = transform.position;

    }
    public void PazeSkip(int a)
    {

        Paze = a;
        if (Paze == 1)
        {
            transform.position = new Vector3(-5.6f, 0, 90);
        }
        else if (Paze == 2)
        {
            transform.position = new Vector3(0, 0, 90);
        }
        else if (Paze == 3)
        {
            transform.position = new Vector3(5.6f, 0, 90);
        }
        PanelPosision = transform.position;
    }
}
