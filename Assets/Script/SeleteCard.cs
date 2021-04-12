using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeleteCard : MonoBehaviour
{
    public int CardNum;
    public GameObject[] obj;
    Vector3 ve;
    // Start is called before the first frame update
    void Start()
    {
        ve = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        this.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
        GameManager.GetInstance().tpoi = false;

    }
    private void OnMouseUp()
    {
        for (int i = 0; i < 8; i++)
        {

            if (Mathf.Abs(obj[i].transform.position.x - this.transform.position.x) < 0.5f && Mathf.Abs(obj[i].transform.position.y - this.transform.position.y) < 0.5f)
            {
                GameManager.GetInstance().MyDeck[i] = CardNum;
            }
           
        }
        transform.position = ve;
        GameManager.GetInstance().tpoi = false;
    }
}
