using System;
using System.Text;

namespace CChessEngine
{
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            CChessBoard board = new CChessBoard();
            board.IsBlackTurn = true;
            //var a = CChessSystem.GetLegalMoves(board);
            
            //Console.Write(board.PrintBoard());
            //for(int i = 0; i < a.Count; i++)
            //{
            //    Console.WriteLine(CChessSystem.PrintChineseMoveString(board, a[i], true));
            //    //Console.WriteLine(CChessSystem.PrintMoveString(board, a[i]));
            //}
            CChessAI ai = new CChessAI();
            CChessMoveData bestMove;
            CChessBoard oldBoard = new CChessBoard(ai.CurrentBoardNode.Board);
            Console.WriteLine(oldBoard.PrintBoard());
            Console.ReadKey();
            
            ai.Move(1, out bestMove, out _);

            Console.WriteLine($"AI Move: {CChessSystem.PrintChineseMoveString(oldBoard, bestMove.Move)}");
            Console.WriteLine(ai.CurrentBoardNode.Board.PrintBoard());

            Console.WriteLine("End");
            Console.ReadKey();
        }
    }
}
