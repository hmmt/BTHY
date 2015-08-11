using UnityEngine;
using System.Collections;

public class AgentSpeech : MonoBehaviour {

    public TextMesh textDialogue;

    public void showSpeech(string speech)
    {
        if (!textDialogue.gameObject.activeSelf)
        {

            textDialogue.text = speech;

            textDialogue.gameObject.SetActive(true);

            TimerCallback.Create(5.0f, gameObject, delegate()
            {
                textDialogue.gameObject.SetActive(false);
            });
        }

    }

    public void turnOnDoingSkillIcon(bool turnOn)
    {
        textDialogue.gameObject.SetActive(turnOn);
    }
}

/*
 * 
 *         string speech;
        agent.speechTable.TryGetValue("work_start", out speech);
        agent.showSpeech.showSpeech(speech);
 * */
