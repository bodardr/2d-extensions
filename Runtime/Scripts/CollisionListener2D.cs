using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionListener2D : MonoBehaviour
{
    [SerializeField]
    private string requiredTag;

    [SerializeField]
    private UnityEvent TriggerEnter2D;

    [SerializeField]
    private UnityEvent TriggerStay2D;

    [SerializeField]
    private UnityEvent TriggerExit2D;

    [SerializeField]
    private UnityEvent CollisionEnter2D;

    [SerializeField]
    private UnityEvent CollisionStay2D;

    [SerializeField]
    private UnityEvent CollisionExit2D;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (string.IsNullOrEmpty(requiredTag) || other.CompareTag(requiredTag))
            TriggerEnter2D.Invoke();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (string.IsNullOrEmpty(requiredTag) || other.CompareTag(requiredTag))
            TriggerStay2D.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (string.IsNullOrEmpty(requiredTag) || other.CompareTag(requiredTag))
            TriggerExit2D.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (string.IsNullOrEmpty(requiredTag) || other.transform.CompareTag(requiredTag))
            CollisionEnter2D.Invoke();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (string.IsNullOrEmpty(requiredTag) || other.transform.CompareTag(requiredTag))
            CollisionStay2D.Invoke();
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (string.IsNullOrEmpty(requiredTag) || other.transform.CompareTag(requiredTag))
            CollisionExit2D.Invoke();
    }
}