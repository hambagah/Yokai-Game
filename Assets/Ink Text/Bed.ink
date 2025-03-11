=== bed ===
Do you want to sleep?
    * [Yes]
        ~ SleepingEvent(0)
        You fall asleep.
    * [No]
        You relinquish the option to slumber.

- ->END

=== bedShuten ===
You feel hungry.
    * [I should visit the kitchen for a snack]
        ~ SleepingEvent(1)
        You head towards the kitchen.

- -> END