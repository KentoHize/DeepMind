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
        public bool NoBoardRecordMode { get; set; }
        public CChessAI()
           : this(null)
        { }

        public CChessAI(CChessBoard startBoard = null, bool noDiskMode = false, bool noBoardRecordMode = false)
        {
            BoardNodeDataPath = @"C:\Programs\WPF\DeepMind\CChessEngine\Data\Board";
            MoveRecordDataPath = @"C:\Programs\WPF\DeepMind\CChessEngine\Data\Move";
            if (startBoard == null)
                startBoard = CChessBoard.StartingBoard;
            BoardNodes = new SortedDictionary<CChessBoard, CChessBoardNode>();
            MoveRecords = new List<CChessMove>();
            BoardRecords = new List<CChessBoard>();
            NoDiskMode = noDiskMode;
            NoBoardRecordMode = noBoardRecordMode;
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
            if(bn.Board.IsBlackTurn)
            {
                for (int i = 1; i < bn.NextMoves.Count; i++)
                    if (bn.NextMoves[i].BoardNode.Player1Score < bestMoveData.BoardNode.Player1Score)
                        bestMoveData = bn.NextMoves[i];
            }
            else
            {
                for (int i = 1; i < bn.NextMoves.Count; i++)
                    if (bn.NextMoves[i].BoardNode.Player1Score > bestMoveData.BoardNode.Player1Score)
                        bestMoveData = bn.NextMoves[i];
            }
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
                CurrentBoardNode = (CChessBoardNode)bestMoveData.BoardNode.Clone();
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

        public void ExpandBoardNode(CChessBoardNode node, int depth)
        {
            depth--;
            for(int i = 0; i < node.NextMoves.Count; i++)
            {   
                CChessBoard cb = CChessSystem.SimpleMove(node.Board, node.NextMoves[i].Move);
                CChessBoardNode nextBoardNode = LoadOrCreateBoardNode(cb);
                CChessStatus status = CChessSystem.CheckStatus(cb, node.NextMoves[i].Move, nextBoardNode.NextMoves.ToMoveList());
                node.NextMoves[i].BoardNode = nextBoardNode;
                switch (status)
                {
                    case CChessStatus.BlackWin:                    
                        nextBoardNode.Player1Score = 0;
                        continue;
                    case CChessStatus.RedWin:
                        nextBoardNode.Player1Score = CChessBoardNode.MaxScore;
                        continue;
                    case CChessStatus.BlackCheckmate:
                        nextBoardNode.Player1Score = 0;
                        continue;
                    case CChessStatus.RedCheckmate:
                        nextBoardNode.Player1Score = CChessBoardNode.MaxScore;
                        continue;
                    default:
                        if (NoDiskMode && depth == 0)
                            nextBoardNode.Player1Score = MeasureScore(cb);
                        break;
                }
                

                if (BoardRecords.IndexOf(cb) != -1)
                    continue;

                if (depth != 0)
                    ExpandBoardNode(nextBoardNode, depth);
            }

            //Update Score
            decimal totalScore = 0;
            for(int i = 0; i < node.NextMoves.Count; i++)
                totalScore += node.NextMoves[i].BoardNode.Player1Score;
            node.Player1Score = (long)(totalScore / node.NextMoves.Count);

            //if(!NoDiskMode)
            //    UpdateBoardNode();
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

            if(!NoBoardRecordMode && !BoardNodes.ContainsKey(board))
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

        //NoDisk專門使用
        //一般運算
        public static long MeasureScore(CChessBoard board)
        {   
            //車6x分 馬3x分 炮3x分 過河兵2x分 兵x分 象x分 士x分
            //最少32 + 5 + 2 + 2 = 39分
            //最多32 + 10 + 2 + 2 = 44分
            //最小-44分
            long result = 0;
            for (int i = 0; i < 9; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    switch(board[i, j])
                    {
                        case 'p':
                            result -= 1;
                            break;
                        case 'P':
                            result += 1;
                            break;
                        case 'c':
                            result -= 3;
                            break;
                        case 'C':
                            result += 3;
                            break;
                        case 'n':
                            result -= 3;
                            break;
                        case 'N':
                            result += 3;
                            break;
                        case 'r':
                            result -= 6;
                            break;
                        case 'R':
                            result += 6;
                            break;
                        case 'b':
                            result -= 1;
                            break;
                        case 'B':
                            result += 1;
                            break;
                        case 'a':
                            result -= 1;
                            break;
                        case 'A':
                            result += 1;
                            break;
                        default:
                            break;
                    }
                }
            }           
            return (result + 44) * (CChessBoardNode.MaxScore / 90);
        }
    }
}
