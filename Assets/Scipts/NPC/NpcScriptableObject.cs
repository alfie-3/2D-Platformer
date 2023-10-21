using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "CharacterData/Character", order = 1), System.Serializable]
public class NpcScriptableObject : ScriptableObject
{
    public string NPC_Name;

    public Color NPC_NameColour;
    public Color NPC_DialogueColour;
}