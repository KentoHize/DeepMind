using System;
using System.Collections.Generic;
using System.Text;

namespace DeepMind.ChineseChess
{
    public enum CChessStatus
    {
        None = 0,
        Start = 1,
        Proceeding = 2,
        RedWin = 2,
        BlackWin = 3,
        Draw = 4
    }

    public class CChessSystem
    {
        //protected const string RedPieceLetters = "ksxgmbp";
        //protected const string BlackPieceLetters = "KSXGMBP";
        protected string[] PieceLetters => new string[] { "KSXGMBP", "ksxgmbp" };
        public CChessBoard CurrentBoard { get; set; }
        public List<CChessMove> MoveRecord { get; set; }
        public CChessStatus Status { get; set; }
        public CChessSystem()
        {
            CurrentBoard = new CChessBoard();
        }

        public List<CChessMove> LegalMoveList()
        {
            
            //搜尋每一個子
            byte player = CurrentBoard.IsBlackTurn ? (byte)1 : (byte)0;
            List<CChessMove> result = new List<CChessMove>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (PieceLetters[player].Contains(CurrentBoard[i, j]))
                    { 
                        switch(CurrentBoard[i, j])
                        {
                            case 'P':
                            case 'p':
                                if (player == 0)
                                {
                                    //未過河 - 紅
                                    if (j != 0 && !PieceLetters[player].Contains(CurrentBoard[i, j - 1]))
                                        result.Add(new CChessMove(CurrentBoard[i, j], i, j, i, j - 1));
                                }
                                else if(player == 1)                                
                                {
                                    //未過河 - 黑
                                    if (j != 9 && !PieceLetters[player].Contains(CurrentBoard[i, j + 1]))
                                        result.Add(new CChessMove(CurrentBoard[i, j], i, j, i, j + 1));
                                }
                                else if(player == 0 && j <= 4 || player == 1 && j >= 5)
                                {
                                    //已過河                                    
                                    if (i != 0 && !PieceLetters[player].Contains(CurrentBoard[i - 1, j]))
                                        result.Add(new CChessMove(CurrentBoard[i, j], i, j, i - 1, j));
                                    if (i != 8 && !PieceLetters[player].Contains(CurrentBoard[i + 1, j]))
                                        result.Add(new CChessMove(CurrentBoard[i, j], i, j, i + 1, j));
                                }
                                break;
                            case 'B':
                            case 'b':
                                bool isRun = true;
                                bool CheckStraightBao(int x, int y)
                                {                                    
                                    if (isRun && CurrentBoard[x, y] == ' ')
                                        result.Add(new CChessMove(CurrentBoard[i, j], i, j, x, y));
                                    else if (isRun)
                                        isRun = false;
                                    else if (CurrentBoard[x, y] != ' ')                                        
                                    {
                                        if (PieceLetters[1 - player].Contains(CurrentBoard[x, y]))
                                            result.Add(new CChessMove(CurrentBoard[i, j], i, j, x, y));
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
                                    if (CurrentBoard[x, y] == ' ')
                                        result.Add(new CChessMove(CurrentBoard[i, j], i, j, x, y));                                    
                                    else if (CurrentBoard[x, y] != ' ')
                                    {
                                        if (PieceLetters[1 - player].Contains(CurrentBoard[x, y]))
                                            result.Add(new CChessMove(CurrentBoard[i, j], i, j, x, y));
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
                                if (i > 0 && j > 1 && CurrentBoard[i, j - 1] == ' ' &&
                                    !PieceLetters[player].Contains(CurrentBoard[i - 1, j - 2]))
                                    result.Add(new CChessMove(CurrentBoard[i, j], i, j, i - 1, j - 2));
                                if (i > 1 && j > 0 && CurrentBoard[i - 1, j] == ' ' &&
                                    !PieceLetters[player].Contains(CurrentBoard[i - 2, j - 1]))
                                    result.Add(new CChessMove(CurrentBoard[i, j], i, j, i - 2, j - 1));
                                if (i > 1 && j < 9 && CurrentBoard[i - 1, j] == ' ' &&
                                    !PieceLetters[player].Contains(CurrentBoard[i - 2, j + 1]))
                                    result.Add(new CChessMove(CurrentBoard[i, j], i, j, i - 2, j + 1));
                                if (i > 0 && j < 8 && CurrentBoard[i, j + 1] == ' ' &&
                                    !PieceLetters[player].Contains(CurrentBoard[i - 1, j + 2]))
                                    result.Add(new CChessMove(CurrentBoard[i, j], i, j, i - 1, j + 2));
                                if (i < 9 && j < 8 && CurrentBoard[i, j + 1] == ' ' &&
                                    !PieceLetters[player].Contains(CurrentBoard[i + 1, j + 2]))
                                    result.Add(new CChessMove(CurrentBoard[i, j], i, j, i + 1, j + 2));
                                if (i < 8 && j < 9 && CurrentBoard[i + 1, j] == ' ' &&
                                    !PieceLetters[player].Contains(CurrentBoard[i + 2, j + 1]))
                                    result.Add(new CChessMove(CurrentBoard[i, j], i, j, i + 2, j + 1));
                                if (i < 8 && j > 0 && CurrentBoard[i + 1, j] == ' ' &&
                                    !PieceLetters[player].Contains(CurrentBoard[i + 2, j - 1]))
                                    result.Add(new CChessMove(CurrentBoard[i, j], i, j, i + 2, j - 1));
                                if (i < 9 && j > 1 && CurrentBoard[i, j - 1] == ' ' &&
                                    !PieceLetters[player].Contains(CurrentBoard[i + 1, j - 2]))
                                    result.Add(new CChessMove(CurrentBoard[i, j], i, j, i + 1, j - 2));
                                break;
                            case 'X':
                            case 'x':
                                break;
                            case 'S':
                            case 's':
                                break;
                            case 'K':
                            case 'k':
                                break;
                        }
                    }
                }
            }

            return result;
        }

        public void Move(CChessMove move)
        {

        }

        public void Restart(CChessBoard board = null)
        {
            if (board == null)
                board = new CChessBoard();
            CurrentBoard = board;
            Status = CChessStatus.Start;
        }

        public CChessStatus CheckResult()
        {
            return CChessStatus.Proceeding;
        }


    }
}
