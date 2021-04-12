using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    public bool AllStop;
    public int MyEye;
    public Sprite[] DiceSprite = new Sprite[6];
    private SpriteRenderer spriteRenderer;
    private bool Stop = false;
    private GameObject[] obj;
    public Camera cam;
    public Vector3 firstpos;
    private Vector2 campos;
    // Start is called before the first frame update

    void Start()
    {
        firstpos = this.transform.position;
        this.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, Random.Range(100, 200)));
        this.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50)));

    }

    // Update is called once per frame
    void Update()
    {
        obj = new GameObject[3];
        obj[0] = GameObject.Find("Position1");
        obj[1] = GameObject.Find("Position2");
        obj[2] = GameObject.Find("Position3");
        //Debug.Log(obj[0].position);
        //Debug.Log(obj[1].position);
        //Debug.Log(obj[2].position);

        if (UIManager.GetInstance().Next == true)
        {
            Destroy(this.gameObject);
            Debug.Log("gd");
        }

    }
    public void DelDice()
    {
        Destroy(this.gameObject);
    }

    void OnMouseDrag()
    {
        if (GameManager.GetInstance().NewTurn == false)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -60);
            this.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(mousePosition).x, -20, Camera.main.ScreenToWorldPoint(mousePosition).z);
        if (this.transform.parent != null)
        {
            this.transform.parent = null;
        }
        }
    }

    private void OnMouseUp()
    {
        if (GameManager.GetInstance().NewTurn == false)
        {
            if (Mathf.Abs(obj[0].transform.position.x - this.transform.position.x) < 0.5f && Mathf.Abs(obj[0].transform.position.z - this.transform.position.z) < 0.5f)
            {
                this.transform.position = obj[0].transform.position;
                if (obj[0].transform.childCount == 0)
                {
                    this.transform.parent = obj[0].transform;
                }
                else if (obj[0].transform.childCount == 1)
                {
                    if (obj[0].transform.GetChild(0).gameObject != this)
                    {
                        obj[0].transform.GetChild(0).gameObject.transform.position = obj[0].transform.GetChild(0).gameObject.GetComponent<Die>().firstpos;
                        obj[0].transform.GetChild(0).gameObject.transform.parent = null;
                    }
                    this.transform.SetParent(obj[0].transform);
                }
            }
            else if (Mathf.Abs(obj[1].transform.position.x - this.transform.position.x) < 0.5f && Mathf.Abs(obj[1].transform.position.z - this.transform.position.z) < 0.5f)
            {
                this.transform.position = obj[1].transform.position;
                if (obj[1].transform.childCount == 0)
                {
                    this.transform.parent = obj[1].transform;
                }
                else if (obj[1].transform.childCount == 1)
                {
                    if (obj[1].transform.GetChild(0).gameObject != this)
                    {
                        obj[1].transform.GetChild(0).gameObject.transform.position = obj[1].transform.GetChild(0).gameObject.GetComponent<Die>().firstpos;
                        obj[1].transform.GetChild(0).gameObject.transform.parent = null;
                    }
                    this.transform.SetParent(obj[1].transform);
                }
            }
            else if (Mathf.Abs(obj[2].transform.position.x - this.transform.position.x) < 0.5f && Mathf.Abs(obj[2].transform.position.z - this.transform.position.z) < 0.5f)
            {
                this.transform.position = obj[2].transform.position;
                if (obj[2].transform.childCount == 0)
                {
                    this.transform.parent = obj[2].transform;
                }
                else if (obj[2].transform.childCount == 1)
                {
                    if (obj[2].transform.GetChild(0).gameObject != this)
                    {
                        obj[2].transform.GetChild(0).gameObject.transform.position = obj[2].transform.GetChild(0).gameObject.GetComponent<Die>().firstpos;
                        obj[2].transform.GetChild(0).gameObject.transform.parent = null;
                    }
                    this.transform.SetParent(obj[2].transform);
                }
            }
            else
            {
                this.transform.position = firstpos;
                this.transform.parent = null;
            }
        }
    }
}




