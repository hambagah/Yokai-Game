INCLUDE globals.ink

Do you want to sleep?
    * Yes
        You fall asleep.
        ~ startQuest("sleeping")
    * No
        You relinquish the option to slumber.
-> END