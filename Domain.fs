namespace DinoGame

[<Struct>]
type Rect =
    { X: int
      Y: int
      W: int
      H: int }

type ObstacleKind =
    | Cactus
    | Bird

type Obstacle =
    { Kind: ObstacleKind
      Shape: int
      Lane: int
      mutable X: float }

type EndReason =
    | Crash
    | Quit

module Collision =
    let intersects (a: Rect) (b: Rect) =
        a.X < b.X + b.W
        && a.X + a.W > b.X
        && a.Y < b.Y + b.H
        && a.Y + a.H > b.Y
