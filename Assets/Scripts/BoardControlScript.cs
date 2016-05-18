using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BoardControlScript : MonoBehaviour {
    [System.Serializable]
    public class TargetObject {
        public RectTransform panel;
        public Animator animator;
        public GameObject button;
        public ICollectionWindow script;
        public int index;
    }
    public static string animController = "Slide";
    public TargetObject agent;
    public AgentList agentScript;

    public TargetObject creature;
    public CollectionListScript creatureScript;

    private TargetObject openTarget = null;
    private TargetObject hideButton = null;
    private TargetObject appearButton = null;
    private RectTransform rect = null;
    private bool opened = false;
    private int openIndex = -1;

    public void Start() {
        agent.script = agentScript;
        creature.script = creatureScript;
    }

    public void ButtonSelect(int index) {
        TargetObject temp = null;
        TargetObject nonSelect = null;
        switch (index) { 
            case 0:
                temp = agent;
                nonSelect = creature;
                break;
            case 1:
                temp = creature;
                nonSelect = agent;
                break;
        }

        if (!opened)
        {
            opened = true;
            openIndex = index;
            openTarget = temp;

            openTarget.animator.SetBool(animController, true);

            openTarget.script.Open();
            nonSelect.button.SetActive(false);
        }
        else
        {
            if (openIndex == index)
            {
                //close
                openTarget.animator.SetBool(animController, false);
                openTarget.script.Close();
                opened = false;
                openIndex = -1;
                StartCoroutine(ActiveBoth(openTarget));
                openTarget = null;
                
            }
            else
            {
                //change
                openTarget.animator.SetBool(animController, false);
                temp.animator.SetBool(animController, true);
                temp.script.Open();
                openTarget.script.Close();

                StartCoroutine(HideButton(openTarget));
                StartCoroutine(ActiveBoth(temp));

                hideButton = openTarget;
                openTarget = temp;
                openIndex = index;
            }
        }
    }

    public IEnumerator HideButton(TargetObject target) {
        while (target.panel.anchoredPosition.x > -800) {
            yield return new WaitForEndOfFrame();
        }
        target.button.SetActive(false);
    }

    public IEnumerator ActiveButton(TargetObject target) {
        while (target.panel.anchoredPosition.x < -800) {
            yield return new WaitForEndOfFrame();
        }
        target.button.SetActive(true);
    }

    public IEnumerator ActiveBoth(TargetObject target) {
        while (target.panel.anchoredPosition.x > -955) {
            yield return new WaitForEndOfFrame();
        }
        agent.button.SetActive(true);
        creature.button.SetActive(true);
    }

}
