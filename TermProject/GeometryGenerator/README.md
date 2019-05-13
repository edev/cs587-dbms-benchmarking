# Geometry Generator

This program generates 2-dimensional data sets with a limited degree of randomness using the listed controls.

Grid min, max, and interval define a square grid of starting points anchored at point (min, min), with points at the given interval.

To generate the final points, the program will visit each starting point and generate a random point by offsetting the starting point's x- and y-coordinates by a random value in the range [-Random Jitter, +Random Jitter).

Precision specifies the number of decimal digits in final CSV.

The points will be written to the given filename in CSV format, where each row contains an X-coordinate and a Y-coordinate. 
