namespace DinoGame

open System
open System.Globalization
open System.IO
open System.Text

module Scores =
    let scoreFile =
        "dino-scores.csv"

    let ensureScoreFile () =
        if not (File.Exists(scoreFile)) then
            File.WriteAllText(scoreFile, "playedAt,score" + Environment.NewLine, Encoding.UTF8)

    let loadScores () =
        ensureScoreFile ()

        File.ReadAllLines(scoreFile)
        |> Seq.skip 1
        |> Seq.choose (fun line ->
            let parts = line.Split(',')

            if parts.Length >= 2 then
                match Int32.TryParse(parts.[1].Trim()) with
                | true, score -> Some score
                | _ -> None
            else
                None)
        |> Seq.toList

    let saveScore score =
        ensureScoreFile ()

        let stamp =
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)

        let line =
            sprintf "%s,%d%s" stamp score Environment.NewLine

        File.AppendAllText(scoreFile, line, Encoding.UTF8)

    let highScoreOf scores =
        scores |> List.fold max 0
