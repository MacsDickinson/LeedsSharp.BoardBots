using NUnit.Framework;
using BoardBots.Shared;

namespace BasicBot.Tests
{
    // you may want to rename this to reflect what you called your bot
    [TestFixture]
    public class BasicBot_Tests
    {
        [Test]
        public void TakeTurn_EmptyBoard_PlaysInColumn0Row0()
        {
            // arrange
            Hal player = new Hal();

            // act
            var result = player.TakeTurn(new FakeBoard());

            // assert
            Assert.That(result.Column, Is.EqualTo(0));
            Assert.That(result.Row, Is.EqualTo(0));
        }

        [Test]
        public void TakeTurn_Column0Row0AlreadyHasPiece_PlaysInColumn2Row2()
        {
            // arrange
            Hal player = new Hal();
            FakeBoard partiallyFullBoard = new FakeBoard();
            partiallyFullBoard.Tokens[0, 0] = PlayerToken.Opponent;

            // act
            var result = player.TakeTurn(partiallyFullBoard);

            // assert
            Assert.That(result.Column, Is.EqualTo(2));
            Assert.That(result.Row, Is.EqualTo(2));
        }

        [TestCase(0, 1, 2)]
        [TestCase(1, 2, 0)]
        [TestCase(0, 2, 1)]
        public void TakeTurn_TwoOfOwnInAColumn_CompletesTheThree(int y1, int y2, int y3)
        {
            // arrange
            Hal player = new Hal();
            FakeBoard partiallyFullBoard = new FakeBoard();
            partiallyFullBoard.Tokens[0, y1] = PlayerToken.Me;
            partiallyFullBoard.Tokens[0, y2] = PlayerToken.Me;

            // act
            var result = player.TakeTurn(partiallyFullBoard);

            // assert
            Assert.That(result.Column, Is.EqualTo(0));
            Assert.That(result.Row, Is.EqualTo(y3));
        }

        [TestCase(0, 1, 2)]
        [TestCase(1, 2, 0)]
        [TestCase(0, 2, 1)]
        public void TakeTurn_TwoForOpponentInAColumn_BlocksTheThree(int y1, int y2, int y3)
        {
            // arrange
            Hal player = new Hal();
            FakeBoard partiallyFullBoard = new FakeBoard();
            partiallyFullBoard.Tokens[0, y1] = PlayerToken.Opponent;
            partiallyFullBoard.Tokens[0, y2] = PlayerToken.Opponent;

            // act
            var result = player.TakeTurn(partiallyFullBoard);

            // assert
            Assert.That(result.Column, Is.EqualTo(0));
            Assert.That(result.Row, Is.EqualTo(y3));
        }

        [TestCase(0, 1, 2)]
        [TestCase(1, 2, 0)]
        [TestCase(0, 2, 1)]
        public void TakeTurn_TwoOfOwnInARow_CompletesTheThree(int x1, int x2, int x3)
        {
            // arrange
            Hal player = new Hal();
            FakeBoard partiallyFullBoard = new FakeBoard();
            partiallyFullBoard.Tokens[x1, 0] = PlayerToken.Me;
            partiallyFullBoard.Tokens[x2, 0] = PlayerToken.Me;

            // act
            var result = player.TakeTurn(partiallyFullBoard);

            // assert
            Assert.That(result.Column, Is.EqualTo(x3));
            Assert.That(result.Row, Is.EqualTo(0));
        }

        [TestCase(0, 1, 2)]
        [TestCase(1, 2, 0)]
        [TestCase(0, 2, 1)]
        public void TakeTurn_TwoForOpponentInARow_BlocksTheThree(int x1, int x2, int x3)
        {
            // arrange
            Hal player = new Hal();
            FakeBoard partiallyFullBoard = new FakeBoard();
            partiallyFullBoard.Tokens[x1, 0] = PlayerToken.Opponent;
            partiallyFullBoard.Tokens[x2, 0] = PlayerToken.Opponent;

            // act
            var result = player.TakeTurn(partiallyFullBoard);

            // assert
            Assert.That(result.Column, Is.EqualTo(x3));
            Assert.That(result.Row, Is.EqualTo(0));
        }

        // PlayerBoard we can use for testing
        internal class FakeBoard : IPlayerBoard
        {
            internal PlayerToken[,] Tokens = new PlayerToken[3, 3];

            internal FakeBoard()
            {
                InitialiseBoard();
            }

            public PlayerToken TokenAt(BoardPosition position)
            {
                return Tokens[position.Column, position.Row];
            }

            // Set empty fields to PlayerToken.None, which is what the real board will have (they will not be NULL).
            private void InitialiseBoard()
            {
                for (int columns = 0; columns < 3; columns++)
                {
                    for (int rows = 0; rows < 3; rows++)
                    {
                        Tokens[columns, rows] = PlayerToken.None;
                    }
                }
            }
        }
    }
}
