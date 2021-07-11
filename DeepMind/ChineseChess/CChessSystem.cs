using System;
using System.Collections.Generic;
using System.Text;

namespace DeepMind.ChineseChess
{
    public enum CChessStatus
    {
        None = 0,
        Start = 1,
        Proceeding = 2,
        RedWin = 2,
        BlackWin = 3,
        Draw = 4
    }

    public class CChessSystem
    {
        public CChessBoard CurrentBoard { get; set; }
        public List<CChessMove> MoveRecord { get; set; }        
        public CChessStatus Status { get; set; }
        public CChessSystem()
        {
            CurrentBoard = new CChessBoard();            
        }

        public List<CChessMove> LegalMoveList()
        {
            return new List<CChessMove>();
        }

        public void Move(CChessMove move)
        {

        }

        public void Restart(CChessBoard board = null)
        {
            if (board == null)
                board = new CChessBoard();
            CurrentBoard = board;
            Status = CChessStatus.Start;
        }

        public CChessStatus CheckResult()
        {
            return CChessStatus.Proceeding;
        }


    }
}
