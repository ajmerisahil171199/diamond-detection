using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
//using Ozonscan;

namespace objectdetection
{

    public partial class Form1 : Form
    {
       // BackgroundWorker bgworker = new BackgroundWorker();
        Image<Bgr, byte> imgInput;
        Image<Bgr, byte> process_img;
        public Bitmap img;
        public int sel_row_id = 0;
        public string img_path = null;


        public Form1()
        {
            InitializeComponent();
          //  bgworker.DoWork += bgworker_DoWork;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                OpenFileDialog ofd = new OpenFileDialog();

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    img_path = ofd.FileName;
                    pictureBox1.Image = new Bitmap(img_path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex.Message");
            }


        }




        private void button2_Click(object sender, EventArgs e)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            imgInput = new Image<Bgr, byte>(img_path);

            if (imgInput == null)
            {
                return;
            }

            try
            {
                var temp = imgInput.Not().SmoothGaussian(5).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(250), new Gray(255));

                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                Mat m = new Mat();

                CvInvoke.FindContours(temp, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                // CvInvoke.GaussianBlur(temp, contours, (Size)new Size(0, 0), (double)2.0);
                int jk = 0;
                dataGridView1.Columns.Add("stone_id", "Stone ID");

                for (int i = 0; i < contours.Size; i++)
                {
                    double perimeter = CvInvoke.ArcLength(contours[i], true);
                    VectorOfPoint approx = new VectorOfPoint();
                    CvInvoke.ApproxPolyDP(contours[i], approx, 0.04 * perimeter, true);

                    var bbox = CvInvoke.BoundingRectangle(contours[i]);
                    process_img = new Image<Bgr, byte>(img_path);
                    process_img.ROI = bbox;
                    var img = process_img.Copy();

                    if (jk == sel_row_id)
                    {
                        CvInvoke.DrawContours(imgInput, contours, i, new MCvScalar(0, 255, 17), 2);

                        if (contours.Size > 0)
                        {

                            //MaxRed(img.ToBitmap());
                            //var YourImage = img.ToBitmap();

                            /*for (var x = 0; x < YourImage.Width; x++)
                                for (var y = 0; y < YourImage.Height; y++)
                                {
                                    var pixel = YourImage.GetPixel(x, y);
                                    if (pixel.R > 200 && pixel.G > 200 && pixel.B > 200)
                                        YourImage.SetPixel(x, y, Color.Transparent);
                                }*/

                            pictureBox5.Image = img.ToBitmap();
                        }
                    }
                    else
                    {
                        CvInvoke.DrawContours(imgInput, contours, i, new MCvScalar(0, 0, 255), 2);
                    }

                    dataGridView1.Rows.Add(jk.ToString());

                    jk++;

                    //moments  center of the shape

                    /*  var moments = CvInvoke.Moments(contours[i]);
                      int x = (int)(moments.M10 / moments.M00);
                      int y = (int)(moments.M01 / moments.M00);*/

                    pictureBox4.Image = imgInput.Bitmap;
                    img.Save("C:\\Users\\ajmer\\OneDrive\\Desktop\\Objects" + "\\" + (i + 1) + ".jpg");

                }
                RefreshGridView();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        }

       /* unsafe static ushort MaxRed(Bitmap img)
        {
            var bd = img.LockBits(new Rectangle(Point.Empty, img.Size), ImageLockMode.ReadOnly, PixelFormat.Format48bppRgb);
            ushort maxRed = 0;
            ushort maxBlue = 0;
            ushort maxGreen = 0;

            for (int y = 0; y < img.Height; y++)
            {
                ushort* ptr = (ushort*)(bd.Scan0 + y * bd.Stride);
                for (int x = 0; x < img.Width; x++)
                {
                    ushort b = *ptr++;
                    ushort g = *ptr++;
                    ushort r = *ptr++;
                    maxRed = Math.Max(maxRed, r);
                    maxBlue = Math.Max(maxBlue, b);
                    maxGreen = Math.Max(maxGreen, g);
                }
            }
            img.UnlockBits(bd);
            return maxRed;
        }*/

