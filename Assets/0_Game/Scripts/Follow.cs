using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform transFollow;

    private Vector3 _offset;
    private float _speed;
    public void Setup(Transform t, float s)
    {
        transFollow = t;
        _speed = s;
        _offset = transform.position - transFollow.position;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, transFollow.position + _offset, Time.deltaTime * _speed);
    }
}
