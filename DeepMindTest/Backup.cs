using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeepMind;
using DeepMind.ChineseChess;
using System;
using Aritiafel.Characters.Heroes;

namespace DeepMindTest
{
    [TestClass]
    public class Backup
    {
        [TestMethod]
        public void BackupProject()
        {
            Tina.SaveProject(ProjectChoice.DeepMind);
        }

        public void BackupProjectData()
        {
            Tina.SaveProjectData(ProjectChoice.DeepMind);            
        }

    }
}
