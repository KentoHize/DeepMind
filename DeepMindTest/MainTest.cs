using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeepMind;
using DeepMind.ChineseChess;
using System;

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
            ccb.HalfMoveCount = 3;
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

        [TestMethod]
        public void GameAndPlay()
        {
            CChessSystem ccs = new CChessSystem();
            ccs.Start();
            CChessMove c8h1 = new CChessMove('C', 7, 2, 1, 2);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => ccs.Move(c8h1, true));

            CChessMove C8h5 = new CChessMove('C', 7, 2, 4, 2);
            ccs.Move(C8h5);

            CChessMove n2a3 = new CChessMove('n', 7, 9, 6, 7);
            ccs.Move(n2a3);

            CChessMove N8a7 = new CChessMove('N', 7, 0, 6, 2);
            ccs.Move(N8a7);

            CChessMove r1h2 = new CChessMove('r', 8, 9, 7, 9);
            ccs.Move(r1h2);

            CChessMove R9h8 = new CChessMove('R', 8, 0, 7, 0);
            ccs.Move(R9h8);
            TestContext.WriteLine(ccs.ToTestString());
        }

        //[TestMethod]
        //public void Real()
        //{

        //}
    }
}

