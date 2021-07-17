using System;
using System.Collections.Generic;
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
            List<CChessMoveData> progress;
            CChessBoard oldBoard = new CChessBoard(ai.CurrentBoardNode.Board);
            Console.WriteLine(oldBoard.PrintBoard());            
            Console.WriteLine("開打");
            Console.ReadKey();
            for (int i = 0; i < 100; i++)
            {
                ai.Move(10, out bestMove, out progress);
                //Console.WriteLine($"AI Move: {CChessSystem.PrintMoveString(oldBoard, bestMove.Move)}");
                Console.WriteLine(ai.CurrentBoardNode.Board.PrintBoard());
                Console.WriteLine($"AI: {CChessSystem.PrintChineseMoveString(oldBoard, bestMove.Move)}");
                Console.WriteLine($"AI評分: {ai.CurrentBoardNode.CChessScore}");
                Console.Write($"AI預估結果:");
                for (int j = 1; j < progress.Count; j++)
                    Console.Write($"{CChessSystem.PrintChineseMoveString(progress[j].BoardNode.Parent.Board, progress[j].Move)} ");
                Console.WriteLine();
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
