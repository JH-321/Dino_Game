namespace DinoGame

open System
open System.Collections.Generic
open System.Threading
open DinoGame.Rendering
open DinoGame.Scores

module Game =
    let private waitForRestartOrQuit () =
        printfn ""
        printfn "Restart: Enter / Space"
        printfn "Quit: Q"

        let mutable decided = false
        let mutable restart = false

        while not decided do
            let key = Console.ReadKey(true).Key

            match key with
            | ConsoleKey.Enter
            | ConsoleKey.Spacebar ->
                restart <- true
                decided <- true
            | ConsoleKey.Q ->
                restart <- false
                decided <- true
            | _ -> ()

        restart

    let private showScores title score saved =
        Console.Clear()

        let scores = loadScores ()
        let high = highScoreOf scores
        let topScores = scores |> List.sortDescending |> List.truncate 5

        printfn "%s" title
        printfn ""
        printfn "Score: %d" score
        printfn "High score: %d" high
        printfn "Score file: %s" scoreFile
        printfn "Save status: %s" (if saved then "Score saved." else "No score saved.")
        printfn ""

        printfn "Top 5"

        if topScores.IsEmpty then
            printfn "No scores yet."
        else
            topScores
            |> List.iteri (fun i s -> printfn "%d. %d" (i + 1) s)

    let private runOnce initialHighScore =
        Console.Clear()

        let rng = Random()
        let width = clampWidth ()
        let renderHeight = 12
        let groundY = 10
        let dinoX = 6

        let obstacles = ResizeArray<Obstacle>()

        let mutable dinoY = 0.0
        let mutable velocity = 0.0
        let gravity = 0.28
        let jumpVelocity = 1.75

        let mutable duckTicks = 0
        let duckHoldTicks = 10
        let mutable ticksUntilSpawn = 28

        let mutable scoreRaw = 0.0
        let mutable speed = 1.0

        let mutable crashed = false
        let mutable quit = false

        while not crashed && not quit do
            let mutable jumpedThisTick = false

            while Console.KeyAvailable do
                let key = Console.ReadKey(true).Key

                match key with
                | ConsoleKey.UpArrow
                | ConsoleKey.Spacebar ->
                    if dinoY <= 0.0 then
                        velocity <- jumpVelocity
                        duckTicks <- 0
                        jumpedThisTick <- true

                | ConsoleKey.DownArrow ->
                    if jumpedThisTick then
                        ()
                    elif dinoY > 0.0 then
                        velocity <- Math.Min(velocity, -1.85)
                    else
                        duckTicks <- duckHoldTicks

                | ConsoleKey.Q ->
                    quit <- true

                | _ -> ()

            let isDucking =
                duckTicks > 0 && dinoY <= 0.0

            if duckTicks > 0 then
                duckTicks <- duckTicks - 1

            if dinoY > 0.0 || velocity > 0.0 then
                dinoY <- dinoY + velocity
                velocity <- velocity - gravity

                if dinoY < 0.0 then
                    dinoY <- 0.0
                    velocity <- 0.0

            let score = int scoreRaw
            speed <- Math.Min(10.0, 1.0 + scoreRaw / 900.0)

            ticksUntilSpawn <- ticksUntilSpawn - 1

            if ticksUntilSpawn <= 0 then
                let kind =
                    if score > 250 && rng.NextDouble() < 0.35 then
                        Bird
                    else
                        Cactus

                obstacles.Add(
                    { Kind = kind
                      Shape = if kind = Cactus then rng.Next(0, 4) else 0
                      Lane = if kind = Bird then rng.Next(0, 3) else 0
                      X = float (width - 6) }
                )

                let minGap = int (Math.Max(16.0, 34.0 - speed * 5.0))
                let maxGap = int (Math.Max(24.0, 54.0 - speed * 7.0))
                ticksUntilSpawn <- rng.Next(minGap, maxGap)

            for obstacle in obstacles do
                obstacle.X <- obstacle.X - speed

            obstacles.RemoveAll(fun obstacle -> obstacle.X < -8.0) |> ignore

            let dinoHitbox =
                dinoRect dinoX groundY dinoY isDucking

            for obstacle in obstacles do
                let obstacleHitbox = obstacleRect groundY obstacle

                if Collision.intersects dinoHitbox obstacleHitbox then
                    crashed <- true

            scoreRaw <- scoreRaw + speed

            let currentScore = int scoreRaw
            let displayHigh = max initialHighScore currentScore

            render
                width
                renderHeight
                groundY
                dinoX
                dinoY
                isDucking
                obstacles
                currentScore
                displayHigh
                speed

            Thread.Sleep(55)

        let finalScore = int scoreRaw

        if quit then
            Quit, finalScore
        else
            Crash, finalScore

    let gameLoop () =
        ensureScoreFile ()

        let mutable keepPlaying = true

        while keepPlaying do
            let highScore = loadScores () |> highScoreOf
            let reason, score = runOnce highScore
            let saved = score > 0

            if saved then
                saveScore score

            match reason with
            | Quit ->
                showScores "QUIT" score saved
                keepPlaying <- false

            | Crash ->
                showScores "GAME OVER" score saved
                keepPlaying <- waitForRestartOrQuit ()
