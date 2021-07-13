using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DeepMind.ChineseChess
{
    public class CChessSystem : IChessGameSystem
    {
        protected static string[] PieceLetters => new string[] { "KABRNCP", "kabrncp" };
        public CChessBoard CurrentBoard { get; set; }
        public List<CChessMove> CurrentLegalMoves { get; protected set; }
        public List<CChessMove> MoveRecords { get; set; }        
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
                                    if (j != 9 && !PieceLetters[player].Contains(ccb[i, j + 1]))
                                        result.Add(new CChessMove(ccb[i, j], i, j, i, j + 1));
                                }
                                else if (player == 1)
                                {
                                    //未過河 - 黑
                                    if (j != 0 && !PieceLetters[player].Contains(ccb[i, j - 1]))
                                        result.Add(new CChessMove(ccb[i, j], i, j, i, j - 1));
                                }
                                if (player == 0 && j >= 4 || player == 1 && j <= 5)
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
                                if (i < 8 && j < 8 && ccb[i, j + 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i + 1, j + 2]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i + 1, j + 2));
                                if (i < 7 && j < 9 && ccb[i + 1, j] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i + 2, j + 1]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i + 2, j + 1));
                                if (i < 7 && j > 0 && ccb[i + 1, j] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i + 2, j - 1]))
                                    result.Add(new CChessMove(ccb[i, j], i, j, i + 2, j - 1));
                                if (i < 8 && j > 1 && ccb[i, j - 1] == ' ' &&
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

        public List<IChessMove> GetNextMoves()
        {   
            return GetLegalMoves().ToList<IChessMove>();
        }            

        public static void Move(CChessBoard ccb, CChessMove move, bool checkLegal = false)
        {
            if(checkLegal)
            {
                List<CChessMove> legalMoves = GetLegalMoves(ccb);
                if (legalMoves.IndexOf(move) == -1)
                    throw new ArgumentOutOfRangeException(nameof(move));
            }


            char piece = ccb[move.X1, move.Y1];
            ccb[move.X2, move.Y2] = piece;
            ccb[move.X1, move.Y1] = ' ';
            ccb.IsBlackTurn = !ccb.IsBlackTurn;
            CheckResult(ccb);
        }

        public void Move(CChessMove move, bool checkLegal = false)
        {
            if (checkLegal)
            {   
                if (CurrentLegalMoves.IndexOf(move) == -1)
                    throw new ArgumentOutOfRangeException(nameof(move));
            }
            Move(CurrentBoard, move);
            MoveRecords.Add(move);
            CheckResult();
        }

        public GameResult Move(IChessMove move)
        {
            Move(move as CChessMove);
            if (Status == CChessStatus.BlackWin)
                return GameResult.Player2Win;
            else if (Status == CChessStatus.RedWin)
                return GameResult.Player1Win;
            else if (Status == CChessStatus.Draw)
                return GameResult.Draw;
            return GameResult.None;
        }

        public void Initialize()
        {
            CurrentBoard = new CChessBoard();
            MoveRecords = new List<CChessMove>();
            Status = CChessStatus.None;
        }

        public void Start()
        {
            CheckResult();
        }

        public void Abort()
        {
            Initialize();
        }
        public void Restart()
            => Restart(null);

        public void Restart(CChessBoard board)
        {
            Initialize();
            if (board != null)
                CurrentBoard = board;
            Start();
        }

        public void CheckResult()
        {
            CurrentLegalMoves = GetLegalMoves();
            Status = CheckResult(CurrentBoard, CurrentLegalMoves);
        }

        public static CChessStatus CheckResult(CChessBoard ccb, List<CChessMove> legalMoves = null)
        {
            if (ccb == CChessBoard.StartingBoard)
                return CChessStatus.AtStart;

            if (legalMoves == null)
                legalMoves = GetLegalMoves(ccb);
            for (int i = 0; i < legalMoves.Count; i++)
            {
                if (ccb[legalMoves[i].X2, legalMoves[i].Y2] == 'K')
                    if (ccb.IsBlackTurn)
                        return CChessStatus.BlackWin;
                    else
                        return CChessStatus.BlackCheck;

                if (ccb[legalMoves[i].X2, legalMoves[i].Y2] == 'k')
                    if (ccb.IsBlackTurn)
                        return CChessStatus.RedCheck;
                    else
                        return CChessStatus.RedWin;
            }
            if (legalMoves.Count == 0)
                return ccb.IsBlackTurn ? CChessStatus.RedWin : CChessStatus.BlackWin;
            return CChessStatus.Smooth;
        }

        public List<IChessMove> GetMoveRecords()
            => MoveRecords.ToList<IChessMove>();

        public IChessBoard GetCurrentBoard()
            => CurrentBoard;

        public string ToTestString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(CurrentBoard.PrintBoard());
            result.Append("狀態：");
            result.AppendLine(Status.ToCString());
            result.Append("棋譜：");
            for (int i = 0; i < MoveRecords.Count; i++)
            {
                if (i % 2 == 0)
                    result.AppendLine();
                result.AppendFormat("{0} ", MoveRecords[i].ToTestString());
            }
            return result.ToString();
        }

        
        public static string GetChineseStepString(CChessBoard ccb, CChessMove ccm)
        {
            //只有兵車炮有順序問題
            
            //char pieceChar = ccb[ccm.X1, ccm.Y1];
            //char orderChar = '\0';
            //int order = 0;
            //if(ccb.IsBlackTurn)

            //for (int i = 0; i < 10; i++)
            //{
            //    ccb[ccm.X1, i]
            //}
            return "";
        }

        
    }
}
