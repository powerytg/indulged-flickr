using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda
{
    public class Anaconda
    {
        // API core
        private static AnacondaCore anacondaCore;

        // "per_page" parameter
        public const int DefaultItemsPerPage = 20;

        public static AnacondaCore AnacondaCore
        {
            get
            {
                if (anacondaCore == null)
                {
                    anacondaCore = new AnacondaCore();
                }

                return anacondaCore;
            }
        }

        // Constructor
        private Anaconda()
        {
        }

    }
}
