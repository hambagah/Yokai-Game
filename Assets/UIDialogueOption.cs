using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace At0m1c.DialogueSystem {
    public class UIDialogueOption : MonoBehaviour
    {
        DialogueScript dialogueScript;
        DialogueObject dialogueObject;

        [SerializeField] TextMeshProUGUI dialogueText;

        public void Setup (DialogueScript _dialogueScript, DialogueObject _dialogueObject, string _dialogueText) {
            dialogueScript = _dialogueScript;
            dialogueObject = _dialogueObject;
            dialogueText.text = _dialogueText;
        }

        public void SelectOption() {
            dialogueScript.OptionSelected (dialogueObject);
        }
    }

}