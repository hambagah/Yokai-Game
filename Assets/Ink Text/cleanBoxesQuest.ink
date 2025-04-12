
=== cleanBoxesQuest ===
{ CleanBoxesQuestState :
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START": -> canStart
    - "IN_PROGRESS": -> inProgress
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> END
}

= requirementsNotMet
Hmm you can't start.
-> END

= canStart
You're finally here.
I'm Fujiwara welcome to my humble abode. 
Since you're staying here for a long time I'll be expecting you to help with maintenance.
Speaking of the house is a bit... messy.
There's a couple of items that we need to relocate.
Can you clean 5 boxes?
* [Sure]
    ~ StartQuest("CleanBoxesQuest")
    Good.
    Come back to me when you're done.
- -> END

= inProgress
What.
-> END

= canFinish
~ CallFinishQuest("CleanBoxesQuest")
//~ FinishQuest("CleanBoxesQuest")
Are you done now? Thanks here's your reward.
It's getting late now. 
You may go inside.
Konoe will tour you through the house.
-> END

= finished 
You may leave now.
-> END
