using UnityEngine;

public class NPC : MonoBehaviour
{
    private void OnMouseDown()
    {
        DialogueManager.Instance.StartConversation(gameObject.name);
    }
}
