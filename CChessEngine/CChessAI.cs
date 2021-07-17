using Aritiafel.Characters.Heroes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


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
        public string LogDataPath { get; set; }
        public CChessBoardNode StartBoardNode { get; set; }
        public CChessBoardNode CurrentBoardNode { get; set; }
        public SortedDictionary<CChessBoard, CChessBoardNode> BoardNodes { get; set; }
        public SortedList<long, CChessBoardNode> BestBoardNodes { get; set; }
        public List<CChessMove> MoveRecords { get; set; }
        public List<CChessBoard> BoardRecords { get; set; }
        public bool NoDiskMode { get; set; }
        public bool NoBoardRecordMode { get; set; }
        public int MeasureScoreLevel { get; set; }
        public CChessAI()
           : this(null)
        { }

        public CChessAI(CChessBoard startBoard = null, bool noDiskMode = false, bool noBoardRecordMode = false, int measureLevel = 2)
        {
            BoardNodeDataPath = @"C:\Programs\WPF\DeepMind\CChessEngine\Data\Board";
            MoveRecordDataPath = @"C:\Programs\WPF\DeepMind\CChessEngine\Data\Move";
            LogDataPath = @"C:\Programs\WPF\DeepMind\CChessEngine\Data\Log";
            if (startBoard == null)
                startBoard = CChessBoard.StartingBoard;
            BoardNodes = new SortedDictionary<CChessBoard, CChessBoardNode>();
            BestBoardNodes = new SortedList<long, CChessBoardNode>();
            MoveRecords = new List<CChessMove>();
            BoardRecords = new List<CChessBoard>();
            NoDiskMode = noDiskMode;
            NoBoardRecordMode = noBoardRecordMode;
            MeasureScoreLevel = measureLevel;
            StartBoardNode = LoadOrCreateBoardNode(startBoard, null);
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
                bn = LoadOrCreateBoardNode(board, null);
            return Go(bn, depth, out bestMoveData, out estimateMoves);
        }

        public MoveStatus Move(int depth, out CChessMoveData bestMoveData, out List<CChessMoveData> estimateMoves)
        {
            MoveStatus result = Go(CurrentBoardNode, depth, out bestMoveData, out estimateMoves);
            if (result != MoveStatus.Resign)
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

        public string PrintChilds(CChessBoardNode node, StringBuilder result = null, bool withScore = false, int depth = 0)
        {
            if (result == null)
                result = new StringBuilder();
            if (node.Searched)
            {
                foreach (CChessMoveData cmd in node.NextMoves)
                {
                    for (int i = 0; i < depth; i++)
                        result.Append(" -> ");
                    if (depth == 0)
                        result.Append("◎");
                    result.Append(CChessSystem.PrintChineseMoveString(node.Board, cmd.Move));
                    result.Append(" ");
                    if (withScore)
                    {
                        result.Append(cmd.BoardNode.CChessScore);
                        result.Append(" ");
                    }
                    if (cmd.BoardNode.Searched)
                        result.AppendLine();
                    PrintChilds(cmd.BoardNode, result, withScore, depth + 1);
                }
                if (depth == 0)
                    return result.ToString();

                return null;
            }
            else
            {
                if (!withScore)
                    result.AppendLine(node.CChessScore.ToString());
                else
                    result.AppendLine();

                return null;
            }
        }

        public void PrintNodeTree(CChessBoardNode rootNode, bool withScore = false, bool toConsole = false)
        {
            StringBuilder result = new StringBuilder();
            result.Append(rootNode.Board.PrintBoard());
            result.AppendLine("可能走法：");
            result.Append(PrintChilds(rootNode, null, withScore));
            result.Append("-------");
            if (toConsole)
                Console.WriteLine(result);
            else
                Tina.SaveTextFile(LogDataPath, $"{rootNode.Board.PrintBoardString(true)}.txt", result.ToString());
        }

        public void PrintBestNodeTree(List<CChessBoardNode> bestNodes)
        {
            Console.WriteLine("最好棋：");
            CChessBoardNode node;
            for (int i = 0; i < bestNodes.Count; i++)
            {
                StringBuilder sb = new StringBuilder();
                node = bestNodes[i];
                //sb.Append(" ");

                while (node.Parent != null)
                {
                    foreach (CChessMoveData cmd in node.Parent.NextMoves)
                        if (cmd.BoardNode.Board == node.Board)
                            sb.Insert(0, $" {CChessSystem.PrintChineseMoveString(node.Parent.Board, cmd.Move)}");
                    node = node.Parent;
                }
                sb.Insert(0, bestNodes[i].CChessScore);
                Console.WriteLine(sb.ToString());
            }
            Console.WriteLine("----");
        }

        public List<CChessMoveData> BestFS(CChessBoardNode startNode, long nodeCount = 100, int width = 3)
        {
            if (width < 1)
                throw new ArgumentOutOfRangeException(nameof(width));

            ExpandNode(startNode);
            UpdateNodeCChessScore(startNode);
            List<CChessBoardNode> bestNodes = GetBestNodes(startNode, width);
            while (nodeCount > 0)
            {
                for (int i = 0; i < bestNodes.Count; i++)
                {
                    nodeCount--;
                    ExpandNode(bestNodes[i]);
                    UpdateNodeCChessScore(bestNodes[i]);
                    UpdateParentNodeCChessScore(bestNodes[i]);
                }
                bestNodes = GetBestNodes(startNode, width);
            }

            //bestNodes = GetBestNodes(startNode, width, startNode.Board.IsBlackTurn);

            PrintNodeTree(startNode, true);
            PrintBestNodeTree(bestNodes);
            //if (startNode.Board.IsBlackTurn)
            //    Console.WriteLine($"結果：{CChessSystem.PrintChineseMoveString(startNode.Board, startNode.NextMoves[0].Move)}");
            //else
            //    Console.WriteLine($"結果：{CChessSystem.PrintChineseMoveString(startNode.Board, startNode.NextMoves[startNode.NextMoves.Count - 1].Move)}");

            List<CChessMoveData> result = new List<CChessMoveData>();
            CChessBoardNode node = startNode;
            while (node.Searched)
            {
                if (node.Board.IsBlackTurn)
                {
                    result.Add(node.NextMoves[0]);
                    node = node.NextMoves[0].BoardNode;
                }
                else
                {
                    result.Add(node.NextMoves[node.NextMoves.Count - 1]);
                    node = node.NextMoves[node.NextMoves.Count - 1].BoardNode;
                }
            }
            return result;
        }

        public void UpdateNodeCChessScore(CChessBoardNode node)
        {
            if (node.Searched)
            {
                if (node.NextMoves[0].BoardNode.Searched)
                {
                    foreach (CChessMoveData cmd in node.NextMoves)
                        UpdateNodeCChessScore(node.NextMoves[0].BoardNode);
                    node.NextMoves.Sort();
                }
                else
                {
                    if (node.Board.IsBlackTurn && node.CChessScore > node.NextMoves[0].BoardNode.CChessScore)
                        node.CChessScore = node.NextMoves[0].BoardNode.CChessScore;
                    else if (!node.Board.IsBlackTurn && node.CChessScore < node.NextMoves[node.NextMoves.Count - 1].BoardNode.CChessScore)
                        node.CChessScore = node.NextMoves[node.NextMoves.Count - 1].BoardNode.CChessScore;
                }
            }
        }

        public void UpdateParentNodeCChessScore(CChessBoardNode node)
        {
            if (node.Parent != null)
            {
                node.Parent.NextMoves.Sort();
                if ((node.Board.IsBlackTurn && node.Parent.CChessScore < node.CChessScore) ||
                    (!node.Board.IsBlackTurn && node.Parent.CChessScore > node.CChessScore))
                {
                    node.Parent.CChessScore = node.CChessScore;
                    UpdateParentNodeCChessScore(node.Parent);
                }
            }
        }

        public List<CChessBoardNode> GetBestNodes(CChessBoardNode startNode, int count = 3, bool start = true)
        {
            //挑3個最好的路
            //再往下分3、再往下分3，
            //尾端加入節點
            //排序
            if (!startNode.Searched)
                throw new ArgumentException(nameof(startNode));

            List<CChessBoardNode> result = new List<CChessBoardNode>();            
            //初始化
            result.Add(startNode);
            result.Add(null);
            int skipIndex = 0;
            //探索開始
            while (true)
            {
                if (result[skipIndex] == null)
                {
                    result.RemoveAt(skipIndex);
                    if (result.Count == 0)
                        break;
                    result.Sort();
                    skipIndex = 0;
                    if(result.Count > 3)
                    {
                        if (result[result.Count - 1].Board.IsBlackTurn)
                            result.RemoveRange(count, result.Count - count);
                        else
                            result.RemoveRange(0, result.Count - count);
                    }   

                    if (result.TrueForAll(m => !m.Searched))
                        break;
                    result.Add(null);
                }
                else if (result[skipIndex].Searched)
                {
                    if (result[skipIndex].Board.IsBlackTurn)
                        for (int i = 0; i < count && i < result[skipIndex].NextMoves.Count; i++)
                            result.Add(result[skipIndex].NextMoves[i].BoardNode);
                    else
                        for (int i = result[skipIndex].NextMoves.Count - 1; i >= 0 && i >= result[skipIndex].NextMoves.Count - 3; i--)
                            result.Add(result[skipIndex].NextMoves[i].BoardNode);
                    result.RemoveAt(skipIndex);
                }
                else
                    skipIndex++;
            }
            return result;
        }

        //兩輪之後更新一次
        //一層是一回合
        public void ExpandNode(CChessBoardNode node, int depth = 2)
        {
            if (depth < 0)
                throw new ArgumentException(nameof(depth));
            depth--;

            //Expand 1
            foreach (CChessMoveData cmd in node.NextMoves)
            {
                CChessBoard nextBoard = CChessSystem.SimpleMove(node.Board, cmd.Move);
                CChessBoardNode nextBoardNode = LoadOrCreateBoardNode(nextBoard, node);
                cmd.BoardNode = nextBoardNode;
                if (nextBoardNode.Status == CChessStatus.None)
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
                            if (MeasureScoreLevel == 1)
                                nextBoardNode.CChessScore = CChessBoardScoreCalculator.MeasureScore(nextBoard);
                            else if (MeasureScoreLevel == 2)
                                nextBoardNode.CChessScore = CChessBoardScoreCalculator.MeasureScorePrecision(nextBoard);
                            break;
                    }
                }

                if (BoardRecords.IndexOf(nextBoard) != -1)
                    continue;

                if (depth != 0)
                    ExpandNode(nextBoardNode, depth);
            }
            node.NextMoves.Sort();
            node.Searched = true;
        }

        public CChessBoardNode LoadOrCreateBoardNode(CChessBoard board, CChessBoardNode parent = null)
        {
            CChessBoardNode cbn;
            if (!NoDiskMode)
            {
                string filePath = Path.Combine(BoardNodeDataPath, $"{board.PrintBoardString(true)}.json");
                if (File.Exists(filePath))
                    cbn = Tina.LoadJsonFile<CChessBoardNode>(filePath);
                else
                {
                    cbn = new CChessBoardNode(board, parent);
                    Tina.SaveJsonFile(filePath, cbn, true);
                }
            }
            else
                cbn = new CChessBoardNode(board, parent);

            if (!NoBoardRecordMode && !BoardNodes.ContainsKey(board))
                BoardNodes.Add(board, cbn);
            return cbn;
        }

        public void UpdateBoardNode()
        {
            if (NoDiskMode)
                return;
            foreach (CChessBoardNode node in BoardNodes.Values)
            {
                string filePath = Path.Combine(BoardNodeDataPath, $"{node.Board.PrintBoardString(true)}.json");
                Tina.SaveJsonFile(filePath, node, true);
            }
        }


    }
}
