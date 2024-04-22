using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerButton : MonoBehaviour
{
    private bool _arrived;
    private float _speed = 3;
    private Transform _target;

    public void SetTarget(Transform target)
    {
        _arrived = false;
        _target = target;
    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null || _arrived)
            return;

        Vector3 direction = (_target.transform.position - transform.position).normalized;
        transform.position += direction * _speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, _target.transform.position) < 0.5f)
        {
            _arrived = true;
            transform.position = _target.transform.position;
        }
    }
}
