using System;
using System.Collections.Generic;
using System.Text;

namespace DeepMind.ChineseChess
{
    public class CChessMove : IChessMove
    {
        public char Piece { get; set; }
        public byte X1 { get; set; }
        public byte Y1 { get; set; }
        public byte X2 { get; set; }
        public byte Y2 { get; set; }
        public CChessMove()
        { }

        public CChessMove(char piece, byte x1, byte y1, byte x2, byte y2)
        {
            Piece = piece;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public CChessMove(char piece, int x1, int y1, int x2, int y2)
            : this(piece, (byte)x1, (byte)y1, (byte)x2, (byte)y2)
        { }

        public override string ToString()
            => $"{(char)('a' + X1)}{Y1}{(char)('a' + X2)}{Y2}";

        //public string ToChineseString()
        //{
        //    StringBuilder result = new StringBuilder();

        //    if (Order == 0)
        //    {
        //        result.Append(CChessBoard.LetterToChineseWord[Piece]);
        //        result.Append(X1 + 1);
        //    }
        //    else
        //    {
        //        if (Order == 1)
        //            result.Append("前");
        //        else if(Order == 2)
        //            result.Append("後");
        //        else if (Order == 3)
        //            result.Append("一");
        //        else if (Order == 4)
        //            result.Append("二");
        //        else if(Order == 5)
        //            result.Append("三");
        //        else if (Order == 6)
        //            result.Append("四");
        //        else if (Order == 7)
        //            result.Append("五");
        //        result.Append(CChessBoard.LetterToChineseWord[Piece]);
        //    }
           
        //    if (Y2 > Y1)
        //    {
        //        result.Append("退");
        //        if (X1 == X2)
        //            result.Append(Y2 - Y1);
        //        else
        //            result.Append(X2 + 1);
        //    }                
        //    else if (Y2 == Y1)
        //    {
        //        result.Append("平");
        //        result.Append(X2 + 1);
        //    }                
        //    else if (Y2 < Y1)
        //    {
        //        result.Append("進");
        //        if (X1 == X2)
        //            result.Append(Y1 - Y2);
        //        else
        //            result.Append(X2 + 1);
        //    }
        //    return result.ToString();
        //}
        public string ToTestString()
            => $"{CChessBoard.LetterToChineseWord[Piece]} {X1},{Y1} => {X2},{Y2}";

        public override bool Equals(object obj)
        {
            if (!(obj is CChessMove ccm))
                return false;
            if (Piece == ccm.Piece && X1 == ccm.X1 && X2 == ccm.X2 && Y1 == ccm.Y1 && Y2 == ccm.Y2)
                return true;
            return false;
        }   

        public override int GetHashCode()
            => Piece.GetHashCode() ^ X1.GetHashCode() ^ X2.GetHashCode() ^ Y1.GetHashCode() ^ Y2.GetHashCode();
    }
}
