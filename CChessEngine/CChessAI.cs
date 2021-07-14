using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Aritiafel.Characters.Heroes;


namespace CChessEngine
{
    public enum MoveStatus
    {
        BestMove,
        NoBestMove,
        RequestADraw,
        Resign
    }

    //AI思考步驟
    //1.取得此盤面的Data(沒有設置新盤面)    
    //2.參考盤面的走法得分及勝率決定下法
    //3.有勝負之後更新盤面分(僅限開局)
    //內崁Deep Learning

    public class CChessAI
    {
        public string BoardNodeDataPath { get; set; }
        public string MoveRecordDataPath { get; set; }
        public CChessBoardNode StartBoardNode { get; set; }
        public SortedDictionary<CChessBoard, CChessBoardNode> BoardNodes { get; set; }
        public List<CChessMove> MoveRecords { get; set; }
        public List<CChessBoard> BoardRecords { get; set; }
        public MoveStatus Go(CChessBoard board, int depth = 0, out CChessMoveData bestMoveData, out List<CChessMoveData> estimateMoves)
        {
            bestMoveData = null;
            estimateMoves = null;

            //Load Update BoardNode
            CChessBoardNode bn;
            if (BoardNodes.ContainsKey(board))
                bn = BoardNodes[board];
            else
                bn = LoadOrCreateBoardNode(board);

            if (bn.NextMoves.Count == 0)
                return MoveStatus.Resign;

            ExpandBoardNode(bn, depth);
            if (depth == 0)
                return MoveStatus.NoBestMove;
            
            bestMoveData = bn.NextMoves[0];
            for (int i = 1; i < bn.NextMoves.Count; i++)
            {
                if (bn.NextMoves[i].BoardNode.Score > bestMoveData.BoardNode.Score)
                    bestMoveData = bn.NextMoves[i];
            }
            
            return MoveStatus.BestMove;
        }

        public void ExpandBoardNode(CChessBoardNode node, int depth = 1)
        {
            depth--;
            for(int i = 0; i < node.NextMoves.Count; i++)
            {
                CChessBoard cb;
                cb = CChessSystem.SimpleMove(node.Board, node.NextMoves[i].Move);
                List<CChessMove> nextLegalMoves = CChessSystem.GetLegalMoves(cb);
                CChessStatus status = CChessSystem.CheckStatus(cb, node.NextMoves[i].Move, nextLegalMoves);
                CChessBoardNode cbn = LoadOrCreateBoardNode(cb);
                switch (status)
                {
                    case CChessStatus.BlackWin:                    
                        node.Score = CChessBoardNode.MaxScore;
                        break;
                    case CChessStatus.RedWin:                    
                        node.Score = CChessBoardNode.MaxScore;
                        break;
                    case CChessStatus.BlackCheckmate:
                        node.Score = 0;
                        break;
                    case CChessStatus.RedCheckmate:
                        node.Score = 0;
                        break;
                    default:                        
                        break;
                }
                node.NextMoves[i].BoardNode = cbn;

                if (BoardRecords.IndexOf(cb) != -1)
                    continue;

                if (depth != 0)
                    ExpandBoardNode(cbn, depth - 1);
            }

            //Update Score
            decimal totalScore = 0;
            for(int i = 0; i < node.NextMoves.Count; i++)
                totalScore += node.NextMoves[i].BoardNode.Score;
            node.Score = CChessBoardNode.MaxScore - (long)(totalScore / node.NextMoves.Count);
        }

        public CChessAI()
            : this(null)
        { }

        public CChessAI(CChessBoard startBoard = null)
        {
            BoardNodeDataPath = @"C:\Programs\WPF\DeepMind\CChessEngine\Data\Board";
            MoveRecordDataPath = @"C:\Programs\WPF\DeepMind\CChessEngine\Data\Move";
            if (startBoard == null)
                startBoard = CChessBoard.StartingBoard;
            BoardNodes = new SortedDictionary<CChessBoard, CChessBoardNode>();
            MoveRecords = new List<CChessMove>();
            BoardRecords = new List<CChessBoard>();
            StartBoardNode = LoadOrCreateBoardNode(startBoard);
        
        }

        public CChessBoardNode LoadOrCreateBoardNode(CChessBoard board)
        {
            CChessBoardNode cbn;
            string filePath = Path.Combine(BoardNodeDataPath, $"{board.PrintBoardString(true)}.json");
            if (File.Exists(filePath))
                cbn = Tina.LoadJsonFile<CChessBoardNode>(filePath);
            else
            {
                cbn = new CChessBoardNode(board);
                Tina.SaveJsonFile(filePath, cbn, true);
            }
            BoardNodes.Add(board, cbn);
            return cbn;
        }
    }
}
