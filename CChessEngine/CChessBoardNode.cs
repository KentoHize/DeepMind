using System;
using System.Collections.Generic;
using System.Text;

namespace CChessEngine
{
    public class CChessBoardNode : ICloneable
    {
        public const long DefaultScore = 50000000000000000;
        public const long MaxScore = 100000000000000000;
        public CChessBoard Board { get; set; }
        public List<CChessMoveData> NextMoves { get; set; }
        public long Player1WinCount { get; set; }
        public long Player2WinCount { get; set; }
        public long DrawCount { get; set; }
        public long Score { get; set; }
        public bool DrawNode { get; set; }
        public bool CompleteNode { get; set; }

        public CChessBoardNode()
            : this(null)
        { }

        public CChessBoardNode(CChessBoard board, List<CChessMoveData> nextMoves = null)
        {
            Board = new CChessBoard(board);
            Score = DefaultScore;
            if (board == null)
                return;
            if (nextMoves == null)
                nextMoves = CChessSystem.GetLegalMoves(board).ToMoveDataList();
            NextMoves = new List<CChessMoveData>(nextMoves);
        }

        public CChessBoardNode(CChessBoardNode node)
        {
            Board = new CChessBoard(node.Board);
            NextMoves = new List<CChessMoveData>(node.NextMoves);
            Player1WinCount = node.Player1WinCount;
            Player2WinCount = node.Player2WinCount;
            DrawCount = node.DrawCount;
            DrawNode = node.DrawNode;
            Score = node.Score;
            CompleteNode = node.CompleteNode;            
        }

        public object Clone()
            => new CChessBoardNode(this);

        //public CChessBoardNode(CChessBoard board, List<C>)

        //Expand
        //Save
        //Load
    }
}
