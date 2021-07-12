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

    }

    public interface IChessMove
    {
        
    }

    public enum GameResult
    {
        None = 0,
        Player1Win, 
        Player2Win,
        Draw
    }
}
