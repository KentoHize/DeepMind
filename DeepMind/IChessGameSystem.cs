using System;
using System.Collections.Generic;
using System.Text;

namespace DeepMind
{
    public interface IChessGameSystem
    {
        void Start();
        void Restart();
        void Abort();
        GameResult Move(IChessMove move);
        List<IChessMove> GetNextMoves();
        List<IChessMove> GetMoveRecords();
        IChessBoard GetCurrentBoard();
    }

    public interface IChessMove
    {
        string ToString();
    }

    public interface IChessBoard
    {
        string ToString();
    }

    public enum GameResult
    {
        None = 0,
        Player1Win, 
        Player2Win,
        Draw
    }
}
