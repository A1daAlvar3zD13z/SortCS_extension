<h1 align="center">
SortCS_extension - A Multiple Object Tracker <i>Extension</i>
</h1>

SortCS_extension deploys a 'Multiple Object Tracker' as described in [this paper](https://arxiv.org/abs/1602.00763), implemented in C#.

> SORT is a barebones implementation of a visual multiple object tracking framework based on rudimentary data association and state estimation techniques. It is designed for online tracking applications where only past and current frames are available and the method produces object identities on the fly. While this minimalistic tracker doesn't handle occlusion or re-entering objects its purpose is to serve as a baseline and testbed for the development of future trackers.

> SORT was initially described in this paper. At the time of the initial publication, SORT was ranked the best open source multiple object tracker on the MOT benchmark.

This repo is a modified version of the [SortCS](https://github.com/keesschollaart81/SortCS) implementation. The most notable changes are:
* Force the desired FrameWORK
* Set up the packages versions
* Clean up non-essential packages
* Extension of the Tracker to use Measurement class to propagate the classId and Score associated with the bounding boxes