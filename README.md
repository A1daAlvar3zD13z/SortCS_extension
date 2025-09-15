<h1 align="center">

<img src="https://raw.githubusercontent.com/keesschollaart81/SortCS/main/resources/logo.png" width="150" alt="SortCS"/>
<br/>
SortCS - A Multiple Object Tracker
</h1>

<div align="center">
    
[![GitHub license](https://img.shields.io/badge/license-GNU-blue.svg)](https://github.com/keesschollaart81/SortCS/blob/master/LICENSE)
[![BCH compliance](https://bettercodehub.com/edge/badge/keesschollaart81/SortCS?branch=main)](https://bettercodehub.com/)

</div> 

SortCS is a 'Multiple Object Tracker' as described in [this paper](https://arxiv.org/abs/1602.00763), implemented in C#.

> SORT is a barebones implementation of a visual multiple object tracking framework based on rudimentary data association and state estimation techniques. It is designed for online tracking applications where only past and current frames are available and the method produces object identities on the fly. While this minimalistic tracker doesn't handle occlusion or re-entering objects its purpose is to serve as a baseline and testbed for the development of future trackers.

> SORT was initially described in this paper. At the time of the initial publication, SORT was ranked the best open source multiple object tracker on the MOT benchmark.

## Using

```cs
using SortCS;

ITracker tracker = new SortTracker(iouThreshold = 0.3f, maxMisses = 3);
tracker.Track(new[]
{
    new RectangleF(1695,383,159,343),
    new RectangleF(1293,455,83,213)
});
tracker.Track(new[]
{
    new RectangleF(1699,383,159,341),
    new RectangleF(1293,455,83,213)
});
tracker.Track(new[]
{
    new RectangleF(1697,383,159,343),
    new RectangleF(1293,455,83,213)
});
var tracks = tracker.Track(new[]
{
    new RectangleF(1695,383,159,343),
    new RectangleF(1293,455,83,213)
});

Assert.AreEqual(2 tracks.Count());
Assert.AreEqual(TrackState.Active, tracks.First().State);
Assert.AreEqual(4, tracks.First().History.Count);

```
