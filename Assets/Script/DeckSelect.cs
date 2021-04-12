using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelect : MonoBehaviour
{
    private int Dex;
    public int[] NewDeck = new int[10];
    public List<int[]> DeckKind = new List<int[]>();
    // Start is called before the first frame update
    void Start()
    {
        DeckKind.Add(NewDeck);
    }

    void DeckNum(int a)
    {
        Dex = a;
    }
    // Update is called once per frame
   
}
