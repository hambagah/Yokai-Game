
=== visitSpotsQuest ===
{ VisitSpotsQuestState :
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
Here's a little tour guide through the house. 

* [Yes]
    ~ StartQuest("VisitSpotsQuest")
    Great!
* [No]
    That's fine too.
- -> END

= inProgress
How many boxes have you cleaned so far?
-> END

= canFinish
Are you done now? Thanks here's your reward.
~ FinishQuest("VisitSpotsQuest")
-> END

= finished 
Good job. You sure showed those boxes.
-> END
