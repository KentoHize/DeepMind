using System;
using System.Collections.Generic;
using System.Text;

namespace CChessEngine
{
    public class CChessMoveData
    {
        public CChessMove Move { get; set; }
        public CChessBoardNode NextBoard { get; set; }
    }
}
