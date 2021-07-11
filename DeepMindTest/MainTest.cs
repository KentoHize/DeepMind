using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeepMind;
using DeepMind.ChineseChess;

namespace DeepMindTest
{
    [TestClass]
    public class MainTest
    {   
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void LoadBoardTest()
        {
            CChessBoard ccb = new CChessBoard();            
            //TestContext.WriteLine(ccb.PrintBoardString());
            TestContext.WriteLine(ccb.PrintBoard());


        }

        [TestMethod]
        public void GetLegalMoveTest()
        {
            CChessSystem ccs = new CChessSystem();
            var a = ccs.GetLegalMoves();
            for (int i = 0; i < a.Count; i++)
                TestContext.WriteLine(a[i].ToTestString());
        }
    }
}

