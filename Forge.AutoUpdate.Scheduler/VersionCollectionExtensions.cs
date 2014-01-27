using System;
using System.Collections.Generic;
using System.Linq;

namespace Forge.AutoUpdate.Scheduler
{
    static class VersionCollectionExtensions
    {
        internal static Version GetLatest(this IEnumerable<Version> versions)
        {
            return versions
                .OrderBy(x => x)
                .LastOrDefault();
        }
    }
}