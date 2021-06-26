using System;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private float _speed;
    [SerializeField] private float _lifeSpan;
    private Rigidbody _rigidbody;
    private Transform _transform;

    public int Team { get; set; }
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = transform;
        if (_lifeSpan > 0)
            Destroy(gameObject, _lifeSpan);
    }

    void FixedUpdate()
    {
        _rigidbody.MovePosition(_speed * Time.deltaTime * _transform.forward + _transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.SendMessage("ApplyDamage", _damage, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}
