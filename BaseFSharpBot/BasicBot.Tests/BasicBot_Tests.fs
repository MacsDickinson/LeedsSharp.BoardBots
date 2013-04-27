module BasicBot_Tests

open NUnit.Framework
open FsUnit
open BoardBots.Shared
open BasicBot

type FakeBoard()=
    let mutable internalTokens = 
        [|
            [| PlayerToken.None; PlayerToken.None; PlayerToken.None  |]
            [| PlayerToken.None; PlayerToken.None; PlayerToken.None  |]
            [| PlayerToken.None; PlayerToken.None; PlayerToken.None  |]
        |]

    member this.Tokens
        with get() = internalTokens
        and set(value) = internalTokens <- value

    interface IPlayerBoard with
        member this.TokenAt(position) = 
            internalTokens.[position.Column].[position.Row]

let shouldEqual expected (actual:BoardPosition) =
    (actual.Column, actual.Row) |> should equal expected

let shouldNotEqual unexpected (actual:BoardPosition) =
    (actual.Column, actual.Row) |> should not' (equal unexpected)

let shouldBeIn expected (actual:BoardPosition) = 
    expected |> should contain (actual.Column, actual.Row)

let testBot = new BasicBot()

[<Test>]
let ``When board is empty should play in centre`` () =
    let board = new FakeBoard()
    board |> testBot.TakeTurn |> shouldEqual (1,1)

[<Test>]
let ``When centre is taken should not play in centre`` () =
    let board = new FakeBoard()
    board.Tokens.[1].[1] <- PlayerToken.Opponent
    board |> testBot.TakeTurn |> shouldNotEqual (1,1)

[<Test>]
let ``When centre is taken should play in a corner`` () =
    let board = new FakeBoard()
    board.Tokens.[1].[1] <- PlayerToken.Opponent
    board |> testBot.TakeTurn |> shouldBeIn [(0,0);(0,2);(2,0);(2,2)]