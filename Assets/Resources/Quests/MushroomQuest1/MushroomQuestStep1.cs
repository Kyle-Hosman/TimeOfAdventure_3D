using UnityEngine;

public class MushroomQuestStep1 : QuestStep
{
    private int mushroomsCollected = 0;
    private int mushroomsToComplete = 1;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        GameEventsManager.instance.miscEvents.onMushroomCollected += MushroomsCollected;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.onMushroomCollected -= MushroomsCollected;
    }

    private void MushroomsCollected()
    {
        if (mushroomsCollected < mushroomsToComplete)
        {
            mushroomsCollected++;
            UpdateState();
        }

        if (mushroomsCollected >= mushroomsToComplete)
        {
            FinishQuestStep();
            
        }
    }

    private void UpdateState()
    {
        string state = mushroomsCollected.ToString();
        string status = "Collected " + mushroomsCollected + " / " + mushroomsToComplete + " mushrooms.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        this.mushroomsCollected = System.Int32.Parse(state);
        UpdateState();
    }
}
