using System;

namespace CChessEngine
{
    //AI思考步驟
    //1.取得此盤面的Data(沒有設置新盤面)    
    //2.參考盤面的走法得分及勝率決定下法
    //3.有勝負之後更新盤面分

    //內崁Deep Learning
    class Program
    {
        static void Main(string[] args)
        {
            CChessBoard board = new CChessBoard();
            var a = CChessSystem.GetLegalMoves(board);
            Console.Write(board.PrintBoard());
            Console.Write(CChessSystem.PrintChineseMoveString(board, a[0]));
        }
    }
}
