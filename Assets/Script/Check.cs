using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check : MonoBehaviour
{
    private Vector3 NowPos;
    public GameObject Die;
    public int DieNum;
    float timer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

    }
    private void OnTriggerStay(Collider other)
    {

        timer += Time.deltaTime;
        if (other.CompareTag("Floor"))
        {

            if (timer > 2f)
            {
                if (NowPos == transform.position)
                {
                   Die.GetComponent<Die>().MyEye = DieNum;
                   Die.GetComponent<Die>().AllStop = true;
                }
            }

            NowPos = transform.position;
        }
        else
        {
            timer = 0;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        timer = 0;
    }
}
