﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CChessEngine
{
    public class CChessBoardNode : ICloneable
    {
        public const long DefaultScore = 50000000000000000;
        public const long MaxScore = 100000000000000000;
        public CChessBoard Board { get; set; }
        public CChessStatus Status { get; set; }
        public SortedSet<CChessMoveData> NextMoves { get; set; }
        public CChessBoardNode Parent { get; set; }
        public long Player1WinCount { get; set; }
        public long Player2WinCount { get; set; }
        public long DrawCount { get; set; }
        public long CChessScore { get; set; }
        public long Player1WinScore { get; set; }        
        public bool DrawNode { get; set; }
        public bool CompleteNode { get; set; }
        public bool Searched { get; set; }

        public CChessBoardNode()
            : this(null)
        { }

        public CChessBoardNode(CChessBoard board, CChessBoardNode parent = null, SortedSet<CChessMoveData> nextMoves = null)
        {
            Board = new CChessBoard(board);
            Player1WinScore = DefaultScore;
            CChessScore = DefaultScore;
            if (board == null)
                return;
            Parent = parent;            
            if (nextMoves == null)
                nextMoves = CChessSystem.GetLegalMoves(board).ToMoveDataList();
            NextMoves = new SortedSet<CChessMoveData>(nextMoves);
        }

        public CChessBoardNode(CChessBoardNode node)
        {
            Board = new CChessBoard(node.Board);
            NextMoves = new SortedSet<CChessMoveData>(node.NextMoves);
            Player1WinCount = node.Player1WinCount;
            Player2WinCount = node.Player2WinCount;
            DrawCount = node.DrawCount;
            DrawNode = node.DrawNode;
            Player1WinScore = node.Player1WinScore;
            CChessScore = node.CChessScore;
            CompleteNode = node.CompleteNode;
            Searched = node.Searched;
            Status = node.Status;
            Parent = node.Parent;
        }

        public object Clone()
            => new CChessBoardNode(this);

        //public CChessBoardNode(CChessBoard board, List<C>)

        //Expand
        //Save
        //Load
    }
}
