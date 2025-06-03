using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    [Header("Ink Story")]
    [SerializeField] private TextAsset inkJson;

    private Story story;
    private int currentChoiceIndex = -1;

    private bool dialoguePlaying = false;

    private InkExternalFunctions inkExternalFunctions;
    private InkDialogueVariables inkDialogueVariables;

    private float dialogueCooldown = 0.2f; // seconds
    private float lastDialogueEndTime = -1f;

    private void Awake() 
    {
        story = new Story(inkJson.text);
        inkExternalFunctions = new InkExternalFunctions();
        inkExternalFunctions.Bind(story);
        inkDialogueVariables = new InkDialogueVariables(story);
    }

    private void OnDestroy() 
    {
        inkExternalFunctions.Unbind(story);
    }

    private void OnEnable() 
    {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue += EnterDialogue;
        GameEventsManager.instance.inputEvents.onInteractPressed += InteractPressed;
        GameEventsManager.instance.dialogueEvents.onUpdateChoiceIndex += UpdateChoiceIndex;
        GameEventsManager.instance.dialogueEvents.onUpdateInkDialogueVariable += UpdateInkDialogueVariable;
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
    }

    private void OnDisable() 
    {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue -= EnterDialogue;
        GameEventsManager.instance.inputEvents.onInteractPressed -= InteractPressed;
        GameEventsManager.instance.dialogueEvents.onUpdateChoiceIndex -= UpdateChoiceIndex;
        GameEventsManager.instance.dialogueEvents.onUpdateInkDialogueVariable -= UpdateInkDialogueVariable;
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void QuestStateChange(Quest quest) 
    {
        GameEventsManager.instance.dialogueEvents.UpdateInkDialogueVariable(
            quest.info.id + "State",
            new StringValue(quest.state.ToString())
        );
    }

    private void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value) 
    {
        inkDialogueVariables.UpdateVariableState(name, value);
    }

    private void UpdateChoiceIndex(int choiceIndex) 
    {
        this.currentChoiceIndex = choiceIndex;
    }

    private void InteractPressed() 
    {
        if (GameEventsManager.instance.inputEvents.inputEventContext != InputEventContext.DIALOGUE)
        {
            //Debug.Log("Input context is not DIALOGUE. Ignoring input.");
            return;
        }

        ContinueOrExitStory();
    }

    private void EnterDialogue(string knotName) 
    {
        // don't enter dialogue if we've already entered
        if (dialoguePlaying) 
        {
            return;
        }
        // Prevent re-entering dialogue too soon after exiting
        if (Time.time - lastDialogueEndTime < dialogueCooldown)
        {
            Debug.Log("Dialogue cooldown active, not entering dialogue.");
            return;
        }

        dialoguePlaying = true;

        // inform other parts of our system that we've started dialogue
        GameEventsManager.instance.dialogueEvents.DialogueStarted();

        // freeze player movement
        GameEventsManager.instance.playerEvents.DisablePlayerMovement();

        // input event context
        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DIALOGUE);
        
        // jump to the knot
        if (!knotName.Equals(""))
        {
            story.ChoosePathString(knotName);
        }
        else 
        {
            Debug.LogWarning("Knot name was the empty string when entering dialogue.");
        }

        // start listening for variables
        inkDialogueVariables.SyncVariablesAndStartListening(story);

        // kick off the story
        ContinueOrExitStory();
    }

    private void ContinueOrExitStory() 
    {
        //Debug.Log($"ContinueOrExitStory called. story.canContinue: {story.canContinue}, currentChoices: {story.currentChoices.Count}, currentChoiceIndex: {currentChoiceIndex}");
        // make a choice, if applicable
        if (story.currentChoices.Count > 0 && currentChoiceIndex != -1)
        {
            story.ChooseChoiceIndex(currentChoiceIndex);
            // reset choice index for next time
            currentChoiceIndex = -1;
        }

        if (story.canContinue)
        {
            string dialogueLine = story.Continue();

            // handle the case where there's an empty line of dialogue
            // by continuing until we get a line with content
            while (IsLineBlank(dialogueLine) && story.canContinue) 
            {
                dialogueLine = story.Continue();
            }
            // handle the case where the last line of dialogue is blank
            // (empty choice, external function, etc...)
            if (IsLineBlank(dialogueLine) && !story.canContinue) 
            {
                //Debug.Log("Dialogue line is blank and story cannot continue. Exiting dialogue.");
                ExitDialogue();
            }
            else 
            {
                GameEventsManager.instance.dialogueEvents.DisplayDialogue(dialogueLine, story.currentChoices);
            }
        }
        else if (story.currentChoices.Count == 0)
        {
            //Debug.Log("No more choices and story cannot continue. Exiting dialogue.");
            ExitDialogue();
        }
    }

    private void ExitDialogue()
    {
        //Debug.Log("ExitDialogue called. Resetting input context and dialogue state.");
        dialoguePlaying = false;

        // inform other parts of our system that we've finished dialogue
        GameEventsManager.instance.dialogueEvents.DialogueFinished();

        // let player move again
        GameEventsManager.instance.playerEvents.EnablePlayerMovement();

        // input event context
        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DEFAULT);

        // stop listening for dialogue variables
        inkDialogueVariables.StopListening(story);

        // reset story state
        story.ResetState();
        lastDialogueEndTime = Time.time;
    }

    private bool IsLineBlank(string dialogueLine)
    {
        return dialogueLine.Trim().Equals("") || dialogueLine.Trim().Equals("\n");
    }
}
