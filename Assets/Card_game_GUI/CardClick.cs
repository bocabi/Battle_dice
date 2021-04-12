using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardClick : MonoBehaviour
{
    public Sprite[] sprite;
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<SpriteRenderer>().sprite = sprite[obj.GetComponent<Card>().a-1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
