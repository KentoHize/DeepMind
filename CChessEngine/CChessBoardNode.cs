using System;
using System.Collections.Generic;
using System.Text;

namespace CChessEngine
{
    public class CChessBoardNode
    {
        public const long DefaultScore = 5000000;
        public const long MaxScore = 10000000;
        public CChessBoard Board { get; set; }
        public List<CChessMoveData> LegalMoves { get; set; }
        public long Player1WinCount { get; set; }
        public long Player2WinCount { get; set; }
        public long DrawCount { get; set; }
        public long Score { get; set; }

        public CChessBoardNode()
            : this(null)
        { }

        public CChessBoardNode(CChessBoard board)
        {
            Board = board;
            Score = DefaultScore;
        }

        //Expand
        //Save
        //Load
    }
}
