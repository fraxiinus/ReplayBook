using System;
using System.Collections.Generic;
using System.Text;

namespace Rofl.Files
{
    /// <summary>
    /// Acts as the cache for replay parsing.
    /// </summary>
    public class DatabaseRepository
    {
        // Program flow will be like this:
        // Open program -> check folders for new files to parse -> send files to rofl.reader 
        // -> add results to database -> notify ..something? that there are new entries to display
    }
}
