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

    public class CChessBoard
    {
        private const string ExceptionString = "\"{0}\" is not a valid Chinese Chess board data string.";
        private const string ChessLetters = "KSXGMBPksxgmbp ";

        protected char[][] _Data;

        public char[][] Data
        {
            get => _Data;
            set
            {   
                if(value.Length != 10)
                    throw new ArgumentException(string.Format(ExceptionString, "This"));
                for(int i = 0; i < 10; i++)
                {
                    if (value[i].Length != 9)
                        throw new ArgumentException(string.Format(ExceptionString, "This"));
                    for(int j = 0; j < 9; j++)
                        if(!ChessLetters.Contains(value[i][j]))
                            throw new ArgumentException(string.Format(ExceptionString, "This"));
                }
                _Data = value;
            }
        }

        public void SetBoardWithoutCheck(char[][] value)
        {
            _Data = value;
        }

        public void LoadBoardString(string value)
        {   
            if (!value.Contains('/'))
                throw new ArgumentException(string.Format(ExceptionString, value));
            string[] buffer = value.Split('/');
            if (buffer.Length != 10)
                throw new ArgumentException(string.Format(ExceptionString, value));

            char[][] result = new char[10][];
            for(int i = 0; i < 10; i++)
            {
                result[i] = new char[9];
                int p = 0;
                for (int j = 0; j < buffer[i].Length; j++)
                {   
                    if (Char.IsDigit(buffer[i], j))
                    {
                        int m = Convert.ToInt32(buffer[i][j]);
                        if (m == 0)
                            throw new ArgumentException(string.Format(ExceptionString, value));
                        for (int k = 0; k < m; k++)
                            result[i][p++] = ' ';
                    }
                    else
                    {
                        if(!ChessLetters.Contains(buffer[i][j]))
                            throw new ArgumentException(string.Format(ExceptionString, value));
                        result[i][p++] = buffer[i][j];
                    }   
                }
            }            
            _Data = result;
        }

        public string PrintBoardString()
        {
            StringBuilder result = new StringBuilder();
            for(int i = 0; i < 10; i++)
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
                result.Append('/');
            }
            result.Remove(result.Length - 1, 1);
            return result.ToString();
        }

    }
}
