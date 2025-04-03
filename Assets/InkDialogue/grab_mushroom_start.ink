=== grabMushroomStart ===
{ MushroomQuest1State :
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START": -> canStart
    - "IN_PROGRESS": -> inProgress
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> END
}

= requirementsNotMet
// not possible for this quest, but putting something here anyways
Come back once you've talked to that other guy.
-> END

= canStart
Hey stranger.. how about you pick one of those red mushrooms over there..
Heheh..
* [Ok..]
    ~ StartQuest(MushroomQuest1Id)
    ~ PlayAnimation("MysteriousMan_NPC_dark,Hood_Up")
    Heheh..
* [No]
    Your loss, stranger..
- -> END

= inProgress
How is collecting that mushroom going? You blind or something?
-> END

= canFinish
Nicely done stranger.. now go give it to that purple guy over there..
-> END

= finished
What do you want?
-> END