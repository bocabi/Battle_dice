using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCard : MonoBehaviour
{
    
    public int a;
    int c = 1;
    // Start is called before the first frame update
    public Sprite[] sprites ;

    void Start()
    {
        WorldManasger.instance.objlist.Add(this.gameObject);
        transform.GetComponent<SpriteRenderer>().sprite = sprites[a - 1];
    }
   
    public void deletegameobject()
    {
        Debug.Log("되는거야마는거양");
        WorldManasger.instance.objlist.Remove(this.gameObject);
        Destroy(this);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void NowEmemyCard()
    {
        
    }
}