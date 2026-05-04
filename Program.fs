open System
open System.Text
open DinoGame.Game

[<EntryPoint>]
let main _ =
    Console.OutputEncoding <- Encoding.UTF8

    let originalCursorVisible =
        try
            Some Console.CursorVisible
        with
        | _ -> None

    try
        Console.CursorVisible <- false
        gameLoop ()
        0
    finally
        match originalCursorVisible with
        | Some visible -> Console.CursorVisible <- visible
        | None -> ()
