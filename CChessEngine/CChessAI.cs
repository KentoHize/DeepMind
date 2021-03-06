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
            //BestNodes = new SortedList<long, CChessBoardNode>();
            MoveRecords = new List<CChessMove>();
            BoardRecords = new List<CChessBoard>();
            NoDiskMode = noDiskMode;
            NoBoardRecordMode = noBoardRecordMode;
            MeasureScoreLevel = measureLevel;
            StartBoardNode = LoadOrCreateBoardNode(startBoard, null);
            CurrentBoardNode = StartBoardNode;
        }

        public MoveStatus Go(CChessBoardNode bn, int depth, int width, out CChessMoveData bestMoveData, out List<CChessMoveData> estimateMoves)
        {
            bestMoveData = null;
            estimateMoves = null;

            //if (bn.NextMoves.Count == 0)
            //    return MoveStatus.Resign;
            //To Do

            List<CChessMoveData> result = BestFS(bn, depth, width);


            if (result.Count == 0)
                return MoveStatus.NoBestMove;

            bestMoveData = result[0];
            estimateMoves = result;
            return MoveStatus.BestMove;
        }

        public MoveStatus Go(CChessBoard board, int depth, int width, out CChessMoveData bestMoveData, out List<CChessMoveData> estimateMoves)
        {
            CChessBoardNode bn;
            if (BoardNodes.ContainsKey(board))
                bn = BoardNodes[board];
            else
                bn = LoadOrCreateBoardNode(board, null);
            return Go(bn, depth, width, out bestMoveData, out estimateMoves);
        }

        public MoveStatus Move(int depth, int width, out CChessMoveData bestMoveData, out List<CChessMoveData> estimateMoves)
        {
            MoveStatus result = Go(CurrentBoardNode, depth, width, out bestMoveData, out estimateMoves);
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
                if (!node.Board.IsBlackTurn)
                    node.NextMoves.Reverse();
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
                if (!node.Board.IsBlackTurn)
                    node.NextMoves.Reverse();
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

        public List<CChessMoveData> BestFS(CChessBoardNode startNode, long nodeCount = 100, int width = 10)
        {
            if (width < 1)
                throw new ArgumentOutOfRangeException(nameof(width));


            ExpandNode(startNode);
            //for (int i = 0; i < startNode.NextMoves.Count; i++)
            //{
            //    if (startNode.NextMoves[i].BoardNode != null)
            //        for (int j = 0; j < startNode.NextMoves[i].BoardNode.NextMoves.Count; j++)
            //            ExpandNode(startNode.NextMoves[i].BoardNode.NextMoves[j].BoardNode);
            //}
            UpdateNodeCChessScore(startNode);

            
            List<CChessBoardNode> bestNodes;
            while (nodeCount > 0)
            {
                bestNodes = GetBestNodes(startNode, width);
                for (int i = 0; i < bestNodes.Count; i++)
                {
                    nodeCount--;
                    ExpandNode(bestNodes[i]);
                    UpdateNodeCChessScore(bestNodes[i]);
                    UpdateParentNodeCChessScore(bestNodes[i]);
                }
                //PrintBestNodeTree(bestNodes);
            }

            //bestNodes = GetBestNodes(startNode, width);
            //PrintNodeTree(startNode, true);

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
                        UpdateNodeCChessScore(cmd.BoardNode);

                    node.NextMoves.Sort();
                }

                if (node.Board.IsBlackTurn && node.CChessScore > node.NextMoves[0].BoardNode.CChessScore)
                    node.CChessScore = node.NextMoves[0].BoardNode.CChessScore;
                else if (!node.Board.IsBlackTurn && node.CChessScore < node.NextMoves[node.NextMoves.Count - 1].BoardNode.CChessScore)
                    node.CChessScore = node.NextMoves[node.NextMoves.Count - 1].BoardNode.CChessScore;

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
            else
                node.NextMoves.Sort();
        }

        public static CChessBoardNode GetBestNode(CChessBoardNode startNode, out List<CChessBoardNode> routeNodes)
        {
            CChessBoardNode node = startNode;
            routeNodes = new List<CChessBoardNode>();
            while (node.Searched)
            {
                routeNodes.Add(node);
                if (node.Board.IsBlackTurn)
                    node = node.NextMoves[0].BoardNode;
                else
                    node = node.NextMoves[node.NextMoves.Count - 1].BoardNode;
            }
            return node;
        }

        public List<CChessBoardNode> GetBestNodes(CChessBoardNode startNode, int count = 3)
        {
            //首先取得最佳節點
            //接下來分析其路線中的分支節點找尋次佳節點
            //以扇形的方式分布
            if (!startNode.Searched)
                throw new ArgumentException(nameof(startNode));

            List<CChessBoardNode> routeNodes;
            List<CChessBoardNode> result = new List<CChessBoardNode>();
            //第一節點
            CChessBoardNode node = GetBestNode(startNode, out routeNodes);
            result.Add(node);
            if (count == 1)
                return result;

            //第二類節點 - 直
            for (int i = routeNodes.Count - 1; i >= 0; i--)
            {
                if (routeNodes[i].NextMoves.Count < 2)
                    continue;
                if (routeNodes[i].Board.IsBlackTurn)
                    result.Add(GetBestNode(routeNodes[i].NextMoves[1].BoardNode, out _));
                else
                    result.Add(GetBestNode(routeNodes[i].NextMoves[routeNodes[i].NextMoves.Count - 2].BoardNode, out _));
                if (result.Count == count)
                {
                    result.Sort();
                    return result;
                }
            }

            //第三類節點 - 扇
            for (int i = routeNodes.Count - 1; i >= 0; i--)
            {
                for (int j = 2; j < i; j++)
                {
                    if (routeNodes[i].NextMoves.Count <= j)
                        continue;
                    if (routeNodes[i].Board.IsBlackTurn)
                        result.Add(GetBestNode(routeNodes[i].NextMoves[j].BoardNode, out _));
                    else
                        result.Add(GetBestNode(routeNodes[i].NextMoves[routeNodes[i].NextMoves.Count - j].BoardNode, out _));

                    if (result.Count == count)
                    {
                        result.Sort();
                        return result;
                    }
                }
            }
            result.Sort();
            return result;
        }

        public void ExpandNextMoves(CChessBoardNode node)
        {   
            node.NextMoves = CChessSystem.GetLegalMoves(node.Board).ToMoveDataList();
            for (int i = 0; i < node.NextMoves.Count; i++)
                node.NextMoves[i].BoardNode = LoadOrCreateBoardNode(CChessSystem.SimpleMove(node.Board, node.NextMoves[i].Move), node);
        }

        //兩輪之後更新一次
        //一層是一回合
        public void ExpandNode(CChessBoardNode node, int depth = 2)
        {
            if (depth < 0)
                throw new ArgumentException(nameof(depth));
            depth--;

            ExpandNextMoves(node);            
            foreach (CChessMoveData cmd in node.NextMoves)
            {   
                if (cmd.BoardNode.Status == CChessStatus.None)
                {
                    CChessStatus nextBoardStatus = CChessSystem.CheckStatus(cmd.BoardNode.Board, cmd.Move, cmd.BoardNode.NextMoves.ToMoveList());
                    switch (nextBoardStatus)
                    {
                        case CChessStatus.BlackWin:
                            cmd.BoardNode.Player1WinScore =
                            cmd.BoardNode.CChessScore = 0;
                            continue;
                        case CChessStatus.RedWin:
                            cmd.BoardNode.Player1WinScore =
                            cmd.BoardNode.CChessScore =
                            CChessBoardNode.MaxScore;
                            continue;
                        case CChessStatus.BlackCheckmate:
                            cmd.BoardNode.Player1WinScore =
                            cmd.BoardNode.CChessScore = 0;
                            continue;
                        case CChessStatus.RedCheckmate:
                            cmd.BoardNode.Player1WinScore =
                            cmd.BoardNode.CChessScore =
                            CChessBoardNode.MaxScore;
                            continue;
                        default:
                            if (MeasureScoreLevel == 1)
                                cmd.BoardNode.CChessScore = CChessBoardScoreCalculator.MeasureScore(cmd.BoardNode.Board);
                            else if (MeasureScoreLevel == 2)
                                cmd.BoardNode.CChessScore = CChessBoardScoreCalculator.MeasureScorePrecision(cmd.BoardNode.Board);
                            break;
                    }
                }

                if (BoardRecords.IndexOf(cmd.BoardNode.Board) != -1)
                    continue;

                if (depth != 0)
                    ExpandNode(cmd.BoardNode, depth);
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
