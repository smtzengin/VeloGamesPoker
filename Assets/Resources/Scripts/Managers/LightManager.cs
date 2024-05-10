using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public static LightManager Instance { get; private set; }
    [SerializeField] private Light _turnIndicatorLight; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MoveTurnIndicator(Vector3 playerPosition)
    {
        _turnIndicatorLight.transform.position = playerPosition + new Vector3(0, 6, 0);
    }
}