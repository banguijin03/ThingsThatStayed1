using UnityEngine;

public class NPCModule : CharacterModule
{
    [SerializeField] string npcID;
    [SerializeField] string npcName;

    public string NPCID => npcID;
    public string NPCName => npcName;

    public sealed override System.Type RegistrationType => typeof(NPCModule);
}
