using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class CardSO : ScriptableObject
{
    public Sprite CardSprite;
    public CardSign Sign;

    public CardValue Value;

    public int Id;
}
