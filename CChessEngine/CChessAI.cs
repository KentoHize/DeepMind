﻿using System;
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

        public string PrintChilds(CChessBoardNode node, string startString = "")
        {
            if (node.Searched)
            {
                StringBuilder result = new StringBuilder();
                foreach (CChessMoveData cmd in node.NextMoves)
                {
                    string buffer = string.Concat(startString, CChessSystem.PrintChineseMoveString(node.Board, cmd.Move), " ");
                    result.Append(buffer);
                    result.Append(PrintChilds(cmd.BoardNode, buffer));
                }
                return result.ToString();
            }
            return string.Concat(node.CChessScore, "\n");
        }

        public void PrintNodeTree(CChessBoardNode rootNode, List<CChessBoardNode> bestNodes)
        {
            Console.WriteLine("可能局面:");
            Console.WriteLine(PrintChilds(rootNode));
            Console.WriteLine("-----");
        }

        public List<CChessMoveData> BestFS(CChessBoardNode startNode, long nodeCount = 100, int width = 3)
        {
            if (width < 1)
                throw new ArgumentOutOfRangeException(nameof(width));

            ExpandNode(startNode);
            //foreach (CChessMoveData cmd in startNode.NextMoves)
            //{
            //    Console.WriteLine(string.Concat(CChessSystem.PrintChineseMoveString(startNode.Board, cmd.Move, true),
            //        ":", cmd.BoardNode.CChessScore));
            //}
            //Console.WriteLine("-----");
            UpdateNodeCChessScore(startNode);
            //foreach(CChessMoveData cmd in startNode.NextMoves)
            //{
            //    Console.WriteLine(string.Concat(CChessSystem.PrintChineseMoveString(startNode.Board, cmd.Move, true),
            //        ":", cmd.BoardNode.CChessScore));
            //}
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

            PrintNodeTree(startNode, bestNodes);

            List<CChessMoveData> result = new List<CChessMoveData>();
            CChessBoardNode node = startNode;
            while (node.Searched)
            {
                if (node.Board.IsBlackTurn)
                {
                    result.Add(node.NextMoves.Min);
                    node = node.NextMoves.Min.BoardNode;
                }
                else
                {
                    result.Add(node.NextMoves.Max);
                    node = node.NextMoves.Max.BoardNode;
                }
            }
            return result;
        }

        public CChessBoardNode UpdateNodeCChessScore(CChessBoardNode node)
        {
            if (node.Searched)
            {
                CChessBoardNode result;
                result = node.NextMoves.Min.BoardNode;
                if (node.Board.IsBlackTurn)
                {
                    foreach (CChessMoveData cmd in node.NextMoves)
                    {
                        CChessBoardNode cbn = UpdateNodeCChessScore(cmd.BoardNode);
                        if (result.CChessScore > cbn.CChessScore)
                            result = cbn;
                    }
                }
                else
                {
                    foreach (CChessMoveData cmd in node.NextMoves)
                    {
                        CChessBoardNode cbn = UpdateNodeCChessScore(cmd.BoardNode);
                        if (result.CChessScore < cbn.CChessScore)
                            result = cbn;
                    }
                }
                return result;
            }
            else
                return node;
        }

        public void UpdateParentNodeCChessScore(CChessBoardNode node)
        {
            if (node.Parent != null)
            {
                if ((node.Board.IsBlackTurn && node.Parent.CChessScore < node.CChessScore) ||
                    (!node.Board.IsBlackTurn && node.Parent.CChessScore > node.CChessScore))
                {
                    node.Parent.CChessScore = node.CChessScore;
                    UpdateParentNodeCChessScore(node.Parent);
                }
            }
        }

        public List<CChessBoardNode> GetBestNodes(CChessBoardNode node, int count = 3)
        {
            //挑3個最好的路
            //往下再算3個，從這9個節點中在挑3個
            //再往下算3個，以此類推
            if (!node.Searched)
                throw new ArgumentException(nameof(node));

            List<CChessBoardNode> bestNodes = new List<CChessBoardNode>();
            List<CChessBoardNode> result = new List<CChessBoardNode>();

            int i = 0;
            if (node.Board.IsBlackTurn)
            {
                foreach (CChessMoveData cmd in node.NextMoves)
                {
                    i++;
                    bestNodes.Add(cmd.BoardNode);
                    if (i == count)
                        break;
                }
            }
            else
            {
                foreach (CChessMoveData cmd in node.NextMoves.Reverse())
                {
                    i++;
                    bestNodes.Add(cmd.BoardNode);
                    if (i == count)
                        break;
                }
            }

            for (i = 0; i < bestNodes.Count; i++)
                if (bestNodes[i].Searched)
                    result.AddRange(GetBestNodes(bestNodes[i], count));
                else
                    result.Add(bestNodes[i]);


            if (node.Board.IsBlackTurn)
            {
                result.Sort((x, y) =>
                {
                    if (x.CChessScore == y.CChessScore)
                        return 0;
                    else if (x.CChessScore > y.CChessScore)
                        return 1;
                    return -1;
                });
            }
            else
            {
                result.Sort((x, y) =>
                {
                    if (x.CChessScore == y.CChessScore)
                        return 0;
                    else if (x.CChessScore < y.CChessScore)
                        return 1;
                    return -1;
                });
            }

            result.RemoveRange(3, result.Count - 3);
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
                            if(MeasureScoreLevel == 1)
                                nextBoardNode.CChessScore = CChessBoardScoreCalculator.MeasureScore(nextBoard);
                            else if(MeasureScoreLevel == 2)
                                nextBoardNode.CChessScore = CChessBoardScoreCalculator.MeasureScorePrecision(nextBoard);
                            break;
                    }
                }


                if (BoardRecords.IndexOf(nextBoard) != -1)
                    continue;

                if (depth != 0)
                    ExpandNode(nextBoardNode, depth);
            }
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
