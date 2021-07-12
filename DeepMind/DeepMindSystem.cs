using System;
using System.Collections.Generic;
using System.Text;

namespace DeepMind
{
    public class DeepMindSystem
    {
        public string MoveDataFolderPath { get; set; }
        public string BoardDataFolderPath { get; set; }
        IChessGameSystem ChessGameSystem { get; set; }
        public DeepMindSystem(IChessGameSystem chessGameSystem)
        {
            MoveDataFolderPath = Constant.MoveDataPath;
            BoardDataFolderPath = Constant.BoardDataPath;
            ChessGameSystem = chessGameSystem;
        }

        public bool RecordAGame()
        {
            ChessGameSystem.Restart();
            List<IChessMove> moves = ChessGameSystem.GetNextMoves();
            
            return true;
        }

        //public void StartNode()
        //{

        //}
        //public void ExpandNode()
        //{

        //}
    }
}
