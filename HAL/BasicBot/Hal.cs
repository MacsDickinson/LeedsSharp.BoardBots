using System.Collections.Generic;
using System.Linq;
using BoardBots.Shared;

// Please rename your assembly!  Otherwise we'll all be creating the same one.  (Doesn't matter about project name or namespace)
// Right click project -> properties -> change 'assembly name'
namespace BasicBot
{
    // Please rename your bot! (right-click -> refactor -> rename)
    public class Hal : IPlayer
    {
        public List<List<int[]>> WinningCombinations
        {
            get
            {
                return new List<List<int[]>>
                {
                    new List<int[]> {new[]{0,0}, new[]{0,1}, new[]{0,2}},
                    new List<int[]> {new[]{1,0}, new[]{1,1}, new[]{1,2}},
                    new List<int[]> {new[]{2,0}, new[]{2,1}, new[]{2,2}},
                    new List<int[]> {new[]{0,0}, new[]{1,0}, new[]{2,0}},
                    new List<int[]> {new[]{0,1}, new[]{1,1}, new[]{2,1}},
                    new List<int[]> {new[]{0,2}, new[]{1,2}, new[]{2,2}},
                    new List<int[]> {new[]{0,0}, new[]{1,1}, new[]{2,2}},
                    new List<int[]> {new[]{0,2}, new[]{1,1}, new[]{2,0}}
                };
            }
        }
        public List<int[]> Corners { 
            get
            {
                return new List<int[]>
                {
                    new[]{0,0},
                    new[]{2,2},
                    new[]{0,2},
                    new[]{2,0}
                };
            }
        }

        public BoardPosition TakeTurn(IPlayerBoard board)
        {
            // Any winning moves?
            BoardPosition winningMove = CompleteThree(board, PlayerToken.Me);
            if (winningMove != null)
            {
                return winningMove;
            }
            // Does oppenent have any winning moves that need blocking
            BoardPosition blockingMove = CompleteThree(board, PlayerToken.Opponent);
            if (blockingMove != null)
            {
                return blockingMove;
            }
            // Any corners free
            foreach (int[] corner in Corners.Where(corner => board.TokenAt(BoardPosition.At(corner[0], corner[1])) == PlayerToken.None))
            {
                return BoardPosition.At(corner[0], corner[1]);
            }
            // Just wap it in the next free spot
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board.TokenAt(BoardPosition.At(i, j)) == PlayerToken.None)
                    {
                        return BoardPosition.At(i, j);
                    }
                }
            }
            // Something has gone horribly wrong
            return BoardPosition.At(0, 0);
        }

        public bool ShouldDefent(IPlayerBoard board)
        {
            return false;
        }

        public BoardPosition CompleteThree(IPlayerBoard board, PlayerToken token)
        {
            return (from winningCombination in WinningCombinations 
                    let matchingSquares = winningCombination.Where(position => board.TokenAt(BoardPosition.At(position[0], position[1])) == token).ToList() 
                    where matchingSquares.Count == 2 
                    select winningCombination.First(x => !matchingSquares.Contains(x)) 
                    into remainingSquare 
                    where board.TokenAt(BoardPosition.At(remainingSquare[0], remainingSquare[1])) == PlayerToken.None 
                    select BoardPosition.At(remainingSquare[0], remainingSquare[1])).FirstOrDefault();
        }
    }

    public class SimpleBot : IPlayer
    {
        public BoardPosition TakeTurn(IPlayerBoard board)
        {
            // Just wap it in the next free spot
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board.TokenAt(BoardPosition.At(i, j)) == PlayerToken.None)
                    {
                        // No? Sweet! Let's play there.
                        return BoardPosition.At(i, j);
                    }
                }
            }
            // Sod it, I'm going to play there anyway..
            return BoardPosition.At(0, 0);
        }
    }
}