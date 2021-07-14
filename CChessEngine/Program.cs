using System;
using System.Text;

namespace CChessEngine
{
    
    class Program
    {
        static void Main(string[] args)
        {
            CChessBoard board = new CChessBoard();
            board.IsBlackTurn = true;
            //var a = CChessSystem.GetLegalMoves(board);
            //Console.OutputEncoding = Encoding.UTF8;
            //Console.Write(board.PrintBoard());
            //for(int i = 0; i < a.Count; i++)
            //{
            //    Console.WriteLine(CChessSystem.PrintChineseMoveString(board, a[i], true));
            //    //Console.WriteLine(CChessSystem.PrintMoveString(board, a[i]));
            //}
            CChessAI ai = new CChessAI();
            Console.WriteLine("Complete");
            Console.ReadKey();
        }
    }
}
