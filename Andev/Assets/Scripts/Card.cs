using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public int Number => _number;
    private int _number;

    public Card(int num)
    {
        _number = num;
    }
}