        private void RefreshGridView()
        {
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke((MethodInvoker)delegate ()
                {
                    RefreshGridView();
                });
            }
            else
                dataGridView1.Refresh();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            sel_row_id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value);


        }

        private void R_txtbox_TextChanged(object sender, EventArgs e)
        {

        }
        Bitmap bmp1, bmp2, bmp3, syntheticbmp, bmpback;

        public Bitmap CreateNonIndexedImage(Bitmap imgInput)
        {
            Bitmap newBmp = new Bitmap(imgInput.Width, imgInput.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics gfx = Graphics.FromImage(newBmp))
            {
                gfx.DrawImage(imgInput, 0, 0);
            }
            return newBmp;
        }

       /* public Bitmap processImage(Bitmap imgInput)
        {
            FastBitmap fbmp1 = new FastBitmap(imgInput);
            // FastBitmap fbmp2 = new FastBitmap(bmp2);
            Bitmap back = CreateNonIndexedImage(imgInput);
            FastBitmap fb = new FastBitmap(back);
            fbmp1.LockImage();
            // fbmp2.LockImage();
            fb.LockImage();
            double red = 0.0, green = 0.0, blue = 0.0;
            int per = fbmp1.workingBitmap.Width / 10;
            for (int width = 0; width < imgInput.Width; width++)
            {
                for (int height = 0; height < imgInput.Height; height++)
                {
                    Color clr = fbmp1.GetPixel(width, height);
                    red += clr.R;//red+rgb.red()+rgb1.red()+rgb2.red()+rgb3.red()+rgb4.red();
                    green += clr.G;//green+rgb.green()+rgb1.green()+rgb2.green()+rgb3.green()+rgb4.green();
                    blue += clr.B;//blue+rgb.blue()+rgb1.blue()+rgb2.blue()+rgb3.blue()+rgb4.blue();
                }
            }
            double result = ((red + blue + green) / (imgInput.Width * imgInput.Height * 3.0)) + 20;//12
            for (int i = per; i < imgInput.Width - per; i++)
            {
                for (int j = 0; j < imgInput.Height; j++)
                {
                    int ravg, gavg, bavg;
                    Color clr = fbmp1.GetPixel(i, j);
                    ravg = clr.R;
                    gavg = clr.G;
                    bavg = clr.B;
                    if ((ravg + gavg + bavg) > result * 3)
                    {
                        if (ravg * 1.10 < gavg && ravg * 1.10 < bavg)
                        {
                            fbmp1.SetPixel(i, j, Color.Red);
                        }
                        else
                        {
                            fbmp1.SetPixel(i, j, Color.Green);
                        }
                    }
                }
            }
            for (int i = per; i < imgInput.Width - per; i++)
            {
                for (int j = 0; j < imgInput.Height; j++)
                {
                    if (i > 0 && i < imgInput.Width - 1 && j > 0 && j < bmp1.Height - 1)
                    {
                        Color rgb = fbmp1.GetPixel(i, j);
                        Color rgbup = fbmp1.GetPixel(i, j - 1);
                        Color rgbdown = fbmp1.GetPixel(i, j + 1);
                        Color rgbleft = fbmp1.GetPixel(i - 1, j);
                        Color rgbright = fbmp1.GetPixel(i + 1, j);
                        if (rgb.R == 255 && rgb.G == 0 && rgb.B == 0 && rgbup.R != 255 && rgbdown.R != 255 && rgbleft.R != 255 && rgbright.R != 255)
                        {
                            fbmp1.SetPixel(i, j, fb.GetPixel(i, j));
                        }
                        if (rgb.G == 255 && rgb.G == 0 && rgb.R == 0 && rgbup.G != 255 && rgbdown.G != 255 && rgbleft.G != 255 && rgbright.G != 255)
                        {
                            fbmp1.SetPixel(i, j, fb.GetPixel(i, j));
                        }
                    }
                }
            }
            fbmp1.UnlockImage();
            //  fbmp2.UnlockImage();
            fb.UnlockImage();
            return fbmp1.workingBitmap;
        }

        void bgworker_DoWork(object sender, DoWorkEventArgs e)
        {
            *//*cmdMg.send_Command("5\n");
            Thread.Sleep(100);
            cmdMg.send_Command("1\n");
            Thread.Sleep(100);
            cmdMg.send_Command("4\n");
            Thread.Sleep(3000);
            LaunchCommandLineApp();
            Thread.Sleep(5000);
            bmp1 = CopyDataToBitmap(File.ReadAllBytes(@"C:\win.tmp"));
            bmp1.Save("bmp1.png");
            File.Delete(@"C:\win.tmp");
            *//*cmdMg.send_Command("1\n");
            LaunchCommandLineApp();
            Thread.Sleep(10000);
            bmp2 = CopyDataToBitmap(File.ReadAllBytes(@"C:\win.tmp"));
            bmpback = bmp2;
            bmp2.Save("bmp2.png");
            File.Delete(@"C:\win.tmp");*//*
            cmdMg.send_Command("3\n");
            Thread.Sleep(100);
            cmdMg.send_Command("2\n");
            Thread.Sleep(40000);

            cmdMg.send_Command("6\n");
            Thread.Sleep(100);
            cmdMg.send_Command("4\n");
            Thread.Sleep(100);
            LaunchCommandLineApp();
            Thread.Sleep(5000);

            bmp3 = CopyDataToBitmap(File.ReadAllBytes(@"C:\win.tmp"));
            bmp3.Save("bmp3.png");
            File.Delete(@"C:\win.tmp");*//*

            //syntheticbmp = Subtraction(bmp3, bmp1);
            //syntheticbmp.Save("synth.png");

            //AForge.Imaging.Filters.Grayscale gr = new AForge.Imaging.Filters.Grayscale(0.33, 0.33, 0.33);
            //syntheticbmp = gr.Apply(syntheticbmp);
            //int threshold = (int)(150);
            //AForge.Imaging.Filters.Threshold th = new AForge.Imaging.Filters.Threshold(threshold);
            //syntheticbmp = th.Apply(syntheticbmp);
            *//*BlobCounter bc = new BlobCounter();
            bc.FilterBlobs = true;
            bc.MinWidth = 20;
            bc.MinHeight = 20;
            bc.MaxHeight = 40;
            bc.MaxWidth = 40;
            bc.ProcessImage(syntheticbmp);
            Blob[] blobs = bc.GetObjectsInformation();
            Blob temp;
            for (int a = 1; a <= blobs.Length; a++)
            {
                for (int b = 0; b < blobs.Length - a; b++)
                {
                    if (blobs[b].Area > blobs[b + 1].Area)
                    {
                        temp = blobs[b];
                        blobs[b] = blobs[b + 1];
                        blobs[b + 1] = temp;
                    }
                }
            }
            int syncnt = blobs.Count();
            MessageBox.Show("Synthetic Diamonds" + syncnt);

           // gr = new AForge.Imaging.Filters.Grayscale(0.33, 0.33, 0.33);
            //bmp2 = gr.Apply(bmp2);
            //threshold = (int)(150);
            //th = new AForge.Imaging.Filters.Threshold(threshold);
            //bmp2 = th.Apply(bmp2);
            bc = new BlobCounter();
            bc.FilterBlobs = true;
            bc.MinWidth = 20;
            bc.MinHeight = 20;
            bc.MaxHeight = 40;
            bc.MaxWidth = 40;
            bc.ProcessImage(bmp2);
            Blob[]  blobs1 = bc.GetObjectsInformation();
           
            for (int a = 1; a <= blobs1.Length; a++)
            {
                for (int b = 0; b < blobs1.Length - a; b++)
                {
                    if (blobs1[b].Area > blobs1[b + 1].Area)
                    {
                        temp = blobs1[b];
                        blobs1[b] = blobs1[b + 1];
                        blobs1[b + 1] = temp;
                    }
                }
            }
            int totcnt=blobs1.Count();
            MessageBox.Show("Natural Diamonds:" + (totcnt - syncnt));
            bmpback = CreateNonIndexedImage(bmpback); 
            foreach (Blob blb in blobs)
            {
                try
                {
                    using (Graphics grphc = Graphics.FromImage(bmpback))
                    {
                        grphc.DrawRectangle(new Pen(Color.Red), blb.Rectangle);
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }*//*
            Bitmap rbmp = processImage(imgInput.ToBitmap());
            rbmp.Save("result.png");
            pictureBox5.Image = rbmp;
        }*/

        public Bitmap denoise(Bitmap imgInput)
        {
            byte[] window = new byte[9];
            byte[] window1 = new byte[9];
            byte[] window2 = new byte[9];
            long totalavgred = 0, totalavggreen = 0, totalavgblue = 0, total = 0;
            for (int y = 1; y < imgInput.Height - 1; y++)
            {
                for (int x = 1; x < imgInput.Width - 1; x++)
                {
                    totalavgred = 0;
                    totalavggreen = 0;
                    totalavgblue = 0;
                    // Pick up window element
                    for (int a = 0; a < 2; a++)
                    {
                        Color rgb0 = imgInput.GetPixel((x - 1), y);//ptr[a][y*img.width()+(x-1)];
                        Color rgb1 = imgInput.GetPixel((x - 1), y - 1);//ptr[a][(y-1)*img.width()+(x-1)];
                        Color rgb2 = imgInput.GetPixel((x - 1), y + 1);//ptr[a][(y+1)*img.width()+(x-1)];
                        Color rgb3 = imgInput.GetPixel((x), y);//ptr[a][y*img.width()+x];
                        Color rgb4 = imgInput.GetPixel((x), y - 1);//ptr[a][(y-1)*img.width()+x];
                        Color rgb5 = imgInput.GetPixel((x), y + 1);//ptr[a][(y+1)*img.width()+x];
                        Color rgb6 = imgInput.GetPixel((x + 1), y);//ptr[a][y*img.width()+(x+1)];
                        Color rgb7 = imgInput.GetPixel((x + 1), y - 1);//ptr[a][(y-1)*img.width()+(x+1)];
                        Color rgb8 = imgInput.GetPixel((x + 1), y + 1);//ptr[a][(y+1)*img.width()+(x+1)] ;

                        window[0] = rgb0.R;
                        window[1] = rgb1.R; ;
                        window[2] = rgb2.R; ;
                        window[3] = rgb3.R; ;
                        window[4] = rgb4.R; ;
                        window[5] = rgb5.R; ;
                        window[6] = rgb6.R; ;
                        window[7] = rgb7.R; ;
                        window[8] = rgb8.R; ;

                        window1[0] = rgb0.G;
                        window1[1] = rgb1.G;
                        window1[2] = rgb2.G;
                        window1[3] = rgb3.G;
                        window1[4] = rgb4.G;
                        window1[5] = rgb5.G;
                        window1[6] = rgb6.G;
                        window1[7] = rgb7.G;
                        window1[8] = rgb8.G;

                        window2[0] = rgb0.B;
                        window2[1] = rgb1.B;
                        window2[2] = rgb2.B;
                        window2[3] = rgb3.B;
                        window2[4] = rgb4.B;
                        window2[5] = rgb5.B;
                        window2[6] = rgb6.B;
                        window2[7] = rgb7.B;
                        window2[8] = rgb8.B;

                        int first1 = window[0], second1 = window[1];
                        //insertionSort(window);
                        for (int i = 2; i < 9; i++)
                        {
                            if (window[i] > first1)
                            {
                                second1 = first1;
                                first1 = window[i];
                            }
                            if (window[i] > second1)
                            {
                                second1 = window[i];
                            }
                        }
                        int first2 = window1[0], second2 = window1[1];
                        for (int i = 2; i < 9; i++)
                        {
                            if (window1[i] > first2)
                            {
                                second2 = first2;
                                first2 = window1[i];
                            }
                            if (window1[i] > second2)
                            {
                                second2 = window1[i];
                            }
                        }

                        int first3 = window2[0], second3 = window2[1];
                        for (int i = 2; i < 9; i++)
                        {
                            if (window2[i] > first3)
                            {
                                second3 = first3;
                                first3 = window2[i];
                            }
                            if (window2[i] > second3)
                            {
                                second3 = window2[i];
                            }
                        }
                        int avgr = 0, avgg = 0, avgb = 0;
                        for (int i = 0; i < 9; i++)
                        {
                            avgb = avgb + window2[i];
                            avgg = avgg + window1[i];
                            avgr = avgr + window[i];
                        }

                        avgr = (avgr - first1 - second1) / 7;// of 6 element
                        avgg = (avgg - first2 - second2) / 7;
                        avgb = (avgb - first3 - second3) / 7;
                        totalavgred += avgr;
                        totalavggreen += avgg;
                        totalavgblue += avgb;
                    }
                    totalavgred /= 2;
                    totalavggreen /= 2;
                    totalavgblue /= 2;
                    if (totalavgblue > 255)
                        totalavgblue = 255;
                    if (totalavgblue < 0)
                        totalavgblue = 0;
                    if (totalavggreen > 255)
                        totalavggreen = 255;
                    if (totalavggreen < 0)
                        totalavggreen = 0;
                    if (totalavgred > 255)
                        totalavgred = 255;
                    if (totalavgred < 0)
                        totalavgred = 0;
                    total = total + totalavgred + totalavggreen + totalavgblue;
                    imgInput.SetPixel(x, y, Color.FromArgb((int)totalavgred, (int)totalavggreen, (int)totalavgblue));
                }
            }
            imgInput.Save("F:\\Objects\\img.png");
            return imgInput;
        }
        private void doProessingCore()
        {
            Bitmap imgInput = new Bitmap("bmp1.png");
            Bitmap bmp2 = new Bitmap("bmp2.png");
            Bitmap bmp3 = new Bitmap("bmp3.png");
            Bitmap b1, b2, b3;
            b1 = CreateNonIndexedImage(imgInput);
            b2 = CreateNonIndexedImage(bmp2);
            b3 = CreateNonIndexedImage(bmp3);
            unsafe
            {
                BitmapData bitmapData = b2.LockBits(new Rectangle(0, 0, b2.Width, b2.Height), ImageLockMode.ReadWrite, b2.PixelFormat);
                BitmapData bitmapData1 = b1.LockBits(new Rectangle(0, 0, b1.Width, b1.Height), ImageLockMode.ReadWrite, b1.PixelFormat);
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b2.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                byte* PtrFirstPixel1 = (byte*)bitmapData1.Scan0;
                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                    byte* currentLine1 = PtrFirstPixel1 + (y * bitmapData1.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {

                        int oldBlue = currentLine[x];
                        int oldGreen = currentLine[x + 1];
                        int oldRed = currentLine[x + 2];
                        //if ((oldBlue != 0 && oldGreen != 0 && oldRed != 0 && oldBlue <= 254 && (oldGreen >= oldRed && oldGreen >= oldBlue && oldGreen <= 255 && oldRed <= /*110*/90 && oldBlue <= /*200*/180 || oldBlue >= oldGreen && oldRed >= /*32*/40 && oldBlue <=/*55*/ 60 && oldGreen <= /*10*/8 || oldBlue <= oldGreen && oldBlue <= oldRed && oldBlue <= 32 || oldBlue >= 32 && oldBlue <= oldGreen + 1 && oldBlue <= oldGreen + 1))/* || (oldGreen > 65 && oldBlue > 65 && oldRed > 65 && oldRed > oldBlue) || (oldRed > 100 && oldGreen > 100 && oldRed > 100*/)/*last 2 Or Condition is for test)*/
                        if ((oldBlue != 0 && oldGreen != 0 && oldRed != 0 && oldBlue <= 254 && (oldGreen >= oldRed && oldGreen >= oldBlue && oldGreen <= 255 && oldRed <= 110 && oldBlue <= 180 || oldBlue >= oldGreen && oldRed >= 20 && oldBlue <= 35 && oldGreen <= 7 || oldBlue <= oldGreen && oldBlue <= oldRed && oldBlue <= 19 || oldBlue >= 20 && oldBlue <= oldGreen && oldBlue <= oldRed)) || ((oldRed > oldBlue && oldRed > oldGreen && oldRed > 50) || ((oldRed > oldBlue && oldRed > oldGreen) && oldBlue < oldGreen)))
                        {
                            currentLine1[x] = (byte)0;
                            currentLine1[x + 1] = (byte)255;
                            currentLine1[x + 2] = (byte)255;
                        }
                    }
                });
                b2.UnlockBits(bitmapData);
                b1.UnlockBits(bitmapData1);
            }
            unsafe
            {
                BitmapData bitmapData = b2.LockBits(new Rectangle(0, 0, b2.Width, b2.Height), ImageLockMode.ReadWrite, b2.PixelFormat);
                BitmapData bitmapData1 = b1.LockBits(new Rectangle(0, 0, b1.Width, b1.Height), ImageLockMode.ReadWrite, b1.PixelFormat);
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b2.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                byte* PtrFirstPixel1 = (byte*)bitmapData1.Scan0;
                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                    byte* currentLine1 = PtrFirstPixel1 + (y * bitmapData1.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {

                        int oldBlue = currentLine[x];
                        int oldGreen = currentLine[x + 1];
                        int oldRed = currentLine[x + 2];
                        if (((oldGreen * 1.15 < oldBlue && oldGreen * 1.15 < oldRed) && (oldRed > 120 && oldBlue > 120)))//else if written by pratap for critical diamonds remove if not work
                        //{
                        // if ((oldBlue != 0 && oldGreen != 0 && oldRed != 0 && oldBlue <= 254 && (oldGreen >= oldRed && oldGreen >= oldBlue && oldGreen <= 255 && oldRed <= 110 && oldBlue <= 200 || oldBlue >= oldGreen && oldRed >= 32 && oldBlue <= 55 && oldGreen <= 10 || oldBlue <= oldGreen && oldBlue <= oldRed && oldBlue <= 32 || oldBlue >= 32 && oldBlue <= oldGreen + 1 && oldBlue <= oldGreen + 1)) || (oldGreen > 65 && oldBlue > 65 && oldRed > 65 && oldRed > oldBlue) || (oldRed > 100 && oldGreen > 100 && oldRed > 100)/*last 2 Or Condition is for test*/)
                        {
                            currentLine1[x] = (byte)255;
                            currentLine1[x + 1] = (byte)255;
                            currentLine1[x + 2] = (byte)0;
                        }
                    }
                });
                b2.UnlockBits(bitmapData);
                b1.UnlockBits(bitmapData1);
            }
            /**
             * For Ptting rgb range in for creating green color
             * **/
            /* ModifyRegistry mr = new ModifyRegistry();
             try
             {
                 if (mr.Read("txtRFrom").ToString() != null && mr.Read("txtRTo").ToString() != null && mr.Read("txtGFrom").ToString() != null && mr.Read("txtGTo").ToString() != null && mr.Read("txtBFrom").ToString() != null && mr.Read("txtBTo").ToString() != null)
                 {
                     unsafe
                     {
                         BitmapData bitmapData = b2.LockBits(new Rectangle(0, 0, b2.Width, b2.Height), ImageLockMode.ReadWrite, b2.PixelFormat);
                         BitmapData bitmapData1 = b1.LockBits(new Rectangle(0, 0, b1.Width, b1.Height), ImageLockMode.ReadWrite, b1.PixelFormat);
                         int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b2.PixelFormat) / 8;
                         int heightInPixels = bitmapData.Height;
                         int widthInBytes = bitmapData.Width * bytesPerPixel;
                         byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                         byte* PtrFirstPixel1 = (byte*)bitmapData1.Scan0;
                         Parallel.For(0, heightInPixels, y =>
                         {
                             byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                             byte* currentLine1 = PtrFirstPixel1 + (y * bitmapData1.Stride);
                             for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                             {

                                 int oldBlue = currentLine[x];
                                 int oldGreen = currentLine[x + 1];
                                 int oldRed = currentLine[x + 2];*/
            //if ((oldBlue != 0 && oldGreen != 0 && oldRed != 0 && oldBlue <= 254 && (oldGreen >= oldRed && oldGreen >= oldBlue && oldGreen <= 255 && oldRed <= /*110*/90 && oldBlue <= /*200*/180 || oldBlue >= oldGreen && oldRed >= /*32*/40 && oldBlue <=/*55*/ 60 && oldGreen <= /*10*/8 || oldBlue <= oldGreen && oldBlue <= oldRed && oldBlue <= 32 || oldBlue >= 32 && oldBlue <= oldGreen + 1 && oldBlue <= oldGreen + 1))/* || (oldGreen > 65 && oldBlue > 65 && oldRed > 65 && oldRed > oldBlue) || (oldRed > 100 && oldGreen > 100 && oldRed > 100*/)/*last 2 Or Condition is for test)*/
            /* if (oldBlue >= Convert.ToInt32(mr.Read("txtBFrom").ToString()) && oldBlue <= Convert.ToInt32(mr.Read("txtBTo").ToString()) && oldGreen >= Convert.ToInt32(mr.Read("txtGFrom").ToString()) && oldGreen <= Convert.ToInt32(mr.Read("txtGTo").ToString()) && oldRed >= Convert.ToInt32(mr.Read("txtRFrom").ToString()) && oldRed <= Convert.ToInt32(mr.Read("txtRTo").ToString()))
             {
                 currentLine1[x] = (byte)0;
                 currentLine1[x + 1] = (byte)255;
                 currentLine1[x + 2] = (byte)0;
             }
         }
     });
     b2.UnlockBits(bitmapData);
     b1.UnlockBits(bitmapData1);
 }
}
}
catch (Exception exce)
{

}*/
            /**
             * 
             * */

            /* unsafe
             {
                 BitmapData bitmapData = b2.LockBits(new Rectangle(0, 0, b2.Width, b2.Height), ImageLockMode.ReadWrite, b2.PixelFormat);
                 BitmapData bitmapData1 = b1.LockBits(new Rectangle(0, 0, b1.Width, b1.Height), ImageLockMode.ReadWrite, b1.PixelFormat);
                 int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b2.PixelFormat) / 8;
                 int heightInPixels = bitmapData.Height;
                 int widthInBytes = bitmapData.Width * bytesPerPixel;
                 byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                 byte* PtrFirstPixel1 = (byte*)bitmapData1.Scan0;
                 Parallel.For(0, heightInPixels, y =>
                 {
                     byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                     byte* currentLine1 = PtrFirstPixel1 + (y * bitmapData1.Stride);
                     for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                     {

                         int oldBlue = currentLine[x];
                         int oldGreen = currentLine[x + 1];
                         int oldRed = currentLine[x + 2];*/
            // if (((oldGreen * 1.15 < oldBlue && oldGreen * 1.15 < oldRed) && (oldRed > 120 && oldBlue > 120)))//else if written by pratap for critical diamonds remove if not work
            //{
            // if ((oldBlue != 0 && oldGreen != 0 && oldRed != 0 && oldBlue <= 254 && (oldGreen >= oldRed && oldGreen >= oldBlue && oldGreen <= 255 && oldRed <= 110 && oldBlue <= 200 || oldBlue >= oldGreen && oldRed >= 32 && oldBlue <= 55 && oldGreen <= 10 || oldBlue <= oldGreen && oldBlue <= oldRed && oldBlue <= 32 || oldBlue >= 32 && oldBlue <= oldGreen + 1 && oldBlue <= oldGreen + 1)) || (oldGreen > 65 && oldBlue > 65 && oldRed > 65 && oldRed > oldBlue) || (oldRed > 100 && oldGreen > 100 && oldRed > 100)/*last 2 Or Condition is for test*/)
            /*{
                currentLine1[x] = (byte)255;
                currentLine1[x + 1] = (byte)0;
                currentLine1[x + 2] = (byte)0;
            }
        }
    });
    b2.UnlockBits(bitmapData);
    b1.UnlockBits(bitmapData1);
}
*/

            /* unsafe
             {
                 BitmapData bitmapData = b3.LockBits(new Rectangle(0, 0, b3.Width, b3.Height), ImageLockMode.ReadWrite, b2.PixelFormat);
                 BitmapData bitmapData1 = b1.LockBits(new Rectangle(0, 0, b1.Width, b1.Height), ImageLockMode.ReadWrite, b1.PixelFormat);
                 int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b2.PixelFormat) / 8;
                 int heightInPixels = bitmapData.Height;
                 int widthInBytes = bitmapData.Width * bytesPerPixel;
                 byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                 byte* PtrFirstPixel1 = (byte*)bitmapData1.Scan0;
                 Parallel.For(0, heightInPixels, y =>
                 {
                     byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                     byte* currentLine1 = PtrFirstPixel1 + (y * bitmapData1.Stride);
                     for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                     {

                         int oldBlue = currentLine[x];
                         int oldGreen = currentLine[x + 1];
                         int oldRed = currentLine[x + 2];
                         if (oldBlue >= oldRed && oldBlue >= 35.0 && oldGreen >= 12.0 || oldGreen >= 148.0 && oldRed >= 148.0)
                         {
                             currentLine1[x] = (byte)0;
                             currentLine1[x + 1] = (byte)0;
                             currentLine1[x + 2] = (byte)255;
                         }
                     }
                 });
                 b3.UnlockBits(bitmapData);
                 b1.UnlockBits(bitmapData1);
                 b1.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sparrow Camera\\CaptureMerge.bmp");*/
            //b1.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sparrow Camera\\CaptureMerge1.bmp");
            //mresult.sa
            //   Rectangle cropRect = new Rectangle(175, 0, 760, 760);
            //Bitmap src = new Bitmap(b1);
            /* Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);
             using (Graphics g = Graphics.FromImage(target))
             {
                 g.DrawImage(b1, new Rectangle(0, 0, target.Width, target.Height),
                 cropRect,
                 GraphicsUnit.Pixel);
             }
             // NoiseRemoval(target);
             denoise(target);
             pictureBox5.Image = target;*///cropImage(mresult.Bitmap,new Rectangle(0,140,770,682));
                                          // TimerTime.Stop();
                                          // TimerTime.Enabled = false;
                                          // lblTime.Text = "" + 0;
                //}


        }

    }
}
