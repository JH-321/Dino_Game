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
