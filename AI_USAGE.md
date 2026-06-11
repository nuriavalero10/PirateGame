**Nuria Valero Bedia - Erasmus**

## Tools Used
* Google Gemini: I used it to make images, ask for advice, and fix some errors.

## How I Used AI

**1. My own work (No AI):**
I wrote almost all the C# code myself. I made the main "while" loop, the classes ("GameObject", "PlayerObject", "Enemy", "Treasure"), the collisions, and the LINQ logic to change levels. I followed the examples from Labs 4 to 7 to do this.

**2. Images:**
I used Gemini to create all the 2D pixel-art images (Pirate, Skeleton, Treasure, and Background).

**3. Chat and advice:**
I asked the AI for help when I had problems, so I didn't get stuck:
* Code structure: In Lab 7, we separated the code into many files. I asked the AI if I should do that for my game. The AI said it is better to keep the game loop in "Program.cs" because my game is small. I followed this advice to keep things simple.
* Black background error: My PNG images had black boxes instead of being transparent. I asked the AI how to fix it, and it told me to use "BlendMode.Blend" in SDL.
* Math for the images: In Lab 9, we used a JSON file for animations. My game only has one character in a simple 2x2 grid. Making a full JSON file was too much work for just one image. I asked the AI for a math formula instead. It gave me the math using modulo ("%") and division ("/") to cut the image correctly when the player moves.
* Save game lag bug: When the game saved the JSON file, the game lagged and completed levels automatically. I asked the AI to find the bug, and it told me to use "timer.Restart()" to reset the Delta Time after saving.

## Fully AI-Generated Regions
* Small parts: The AI only wrote the lines to save and load the JSON file inside "SaveManager.cs". 
* Summary: I did not use AI to write the main mechanics. I wrote much more than 50% of the code myself.