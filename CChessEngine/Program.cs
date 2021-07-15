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
            CChessAI ai = new CChessAI(null, true, true);
            CChessMoveData bestMove;
            CChessBoard oldBoard = new CChessBoard(ai.CurrentBoardNode.Board);
            Console.WriteLine(oldBoard.PrintBoard());            
            Console.WriteLine("開打");
            Console.ReadKey();
            for (int i = 0; i < 100; i++)
            {
                ai.Move(10, out bestMove, out _);                
                //Console.WriteLine($"AI Move: {CChessSystem.PrintMoveString(oldBoard, bestMove.Move)}");
                Console.WriteLine(ai.CurrentBoardNode.Board.PrintBoard());
                Console.WriteLine($"AI: {CChessSystem.PrintChineseMoveString(oldBoard, bestMove.Move)}");
                Console.WriteLine($"AI評分: {ai.CurrentBoardNode.CChessScore}");
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
