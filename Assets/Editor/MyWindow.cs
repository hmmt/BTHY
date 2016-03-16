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
        if (elapsed > 0.05f)
        {
            Repaint();
            elapsed = 0;
        }

    }

    void OnGUI()
    {
		if (MapGraph.instance.loaded == false)
			return;
        GUILayout.Label("AGENT LIST", EditorStyles.boldLabel);
        foreach (AgentModel agent in AgentManager.instance.GetAgentList())
        {
            EditorGUILayout.BeginVertical();            GUILayout.Label("name : " + agent.name, EditorStyles.boldLabel);
            GUILayout.Label("state : " + agent.GetState());
            GUILayout.Label("lifeValue : " + agent.agentLifeValue);
            GUILayout.Label("HP : " + agent.hp);
            GUILayout.Label("mental : " + agent.mental);
			GUILayout.Label ("panicValue : " + agent.panicValue);
			GUILayout.Label ("isMoving : " + agent.GetMovableNode ().IsMoving ());
			/*
            GUILayout.Label("direct skill: " + agent.directSkill.id);
            GUILayout.Label("indirect skill: " + agent.indirectSkill.id);
            GUILayout.Label("block skill: " + agent.blockSkill.id);
            */
            
            EditorGUILayout.EndVertical();
        }
		/*
        foreach (PassageObjectModel passage in MapGraph.instance.GetPassageObjectList())
        {
            if (passage.IsClosable())
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Label("name : " + passage.GetId());
                if (GUILayout.Button("Open"))
                {
                    passage.OpenPassage();
                }
                if (GUILayout.Button("Close"))
                {
                    passage.ClosePassage();
                }
                EditorGUILayout.EndVertical();
            }
        }
        */
        /*
        myString = EditorGUILayout.TextField("Text Field", myString);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();
        */
    }
}