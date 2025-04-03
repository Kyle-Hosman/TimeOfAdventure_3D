=== grabMushroomFinish ===
{ MushroomQuest1State:
    - "FINISHED": -> finished
    - else: -> default
}

= finished
Thank you!
-> END

= default
Hm? What do you want?
* [Nothing, I guess.]
    -> END
* { MushroomQuest1State == "CAN_FINISH" } [That red guy told me to give you this]
    ~ FinishQuest(MushroomQuest1Id)
    What? I dont want that mushroom!
-> END