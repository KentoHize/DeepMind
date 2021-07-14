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
        public CChessBoardNode CurrentBoardNode { get; set; }
        public SortedDictionary<CChessBoard, CChessBoardNode> BoardNodes { get; set; }
        public List<CChessMove> MoveRecords { get; set; }
        public List<CChessBoard> BoardRecords { get; set; }
        public bool NoDiskMode { get; set; }
        public CChessAI()
           : this(null)
        { }

        public CChessAI(CChessBoard startBoard = null, bool noDiskMode = false)
        {
            BoardNodeDataPath = @"C:\Programs\WPF\DeepMind\CChessEngine\Data\Board";
            MoveRecordDataPath = @"C:\Programs\WPF\DeepMind\CChessEngine\Data\Move";
            if (startBoard == null)
                startBoard = CChessBoard.StartingBoard;            
            BoardNodes = new SortedDictionary<CChessBoard, CChessBoardNode>();
            MoveRecords = new List<CChessMove>();
            BoardRecords = new List<CChessBoard>();
            NoDiskMode = noDiskMode;
            StartBoardNode = LoadOrCreateBoardNode(startBoard);
            CurrentBoardNode = StartBoardNode;
            
        }

        public MoveStatus Go(CChessBoardNode bn, int depth, out CChessMoveData bestMoveData, out List<CChessMoveData> estimateMoves)
        {
            bestMoveData = null;
            estimateMoves = null;

            if (bn.NextMoves.Count == 0)
                return MoveStatus.Resign;

            ExpandBoardNode(bn, depth);
            if (depth == 0)
                return MoveStatus.NoBestMove;
            
            bestMoveData = bn.NextMoves[0];
            for (int i = 1; i < bn.NextMoves.Count; i++)
                if (bn.NextMoves[i].BoardNode.Score > bestMoveData.BoardNode.Score)
                    bestMoveData = bn.NextMoves[i];
            
            return MoveStatus.BestMove;
        }

        public MoveStatus Go(CChessBoard board, int depth, out CChessMoveData bestMoveData, out List<CChessMoveData> estimateMoves)
        {
            CChessBoardNode bn;
            if (BoardNodes.ContainsKey(board))
                bn = BoardNodes[board];
            else
                bn = LoadOrCreateBoardNode(board);
            return Go(bn, depth, out bestMoveData, out estimateMoves);
        }

        public MoveStatus Move(int depth, out CChessMoveData bestMoveData, out List<CChessMoveData> estimateMoves)
        {
            MoveStatus result = Go(CurrentBoardNode, depth, out bestMoveData, out estimateMoves);
            if(result != MoveStatus.Resign)
            {
                CurrentBoardNode = bestMoveData.BoardNode;
                MoveRecords.Add(bestMoveData.Move);
                BoardRecords.Add(CurrentBoardNode.Board);
            }
            return result;
        }

        //結束時紀錄全局
        public void RecordGame()
        {
            // To Do
            UpdateBoardNode();
        }

        public void ExpandBoardNode(CChessBoardNode node, int depth = 1)
        {
            depth--;
            for(int i = 0; i < node.NextMoves.Count; i++)
            {
                CChessBoard cb;
                cb = CChessSystem.SimpleMove(node.Board, node.NextMoves[i].Move);
                CChessBoardNode cbn = LoadOrCreateBoardNode(cb);
                CChessStatus status = CChessSystem.CheckStatus(cb, node.NextMoves[i].Move, cbn.NextMoves.ToMoveList());
                
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

            //UpdateBoardNode();
        }

        public CChessBoardNode LoadOrCreateBoardNode(CChessBoard board)
        {
            CChessBoardNode cbn;
            if (!NoDiskMode)
            {
                string filePath = Path.Combine(BoardNodeDataPath, $"{board.PrintBoardString(true)}.json");
                if (File.Exists(filePath))
                    cbn = Tina.LoadJsonFile<CChessBoardNode>(filePath);
                else
                {
                    cbn = new CChessBoardNode(board);
                    Tina.SaveJsonFile(filePath, cbn, true);
                }
            }
            else
                cbn = new CChessBoardNode(board);

            if(!BoardNodes.ContainsKey(board))
                BoardNodes.Add(board, cbn);
            return cbn;
        }

        public void UpdateBoardNode()
        {
            if (NoDiskMode)
                return;
            foreach(CChessBoardNode node in BoardNodes.Values)
            {
                string filePath = Path.Combine(BoardNodeDataPath, $"{node.Board.PrintBoardString(true)}.json");
                Tina.SaveJsonFile(filePath, node, true);
            }
        }
    }
}
