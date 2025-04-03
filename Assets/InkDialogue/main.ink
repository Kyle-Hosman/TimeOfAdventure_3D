// external functions
EXTERNAL StartQuest(questId)
EXTERNAL AdvanceQuest(questId)
EXTERNAL FinishQuest(questId)
EXTERNAL PlayAnimation(args)

// quest ids (questId + "Id" for variable name)
VAR CollectCoinsQuestId = "CollectCoinsQuest"
VAR MushroomQuest1Id = "MushroomQuest1"

// quest states (questId + "State" for variable name)
VAR CollectCoinsQuestState = "REQUIREMENTS_NOT_MET"
VAR MushroomQuest1State = "REQUIREMENTS_NOT_MET"

// ink files
INCLUDE collect_coins_start_npc.ink
INCLUDE collect_coins_finish_npc.ink
INCLUDE grab_mushroom_start.ink

INCLUDE grab_mushroom_finish.ink


