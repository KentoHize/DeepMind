using System;
using System.Collections.Generic;
using System.Text;

namespace DeepMind.ChineseChess
{
    //9 x 10棋盤
    //Piece letters
    //將 帥 K k
    //士 仕 S s
    //象 相 X x
    //車 俥 G g
    //馬 傌 M m
    //包 炮 B b
    //卒 兵 P p

    //Move
    //炮2平5(炮在h8) h8 e8
    //馬8進7(馬在h1) h1 g3

    //1
    //2
    //3
    //4
    //5
    //6
    //7
    //8
    //9
    //10
    //  a b c d e f g h i

    //BoardString
    //A. 盤面
    //B. 輪誰
    //C. 紅方捉數
    //D. 紅方將數
    //E. 黑方捉數
    //F. 黑方將數
    //G. 閒步數
    //H. 總步數

    //主要為A B

    public class CChessBoard
    {
        private const string ExceptionString = "\"{0}\" is not a valid Chinese Chess board data string.";
        private const string ChessLetters = "KSXGMBPksxgmbp ";
        public const string StartingBoardString = "GMXSKSXMG/9/1B5B1/P1P1P1P1P/9/9/p1p1p1p1p/1b5b1/9/gmxsksxmg r";
        public static Dictionary<char, char> LetterToChineseWord => new Dictionary<char, char>()
        { { 'K', '將' }, { 'S', '士' }, { 'X', '象' }, { 'M', '馬' }, { 'G', '車' }, { 'B', '包' }, { 'P', '卒' },
          { 'k', '帥' }, { 's', '仕' }, { 'x', '相' }, { 'm', '傌' }, { 'g', '俥' }, { 'b', '炮' }, { 'p', '兵' },
          { ' ', '　'} };

        protected char[][] _Data;
        public bool IsBlackTurn { get; set; }
        public int[] CatchCount { get; protected set; } = new int[2];
        public int[] CheckmateCount { get; protected set; } = new int[2];
        public int HalfMoveCount { get; set; }
        public int TotalMoveCount { get; set; }
        public CChessBoard(string boardString = StartingBoardString)
            => LoadBoardString(boardString);

        public char[][] Data
        {
            get => _Data;
            set
            {
                if (value.Length != 10)
                    throw new ArgumentException(string.Format(ExceptionString, "This"));
                for (int i = 0; i < 10; i++)
                {
                    if (value[i].Length != 9)
                        throw new ArgumentException(string.Format(ExceptionString, "This"));
                    for (int j = 0; j < 9; j++)
                        if (!ChessLetters.Contains(value[i][j]))
                            throw new ArgumentException(string.Format(ExceptionString, "This"));
                }
                _Data = value;
            }
        }
        
        private void Clear()
        {
            _Data = null;
            IsBlackTurn = false;
            CatchCount[0] = CatchCount[1] =
            CheckmateCount[0] = CheckmateCount[1] =
            HalfMoveCount = TotalMoveCount = 0;
        }

        public void SetBoardWithoutCheck(char[][] value, bool isBlackTurn = false)
        {
            Clear();
            _Data = value;
            IsBlackTurn = isBlackTurn;
        }

        public void LoadBoardString(string value)
        {
            Clear();
            string[] buffer, buffer2;
            if (!value.Contains(' '))
                throw new ArgumentException(string.Format(ExceptionString, value));
            buffer = value.Split(' ');

            if(buffer.Length != 2 && buffer.Length != 8)
                throw new ArgumentException(string.Format(ExceptionString, value));
            if (!buffer[0].Contains('/'))
                throw new ArgumentException(string.Format(ExceptionString, value));
            buffer2 = buffer[0].Split('/');
            if (buffer2.Length != 10)
                throw new ArgumentException(string.Format(ExceptionString, value));

            char[][] data = new char[10][];
            for (int i = 0; i < 10; i++)
            {
                data[i] = new char[9];
                int p = 0;
                for (int j = 0; j < buffer2[i].Length; j++)
                {
                    if (Char.IsDigit(buffer2[i], j))
                    {
                        int m = Convert.ToInt32(buffer2[i][j].ToString());
                        if (m == 0)
                            throw new ArgumentException(string.Format(ExceptionString, value));
                        for (int k = 0; k < m; k++)
                            data[i][p++] = ' ';                            
                    }
                    else
                    {
                        if (!ChessLetters.Contains(buffer2[i][j]))
                            throw new ArgumentException(string.Format(ExceptionString, value));
                        data[i][p++] = buffer2[i][j];
                    }
                }
            }
            _Data = data;

            if (buffer[1] == "r")
                IsBlackTurn = false;
            else if (buffer[1] == "b")
                IsBlackTurn = true;
            else
                throw new ArgumentException(string.Format(ExceptionString, value));

            if (buffer.Length == 8)
            {
                //Full Detail Mode
                CatchCount[0] = Convert.ToInt32(buffer[2]);
                CheckmateCount[0] = Convert.ToInt32(buffer[3]);
                CatchCount[1] = Convert.ToInt32(buffer[4]);
                CheckmateCount[1] = Convert.ToInt32(buffer[5]);
                HalfMoveCount = Convert.ToInt32(buffer[6]);
                TotalMoveCount = Convert.ToInt32(buffer[7]);
            }
        }

        public string PrintBoardString()
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                int emptyCount = 0;
                for (int j = 0; j < 9; j++)
                {
                    if (_Data[i][j] == ' ')
                        emptyCount++;
                    else
                    {
                        if (emptyCount != 0)
                        {
                            result.Append(emptyCount);
                            emptyCount = 0;
                        }
                        result.Append(_Data[i][j]);
                    }
                }
                if (emptyCount != 0)
                    result.Append(emptyCount);
                result.Append('/');
            }
            result.Remove(result.Length - 1, 1);
            result.Append(' ');
            result.Append(IsBlackTurn ? 'b' : 'r');
            if(CatchCount[0] == 0 && CatchCount[1] == 0 &&
               CheckmateCount[0] == 0 && CheckmateCount[1] == 0 &&
               HalfMoveCount == 0 && TotalMoveCount == 0)
                return result.ToString();
            result.AppendFormat(" {0} {1} {2} {3} {4} {5}", CatchCount[0], CheckmateCount[0],
                CatchCount[1], CheckmateCount[1], HalfMoveCount, TotalMoveCount);            
            return result.ToString();
        }

        public string PrintBoard(bool displayPlayerTurn = true)
        {
            StringBuilder result = new StringBuilder();
            for(int i = 0; i < 10; i++)
            {            
                for(int j = 0; j < 9; j++)
                {
                    result.Append(LetterToChineseWord[_Data[i][j]]);
                }
                result.Append('\n');
            }
            if (displayPlayerTurn)
                result.AppendFormat("下一步：{0}方\n", IsBlackTurn ? "黑" : "紅");            
            return result.ToString();
        }

    }
}
