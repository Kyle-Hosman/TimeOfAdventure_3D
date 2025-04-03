using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class InkExternalFunctions
{
    private Animator npcAnimator;

    public void SetNPCAnimator(Animator animator)
    {
        npcAnimator = animator;
    }

    public void Bind(Story story)
    {
        story.BindExternalFunction("StartQuest", (string questId) => StartQuest(questId));
        story.BindExternalFunction("AdvanceQuest", (string questId) => AdvanceQuest(questId));
        story.BindExternalFunction("FinishQuest", (string questId) => FinishQuest(questId));
        story.BindExternalFunction("PlayAnimation", (string args) => PlayAnimation(args));
    }

    public void Unbind(Story story)
    {
        story.UnbindExternalFunction("StartQuest");
        story.UnbindExternalFunction("AdvanceQuest");
        story.UnbindExternalFunction("FinishQuest");
        story.UnbindExternalFunction("PlayAnimation");
    }

    private void StartQuest(string questId) 
    {
        GameEventsManager.instance.questEvents.StartQuest(questId);
    }

    private void AdvanceQuest(string questId) 
    {
        GameEventsManager.instance.questEvents.AdvanceQuest(questId);
    }

    private void FinishQuest(string questId)
    {
        GameEventsManager.instance.questEvents.FinishQuest(questId);
    }

    private void PlayAnimation(string args)
    {
        string[] parameters = args.Split(',');
        if (parameters.Length != 2)
        {
            Debug.LogError("Invalid arguments for PlayAnimation. Expected format: 'npcName,animationName'");
            return;
        }

        string npcName = parameters[0].Trim();
        string animationName = parameters[1].Trim();

        GameObject npc = GameObject.Find(npcName);
        if (npc != null)
        {
            Animator animator = npc.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play(animationName);
            }
            else
            {
                Debug.LogError("Animator component not found on NPC.");
            }
        }
        else
        {
            Debug.LogError("NPC GameObject not found.");
        }
    }
}
