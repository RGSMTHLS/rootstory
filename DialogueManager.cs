using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMP_Text dialogueText;

    [SerializeField] GameObject optionsPanel;
    [SerializeField] Button optionButtonPrefab;

    private Dictionary<string, Dialogue> dialogues = new Dictionary<string, Dialogue>();
    private Dialogue currentDialogue;

    public static DialogueManager Instance;

    private void Awake()
    {
        // Singleton that we can call when we need to start conversation
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        ReadJsonDialogues();
    }

    void ReadJsonDialogues()
    {
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Dialogues");

        foreach (TextAsset jsonDialogue in jsonFiles)
        {
            var filename = jsonDialogue.name;
            Debug.Log("File found! " + filename);
            Dialogue dialogue = JsonUtility.FromJson<Dialogue>(jsonDialogue.text);
            if (dialogue != null)
            {
                // Set dialogue to dictionary
                dialogues[filename] = dialogue;
                Debug.Log("Find dialogue " + filename);
            }
        }
    }

    void SetNode(DialogueNode node)
    {
        // Set the dialogue text from the node, which we get from the JSON
        dialogueText.text = node.text;
        ClearOptions();

        // Setup options buttons from JSON
        SetupOptionButtons(node.options);
    }

    private void SetupOptionButtons(DialogueOption[] options)
    {
        float yPos = 0f;
        if (options != null && options.Length > 0)
        {
            // If we found options belonging to this node in the JSON, loop through them and create buttons
            foreach (var option in options)
            {
                CreateOptionButton(option.text, () => OnOptionSelected(option), ref yPos);
            }
        }
        else
        {
            // Set default "good bye" buttons when conversation is over
            CreateOptionButton("Good bye", () => EndConversation(), ref yPos);
            CreateOptionButton("Thanks for absolutley nothing", () => EndConversation(), ref yPos);
        }
    }

    private void OnOptionSelected(DialogueOption option)
    {
        // Find the next node depending on which option we select
        DialogueNode nextNode = FindNodeById(option.responseNode);

        if (nextNode != null)
            SetNode(nextNode);
        else
            EndConversation();
    }

    // Pass the filename of the dialogue json 
    public void StartConversation(string dialogueName)
    {
        dialoguePanel.SetActive(true);
        optionsPanel.SetActive(true);

        // Set the currentDialogue to the dialogue with the filename we passed
        currentDialogue = dialogues[dialogueName];

        // Set the beginning of the conversation to the first node in the json
        SetNode(currentDialogue.dialogueNodes[0]);
    }

    private void EndConversation()
    {
        dialoguePanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    private DialogueNode FindNodeById(int id)
    {
        return Array.Find(currentDialogue.dialogueNodes, node => node.id == id);
    }

    private void CreateOptionButton(string buttonText, UnityAction onClickAction, ref float yPos)
    {
        // Create button and set the correct parent to show the buttons in the canvas
        Button optionButton = Instantiate(optionButtonPrefab, optionsPanel.transform);

        // Get text object from children of button
        TMP_Text optionButtonText = optionButton.GetComponentInChildren<TMP_Text>();

        // Set the text of the button from the option we get in the JSON
        optionButtonText.text = buttonText;

        // Get the rect transform of the button, so we can set each button below the latest one
        RectTransform buttonRectTransform = optionButton.GetComponent<RectTransform>();
        buttonRectTransform.anchoredPosition = new Vector2(0f, yPos);

        yPos -= (buttonRectTransform.sizeDelta.y + 10f);

        optionButton.onClick.AddListener(onClickAction);
    }

    private void ClearOptions()
    {
        foreach (Transform child in optionsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
