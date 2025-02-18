INCLUDE globals.ink

Do you want to sleep?
    * Yes
        ~ startQuest("sleeping")
        You fall asleep.
    * No
        You relinquish the option to slumber.
-> END