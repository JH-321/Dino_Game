namespace DinoGame

open System
open System.Text

module Rendering =
    let private dinoRunA =
        [| "  __ "
           " /o_)"
           "/|_/ "
           " / \\" |]

    let private dinoRunB =
        [| "  __ "
           " /o_)"
           "/|_/ "
           " \\ \\" |]

    let private dinoDucking =
        [| " __  "
           "/o_)=" |]

    let private cactusShapes =
        [| [| " | "
              "-|-"
              " | "
              " | " |]
           [| " | "
              " | "
              "-|-"
              " | " |]
           [| " | "
              "-| "
              " | "
              "-| " |]
           [| " | "
              " |-"
              " | "
              " |-" |] |]

    let private birdUp =
        [| "\\o/"
           " v " |]

    let private birdDown =
        [| " o "
           "/^\\" |]

    let terminalWidth () =
        try
            Console.WindowWidth
        with
        | _ -> 80

    let clampWidth () =
        let w = terminalWidth () - 1
        Math.Max(50, Math.Min(110, w))

    let private putChar (grid: char[,]) x y ch =
        let height = grid.GetLength(0)
        let width = grid.GetLength(1)

        if x >= 0 && x < width && y >= 0 && y < height && ch <> ' ' then
            grid.[y, x] <- ch

    let private drawText (grid: char[,]) x y (text: string) =
        for i = 0 to text.Length - 1 do
            putChar grid (x + i) y text.[i]

    let private drawSprite (grid: char[,]) x y (sprite: string array) =
        sprite
        |> Array.iteri (fun row text -> drawText grid x (y + row) text)

    let private cactusFor (shape: int) =
        cactusShapes.[Math.Abs(shape) % cactusShapes.Length]

    let private birdY groundY (lane: int) =
        match Math.Abs(lane) % 3 with
        | 0 -> groundY - 8
        | 1 -> groundY - 6
        | _ -> groundY - 4

    let dinoRect dinoX groundY (dinoY: float) isDucking =
        let lift = int (Math.Round(dinoY))

        if isDucking then
            { X = dinoX
              Y = groundY - dinoDucking.Length
              W = 5
              H = dinoDucking.Length }
        else
            { X = dinoX + 1
              Y = groundY - dinoRunA.Length - lift
              W = 4
              H = dinoRunA.Length }

    let obstacleRect groundY (obstacle: Obstacle) : Rect =
        let x = int (Math.Round(obstacle.X))

        match obstacle.Kind with
        | Cactus ->
            let cactus = cactusFor obstacle.Shape

            { X = x
              Y = groundY - cactus.Length
              W = 3
              H = cactus.Length }
        | Bird ->
            { X = x
              Y = birdY groundY obstacle.Lane
              W = 3
              H = 2 }

    let render width renderHeight groundY dinoX (dinoY: float) isDucking obstacles score highScore speed =
        let grid = Array2D.create renderHeight width ' '

        for x = 0 to width - 1 do
            let groundChar =
                if (x + score / 3) % 17 = 0 then '.'
                elif (x + score / 5) % 29 = 0 then '\''
                else '_'

            grid.[groundY, x] <- groundChar

        let lift = int (Math.Round(dinoY))

        if isDucking then
            drawSprite grid dinoX (groundY - dinoDucking.Length) dinoDucking
        else
            let dino = if (score / 6) % 2 = 0 then dinoRunA else dinoRunB
            drawSprite grid dinoX (groundY - dinoRunA.Length - lift) dino

        for obstacle in obstacles do
            let x = int (Math.Round(obstacle.X))

            match obstacle.Kind with
            | Cactus ->
                let cactus = cactusFor obstacle.Shape
                drawSprite grid x (groundY - cactus.Length) cactus
            | Bird ->
                let bird = if (score / 5) % 2 = 0 then birdUp else birdDown
                drawSprite grid x (birdY groundY obstacle.Lane) bird

        let sb = StringBuilder()

        let title =
            "Chrome Dino CLI - Space/Up: Jump | Down: Duck/Fast fall | Q: Quit"

        sb.AppendLine(title.PadRight(width)) |> ignore

        let status =
            sprintf "Score: %05d   High: %05d   Speed: %.2f" score highScore speed

        sb.AppendLine(status.PadRight(width)) |> ignore
        sb.AppendLine(String('-', width)) |> ignore

        for y = 0 to renderHeight - 1 do
            for x = 0 to width - 1 do
                sb.Append(grid.[y, x]) |> ignore

            sb.AppendLine() |> ignore

        Console.SetCursorPosition(0, 0)
        Console.Write(sb.ToString())
