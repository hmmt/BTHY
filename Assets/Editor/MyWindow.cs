//C# Example
using UnityEditor;
using UnityEngine;

public class MyWindow : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/My Window")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(MyWindow));
    }

    float elapsed = 0;
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > 1)
        {
            Repaint();
            elapsed = 0;
        }

    }

    void OnGUI()
    {
        GUILayout.Label("AGENT LIST", EditorStyles.boldLabel);
        foreach (AgentModel agent in AgentManager.instance.GetAgentList())
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label("name : " + agent.name, EditorStyles.boldLabel);
            GUILayout.Label("lifeValue : " + agent.agentLifeValue);
            GUILayout.Label("HP : " + agent.hp);
            GUILayout.Label("mental : " + agent.mental);
            GUILayout.Label("direct skill: " + agent.directSkill.id);
            GUILayout.Label("indirect skill: " + agent.indirectSkill.id);
            GUILayout.Label("block skill: " + agent.blockSkill.id);
            
            EditorGUILayout.EndVertical();
        }
        /*
        myString = EditorGUILayout.TextField("Text Field", myString);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();
        */
    }
}