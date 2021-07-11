using System;
using System.Collections.Generic;
using System.Text;

namespace DeepMind.ChineseChess
{
    public class CChessSystem
    {
        protected static string[] PieceLetters => new string[] { "KABRNCP", "kabrncp" };
        public CChessBoard CurrentBoard { get; set; }
        public List<CChessMove> CurrentLegalMove { get; protected set; }
        public List<CChessMove> MoveRecord { get; set; }
        public CChessStatus Status { get; set; }
        public CChessSystem()
        {
            Initialize();
        }

        public static List<CChessMove> GetLegalMoves(CChessBoard ccb)
        {
            //搜尋每一個子的每一個可能移動
            byte player = ccb.IsBlackTurn ? (byte)1 : (byte)0;
            List<CChessMove> result = new List<CChessMove>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (PieceLetters[player].Contains(ccb[i, j]))
                    {
                        switch (ccb[i, j])
                        {
                            case 'P':
                            case 'p':
                                if (player == 0)
                                {
                                    //未過河 - 紅
                                    if (j != 0 && !PieceLetters[player].Contains(ccb[i, j - 1]))
                                        result.Add(new CChessMove(ccb[i, j], i, j, i, j - 1));
                                }
                                else if (player == 1)
                                {
                                    //未過河 - 黑
                                    if (j != 9 && !PieceLetters[player].Contains(ccb[i, j + 1]))
                                        result.Add(new CChessMove(ccb[i, j], i, j, i, j + 1));
                                }
                                else if (player == 0 && j <= 4 || player == 1 && j >= 5)
                                {
                                    //已過河                                    
                                    if (i != 0 && !PieceLetters[player].Contains(ccb[i - 1, j]))
                                        result.Add(new CChessMove(ccb[i, j], i, j, i - 1, j));
                                    if (i != 8 && !PieceLetters[player].Contains(ccb[i + 1, j]))
                                        result.Add(new CChessMove(ccb[i, j], i, j, i + 1, j));
                                }
                                break;
                            case 'C':
                            case 'c':
                                bool isRun = true;
                                bool CheckStraightBao(int x, int y)
                                {
                                    if (isRun && ccb[x, y] == ' ')
                                        result.Add(new CChessMove(ccb[i, j], i, j, x, y));
                                    else if (isRun)
                                        isRun = false;
                                    else if (ccb[x, y] != ' ')
                                    {
                                        if (PieceLetters[1 - player].Contains(ccb[x, y]))
                                            result.Add(new CChessMove(ccb[i, j], i, j, x, y));
                                        return false;
                                    }
                                    return true;
                                }

                                for (int k = i - 1; k >= 0; k--)
                                    if (!CheckStraightBao(k, j))
                                        break;
                                isRun = true;
                                for (int k = i + 1; k < 9; k++)
                                    if (!CheckStraightBao(k, j))
                                        break;
                                isRun = true;
                                for (int k = j - 1; k >= 0; k--)
                                    if (!CheckStraightBao(i, k))
                                        break;
                                isRun = true;
                                for (int k = j + 1; k < 10; k++)
                                    if (!CheckStraightBao(i, k))
                                        break;
                                break;
                            case 'R':
                            case 'r':
                                bool CheckStraightChe(int x, int y)
                                {
                                    if (ccb[x, y] == ' ')
                                        result.Add(new CChessMove(ccb[i, j], i, j, x, y));
                                    else if (ccb[x, y] != ' ')
                                    {
                                        if (PieceLetters[1 - player].Contains(ccb[x, y]))
                                            result.Add(new CChessMove(ccb[i, j], i, j, x, y));
                                        return false;
                                    }
                                    return true;
                                }

                                for (int k = i - 1; k >= 0; k--)
                                    if (!CheckStraightChe(k, j))
                                        break;
                                for (int k = i + 1; k < 9; k++)
                                    if (!CheckStraightChe(k, j))
                                        break;
                                for (int k = j - 1; k >= 0; k--)
                                    if (!CheckStraightChe(i, k))
                                        break;
                                for (int k = j + 1; k < 10; k++)
                                    if (!CheckStraightChe(i, k))
                                        break;
                                break;
                            case 'N':
                            case 'n':
                                if (i > 0 && j > 1 && ccb[i, j - 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i - 1, j - 2]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i - 1, j - 2));
                                if (i > 1 && j > 0 && ccb[i - 1, j] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i - 2, j - 1]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i - 2, j - 1));
                                if (i > 1 && j < 9 && ccb[i - 1, j] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i - 2, j + 1]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i - 2, j + 1));
                                if (i > 0 && j < 8 && ccb[i, j + 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i - 1, j + 2]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i - 1, j + 2));
                                if (i < 9 && j < 8 && ccb[i, j + 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i + 1, j + 2]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i + 1, j + 2));
                                if (i < 8 && j < 9 && ccb[i + 1, j] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i + 2, j + 1]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i + 2, j + 1));
                                if (i < 8 && j > 0 && ccb[i + 1, j] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i + 2, j - 1]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i + 2, j - 1));
                                if (i < 9 && j > 1 && ccb[i, j - 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i + 1, j - 2]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i + 1, j - 2));
                                break;
                            case 'B':
                            case 'b':
                                if ((player == 0 || j >= 7)
                                    && i > 1 && j > 1 && ccb[i - 1, j - 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i - 2, j - 2]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i - 2, j - 2));
                                if ((player == 0 || j >= 7)
                                    && i < 8 && j > 1 && ccb[i + 1, j - 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i + 2, j - 2]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i + 2, j - 2));
                                if ((player == 1 || j <= 2)
                                    && i < 8 && j < 8 && ccb[i + 1, j + 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i + 2, j + 2]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i + 2, j + 2));
                                if ((player == 1 || j <= 2)
                                    && i > 1 && j < 8 && ccb[i - 1, j + 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i - 2, j + 2]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i - 2, j + 2));
                                break;
                            case 'A':
                            case 'a':
                                if (((player == 0 && i >= 4 && j > 0) || (player == 1 && i >= 4 && j > 7)) &&
                                    !PieceLetters[player].Contains(ccb[i - 1, j - 1]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i - 1, j - 1));
                                if (((player == 0 && i <= 4 && j > 0) || (player == 1 && i <= 4 && j > 7)) &&
                                    !PieceLetters[player].Contains(ccb[i + 1, j - 1]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i + 1, j - 1));
                                if (((player == 0 && i <= 4 && j < 2) || (player == 1 && i <= 4 && j < 9)) &&
                                    !PieceLetters[player].Contains(ccb[i + 1, j + 1]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i + 1, j + 1));
                                if (((player == 0 && i >= 4 && j < 2) || (player == 1 && i >= 4 && j < 9)) &&
                                    !PieceLetters[player].Contains(ccb[i - 1, j + 1]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i - 1, j + 1));
                                break;
                            case 'K':
                            case 'k':
                                if (i >= 4 && !PieceLetters[player].Contains(ccb[i - 1, j]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i - 1, j));
                                if (i <= 4 && !PieceLetters[player].Contains(ccb[i + 1, j]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i + 1, j));
                                if (((player == 0 && j > 1) || (player == 1 && j > 7)) &&
                                    !PieceLetters[player].Contains(ccb[i, j - 1]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i, j - 1));
                                if (((player == 0 && j < 2) || (player == 1 && j < 9)) &&
                                    !PieceLetters[player].Contains(ccb[i, j + 1]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i, j + 1));

                                bool CheckStraightKing(int x, int y)
                                {
                                    if (ccb[x, y] != ' ')
                                    {
                                        if (ccb[x, y] == 'k' || ccb[x, y] == 'K')
                                            result.Add(new CChessMove(ccb[i, j], i, j, x, y));
                                        return false;
                                    }
                                    return true;
                                }

                                if (player == 0)
                                {
                                    for (int k = j + 1; k < 10; k++)
                                        if (!CheckStraightKing(i, k))
                                            break;
                                }
                                else
                                {
                                    for (int k = j - 1; k >= 0; k--)
                                        if (!CheckStraightKing(i, k))
                                            break;
                                }
                                break;
                        }
                    }
                }
            }

            return result;
        }

        public List<CChessMove> GetLegalMoves()
            => GetLegalMoves(CurrentBoard);

        public void Move(CChessMove move)
        {
            if (CurrentLegalMove.IndexOf(move) == -1)
                throw new ArgumentOutOfRangeException(nameof(move));

            char piece = CurrentBoard[move.X1, move.Y1];
            CurrentBoard[move.X2, move.Y2] = piece;
            CurrentBoard[move.X1, move.Y1] = ' ';
            CurrentBoard.IsBlackTurn = !CurrentBoard.IsBlackTurn;
            CheckResult();
        }

        public void Initialize()
        {
            CurrentBoard = new CChessBoard();
            Status = CChessStatus.None;
        }

        public void Start()
        {
            CheckResult();
        }

        public void Restart(CChessBoard board = null)
        {
            Initialize();
            if (board != null)
                CurrentBoard = board;
            Start();
        }

        public void CheckResult()
        {
            CurrentLegalMove = GetLegalMoves();
            Status = CheckResult(CurrentBoard, CurrentLegalMove);
        }            

        public static CChessStatus CheckResult(CChessBoard ccb, List<CChessMove> legalMoves = null)
        {
            if (ccb == CChessBoard.StartingBoard)
                return CChessStatus.AtStart;

            if(legalMoves == null)
                legalMoves = GetLegalMoves(ccb);
            for (int i = 0; i < legalMoves.Count; i++)
            {
                if (ccb.IsBlackTurn)
                {
                    if (ccb[legalMoves[i].X2, legalMoves[i].Y2] == 'K')
                        return CChessStatus.BlackWin;
                }
                else
                {
                    return CChessStatus.BlackCheck;
                }

                if (!ccb.IsBlackTurn)
                {
                    if (ccb[legalMoves[i].X2, legalMoves[i].Y2] == 'k')
                        return CChessStatus.RedWin;
                }
                else
                {
                    return CChessStatus.RedCheck;
                }   
            }
            return CChessStatus.Smooth;
        }     

    }
}
