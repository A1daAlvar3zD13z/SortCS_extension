using System.Drawing;

namespace SortCS;

public class Measurement
{
    public RectangleF bbox { get; set; }
    public float classId { get; set; }

    public float confidence { get; set; }

    public Measurement(RectangleF bbox, float classId, float confidence)
    {
        this.bbox = bbox;
        this.classId = classId;
        this.confidence = confidence;
    }
}