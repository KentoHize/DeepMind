using System;
using System.Collections.Generic;
using System.Text;

namespace DeepMind.ChineseChess
{
    public class CChessMove
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
            => $"{(char)('a' + X1)}{Y1 + 1} {(char)('a' + X2)}{Y2 + 1}";

        public string ToChineseString()
        {
            return "";
        }

        public string ToTestString()
        {
            return $"{CChessBoard.LetterToChineseWord[Piece]} {X1},{Y1} => {X2},{Y2}";
        }
    }
}
