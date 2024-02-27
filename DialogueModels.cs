using System;

[Serializable]
public class Dialogue
{
    public DialogueNode[] dialogueNodes;
}

[Serializable]
public class DialogueOption
{
    public string text;
    public int responseNode;
}

[Serializable]
public class DialogueNode
{
    public int id;
    public string npcName;
    public string text;
    public DialogueOption[] options;
}
