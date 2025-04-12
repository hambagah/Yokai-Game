
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
Hello I heard that you were the new comer.
I'm Konoe.
I'm a humble servant in this house.
Here's a little tour guide through the house. 
The Kitchen is in the farthest compartment of room.
The Shed is the house outside.
And the Lake is in the backyard.
Come back once you're satisfied with the view ~
* [Ok]
    ~ StartQuest("VisitSpotsQuest")
- -> END

= inProgress
Are you done yet? ~
-> END

= canFinish
Are you done now? Hope you enjoyed the view.
It's getting quite late now.
You should go to bed.
See you tomorrow. ~
~ CallFinishQuest("VisitSpotsQuest")
-> END

= finished 
Nighty night ~
-> END
