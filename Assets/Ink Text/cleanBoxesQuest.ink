
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
Can you clean 5 boxes?
* [Yes]
    ~ StartQuest("CleanBoxesQuest")
    Great!
* [No]
    That's fine too.
- -> END

= inProgress
How many boxes have you cleaned so far?
-> END

= canFinish
~ FinishQuest("CleanBoxesQuest")
Are you done now? Thanks here's your reward.
-> END

= finished 
Good job. You sure showed those boxes.
-> END
