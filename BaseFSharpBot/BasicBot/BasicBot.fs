namespace BasicBot

open BoardBots.Shared
open System

type BasicBot() = 

    // a list of all positions on the board
    let allPositions = 
        [ BoardPosition.At(0,2); 
          BoardPosition.At(0,1); 
          BoardPosition.At(0,0); 
          BoardPosition.At(1,2); 
          BoardPosition.At(1,1); 
          BoardPosition.At(1,0); 
          BoardPosition.At(2,2); 
          BoardPosition.At(2,1); 
          BoardPosition.At(2,0); ]

    // A list of all sets of '3-in-a-row' positions
    let winningRows = 
        [
            [BoardPosition.At(0,0); BoardPosition.At(0,1); BoardPosition.At(0,2) ]
            [BoardPosition.At(1,0); BoardPosition.At(1,1); BoardPosition.At(1,2) ]
            [BoardPosition.At(2,0); BoardPosition.At(2,1); BoardPosition.At(2,2) ]
            [BoardPosition.At(0,0); BoardPosition.At(1,0); BoardPosition.At(2,0) ]
            [BoardPosition.At(0,1); BoardPosition.At(1,1); BoardPosition.At(2,1) ]
            [BoardPosition.At(0,2); BoardPosition.At(1,2); BoardPosition.At(2,2) ]
            [BoardPosition.At(2,2); BoardPosition.At(1,1); BoardPosition.At(0,0) ]
            [BoardPosition.At(2,0); BoardPosition.At(1,1); BoardPosition.At(0,2) ]
        ]

    // finds all 'winningRows' which include a given position
    let winningRowsAffectedBy (position:BoardPosition) = 
        winningRows |> List.filter (fun winningRow -> winningRow |> List.exists (fun winningPosition -> winningPosition.Column = position.Column && winningPosition.Row = position.Row))

    // gets the count of a certain token on the board for the specified winningRow
    let getTokenCount (board:IPlayerBoard) token winningRow = 
        winningRow |> List.filter (fun winningPosition -> board.TokenAt(winningPosition) = token) |> List.length

    // example of how to find how many of my tokens are on a given winningRow, for the current board
    let myTokensOnRow (board:IPlayerBoard) = 
        getTokenCount board PlayerToken.Me winningRows.[0]

    // ** the method to implement to make your bot work! **
    let choseWhereToPlay (board:IPlayerBoard) = 
        if board.TokenAt(BoardPosition(1,1)) = PlayerToken.None then
            new BoardPosition(1,1)
        else
            new BoardPosition(0,0)

    // implementing the IPlayer interface
    interface IPlayer with
        member this.TakeTurn(board) = 
            choseWhereToPlay board

    // allows us to call TakeTurn without casting BasicBot to an IPlayer
    member this.TakeTurn(board) = 
            (this :> IPlayer).TakeTurn board
