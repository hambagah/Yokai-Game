EXTERNAL StartQuest (questId)
EXTERNAL AdvanceQuest(questId)
//EXTERNAL CallFinishQuest(questId)
EXTERNAL FinishQuest(questId)
EXTERNAL SleepingEvent(value)

// quest names (questId + Id)
VAR CleanBoxesQuestId = "CleanBoxesQuest"
VAR VisitSpotsQuestId = "VisitSpotsQuest"

// quest states (questId + State)
VAR CleanBoxesQuestState = "REQUIREMENTS_NOT_MET"
VAR VisitSpotsQuestState = "REQUIREMENTS_NOT_MET"

// object names
//VAR Box1Id = "Box1"

INCLUDE cleanBoxesQuest.ink
INCLUDE visitSpotsQuest.ink
INCLUDE box1.ink
INCLUDE box2.ink
INCLUDE box3.ink
INCLUDE bed.ink
INCLUDE kitchen.ink
INCLUDE lake.ink
INCLUDE shed.ink