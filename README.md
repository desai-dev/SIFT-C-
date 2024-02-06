# SIFT-C-
An implementation of SIFT algorithm using EmguCV, a C# wrapper for OpenCV

SIFT is an image transformation algorithm that is invariant to scaling and rotation, invented by David Lowe. The algorithm extracts key features in the test image to transform it into the template image. You can read more about it here: https://en.wikipedia.org/wiki/Scale-invariant_feature_transform

This repo can be used in a .NET application, in case your code is currently written in Python, and you want to get rid of the Python service in your .NET app.

Below is an example of the key features that are detected between the image to be transformed (on the left), and the template image (on the right). After these key features are detected, a homography matrix can be used to transform the test image into the template image.

![alt text](https://github.com/desai-dev/SIFT-CSharp/blob/main/demo-SIFT.png?raw=true)
