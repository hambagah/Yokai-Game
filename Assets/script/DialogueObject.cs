using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace At0m1c.DialogueSystem {
    [CreateAssetMenu (fileName = "DialogueObject", menuName = "Dialogue Object", order = 0)]

    public class DialogueObject : ScriptableObject {
        [Header("Dialogue")]
        public List<DialogueSegment> dialogueSegments = new List<DialogueSegment>();
    }

    [System.Serializable]
    public struct DialogueSegment {
        public string dialogueText;
        public float dialogueDisplayTime;
        public List<DialogueChoice> dialogueChoices;
    }

    [System.Serializable]
    public struct DialogueChoice {
        public string dialogueChoice;
        public DialogueObject followOnDialogue;
    }
}