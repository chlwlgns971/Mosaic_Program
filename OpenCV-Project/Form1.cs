using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using System.Runtime.InteropServices;
using Point = System.Drawing.Point;

namespace OpenCV_Project
{
    public partial class Form1 : Form
    {
        private Point p;
        Image mOriginal;
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                try
                {
                    Image img = pictureBox1.Image.Clone() as Image;
                    Bitmap map = new Bitmap(img);
                    
                    //마우스가 가리키는 좌표와 실제 사진의 좌표가 일치하지 않아 일치하도록 좌표를 변경시켜 줌
                    p.X = map.Width * e.X / pictureBox1.Width;
                    p.Y = map.Height * e.Y / pictureBox1.Height;
                    pictureBox1.Image = mosaicImage(map, trackBar1.Value, p.X, p.Y,trackBar2.Value, trackBar2.Value); //(모자이크 할 이미지, 모자이크 크기, X좌표, Y좌표, 모자이크 범위)
                }
                catch
                {
                    MessageBox.Show("Out of range!","Error");
                }

            }

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //마우스 클릭 위치 저장
                //p.X = e.X;
                //p.Y = e.Y;
                Bitmap b = new Bitmap(pictureBox1.Image);
                //마우스가 가리키는 좌표와 실제 사진의 좌표가 일치하지 않아 일치하도록 좌표를 변경시켜 줌
                p.X = b.Width * e.X / pictureBox1.Width;
                p.Y = b.Height * e.Y / pictureBox1.Height;
                pictureBox1.Image = mosaicImage( b, trackBar1.Value, p.X, p.Y, trackBar2.Value, trackBar2.Value); //(모자이크 할 이미지, 모자이크 크기, X좌표, Y좌표, 모자이크 범위)
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file_path = null;
            openFileDialog1.Title = "Load Directory";
            openFileDialog1.InitialDirectory = "C:\\";//파일 여는 위치설정
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                file_path = openFileDialog1.FileName; //선택한 파일의 풀 경로를 불러와 저장
                pictureBox1.Image = new Bitmap(file_path);
                mOriginal = pictureBox1.Image.Clone() as Image;
            }
        }

        private void button2_Click(object sender, EventArgs e) 
        {
            String filenameFaceCascade = "C:\\Users\\wkdeh\\Desktop\\graduateWorkData\\haarcascade_frontalface_alt.xml";
            CascadeClassifier faceCascade = new CascadeClassifier();
            Bitmap b = new Bitmap(pictureBox1.Image);

            if (!faceCascade.Load(filenameFaceCascade))
            {
                Console.WriteLine("error");
                return;
            }

            // detect 
            Mat mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(b);
            Rect[] faces = faceCascade.DetectMultiScale(mat);
            foreach (var item in faces)
            {
                mosaicImage(b, trackBar1.Value, item.X, item.Y, item.Width, item.Height);
            }
            pictureBox1.Image = b;
        }
        public static Image fullmosaicImage(System.Drawing.Bitmap bitmap, int effectWidth)
        {
            for (int heightOfffset = 0; heightOfffset < bitmap.Height; heightOfffset += effectWidth)
            {
                for (int widthOffset = 0; widthOffset < bitmap.Width; widthOffset += effectWidth)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    for (int i = widthOffset; (i < widthOffset + effectWidth && i < bitmap.Width); i++)
                    {
                        for (int j = heightOfffset; (j < heightOfffset + effectWidth && j < bitmap.Height); j++)
                        {
                            System.Drawing.Color pixel = bitmap.GetPixel(i, j);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    // 픽셀 RGB값의 평균값 구하기
                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;


                    //  위에서 구한 RGB 평균값을 범위 내 모든 픽셀에 적용
                    for (int i = widthOffset; (i < widthOffset + effectWidth && i < bitmap.Width); i++)
                    {
                        for (int j = heightOfffset; (j < heightOfffset + effectWidth && j < bitmap.Height); j++)
                        {

                            System.Drawing.Color newColor = System.Drawing.Color.FromArgb(avgR, avgG, avgB);
                            bitmap.SetPixel(i, j, newColor);
                        }
                    }
                }
            }
            return bitmap;
        }
        
        public static Image mosaicImage(System.Drawing.Bitmap bitmap, int effectWidth, int x, int y, int width, int height)
        {
            for (int heightOfffset = y; (heightOfffset < y + height && heightOfffset < bitmap.Height); heightOfffset += effectWidth)
            {
                for (int widthOffset = x; (widthOffset < x + width && widthOffset < bitmap.Width); widthOffset += effectWidth)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    for (int i = widthOffset; (i < widthOffset + effectWidth && i < x + width && i < bitmap.Width); i++)
                    {
                        for (int j = heightOfffset; (j < heightOfffset + effectWidth && j < y + height && j < bitmap.Height); j++)
                        {
                            System.Drawing.Color pixel = bitmap.GetPixel(i, j);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    // 픽셀 RGB값의 평균값 구하기
                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;


                    //  위에서 구한 RGB 평균값을 범위 내 모든 픽셀에 적용
                    for (int i = widthOffset; (i < widthOffset + effectWidth && i < x + width && i < bitmap.Width); i++)
                    {
                        for (int j = heightOfffset; (j < heightOfffset + effectWidth && j < y + height && j < bitmap.Height); j++)
                        {
                            System.Drawing.Color newColor = System.Drawing.Color.FromArgb(avgR, avgG, avgB);
                            bitmap.SetPixel(i, j, newColor);
                        }
                    }
                }
            }
            return bitmap;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null) {
                Image img = pictureBox1.Image.Clone() as Image;
                Bitmap map = new Bitmap(img);
                pictureBox1.Image = fullmosaicImage(map, trackBar1.Value);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image = mOriginal;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //    //이미지 저장경로 지정
            //    string saveFolder = @"C:\\Users\\wkdeh\\Desktop\\강의자료\\2021년 2학기\\오픈소스개발프로젝트\\OpenCV-Project";

            //    //이미지 저장경로 확인 및 생성
            //    if (!System.IO.Directory.Exists(saveFolder))
            //        System.IO.Directory.CreateDirectory(saveFolder);

            //    //jpg 저장
            //    pictureBox1.Image.Save(saveFolder + "image.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            string save_path = null;
            saveFileDialog1.Title = "Save Directory";
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.Filter = "JPEG File(*.jpg)|*.jpg|Bitmap File(*.bmp)|*.bmp|PNG FIle(*.png)|*.png";
            saveFileDialog1.InitialDirectory = "C:\\";//파일 여는 위치설정
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                save_path = saveFileDialog1.FileName;
                pictureBox1.Image.Save(save_path);
            }
        }
    }
}
