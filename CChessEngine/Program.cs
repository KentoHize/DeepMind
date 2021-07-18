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
            Console.WriteLine("起始局面");
            Console.WriteLine();
            Console.ReadKey();
            while(true)
            {
                MoveStatus ms = ai.Move(100, 5, out bestMove, out progress);
                Console.WriteLine(ai.CurrentBoardNode.Board.PrintBoard());
                if (ms == MoveStatus.Resign)
                {
                    Console.Write($"AI:認輸");
                    if(ai.CurrentBoardNode.Board.IsBlackTurn)
                        Console.Write($"恭喜紅方獲勝");
                    else
                        Console.Write($"恭喜黑方獲勝");
                    break;
                }
                else
                {
                    Console.WriteLine($"AI: {CChessSystem.PrintChineseMoveString(oldBoard, bestMove.Move)}");
                    Console.WriteLine($"AI評分: {ai.CurrentBoardNode.CChessScore}");
                    Console.Write($"AI預估結果:");
                    for (int j = 1; j < progress.Count; j++)
                        Console.Write($"{CChessSystem.PrintChineseMoveString(progress[j].BoardNode.Parent.Board, progress[j].Move)} ");
                    Console.WriteLine();

                    oldBoard = new CChessBoard(ai.CurrentBoardNode.Board);
                    GC.Collect();
                    Console.ReadKey();
                }



            }


            ai.RecordGame();
            Console.WriteLine("End");
            Console.ReadKey();

        }
    }
}
