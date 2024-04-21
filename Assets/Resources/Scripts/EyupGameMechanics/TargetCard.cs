using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCard : MonoBehaviour
{
    [SerializeField] private Player _target;
    [SerializeField] private float _speed;


    public void Setup(Player target)
    {
        _target = target;
    }
    // Update is called once per frame
    void Update()
    {
        if (_target == null)
            return;

        Vector3 direction = ((_target.transform.position + new Vector3(0,2f,0)) - transform.position).normalized;
        transform.position += direction * _speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, _target.transform.position + new Vector3(0, 2f, 0)) < 0.5f)
        {
            _target.AddVisualCardToHand();
            Destroy(gameObject);
        }

    }
}
