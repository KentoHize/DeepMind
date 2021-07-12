using System;
using System.Collections.Generic;
using System.Text;
using Aritiafel.Artifacts;
using System.IO;
using System.Threading.Tasks;

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

        public bool RecordARandomGame()
        {
            ChaosBox cb = new ChaosBox();
            ChessGameSystem.Restart();
            GameResult gr = GameResult.None;
            List<IChessBoard> boards = new List<IChessBoard>();

            while (gr == GameResult.None)
            {
                List<IChessMove> moves = ChessGameSystem.GetNextMoves();
                
                //Choice Move            
                int choiceIndex = cb.DrawOutByte(0, (byte)(moves.Count - 1));
                //Choice Move End
                gr = ChessGameSystem.Move(moves[choiceIndex]);
                boards.Add(ChessGameSystem.GetCurrentBoard());
            }

            List<IChessMove> moverecords = ChessGameSystem.GetMoveRecords();
            StringBuilder fileName = new StringBuilder();
            for(int i = 0; i < 9 && i < moverecords.Count; i++)
                fileName.Append(moverecords[i].ToString());
            using (FileStream fs = new FileStream(Path.Combine(MoveDataFolderPath, $"{fileName}.txt"), FileMode.Append))
            {
                StreamWriter sw = new StreamWriter(fs);
                for (int i = 0; i < moverecords.Count; i++)
                    sw.Write($"{moverecords[i]} ");
                if (gr == GameResult.Player1Win)
                    sw.Write("1-0");
                else if (gr == GameResult.Player2Win)
                    sw.Write("0-1");
                else
                    sw.Write("1/2-1/2");
                sw.Flush();
                sw.Close();
            }

            //fileName.Clear();
            //fileName = 
            //using (FileStream fs = new FileStream(Path.Combine(BoardDataFolderPath, $"{fileName}.txt"), FileMode.Append))
            //{
            //}

            return true;
        }        
    }
}
