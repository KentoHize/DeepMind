using System;
using System.Text;
using System.Threading;

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
            CChessAI ai = new CChessAI(null, true, true);
            CChessMoveData bestMove;
            CChessBoard oldBoard = new CChessBoard(ai.CurrentBoardNode.Board);
            Console.WriteLine(oldBoard.PrintBoard());            
            Console.WriteLine("開打");
            Console.ReadKey();
            for (int i = 0; i < 100; i++)
            {
                ai.Move(2, out bestMove, out _);                
                //Console.WriteLine($"AI Move: {CChessSystem.PrintMoveString(oldBoard, bestMove.Move)}");
                Console.WriteLine(ai.CurrentBoardNode.Board.PrintBoard());
                Console.WriteLine($"AI: {CChessSystem.PrintChineseMoveString(oldBoard, bestMove.Move)}");
                Console.WriteLine($"AI評分: {ai.CurrentBoardNode.Score}");
                //Console.WriteLine(oldBoard.PrintBoardString());
                oldBoard = new CChessBoard(ai.CurrentBoardNode.Board);
                GC.Collect();
                Console.ReadKey();
            }
            

            ai.RecordGame();
            Console.WriteLine("End");
            Console.ReadKey();
            
        }
    }
}
