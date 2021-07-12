using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeepMind;
using DeepMind.ChineseChess;
using System;
using System.Threading.Tasks;

namespace DeepMindTest
{
    [TestClass]
    public class DeepMindTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void ClearRecords()
        {
            DeepMindSystem dms = new DeepMindSystem(new CChessSystem());
            dms.ClearRecords();
        }

        [TestMethod]
        public void RecordARandomGame()
        {
            DeepMindSystem dms = new DeepMindSystem(new CChessSystem());
            dms.RecordARandomGame();
        }

        [TestMethod]
        public void NRecord100RandomGames()
        {
            for(int i = 0; i < 100; i++)
            {
                DeepMindSystem dms1 = new DeepMindSystem(new CChessSystem());
                dms1.RecordARandomGame();
            }
                
        }

        [TestMethod]
        public void Record100RandomGames()
        {
            Task t1 = new Task(() => {}), 
                t2 = new Task(() => { }), 
                t3 = new Task(() => { }),
                t4 = new Task(() => { });
            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();
            for (int i = 0; i < 25; i++)
            {                
                while (t1.Status != TaskStatus.RanToCompletion)
                    ;
                DeepMindSystem dms1 = new DeepMindSystem(new CChessSystem());
                t1 = new Task(() => dms1.RecordARandomGame());
                t1.Start();

                while (t2.Status != TaskStatus.RanToCompletion)
                    ;
                DeepMindSystem dms2 = new DeepMindSystem(new CChessSystem());
                t2 = new Task(() => dms2.RecordARandomGame());
                t2.Start();

                while (t3.Status != TaskStatus.RanToCompletion)
                    ;
                DeepMindSystem dms3 = new DeepMindSystem(new CChessSystem());
                t3 = new Task(() => dms3.RecordARandomGame());
                t3.Start();

                while (t4.Status != TaskStatus.RanToCompletion)
                    ;
                DeepMindSystem dms4 = new DeepMindSystem(new CChessSystem());
                t4 = new Task(() => dms4.RecordARandomGame());
                t4.Start();
            }
        }
    }
}
