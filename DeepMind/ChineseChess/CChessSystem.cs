using System;
using System.Collections.Generic;
using System.Text;

namespace DeepMind.ChineseChess
{
    public enum CChessStatus
    {
        None = 0,
        AtStart = 1, //開局
        Proceeding, //平穩進行 
        RedCatch, //紅方捉子
        BlackCatch, //黑方捉子
        RedCheckmate, //紅方將軍
        BlackCheckmate, //黑方將軍
        RedWin, //紅方獲勝
        BlackWin, //黑方獲勝
        Draw //平手
    }

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
                            case 'B':
                            case 'b':
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
                            case 'G':
                            case 'g':
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
                            case 'M':
                            case 'm':
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
                            case 'X':
                            case 'x':
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
                            case 'S':
                            case 's':
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

        }

        public void Initialize()
        {
            CurrentBoard = new CChessBoard();
        }

        public void Start()
        {
            if(Status == CChessStatus.None)
                Status = CChessStatus.AtStart;
            CurrentLegalMove = GetLegalMoves();
        }

        public void Restart(CChessBoard board = null)
        {
            Initialize();
            if (board != null)                
                CurrentBoard  = board;            
            Start();
        }

        public static CChessStatus CheckResult(CChessBoard ccb)
        {
            if (ccb == CChessBoard.StartingBoard)
                return CChessStatus.AtStart;

            GetLegalMoves(ccb);

            return CChessStatus.Proceeding;
        }


    }
}
