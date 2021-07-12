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

        public void ClearRecords()
        {
            string[] files = Directory.GetFiles(BoardDataFolderPath);
            for (int i = 0; i < files.Length; i++)
                File.Delete(files[i]);
            files = Directory.GetFiles(MoveDataFolderPath);
            for (int i = 0; i < files.Length; i++)
                File.Delete(files[i]);
        }

        public bool RecordARandomGame()
        {
            ChaosBox cb = new ChaosBox();
            ChessGameSystem.Restart();
            GameResult gr = GameResult.None;
            List<string> boardsrecords = new List<string>();

            while (gr == GameResult.None)
            {
                List<IChessMove> moves = ChessGameSystem.GetNextMoves();
                
                //Choice Move            
                int choiceIndex = cb.DrawOutByte(0, (byte)(moves.Count - 1));
                //Choice Move End
                gr = ChessGameSystem.Move(moves[choiceIndex]);
                boardsrecords.Add(ChessGameSystem.GetCurrentBoard().ToString());
                //Console.WriteLine(boardsrecords[boardsrecords.Count - 1].ToString());
            }

            List<IChessMove> moverecords = ChessGameSystem.GetMoveRecords();
            StringBuilder fileName = new StringBuilder();            
            for (int i = 0; i < 9 && i < moverecords.Count; i++)
                fileName.Append(moverecords[i].ToString());
            string fullFileName = Path.Combine(MoveDataFolderPath, $"{fileName}.txt");
            
            using (FileStream fs = new FileStream(fullFileName, FileMode.Append))
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

            for(int i = 0; i < moverecords.Count; i++)
            {
                fileName.Clear();
                fileName.Append(boardsrecords[i]);                
                fullFileName = Path.Combine(BoardDataFolderPath, $"{fileName}.txt");
                int p1wins = 0, p2wins = 0, draws = 0;
                if (File.Exists(fullFileName))
                {
                    using (FileStream fs = new FileStream(fullFileName, FileMode.Open))
                    {
                        StreamReader sr = new StreamReader(fs);
                        p1wins = Convert.ToInt32(sr.ReadLine());
                        p2wins = Convert.ToInt32(sr.ReadLine());
                        draws = Convert.ToInt32(sr.ReadLine());
                        sr.Close();
                    }
                }

                if (gr == GameResult.Player1Win)
                    p1wins++;
                else if (gr == GameResult.Player2Win)
                    p2wins++;
                else if (gr == GameResult.Draw)
                    draws++;

                using (FileStream fs = new FileStream(fullFileName, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(p1wins);
                    sw.WriteLine(p2wins);
                    sw.WriteLine(draws);
                    sw.Flush();
                    sw.Close();
                }
            }

            return true;
        }        
    }
}
