using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morales_Cohen_Sutherland
{
    class CLine
    {
        public CPoint P1 { get; set; }
        public CPoint P2 { get; set; }

        public CLine(CPoint p1, CPoint p2)
        {
            P1 = p1;
            P2 = p2;
        }
    }
}
