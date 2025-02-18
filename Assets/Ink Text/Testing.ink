INCLUDE globals.ink
-> main

=== main ===
This is a test to check your choices. #speaker:DemoNPC #portrait:DemoNPC_default #layout:left
Do you want to try it? 
    + [Yes]
        Alrighty then. #portrait:DemoNPC_happy
        This choice system will loop until you trigger the end.
        -> main
    + [Maybe]
        Pardon? Can you repeat that? #portrait:DemoNPC_sad
        ~ playEmote("question")
        -> maybe
    + [No]
        Ok then. #portrait:DemoNPC_sad
-> END

=== maybe ===
What does that mean?
    + [It means I will destroy you]
        Zomg #portrait:DemoNPC_surprised #layout:right
        ~ playEmote("exclamation")
        -> END
    + [I wanna try again] 
        Ok then. #portrait:DemoNPC_default
        -> main
-> END