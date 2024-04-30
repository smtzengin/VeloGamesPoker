using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCard : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed;
    private Vector3 _offset = Vector3.zero;

    public void Setup(Transform target)
    {
        _target = target;

        if (_target.TryGetComponent(out Player p))
            _offset = new Vector3(0, 2f, 0);

        Vector3 targetPostition = new Vector3(target.transform.position.x,
                                       this.transform.position.y,
                                       target.transform.position.z);
        transform.LookAt(targetPostition);
    }
    // Update is called once per frame
    void Update()
    {
        if (_target == null)
            return;

        Vector3 direction = ((_target.transform.position + _offset) - transform.position).normalized;
        transform.position += direction * _speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, _target.transform.position + _offset) < 0.2f)
        {
            if (_target.TryGetComponent(out Player x))
                x.AddVisualCardToHand();
            else
                Table.Instance.OpenNextCard();
            Destroy(gameObject);
        }

    }
}
