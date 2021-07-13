using System;
using System.Collections.Generic;
using System.Text;

namespace CChessEngine
{
    public class CChessSystem
    {
        protected static string[] PieceLetters => new string[] { "KABRNCP", "kabrncp" };
        protected static char[] ChineseNumbers => new char[] { '零', '一', '二', '三', '四', '五', '六', '七', '八', '九', '十' };

        //由盤面確認下一步
        public static List<CChessMove> GetLegalMoves(CChessBoard ccb)
        {   
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
                                        result.Add(new CChessMove(i, j, i, j - 1));
                                }
                                else if (player == 1)
                                {
                                    //未過河 - 黑
                                    if (j != 9 && !PieceLetters[player].Contains(ccb[i, j + 1]))
                                        result.Add(new CChessMove(i, j, i, j + 1));
                                }
                                else if (player == 0 && j <= 4 || player == 1 && j >= 5)
                                {
                                    //已過河                                    
                                    if (i != 0 && !PieceLetters[player].Contains(ccb[i - 1, j]))
                                        result.Add(new CChessMove(i, j, i - 1, j));
                                    if (i != 8 && !PieceLetters[player].Contains(ccb[i + 1, j]))
                                        result.Add(new CChessMove(i, j, i + 1, j));
                                }
                                break;
                            case 'C':
                            case 'c':
                                bool isRun = true;
                                bool CheckStraightBao(int x, int y)
                                {
                                    if (isRun && ccb[x, y] == ' ')
                                        result.Add(new CChessMove(i, j, x, y));
                                    else if (isRun)
                                        isRun = false;
                                    else if (ccb[x, y] != ' ')
                                    {
                                        if (PieceLetters[1 - player].Contains(ccb[x, y]))
                                            result.Add(new CChessMove(i, j, x, y));
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
                                        result.Add(new CChessMove(i, j, x, y));
                                    else if (ccb[x, y] != ' ')
                                    {
                                        if (PieceLetters[1 - player].Contains(ccb[x, y]))
                                            result.Add(new CChessMove(i, j, x, y));
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
                                    result.Add(new CChessMove(i, j, i - 1, j - 2));
                                if (i > 1 && j > 0 && ccb[i - 1, j] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i - 2, j - 1]))
                                    result.Add(new CChessMove(i, j, i - 2, j - 1));
                                if (i > 1 && j < 9 && ccb[i - 1, j] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i - 2, j + 1]))
                                    result.Add(new CChessMove(i, j, i - 2, j + 1));
                                if (i > 0 && j < 8 && ccb[i, j + 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i - 1, j + 2]))
                                    result.Add(new CChessMove(i, j, i - 1, j + 2));
                                if (i < 8 && j < 8 && ccb[i, j + 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i + 1, j + 2]))
                                    result.Add(new CChessMove(i, j, i + 1, j + 2));
                                if (i < 7 && j < 9 && ccb[i + 1, j] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i + 2, j + 1]))
                                    result.Add(new CChessMove(i, j, i + 2, j + 1));
                                if (i < 7 && j > 0 && ccb[i + 1, j] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i + 2, j - 1]))
                                    result.Add(new CChessMove(i, j, i + 2, j - 1));
                                if (i < 8 && j > 1 && ccb[i, j - 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i + 1, j - 2]))
                                    result.Add(new CChessMove(i, j, i + 1, j - 2));
                                break;
                            case 'B':
                            case 'b':
                                if ((player == 0 || j >= 7)
                                    && i > 1 && j > 1 && ccb[i - 1, j - 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i - 2, j - 2]))
                                    result.Add(new CChessMove(i, j, i - 2, j - 2));
                                if ((player == 0 || j >= 7)
                                    && i < 8 && j > 1 && ccb[i + 1, j - 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i + 2, j - 2]))
                                    result.Add(new CChessMove(i, j, i + 2, j - 2));
                                if ((player == 1 || j <= 2)
                                    && i < 8 && j < 8 && ccb[i + 1, j + 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i + 2, j + 2]))
                                    result.Add(new CChessMove(i, j, i + 2, j + 2));
                                if ((player == 1 || j <= 2)
                                    && i > 1 && j < 8 && ccb[i - 1, j + 1] == ' ' &&
                                    !PieceLetters[player].Contains(ccb[i - 2, j + 2]))
                                    result.Add(new CChessMove(i, j, i - 2, j + 2));
                                break;
                            case 'A':
                            case 'a':
                                if (((player == 0 && i >= 4 && j > 0) || (player == 1 && i >= 4 && j > 7)) &&
                                    !PieceLetters[player].Contains(ccb[i - 1, j - 1]))
                                    result.Add(new CChessMove(i, j, i - 1, j - 1));
                                if (((player == 0 && i <= 4 && j > 0) || (player == 1 && i <= 4 && j > 7)) &&
                                    !PieceLetters[player].Contains(ccb[i + 1, j - 1]))
                                    result.Add(new CChessMove(i, j, i + 1, j - 1));
                                if (((player == 0 && i <= 4 && j < 2) || (player == 1 && i <= 4 && j < 9)) &&
                                    !PieceLetters[player].Contains(ccb[i + 1, j + 1]))
                                    result.Add(new CChessMove(i, j, i + 1, j + 1));
                                if (((player == 0 && i >= 4 && j < 2) || (player == 1 && i >= 4 && j < 9)) &&
                                    !PieceLetters[player].Contains(ccb[i - 1, j + 1]))
                                    result.Add(new CChessMove(i, j, i - 1, j + 1));
                                break;
                            case 'K':
                            case 'k':
                                if (i >= 4 && !PieceLetters[player].Contains(ccb[i - 1, j]))
                                    result.Add(new CChessMove(i, j, i - 1, j));
                                if (i <= 4 && !PieceLetters[player].Contains(ccb[i + 1, j]))
                                    result.Add(new CChessMove(i, j, i + 1, j));
                                if (((player == 0 && j > 1) || (player == 1 && j > 7)) &&
                                    !PieceLetters[player].Contains(ccb[i, j - 1]))
                                    result.Add(new CChessMove(i, j, i, j - 1));
                                if (((player == 0 && j < 2) || (player == 1 && j < 9)) &&
                                    !PieceLetters[player].Contains(ccb[i, j + 1]))
                                    result.Add(new CChessMove(i, j, i, j + 1));

                                bool CheckStraightKing(int x, int y)
                                {
                                    if (ccb[x, y] != ' ')
                                    {
                                        if (ccb[x, y] == 'k' || ccb[x, y] == 'K')
                                            result.Add(new CChessMove(i, j, x, y));
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

        public static CChessStatus CheckStatus(CChessBoard board, CChessMove move, List<CChessMove> legalMoves = null)
        {
            if (board == CChessBoard.StartingBoard)
                return CChessStatus.AtStart;
            if (legalMoves == null)
                legalMoves = GetLegalMoves(board);

            for (int i = 0; i < legalMoves.Count; i++)
            {
                if (board[legalMoves[i].X2, legalMoves[i].Y2] == 'K')
                    if (board.IsBlackTurn)
                        return CChessStatus.BlackWin;
                    else
                        return CChessStatus.BlackCheck;

                if (board[legalMoves[i].X2, legalMoves[i].Y2] == 'k')
                    if (board.IsBlackTurn)
                        return CChessStatus.RedCheck;
                    else
                        return CChessStatus.RedWin;
            }
            if (legalMoves.Count == 0)
                return board.IsBlackTurn ? CChessStatus.RedWin : CChessStatus.BlackWin;
            return CChessStatus.Smooth;
        }
        
        public static string PrintChineseMoveString(CChessBoard board, CChessMove move, bool alwayUseChineseNumber = false)
        {
            StringBuilder result = new StringBuilder();

            //board[move.X1, move.Y1]


            if (move.Y1 == move.Y2)
            {
                result.Append('平');
                if(!alwayUseChineseNumber)
                    result.Append(board.IsBlackTurn ? move.X2.ToString() : ChineseNumbers[move.X2].ToString());
                else
                    result.Append(ChineseNumbers[move.X2].ToString());
            }   
            else if (board.IsBlackTurn)
            {
                result.Append(move.Y1 > move.Y2 ? '進' : '退');
                if(!alwayUseChineseNumber)
                    result.Append(move.X1 == move.X2 ? Math.Abs(move.Y1 - move.Y2) : move.X2);
                else
                    result.Append(move.X1 == move.X2 ? ChineseNumbers[Math.Abs(move.Y1 - move.Y2)]
                    : ChineseNumbers[move.X2]);
            }   
            else
            {
                result.Append(move.Y1 < move.Y2 ? '進' : '退');
                result.Append(move.X1 == move.X2 ? ChineseNumbers[Math.Abs(move.Y1 - move.Y2)]
                    : ChineseNumbers[move.X2]);
            }
            return result.ToString();
        }

        public static string PrintMoveString(CChessBoard board,CChessMove move)
        {
            return "";
        }


        //public string ToTestString()
        //{
        //    StringBuilder result = new StringBuilder();
        //    result.Append(CurrentBoard.PrintBoard());
        //    result.Append("狀態：");
        //    result.AppendLine(Status.ToCString());
        //    result.Append("棋譜：");
        //    for (int i = 0; i < MoveRecords.Count; i++)
        //    {
        //        if (i % 2 == 0)
        //            result.AppendLine();
        //        result.AppendFormat("{0} ", MoveRecords[i].ToTestString());
        //    }
        //    return result.ToString();
        //}
    }
}
