using System.Collections.Generic;
using System.Drawing;

namespace SortCS;

public record Track
{
    public int TrackId { get; set; }

    public float ClassId { get; set; }

    public float Confidence { get; set; }

    public int TotalMisses { get; set; }

    public int Misses { get; set; }

    public List<RectangleF> History { get; set; }

    public TrackState State { get; set; }

    public RectangleF Prediction { get; set; }
}