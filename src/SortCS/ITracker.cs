using System.Collections.Generic;

namespace SortCS;

public interface ITracker
{
    IEnumerable<Track> Track(IEnumerable<Measurement> measurements);
}