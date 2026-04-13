namespace Uwu.Games.Reversi.Student;

using System.Collections.Generic;
using ReversiBehavior = GamePlaying.Behavior<State, Player, Move, Player, int>;
public class Minimax : ReversiBehavior
{
    public Minimax() { }

    // Starts look-ahead process to find best move.
    public Move Run(State board, int lookAheadDepth) => GetBestMove(board, lookAheadDepth);

    // Uses look ahead to evaluate all valid moves for given player color & returns best move
    // found. This method will only be called if there's at least one valid move for player.
    private Move GetBestMove(State board, int depth)
    {
        var maxPlayer = board.Current;
        var moves = GetValidMoves(board, maxPlayer);

        int bestScore = int.MinValue;
        Move bestMove = moves[0];

        foreach (var move in moves)
        {
            var next = new State(board);
            next.MakeMove(move);
            next.EndMove();

            int score = MinimaxScore(next, depth - 1, maxPlayer);
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    #region Recommended Helper Functions
    // Evaluate state from the perspective of maxPlayer (the player who started the search).
    private int Evaluate(State board, Player maxPlayer)
    {
        int total = 0;

        for (int row = 0; row < State.GRID_SIZE; row++)
        {
            for (int col = 0; col < State.GRID_SIZE; col++)
            {
                int cellValue = board[row, col] == Player.White ? 1 :
                                board[row, col] == Player.Black ? -1 : 0;

                bool isCorner = (row == 0 || row == State.GRID_SIZE - 1) &&
                                (col == 0 || col == State.GRID_SIZE - 1);
                bool isEdge = row == 0 || row == State.GRID_SIZE - 1 ||
                              col == 0 || col == State.GRID_SIZE - 1;

                if (isCorner)
                    cellValue *= 100;
                else if (isEdge)
                    cellValue *= 10;

                total += cellValue;
            }
        }

        if (board.IsTerminalState())
        {
            if (board.Score > 0)
                total += 10000;
            else if (board.Score < 0)
                total -= 10000;
        }

        if (maxPlayer == Player.Black)
            total = -total;

        return total;
    }

    private int MinimaxScore(State board, int depth, Player maxPlayer)
    {
        if (depth <= 0 || board.IsTerminalState())
            return Evaluate(board, maxPlayer);

        var moves = GetValidMoves(board, board.Current);

        if (moves.Count == 0)
        {
            if (board.IsTerminalState())
                return Evaluate(board, maxPlayer);

            var passed = new State(board);
            passed.EndMove();
            return MinimaxScore(passed, depth - 1, maxPlayer);
        }

        if (board.Current == maxPlayer)
        {
            int best = int.MinValue;
            foreach (var move in moves)
            {
                var next = new State(board);
                next.MakeMove(move);
                next.EndMove();
                int score = MinimaxScore(next, depth - 1, maxPlayer);
                if (score > best)
                    best = score;
            }
            return best;
        }
        else
        {
            int best = int.MaxValue;
            foreach (var move in moves)
            {
                var next = new State(board);
                next.MakeMove(move);
                next.EndMove();
                int score = MinimaxScore(next, depth - 1, maxPlayer);
                if (score < best)
                    best = score;
            }
            return best;
        }
    }

    private static List<Move> GetValidMoves(State board, Player player)
    {
        var moves = new List<Move>();
        for (int r = 0; r < State.GRID_SIZE; r++)
        {
            for (int c = 0; c < State.GRID_SIZE; c++)
            {
                var m = State.NewMove(r, c);
                if (board.IsValidMove(player, m))
                    moves.Add(m);
            }
        }
        return moves;
    }

    #endregion
}
