    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Morales_Cohen_Sutherland
    {
        class CCohenSutherland
        {
            private const int INSIDE = 0; // 0000
            private const int LEFT = 1;   // 0001
            private const int RIGHT = 2;  // 0010
            private const int BOTTOM = 4; // 0100
            private const int TOP = 8;    // 1000

            public Rectangle ClippingWindow { get; }

            public CCohenSutherland(Rectangle window)
            {
                ClippingWindow = window;
            }

            private int ComputeOutCode(int x, int y)
            {
                int code = INSIDE;

                if (x < ClippingWindow.Left)
                    code |= LEFT;
                else if (x > ClippingWindow.Right)
                    code |= RIGHT;
                if (y < ClippingWindow.Top)
                    code |= TOP;
                else if (y > ClippingWindow.Bottom)
                    code |= BOTTOM;

                return code;
            }

            public bool ClipLine(ref CPoint p1, ref CPoint p2)
            {
                int x0 = p1.X, y0 = p1.Y;
                int x1 = p2.X, y1 = p2.Y;

                int outcode0 = ComputeOutCode(x0, y0);
                int outcode1 = ComputeOutCode(x1, y1);

                bool accept = false;

                while (true)
                {
                    if ((outcode0 | outcode1) == 0)
                    {
                        accept = true;
                        break;
                    }
                    else if ((outcode0 & outcode1) != 0)
                    {
                        break;
                    }
                    else
                    {
                        int outcodeOut = (outcode0 != 0) ? outcode0 : outcode1;

                        int x = 0, y = 0;

                        if ((outcodeOut & TOP) != 0)
                        {
                            x = x0 + (x1 - x0) * (ClippingWindow.Top - y0) / (y1 - y0);
                            y = ClippingWindow.Top;
                        }
                        else if ((outcodeOut & BOTTOM) != 0)
                        {
                            x = x0 + (x1 - x0) * (ClippingWindow.Bottom - y0) / (y1 - y0);
                            y = ClippingWindow.Bottom;
                        }
                        else if ((outcodeOut & RIGHT) != 0)
                        {
                            y = y0 + (y1 - y0) * (ClippingWindow.Right - x0) / (x1 - x0);
                            x = ClippingWindow.Right;
                        }
                        else if ((outcodeOut & LEFT) != 0)
                        {
                            y = y0 + (y1 - y0) * (ClippingWindow.Left - x0) / (x1 - x0);
                            x = ClippingWindow.Left;
                        }

                        if (outcodeOut == outcode0)
                        {
                            x0 = x;
                            y0 = y;
                            outcode0 = ComputeOutCode(x0, y0);
                        }
                        else
                        {
                            x1 = x;
                            y1 = y;
                            outcode1 = ComputeOutCode(x1, y1);
                        }
                    }
                }

                if (accept)
                {
                    p1 = new CPoint(x0, y0);
                    p2 = new CPoint(x1, y1);
                }

                return accept;
            }
            }
    }
