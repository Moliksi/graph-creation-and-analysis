using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace GRAF
{
    class Edge
    {
        public int v1, v2;
       public Pen color;
        public Edge(int v1, int v2, Pen color)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.color = color;
        }
    }
}
