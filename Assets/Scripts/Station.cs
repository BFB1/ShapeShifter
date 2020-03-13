using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Station : MonoBehaviour
{
    private int shape;
    public int Shape
    {
        get => shape;
        set {
            queue.RemoveAll((item) => (item == value));
            shape = value;
        }
    }

    [NonSerialized] public const int Capacity = 20;
    public List<int> queue;
    
    private void Awake()
    {
        queue = new List<int>();
    }
    
    private void Update()
    {
        if (Random.value < 0.006f)
        {
            AddNewPassenger();
        }

        if (Capacity < queue.Count - 5) // 5 Passengers grace for the player to solve the problem
        // TODO This should not end the game immediately but rather start a countdown to give the player a chance.
        {
            GameMaster.Instance.GameOver();
        }
    }


    private void AddNewPassenger()
    {
        int newPassenger;
        do
        {
            newPassenger = Random.Range(0, GameMaster.Instance.AvailableShapes);
        } while (newPassenger == shape);
        queue.Add(newPassenger);
    }
}
