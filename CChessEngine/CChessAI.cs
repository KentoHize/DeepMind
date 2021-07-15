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

            List<CChessMoveData> result = BestFS(bn, depth);
            if (result.Count == 0)
                return MoveStatus.NoBestMove;

            bestMoveData = result[0];
            estimateMoves = result;            
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

        public List<CChessMoveData> BestFS(CChessBoardNode startNode, long nodeCount = 10000, int width = 3)
        {
            ExpandNode(startNode);

            //startNode.NextMoves[0].BoardNode.CChessScore

            return null;
            
        }

        //兩輪之後更新一次
        //一層是一回合
        public void ExpandNode(CChessBoardNode node, int depth = 2)
        {
            if (depth < 0)
                throw new ArgumentException(nameof(depth));
            depth--;

            //Expand 1
            foreach(CChessMoveData cmd in node.NextMoves)            
            {
                CChessBoard nextBoard = CChessSystem.SimpleMove(node.Board, cmd.Move);
                CChessBoardNode nextBoardNode = LoadOrCreateBoardNode(nextBoard);
                if(nextBoardNode.Status == CChessStatus.None)
                {
                    CChessStatus nextBoardStatus = CChessSystem.CheckStatus(nextBoard, cmd.Move, nextBoardNode.NextMoves.ToMoveList());                    
                    switch (nextBoardStatus)
                    {
                        case CChessStatus.BlackWin:
                            nextBoardNode.Player1WinScore = 
                            nextBoardNode.CChessScore = 0;
                            continue;
                        case CChessStatus.RedWin:
                            nextBoardNode.Player1WinScore =
                            nextBoardNode.CChessScore =
                            CChessBoardNode.MaxScore;
                            continue;
                        case CChessStatus.BlackCheckmate:
                            nextBoardNode.Player1WinScore =
                            nextBoardNode.CChessScore = 0;
                            continue;
                        case CChessStatus.RedCheckmate:
                            nextBoardNode.Player1WinScore =
                            nextBoardNode.CChessScore = 
                            CChessBoardNode.MaxScore;
                            continue;
                        default:
                            nextBoardNode.CChessScore = MeasureScore(nextBoard);
                            break;
                    }
                }
                cmd.BoardNode = nextBoardNode;

                if (BoardRecords.IndexOf(nextBoard) != -1)
                    continue;

                if (depth != 0)
                    ExpandNode(nextBoardNode, depth);
            }

            //Update Score
            node.CChessScore = node.NextMoves.Min.BoardNode.CChessScore;
            if (node.Board.IsBlackTurn)
            {
                foreach(CChessMoveData cmd in node.NextMoves)                
                    if (cmd.BoardNode.CChessScore > node.CChessScore)
                        node.CChessScore = cmd.BoardNode.CChessScore;
            }
            else
            {
                foreach (CChessMoveData cmd in node.NextMoves)
                    if (cmd.BoardNode.CChessScore < node.CChessScore)
                        node.CChessScore = cmd.BoardNode.CChessScore;
            }
            node.Searched = true;
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
            long result = 44;
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
            return result * (CChessBoardNode.MaxScore / 90);
        }
    }
}
