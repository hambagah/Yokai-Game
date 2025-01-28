using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace At0m1c.DialogueSystem {
    public class UIInteract : MonoBehaviour
    {
        /*[SerializeField] Canvas canvas;
        [SerializeField] UnityEvent OnInteract;

        void Awake() {
            canvas.enabled = false;
        }

        void Update() {
            transform.LookAt(Camera.main.transform, Vector3.back);
            transform.Rotate(Vector3.right * 180f);
        }

        void OnEnable() {
            Player.OnInteract += Interact;
            Player.OnEnterInteractable += ShowUI;
            Player.OnExitInteractable += HideUI;
        }

        void OnDisable() {
            Player.OnInteract -= Interact;
            Player.OnEnterInteractable -= ShowUI;
            Player.OnExitInteractable -= HideUI;
        }

        void Interact() {
            Debug.Log("Interacting");
            OnInteract.Invoke();
        }

        void ShowUI() {
            canvas.enabled = true;
        }

        void HideUI() {
            canvas.enabled = false;
        }*/
    }
}