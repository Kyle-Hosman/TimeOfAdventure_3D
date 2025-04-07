using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class QuestPoint : MonoBehaviour
{
    [Header("Dialogue (optional)")]
    [SerializeField] private string dialogueKnotName;

    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;

    private bool playerIsNear = false;
    private string questId;
    private QuestState currentQuestState;

    private QuestIcon questIcon;

    private void Awake() 
    {
        questId = questInfoForPoint.id;
        questIcon = GetComponentInChildren<QuestIcon>();

        // Ensure the BoxCollider is set as a trigger
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;
    }

    private void OnEnable()
    {
        if (GameEventsManager.instance == null)
        {
            //Debug.LogWarning("GameEventsManager is not initialized yet. QuestPoint will retry initialization.");
            StartCoroutine(WaitForGameEventsManager());
            return;
        }

        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
    }

    private IEnumerator WaitForGameEventsManager()
    {
        while (GameEventsManager.instance == null)
        {
            yield return null; // Wait for the next frame
        }

        // Subscribe to events once GameEventsManager is ready
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
    }

    private void OnDisable()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
            GameEventsManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
        }
    }

    private void SubmitPressed(InputEventContext inputEventContext)
    {
        //Debug.Log("SubmitPressed called on QuestPoint: " + gameObject.name + ", playerIsNear: " + playerIsNear + ", inputEventContext: " + inputEventContext);
        if (!playerIsNear || !inputEventContext.Equals(InputEventContext.DEFAULT))
        {
            return;
        }

        // if we have a knot name defined, try to start dialogue with it
        if (!string.IsNullOrEmpty(dialogueKnotName)) 
        {
            GameEventsManager.instance.dialogueEvents.EnterDialogue(dialogueKnotName);
        }
        // otherwise, start or finish the quest immediately without dialogue
        else 
        {
            // start or finish a quest
            if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
            {
                GameEventsManager.instance.questEvents.StartQuest(questId);
            }
            else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
            {
                GameEventsManager.instance.questEvents.FinishQuest(questId);
            }
        }
    }

    private void QuestStateChange(Quest quest)
    {
        if (quest == null)
        {
            Debug.LogError("Quest is null in QuestStateChange.");
            return;
        }

        if (quest.info == null)
        {
            Debug.LogError($"Quest.info is null for quest: {quest}");
            return;
        }

        if (quest.info.id == null)
        {
            Debug.LogError($"Quest.info.id is null for quest: {quest}");
            return;
        }

        // only update the quest state if this point has the corresponding quest
        if (quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
            if (questIcon != null)
            {
                questIcon.SetState(currentQuestState, startPoint, finishPoint);
            }
            else
            {
                Debug.LogWarning("QuestIcon is null in QuestStateChange.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player entered QuestPoint trigger: " + gameObject.name);
            playerIsNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}
