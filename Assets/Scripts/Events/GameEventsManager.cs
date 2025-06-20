using System;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager instance { get; private set; }

    public InputEvents inputEvents;
    public PlayerEvents playerEvents;
    public GoldEvents goldEvents;
    public MiscEvents miscEvents;
    public QuestEvents questEvents;
    public DialogueEvents dialogueEvents;
    public MushroomEvents mushroomEvents;
    public InventoryEvents inventoryEvents;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("Found more than one Game Events Manager in the scene. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // initialize all events
        inputEvents = new InputEvents();
        playerEvents = new PlayerEvents();
        goldEvents = new GoldEvents();
        miscEvents = new MiscEvents();
        questEvents = new QuestEvents();
        dialogueEvents = new DialogueEvents();
        mushroomEvents = new MushroomEvents();
        inventoryEvents = new InventoryEvents();
    }
}
