using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDice : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Dice;
    public Transform[] SpawnPos1 = new Transform[3];
    public GameObject[] Card = new GameObject[3];
    public Transform[] SpawnPos = new Transform[3];

    void Start()
    {
        GameManager.GetInstance().NewTurn = true;
    }
    private void Update()
    {

      
      
     
    }
}
