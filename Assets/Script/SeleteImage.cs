using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SeleteImage : MonoBehaviour
{
    public Sprite[] sprites = new Sprite[20];
    public int num;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.GetComponent<SpriteRenderer>().sprite = sprites[GameManager.GetInstance().MyDeck[num]-1];
    }
        
}
