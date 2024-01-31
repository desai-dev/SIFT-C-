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
        // Create SIFT Implementation here
    }
}
