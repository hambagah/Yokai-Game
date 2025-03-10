EXTERNAL StartQuest (questId)
EXTERNAL AdvanceQuest(questId)
EXTERNAL FinishQuest(questId)

// quest names (questId + Id)
VAR CleanBoxesQuestId = "CleanBoxesQuest"

// quest states (questId + State)
VAR CleanBoxesQuestState = "REQUIREMENTS_NOT_MET"

// object names
VAR Box1Id = "Box1"

INCLUDE cleanBoxesNPC.ink
INCLUDE Box1.ink