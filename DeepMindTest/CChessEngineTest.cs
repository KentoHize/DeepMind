using Microsoft.VisualStudio.TestTools.UnitTesting;
using CChessEngine;


namespace DeepMindTest
{
    [TestClass]
    public class CChessEngineTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test5Pawn()
        {
            //CChessBoard board = new CChessBoard("4k4/9/4P1P2/4PP3/4P4/9/9/9/9/5K3 w");
            CChessBoard board = new CChessBoard("4k4/9/4P1P2/4P1P2/4P4/9/9/9/9/5K3 w");
            var a = CChessSystem.GetLegalMoves(board);
            TestContext.WriteLine(board.PrintBoard());
            for (int i = 0; i < a.Count; i++)
            {
                TestContext.WriteLine(CChessSystem.PrintChineseMoveString(board, a[i]));                
            }            
        }
    }
}
