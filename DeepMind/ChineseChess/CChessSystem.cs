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
                                //四方向 - 打和移動
                                bool isRun = true;
                                for (int k = i - 1; k >= 0; k--)
                                { 
                                    if (isRun && CurrentBoard[k, j] == ' ')
                                        result.Add(new CChessMove(CurrentBoard[i, j], i, j, k, j));
                                    else if (isRun)
                                        isRun = false;
                                    else if (CurrentBoard[k, j] == ' ')
                                        continue;
                                    else 
                                    {
                                        if (PieceLetters[1 - player].Contains(CurrentBoard[k, j]))
                                            result.Add(new CChessMove(CurrentBoard[i, j], i, j, k, j));
                                        break;
                                    }
                                }
                                isRun = true;
                                for (int k = i + 1; k < 9; k++)
                                {
                                    if (isRun && CurrentBoard[k, j] == ' ')
                                        result.Add(new CChessMove(CurrentBoard[i, j], i, j, k, j));
                                    else if (isRun)
                                        isRun = false;
                                    else if (CurrentBoard[k, j] == ' ')
                                        continue;
                                    else
                                    {
                                        if (PieceLetters[1 - player].Contains(CurrentBoard[k, j]))
                                            result.Add(new CChessMove(CurrentBoard[i, j], i, j, k, j));
                                        break;
                                    }
                                }
                                isRun = true;
                                for (int k = j - 1; k >= 0; k--)
                                {
                                    if (isRun && CurrentBoard[i, k] == ' ')
                                        result.Add(new CChessMove(CurrentBoard[i, j], i, j, i, k));
                                    else if (isRun)
                                        isRun = false;
                                    else if (CurrentBoard[i, k] == ' ')
                                        continue;
                                    else
                                    {
                                        if (PieceLetters[1 - player].Contains(CurrentBoard[i, k]))
                                            result.Add(new CChessMove(CurrentBoard[i, j], i, j, i, k));
                                        break;
                                    }
                                }
                                isRun = true;
                                for (int k = j + 1; k < 10; k++)
                                {
                                    if (isRun && CurrentBoard[i, k] == ' ')
                                        result.Add(new CChessMove(CurrentBoard[i, j], i, j, i, k));
                                    else if (isRun)
                                        isRun = false;
                                    else if (CurrentBoard[i, k] == ' ')
                                        continue;
                                    else
                                    {
                                        if (PieceLetters[1 - player].Contains(CurrentBoard[i, k]))
                                            result.Add(new CChessMove(CurrentBoard[i, j], i, j, i, k));
                                        break;
                                    }
                                }
                                break;
                            case 'G':
                            case 'g':
                                break;
                            case 'M':
                            case 'm':
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
