using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CChessEngine
{
    public class CChessMoveData
    {
        public CChessMove Move { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public CChessBoardNode BoardNode { get; set; }
        public CChessMoveData()
            : this(null)
        { }

        public CChessMoveData(CChessMove move, CChessBoardNode ccbn = null)
        {
            if (move == null)
                return;
            Move = move;
            if(ccbn != null)
                BoardNode = ccbn;
        }
    }

    public static partial class Extension
    {
        public static List<CChessMoveData> ToMoveDataList(this List<CChessMove> movelist)
        {
            if (movelist == null)
                return null;
            List<CChessMoveData> result = new List<CChessMoveData>();
            for (int i = 0; i < movelist.Count; i++)
                result.Add(new CChessMoveData(movelist[i]));
            return result;
        }

        public static List<CChessMove> ToMoveList(this List<CChessMoveData> moveDataList)
        {
            if (moveDataList == null)
                return null;
            List<CChessMove> result = new List<CChessMove>();
            for (int i = 0; i < moveDataList.Count; i++)
                result.Add(new CChessMove(moveDataList[i].Move));
            return result;
        } 
    }
}
