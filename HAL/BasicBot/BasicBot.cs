using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoardBots.Shared;

// Please rename your assembly!  Otherwise we'll all be creating the same one.  (Doesn't matter about project name or namespace)
// Right click project -> properties -> change 'assembly name'
namespace BasicBot
{
    // Please rename your bot! (right-click -> refactor -> rename)
    public class BasicBot : IPlayer
    {
        public BoardPosition TakeTurn(IPlayerBoard board)
        {
            // let's see if anyone's played at 0,0...
            if (board.TokenAt(BoardPosition.At(0, 0)) == PlayerToken.None)
            {
                // No? Sweet! Let's play there.
                return BoardPosition.At(0, 0);
            }

            // Sod it, I'm going to play there anyway..
            return BoardPosition.At(0, 0);
        }
    }


    public class SimpleBot : IPlayer
    {
        public BoardPosition TakeTurn(IPlayerBoard board)
        {
            if (board.TokenAt(BoardPosition.At(0, 0)) == PlayerToken.None)
            {
                return BoardPosition.At(0, 0);
            }

            return BoardPosition.At(2, 2);
        }
    }
}