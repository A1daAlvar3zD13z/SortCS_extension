using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HungarianAlgorithm;
using SortCS.Kalman;

namespace SortCS;

public class SortTracker : ITracker
{
    private readonly Dictionary<int, (Track Track, KalmanBoxTracker Tracker)> _trackers;
    private int _trackerIndex = 1; // MOT Evaluations requires a start index of 1

    public SortTracker(float iouThreshold = 0.3f, int maxMisses = 3)
    {
        _trackers = new Dictionary<int, (Track, KalmanBoxTracker)>();
        IouThreshold = iouThreshold;
        MaxMisses = maxMisses;
    }

    public float IouThreshold { get; private init; }

    public int MaxMisses { get; private init; }

    public IEnumerable<Track> Track(IEnumerable<Measurement> measurements)
    {
        var predictions = new Dictionary<int, RectangleF>();

        foreach (var tracker in _trackers)
        {
            var prediction = tracker.Value.Tracker.Predict();
            predictions.Add(tracker.Key, prediction);
        }


        var measurementsArray = measurements.ToArray();

        var (matchedBoxes, unmatchedBoxes) = MatchDetectionsWithPredictions(measurementsArray, predictions.Values);

        var activeTrackids = new HashSet<int>();
        foreach (var item in matchedBoxes)
        {
            var prediction = predictions.ElementAt(item.Key);
            var track = _trackers[prediction.Key];
            track.Track.History.Add(item.Value.bbox);
            track.Track.ClassId = item.Value.classId;
            track.Track.Confidence = item.Value.confidence;
            track.Track.Misses = 0;
            track.Track.State = TrackState.Active;
            track.Tracker.Update(item.Value.bbox);
            track.Track.Prediction = prediction.Value;

            activeTrackids.Add(track.Track.TrackId);
        }

        var missedTracks = _trackers
            .Where(x => !activeTrackids.Contains(x.Key))
            .Select(x => x.Value.Track);
        foreach (var missedTrack in missedTracks)
        {
            missedTrack.Misses++;
            missedTrack.TotalMisses++;
            missedTrack.State = TrackState.Ending;
        }

        var toRemove = _trackers.Where(x => x.Value.Track.Misses > MaxMisses).ToList();
        foreach (var tr in toRemove)
        {
            tr.Value.Track.State = TrackState.Ended;
            _trackers.Remove(tr.Key);
        }

        foreach (var unmatchedBox in unmatchedBoxes)
        {
            var track = new Track
            {
                TrackId = _trackerIndex++,
                History = new List<RectangleF>() { unmatchedBox.bbox },
                Misses = 0,
                State = TrackState.Started,
                TotalMisses = 0,
                Prediction = unmatchedBox.bbox,
                ClassId = unmatchedBox.classId,
                Confidence = unmatchedBox.confidence
            };
            _trackers.Add(track.TrackId, (track, new KalmanBoxTracker(unmatchedBox.bbox)));
        }

        var result = _trackers.Select(x => x.Value.Track).Concat(toRemove.Select(y => y.Value.Track));
        return result;
    }


    private (Dictionary<int, Measurement> Matched, ICollection<Measurement> Unmatched) MatchDetectionsWithPredictions(
        Measurement[] measurements,
        ICollection<RectangleF> trackPredictions)
    {
        if (trackPredictions.Count == 0)
        {
            return (new(), measurements);
        }

        var matrix = new int[measurements.Length, trackPredictions.Count];
        var trackPredictionsArray = trackPredictions.ToArray();

        for (var i = 0; i < measurements.Length; i++)
        {
            for (var j = 0; j < trackPredictionsArray.Length; j++)
            {
                matrix[i, j] = (int)(-100 * IoU(measurements[i].bbox, trackPredictionsArray[j]));
            }
        }

        if (measurements.Length > trackPredictions.Count)
        {
            var extra = new int[measurements.Length - trackPredictions.Count];
            matrix = Enumerable.Range(0, measurements.Length)
                .SelectMany(row => Enumerable.Range(0, trackPredictions.Count).Select(col => matrix[row, col]).Concat(extra))
                .ToArray(measurements.Length, measurements.Length);
        }

        var original = (int[,])matrix.Clone();
        var minimalThreshold = (int)(-IouThreshold * 100);
        var boxTrackerMapping = matrix.FindAssignments()
            .Select((ti, bi) => (bi, ti))
            .Where(bt => bt.ti < trackPredictions.Count && original[bt.bi, bt.ti] <= minimalThreshold)
            .ToDictionary(bt => bt.bi, bt => bt.ti);

        var unmatchedMeasurements = measurements.Where((_, index) => !boxTrackerMapping.ContainsKey(index)).ToArray();
        var matchedMeasurements = measurements.Select((measurement, index) => boxTrackerMapping.TryGetValue(index, out var tracker)
                ? (Tracker: tracker, Mess: measurement)
                : (Tracker: -1, Mess: null))
            .Where(tb => tb.Tracker != -1)
            .ToDictionary(tb => tb.Tracker, tb => tb.Mess);

        return (matchedMeasurements, unmatchedMeasurements);
    }

    private static float IoU(RectangleF a, RectangleF b)
    {
        var intersection = RectangleF.Intersect(a, b);
        if (intersection.IsEmpty)
        {
            return 0;
        }

        var intersectArea = (1.0f + intersection.Width) * (1.0f + intersection.Height);
        var unionArea = ((1.0f + a.Width) * (1.0f + a.Height)) + ((1.0f + b.Width) * (1.0f + b.Height)) - intersectArea;
        return intersectArea / (unionArea + 1e-5f);
    }
}