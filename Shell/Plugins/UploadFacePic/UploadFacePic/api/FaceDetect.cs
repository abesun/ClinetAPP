﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.Cuda;
using System.Diagnostics;
using Emgu.CV.UI;
using System.Drawing;
using System.IO;

namespace UploadFacePic
{
    class FaceDetect
    {
        private string FaceSamplesPath = System.Windows.Forms.Application.StartupPath + "\\trainedFaces";
        private CascadeClassifier faceClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
        TrainedFaceRecognizer tfr;

        public FaceDetect()
        {
            SetTrainedFaceRecognizer(FaceRecognizerType.EigenFaceRecognizer);
        }

        /// <summary>
        /// 获取已保存的所有样本文件
        /// </summary>
        /// <returns></returns>
        public TrainedFileList SetSampleFacesList()
        {
            TrainedFileList tf = new TrainedFileList();
            DirectoryInfo di = new DirectoryInfo(FaceSamplesPath);
            if (di.Exists == false)
                di.Create();
            int i = 0;
            foreach (FileInfo fi in di.GetFiles())
            {
                tf.trainedImages.Add(new Image<Gray, byte>(fi.FullName));
                tf.trainedLabelOrder.Add(i);
                tf.trainedFileName.Add(fi.Name.Split('_')[0]);
                i++;
            }
            return tf;
        }

        /// <summary>
        /// 训练人脸识别器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TrainedFaceRecognizer SetTrainedFaceRecognizer(FaceRecognizerType type)
        {
            tfr = new TrainedFaceRecognizer();
            tfr.trainedFileList = SetSampleFacesList();

            switch (type)
            {
                case FaceRecognizerType.EigenFaceRecognizer:
                    tfr.faceRecognizer = new Emgu.CV.Face.EigenFaceRecognizer(80, double.PositiveInfinity);

                    break;
                case FaceRecognizerType.FisherFaceRecognizer:
                    tfr.faceRecognizer = new Emgu.CV.Face.FisherFaceRecognizer(80, 3500);
                    break;
                case FaceRecognizerType.LBPHFaceRecognizer:
                    tfr.faceRecognizer = new Emgu.CV.Face.LBPHFaceRecognizer(1, 8, 8, 8, 100);
                    break;
            }
            if(tfr.trainedFileList.trainedImages.Count>0)
                tfr.faceRecognizer.Train(tfr.trainedFileList.trainedImages.ToArray(), tfr.trainedFileList.trainedLabelOrder.ToArray());
            return tfr;
        }

        /// <summary>
        /// 获取制定图片，识别出的人脸矩形框
        /// </summary>
        /// <param name="emguImage"></param>
        /// <returns></returns>
        public faceDetectedObj GetFaceRectangle(Mat emguImage)
        {
            faceDetectedObj fdo = new faceDetectedObj();
            fdo.originalImg = emguImage;
            List<Rectangle> faces = new List<Rectangle>();
            try
            {
                using (UMat ugray = new UMat())
                {
                    CvInvoke.CvtColor(emguImage, ugray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);//灰度化图片
                    CvInvoke.EqualizeHist(ugray, ugray);//均衡化灰度图片

                    Rectangle[] facesDetected = faceClassifier.DetectMultiScale(ugray, 1.1, 10, new Size(15, 15));
                    faces.AddRange(facesDetected);
                }
            }
            catch (Exception ex)
            {
            }
            fdo.facesRectangle = faces;

            return fdo;
        }

        /// <summary>
        /// 人脸识别
        /// </summary>
        /// <param name="emguImage"></param>
        /// <returns></returns>
        public faceDetectedObj faceRecognize(Mat emguImage)
        {
            faceDetectedObj fdo = GetFaceRectangle(emguImage);
            Image<Gray, byte> tempImg = fdo.originalImg.ToImage<Gray, byte>();
            #region 给识别出的所有人脸画矩形框
            using (Graphics g = Graphics.FromImage(fdo.originalImg.Bitmap))
            {
                foreach (Rectangle face in fdo.facesRectangle)
                {
                    g.DrawRectangle(new Pen(Color.Red, 2), face);//给识别出的人脸画矩形框

                    Image<Gray, byte> GrayFace = tempImg.Copy(face).Resize(100, 100, Emgu.CV.CvEnum.Inter.Cubic);
                    GrayFace._EqualizeHist();//得到均衡化人脸的灰度图像

                    //Mat pMat = new Mat();
                    
               //float[] ranges = new float[] { 0.0f, 256.0f };
               //using (Emgu.CV.Util.VectorOfMat vm = new Emgu.CV.Util.VectorOfMat())
               //{
               //    vm.Push(GrayFace);
               //    CvInvoke.CalcHist(vm, new int[] { 0 }, null, pMat, new int[] { 1 }, ranges, false);
               //}


                    #region 得到匹配姓名，并画出
                    Emgu.CV.Face.FaceRecognizer.PredictionResult pr = tfr.faceRecognizer.Predict(GrayFace);
                    System.Diagnostics.Debug.WriteLine(pr.Distance + " " + pr.Label);
                    string recogniseName = tfr.trainedFileList.trainedFileName[pr.Label].ToString();
                    
                    string name = tfr.trainedFileList.trainedFileName[pr.Label].ToString();
                    
                    Font font = new Font("宋体",16,GraphicsUnit.Pixel);
                    SolidBrush fontLine = new SolidBrush(Color.Yellow);
                    float xPos = face.X + (face.Width / 2 - (name.Length * 14) / 2);
                    float yPos = face.Y - 21;
                    g.DrawString(name, font, fontLine, xPos, yPos);
                    #endregion

                    fdo.names.Add(name);
                }
            } 
            GC.Collect();
            
            #endregion
            return fdo;
        }

        #region 自定义类及访问类型
        public class TrainedFileList
        {
            public List<Image<Gray, byte>> trainedImages = new List<Image<Gray, byte>>();
            public List<int> trainedLabelOrder = new List<int>();
            public List<string> trainedFileName = new List<string>();
        }

        public class TrainedFaceRecognizer
        {
            public Emgu.CV.Face.FaceRecognizer faceRecognizer;
            public TrainedFileList trainedFileList;
        }

        public class faceDetectedObj
        {
            public Mat originalImg;
            public List<Rectangle> facesRectangle;
            public List<string> names = new List<string>();
        }

        public enum FaceRecognizerType
        {
            EigenFaceRecognizer = 0,
            FisherFaceRecognizer = 1,
            LBPHFaceRecognizer = 2,
        };

        #endregion
    }


}
