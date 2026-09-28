using UnityEngine;

[System.Serializable]
public class ScenarioStep
{
    public string stepID;
    public string description;
}

[CreateAssetMenu(fileName = "ScenarioData", menuName = "Scenario/Scenario Data")]
public class ScenarioData : ScriptableObject
{
    public string scenarioID;
    public ScenarioStep[] steps;
}