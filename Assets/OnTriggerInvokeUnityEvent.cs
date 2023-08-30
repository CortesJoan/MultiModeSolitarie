using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class OnTrigger2DInvokeUnityEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent<GameObject, Collider2D> onTriggerEnter;
    [SerializeField] private UnityEvent<GameObject, Collider2D> onTriggerExit;
    [SerializeField] private bool skipDisabledGameObjects;

    [Header("Events that only trigger with a specific tag")]
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private UnityEvent<GameObject, Collider2D> onTriggerEnterTag;
    [SerializeField] private UnityEvent<GameObject, Collider2D> onTriggerExitTag;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.activeInHierarchy && skipDisabledGameObjects)
        {
            Debug.Log("collider is inactive, skip collision handling");
            return;
        }
        onTriggerEnter?.Invoke(this.gameObject as GameObject, other);
        if (other.CompareTag(targetTag))
        {
            onTriggerEnterTag?.Invoke(this.gameObject, other);
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {if (!other.gameObject.activeInHierarchy && skipDisabledGameObjects)
        {
            Debug.Log("collider is inactive, skip collision handling");
            return;
        }

        onTriggerExit?.Invoke(this.gameObject as GameObject, other);
        if (other.CompareTag(targetTag))
        {
            onTriggerExitTag?.Invoke(this.gameObject, other);
        }
     }

 

    public void Setup()
    {
        onTriggerEnter = new UnityEvent<GameObject, Collider2D>();
        onTriggerExit = new UnityEvent<GameObject, Collider2D>();
        onTriggerEnterTag = new UnityEvent<GameObject, Collider2D>();
        onTriggerExitTag = new UnityEvent<GameObject, Collider2D>();

    }

    public UnityEvent<GameObject, Collider2D> ONTriggerEnter => onTriggerEnter;

    public UnityEvent<GameObject, Collider2D> ONTriggerExit => onTriggerExit;
    public UnityEvent<GameObject, Collider2D> ONTriggerEnterTag => onTriggerEnterTag;
    public UnityEvent<GameObject, Collider2D> ONTriggerExitTag => onTriggerExitTag;
    public string Tag => targetTag;
}
