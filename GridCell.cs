using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace MineSweeper
{
    public class GridCell
    {
        public bool flagged = false;
        public bool explored = false;
        public int value = 0;

        public GridCell() {

        }
    }
}
