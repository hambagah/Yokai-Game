using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ObjectIcon : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField] private GameObject canFinishIcon;

    private void Awake()
    {
        canFinishIcon.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            canFinishIcon.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            canFinishIcon.SetActive(false);
        }
    }
}
