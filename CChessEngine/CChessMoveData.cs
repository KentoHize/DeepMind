using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;

namespace CChessEngine
{
    public class CChessMoveData : ICloneable, IComparer<CChessMoveData>, IComparable<CChessMoveData>
    {
        public CChessMove Move { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public CChessBoardNode BoardNode { get; set; }
        public CChessMoveData()
            : this(null)
        { }

        public CChessMoveData(CChessMove move, CChessBoardNode ccbn = null)
        {
            Move = move;
            if(ccbn != null)
                BoardNode = ccbn;
        }

        public CChessMoveData(CChessMoveData moveData)
        {
            Move = moveData.Move;
            BoardNode = (CChessBoardNode)moveData.BoardNode.Clone();
        }

        public object Clone()
            => new CChessMoveData(this);

        public int Compare([AllowNull] CChessMoveData x, [AllowNull] CChessMoveData y)
        {
            if (y == null)
                return 1;
            else if (x == null)
                return -1;            
            else if (x.BoardNode == null && y.BoardNode == null)
                return x.Move.CompareTo(y.Move);
            else if (x.BoardNode == null)
                return y.BoardNode.CChessScore > CChessBoardNode.DefaultScore ? -1 : 1;
            else if (y.BoardNode == null)
                return x.BoardNode.CChessScore > CChessBoardNode.DefaultScore ? 1 : -1;
            else if (x.BoardNode.CChessScore == y.BoardNode.CChessScore)
                return x.Move.CompareTo(y.Move);
            else
                return x.BoardNode.CChessScore > y.BoardNode.CChessScore ? 1 : -1;
        }

        public int CompareTo([AllowNull] CChessMoveData other)
            => Compare(this, other);
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
            foreach(CChessMoveData cmd in moveDataList)
                result.Add(new CChessMove(cmd.Move));
            return result;
        } 
    }
}
