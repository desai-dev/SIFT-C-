using Emgu.CV;
using Emgu.CV.CvEnum;
using System;
using System.Runtime.CompilerServices;
using System.Drawing;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Security.Cryptography;

class Program
{
    static void Main()
    {
        TestImages();
    }

    static void TestImages()
    {
        string[] templateImages = {  }; // Paths to template images here
        string[] inputFolders = { }; // Paths to input image folders here
        string[] outputFolders = { } // Paths to folders where outputs should be produced

        for (int i = 0; i < inputFolders.Length; i++)
        {
            string templateImgPath = templateImages[i];
            string inputFolder = inputFolders[i];
            foreach (string inputImgPath in Directory.GetFiles(inputFolder, "*.png"))
            {
                Mat inputImg = CvInvoke.Imread(inputImgPath);
                Mat templateImg = CvInvoke.Imread(templateImgPath);
                Mat transformedImg = MatchImages(inputImg, templateImg, 0.0005);
                CvInvoke.Imwrite(outputFolders[i] + Path.GetFileNameWithoutExtension(inputImgPath) + "-result" + ".png", transformedImg);
            }
        }
    }

    static Mat MatchImages(Mat inputImg, Mat templateImg, double min_det)
    {
        // Resize the image
        CvInvoke.Resize(inputImg, inputImg, new Size(templateImg.Width, templateImg.Height));

        // Make images grayscale
        Mat gray1 = new Mat();
        Mat gray2 = new Mat();
        CvInvoke.CvtColor(inputImg, gray1, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(templateImg, gray2, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);


        // Use orb
        SIFT orb = new SIFT();

        UMat inputDescriptors = new UMat();
        VectorOfKeyPoint inputKeyPoints = new VectorOfKeyPoint();
        orb.DetectAndCompute(gray1, null, inputKeyPoints, inputDescriptors, false);

        UMat templateDescriptors = new UMat();
        VectorOfKeyPoint templateKeyPoints = new VectorOfKeyPoint();
        orb.DetectAndCompute(gray2, null, templateKeyPoints, templateDescriptors, false);

        // Match features
        BFMatcher matcher = new BFMatcher(DistanceType.L2);
        matcher.Add(templateDescriptors);
        VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch();
        matcher.KnnMatch(inputDescriptors, matches, 2, null);

        List<MDMatch> goodMatchesList = new List<MDMatch>();
        List<MDMatch> goodMatchesNoList = new List<MDMatch>();

        foreach (MDMatch[] match in matches.ToArrayOfArray())
        {
            if (match[0].Distance < 0.75 * match[1].Distance)
            {
                goodMatchesList.Add(match[0]);
            }
        }

        VectorOfDMatch goodMatches = new VectorOfDMatch(goodMatchesList.ToArray());

        // Draw top matches
        Mat imMatches = new Mat();
        Features2DToolbox.DrawMatches(gray2, templateKeyPoints, gray1, inputKeyPoints, goodMatches, imMatches,
            new MCvScalar(255, 0, 0), new MCvScalar(0, 0, 255), null, Features2DToolbox.KeypointDrawType.Default);
        CvInvoke.Imwrite("<insertFilePath>", imMatches);
        CvInvoke.Resize(imMatches, imMatches, new Size(600, 600));

        // Extract point data
        List<PointF> srcPtsLst = new List<PointF>();
        List<PointF> dstPtsLst = new List<PointF>();

        foreach (MDMatch match in goodMatches.ToArray())
        {
            int queryIdx = match.QueryIdx;
            int trainIdx = match.TrainIdx;

            srcPtsLst.Add(inputKeyPoints[queryIdx].Point);
            dstPtsLst.Add(templateKeyPoints[trainIdx].Point);
        }

        VectorOfPointF srcPts = new VectorOfPointF(srcPtsLst.ToArray());
        VectorOfPointF dstPts = new VectorOfPointF(dstPtsLst.ToArray());

        // Find homography
        Mat homography = CvInvoke.FindHomography(srcPts, dstPts, RobustEstimationAlgorithm.Ransac);

        // Calculate determinant
        double det = CvInvoke.Determinant(homography);

        if (det < min_det)
        {
            throw new Exception("Transformation failed");
        }

        // Apply homography
        int w = templateImg.Cols;
        int h = templateImg.Rows;

        Mat imgTransformed = new Mat();
        CvInvoke.WarpPerspective(inputImg, imgTransformed, homography, new Size(w, h));
        CvInvoke.Resize(imgTransformed, imgTransformed, new Size(w, h));

        return imgTransformed;
    }
}
