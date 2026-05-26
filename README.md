# Dino_Game

CS20200 2026S F# CLI version of the Chrome Dino game.

## Requirements

- .NET 10 SDK

## Run

```sh
dotnet run
```

## Controls

- Space or Up arrow: jump
- Down arrow: duck on the ground
- Down arrow while jumping: fast fall
- Q during gameplay: save the current score and quit

After a crash:

- Enter or Space: restart
- Q: quit

## Features

- Random cactus and bird obstacles
- Multiple cactus shapes
- Animated bird wings
- Birds at high, middle, and low heights
- Gradually increasing speed
- Relative score file: `dino-scores.csv`
- Top five score display

## Use of Large Language Models

I used an LLM while developing this project. The LLM helped me review the project requirements, organize the README, and think through the structure of the F# command-line Dino game. I used it to discuss the overall game design, including how the screen should be redrawn in the terminal, how the dinosaur, ground, cacti, and birds should be represented with text characters, and how the game could still feel like the Chrome Dino game even though it runs in a command-line interface.

I also used the LLM to reason about gameplay details such as jump timing, ducking, fast-falling, obstacle movement, speed increases, collision checks, score updates, and saving scores after quitting or crashing. For the input controls, it helped me think through how the game should respond to Space, Up arrow, Down arrow, Enter, and Q in different states such as normal gameplay, jumping, ducking, fast-falling, and the crash/restart screen.

I still had to manually check and adjust the code and documentation. Some prompts needed to be clarified because the LLM did not always know the exact project constraints from the first prompt. I had to make sure the final implementation matched my proposal, used the correct .NET/F# project setup, and described only the features that were actually implemented. I also had to manually tune the controls and behavior so that the input keys felt responsive and did not trigger the wrong action in the wrong game state.

The main thing the LLM was not able to do correctly on its own was verify the final game behavior by playing it like a real user. I had to review the controls, restart flow, obstacle behavior, and score-saving behavior myself to make sure the submitted program satisfied the project requirements.
