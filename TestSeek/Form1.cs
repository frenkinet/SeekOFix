/*
Copyright (c) 2014 Stephen Stair (sgstair@akkit.org)
Additional code Miguel Parra (miguelvp@msn.com)
Additional code by Franci Kapel: http://html-color-codes.info/Contact/
(Franci Kapel: A lot of my code was influenced by JadeW's work:  https://github.com/rzva/ThermalView)

Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
*/

using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using winusbdotnet.UsbDevices;
using AForge.Imaging.Filters;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Globalization;

namespace TestSeek
{
    public partial class Form1 : Form
    {
        SeekThermal thermal;
        Thread thermalThread;
		ThermalFrame currentFrame, lastUsableFrame;
		
        bool stopThread;
        bool grabExternalReference = false;
		bool firstAfterCal = false;
        bool usignExternalCal = false;
        bool autoSaveImg = false;
        bool autoRange = true;
        bool dynSliders = false;
        bool SharpenImage = false;
		bool isRunning = true;
		
        string localPath;
		string tempUnit;
		
		ushort[] arrID4 = new ushort[32448];
        ushort[] arrID1 = new ushort[32448];
        ushort[] arrID3 = new ushort[32448];
		
		bool[] badPixelArr = new bool[32448];

        ushort[] gMode = new ushort[1000];

        ushort[,] palleteArr = new ushort[1001,3];//-> ushort
		
        byte[] imgBuffer = new byte[97344];
		
        ushort gModePeakIx = 0;
        ushort gModePeakCnt = 0;
        ushort gModeLeft = 0;
        ushort gModeRight = 0;
        ushort gModeLeftManual = 0;
        ushort gModeRightManual = 0;
        ushort avgID4 = 0;
        ushort avgID1 = 0;
        ushort maxTempRaw = 0;

        double[] gainCalArr = new double[32448];

        Bitmap bitmap = new Bitmap(208, 156, PixelFormat.Format24bppRgb);
        Bitmap croppedBitmap = new Bitmap(206, 156, PixelFormat.Format24bppRgb);
        Bitmap bigBitmap = new Bitmap(412, 312, PixelFormat.Format24bppRgb);
        BitmapData bitmap_data;
		
        //ResizeBicubic bicubicResize = new ResizeBicubic(412, 312);
        ResizeBilinear bilinearResize = new ResizeBilinear(412, 312);
        Crop cropFilter = new Crop(new Rectangle(0, 0, 206, 156));
        Sharpen sfilter = new Sharpen();

        public Form1()
        {
            InitializeComponent();

            localPath = Directory.GetCurrentDirectory().ToString();
            Directory.CreateDirectory(localPath+@"\export");

            rbUnitsK.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
            rbUnitsC.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
            rbUnitsF.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);

            var device = SeekThermal.Enumerate().FirstOrDefault();
            if(device == null)
            {
                MessageBox.Show("No Seek Thermal devices found.");
                return;
            }
            thermal = new SeekThermal(device);

            thermalThread = new Thread(ThermalThreadProc);
            thermalThread.IsBackground = true;
            thermalThread.Start();
        }

        void ThermalThreadProc()
        {
            while (!stopThread && thermal != null)
            {
                bool progress = false;

                currentFrame = thermal.GetFrameBlocking();

                switch (currentFrame.StatusByte)
                {
                    case 4://gain calibration
                        frame4stuff();
                        break;

                    case 1://shutter calibration
                        markBadPixels();
                        if (!usignExternalCal) frame1stuff();
                        firstAfterCal = true;
                        break;
                    
                    case 3://image frame
                        markBadPixels();
                        if (grabExternalReference)//use this image as reference
                        {
                            grabExternalReference = false;
                            usignExternalCal = true;
                            frame1stuff();
                        }
                        else
                        {
                            frame3stuff();
                            lastUsableFrame = currentFrame;
                            progress = true;
                        }
                        break;

                    default:
                        break;
                }

                if(progress)
                {
                    Invalidate();//redraw form
                }
            }
        }

        private void frame4stuff()
        {
            arrID4 = currentFrame.RawDataU16;
            avgID4 = GetMode(arrID4);

            for (int i = 0; i < 32448; i++)
            {
                if (arrID4[i] > 2000 && arrID4[i] < 8000) {
                    gainCalArr[i] = avgID4 / (double)arrID4[i];
				}
                else {
					gainCalArr[i] = 1;
					badPixelArr[i] = true;
				}
            }
        }

        private void frame1stuff()
        {
            arrID1 = currentFrame.RawDataU16;
            //avgID1 = GetMode(arrID1);
        }

        private void frame3stuff()
        {
            arrID3 = currentFrame.RawDataU16;

            for (int i = 0; i < 32448; i++)
            {
                if (arrID3[i] > 2000)
                {
                    arrID3[i] = (ushort)((arrID3[i] - arrID1[i]) * gainCalArr[i] + 7500);
                }
                else
                {
                    arrID3[i] = 0;
                    badPixelArr[i] = true;
                }
            }

            fixBadPixels();
			removeNoise();
            getHistogram();
            fillImgBuffer();
        }

        private void fillImgBuffer()
        {
            ushort v = 0;
            ushort loc = 0;

            double iScaler;
            iScaler = (double)(gModeRight - gModeLeft) / 1000;

            for (int i = 0; i < 32448; i++)
            {
                v = arrID3[i];
                if (v < gModeLeft) v = gModeLeft;
                if (v > gModeRight) v = gModeRight;
                v = (ushort)(v - gModeLeft);
                loc = (ushort)(v / iScaler);

                imgBuffer[i*3] = (byte)palleteArr[loc, 2];
                imgBuffer[i*3+1] = (byte)palleteArr[loc, 1];
                imgBuffer[i*3+2] = (byte)palleteArr[loc, 0];
            }
        }

        private void markBadPixels()
        {
            ushort[] RawDataArr = currentFrame.RawDataU16;

            for (int i = 0; i < RawDataArr.Length; i++)
            {
                if (RawDataArr[i] < 2000 || RawDataArr[i] > 22000)
                {
                    badPixelArr[i] = true;
                }
            }
        }
		
		private void fixBadPixels()
       {
           ushort x = 0;
           ushort y = 0;
           ushort i = 0;
           ushort nr = 0;
           ushort val = 0;

           for (y = 0; y < 156; y++)
           {
               for (x = 0; x < 208; x++, i++)
               {
                   if (badPixelArr[i] && x < 206) {

                       val = 0;
                       nr = 0;

                       if (y > 0 && !badPixelArr[i - 208]) //top pixel
                       {
                           val += arrID3[i - 208];
                           ++nr;
                       }

                       if (y < 155 && !badPixelArr[i + 208]) // bottom pixel
                       {
                           val += arrID3[i + 208];
                           ++nr;
                       }

                       if (x > 0 && !badPixelArr[i - 1]) //Left pixel
                       {
                           val += arrID3[i - 1];
                           ++nr;
                       }

                       if (x < 205 && !badPixelArr[i + 1]) //Right pixel
                       {
                           val += arrID3[i + 1];
                           ++nr;
                       }

                       if (nr>0)
                       {
                           val /= nr;
                           arrID3[i] = val;
                       }
                   }
               }
           }
       }

       private void removeNoise()
       {
           ushort x = 0;
           ushort y = 0;
           ushort i = 0;
           ushort val = 0;
           ushort[] arrColor = new ushort[4];

           for (y = 0; y < 156; y++)
           {
               for (x = 0; x < 208; x++)
               {
                   if (x > 0 && x < 206 && y > 0 && y < 155)
                   {
                       arrColor[0] = arrID3[i - 208];//top
                       arrColor[1] = arrID3[i + 208];//bottom
                       arrColor[2] = arrID3[i - 1];//left
                       arrColor[3] = arrID3[i + 1];//right

                       val = (ushort)((arrColor[0] + arrColor[1] + arrColor[2] + arrColor[3] - Highest(arrColor) - Lowest(arrColor))/2);

                       if (Math.Abs(val - arrID3[i]) > 100 && val != 0)
                       {
                           arrID3[i] = val;
                       }
                   }
                   i++;
               }
           }

       }

       private ushort Highest(ushort[] numbers)
       {
           ushort highest = 0;

           for (ushort i = 0; i < 4; i++)
           {
               if (numbers[i] > highest)
                   highest = numbers[i];
           }

           return highest;
       }

       private ushort Lowest(ushort[] numbers)
       {
           ushort lowest = 30000;

           for (ushort i = 0; i < 4; i++)
           {
               if (numbers[i] < lowest)
                   lowest = numbers[i];
           }

           return lowest;
       }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopThread = true;
            if (thermal != null)
            {
                thermalThread.Join(500);
                thermal.Deinit();
            }
        }

        // Button to capture external reference or switch to internal shutter.
        private void button1_Click(object sender, EventArgs e)
        {
            grabExternalReference = true;
        }

        // Button to toggle between automatic ranging or manual.
        private void button2_Click(object sender, EventArgs e)
        {
            autoRange = !autoRange;
            if (autoRange) { 
                button2.Text = "Switch to manual range";
                cbDynSlidres.Checked = false;
                cbDynSlidres.Visible = false;
            }
            else {
                button2.Text = "Switch to auto range";
                cbDynSlidres.Visible = true;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (!autoRange)
            {
                if (trackBar1.Value > trackBar2.Value + 10)
                {
                    gModeRightManual = (ushort)trackBar1.Value;
                }
                else
                {
                    trackBar1.Value = trackBar2.Value + 10;
                }
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (!autoRange)
            {
                if (trackBar2.Value < trackBar1.Value - 10)
                {
                    gModeLeftManual = (ushort)trackBar2.Value;
                }
                else
                {
                    trackBar2.Value = trackBar1.Value - 10;
                }
            }
        }

        private void cbAutoSave_CheckedChanged(object sender, EventArgs e)
        {
            autoSaveImg = !autoSaveImg;
        }

        private void cbDynSlidres_CheckedChanged(object sender, EventArgs e)
        {
            dynSliders = !dynSliders;

            int currentLeftPos = trackBar2.Value;
            int currentRightPos = trackBar1.Value;
            int currentDiff = currentRightPos - currentLeftPos;

            if (dynSliders)
            {
                if (currentLeftPos - currentDiff > 4000) trackBar2.Minimum = currentLeftPos - currentDiff;//left min
                else trackBar2.Minimum = 4000;

                if (currentLeftPos + currentDiff * 2 < 20000) trackBar2.Maximum = currentLeftPos + currentDiff * 2;//left max
                else trackBar2.Maximum = 20000;

                if (currentRightPos - currentDiff * 2 > 4000) trackBar1.Minimum = currentRightPos - currentDiff * 2;//right min
                else trackBar1.Minimum = 4000;

                if (currentRightPos + currentDiff < 20000) trackBar1.Maximum = currentRightPos + currentDiff;//right max
                else trackBar1.Maximum = 20000;
            }
            else 
            {
                trackBar1.Minimum = 4000;
                trackBar2.Minimum = 4000;
                trackBar1.Maximum = 20000;
                trackBar2.Maximum = 20000;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            SharpenImage = !SharpenImage;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var pngFiles = new DirectoryInfo(Directory.GetCurrentDirectory()+"\\palette").GetFiles("*.png");

            foreach (FileInfo file in pngFiles)
            {
                cbPal.Items.Add(new ComboItem(file.FullName, file.Name.Replace(".png", "")));
            }
            cbPal.SelectedIndex = 1;
        }

        private void cbPal_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboItem newPal = (ComboItem)cbPal.SelectedItem;
            Bitmap paletteImg = new Bitmap(newPal.Key);
            Color picColor;

            for (int i = 0; i < 1001; i++)
            {
                picColor = paletteImg.GetPixel(i, 0);
                palleteArr[i, 0] = picColor.R;
                palleteArr[i, 1] = picColor.G;
                palleteArr[i, 2] = picColor.B;
            }
			
			paletteImg.RotateFlip(RotateFlipType.Rotate270FlipNone);
            pictureBox4.Image = paletteImg;
        }

        class ComboItem
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public ComboItem(string key, string value)
            {
                Key = key; Value = value;
            }
            public override string ToString()
            {
                return Value;
            }
        }

        public ushort GetMode(ushort[] arr)
        {
            ushort[] arrMode = new ushort[320];
            ushort topPos = 0;
            for (ushort i = 0; i < 32448; i++)
            {
                if ((arr[i] > 1000) && (arr[i] / 100 != 0)) arrMode[(arr[i]) / 100]++;
            }

            topPos = (ushort)Array.IndexOf(arrMode, arrMode.Max());

            return (ushort)(topPos * 100);
        }

        public void getHistogram()
        {
            maxTempRaw = arrID3.Max();
            ushort[] arrMode = new ushort[2100];
            ushort topPos = 0;
            for (ushort i = 0; i < 32448; i++)
            {
                if ((arrID3[i] > 1000) && (arrID3[i] / 10 != 0) && !badPixelArr[i]) arrMode[(arrID3[i]) / 10]++;
            }

            topPos = (ushort)Array.IndexOf(arrMode, arrMode.Max());

            gMode = arrMode;
            gModePeakCnt = arrMode.Max();
            gModePeakIx = (ushort)(topPos * 10);

            //lower it to 100px;
            for (ushort i = 0; i < 2100; i++)
            {
                gMode[i] = (ushort)((double)arrMode[i] / gModePeakCnt * 100);
            }

            if (autoRange)
            {
                gModeLeft = gModePeakIx;
                gModeRight = gModePeakIx;
                //find left border:
                for (ushort i = 0; i < topPos; i++)
                {
                    if (arrMode[i] > arrMode[topPos] * 0.01)
                    {
                        gModeLeft = (ushort)(i * 10);
                        break;
                    }
                }

                //find right border:
                for (ushort i = 2099; i > topPos; i--)
                {
                    if (arrMode[i] > arrMode[topPos] * 0.01)
                    {
                        gModeRight = (ushort)(i * 10);
                        break;
                    }
                }
                gModeLeftManual = gModeLeft;
                gModeRightManual = gModeRight;
            }
            else {
                gModeLeft = gModeLeftManual;
                gModeRight = gModeRightManual;
            }
        }

        public void DrawHistogram()
        {
            int imgWidth = (gModeRight - gModeLeft)/10;
            int leftBorder = gModeLeft / 10;
            var hist = new Bitmap(imgWidth, 100, PixelFormat.Format24bppRgb);
            Pen blackPen = new Pen(Color.Black, 1);

            using (var g = Graphics.FromImage(hist))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, imgWidth, 100);

                for (int i = 0; i < imgWidth; i++)
                {
                    g.DrawLine(blackPen, i, 0, i, gMode[leftBorder + i]);
                }
            }

            hist.RotateFlip(RotateFlipType.Rotate180FlipX);
            hist = new Bitmap(hist, new Size(200, 100));

            pictureBox2.Image = hist;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (isRunning) 
            {
                thermalThread.Suspend();
                button3.Text = "START";
            }
            else
            {
                thermalThread.Resume();
                button3.Text = "STOP";
            }
            isRunning = !isRunning;
        }

        private string rawToTemp(int val)
        {
            double tempVal = 0;

            tempVal = (double)(val - 5950) / 40 + 273.15;//K

            switch (tempUnit)
            {
                case "C":
                    tempVal = tempVal - 273.15;//C
                    break;
                case "F":
                    tempVal = tempVal * 9 / 5 - 459.67;//F
                    break;
            }

            return tempVal.ToString("F1", CultureInfo.InvariantCulture);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            usignExternalCal = false;
        }

        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            if (rbUnitsK.Checked) tempUnit = "K";
            if (rbUnitsC.Checked) tempUnit = "C";
            if (rbUnitsF.Checked) tempUnit = "F";
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (lastUsableFrame != null)
            {
                string minTemp = rawToTemp(gModeLeft);
                string maxTemp = rawToTemp(gModeRight);

                lblSliderMin.Text = minTemp;
                lblSliderMax.Text = maxTemp;
                lblMinTemp.Text = minTemp;
                lblMaxTemp.Text = maxTemp;

                lblLeft.Text = gModeLeft.ToString();
                lblRight.Text = gModeRight.ToString();
                label2.Text = maxTempRaw.ToString();

                if (autoRange)//set sliders position
                {
                    trackBar2.Value = gModeLeft;
                    trackBar1.Value = gModeRight;
                }

                bitmap_data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                Marshal.Copy(imgBuffer, 0, bitmap_data.Scan0, imgBuffer.Length);
                bitmap.UnlockBits(bitmap_data);

                //crop image to 206x156
                croppedBitmap = cropFilter.Apply(bitmap);

                //upscale 200 %
                bigBitmap = bilinearResize.Apply(croppedBitmap);

                //sharpen image
                if (SharpenImage) sfilter.ApplyInPlace(bigBitmap);

                pictureBox3.Image = bigBitmap;

                if (firstAfterCal)
                {
                    firstAfterCal = false;
                    pictureBox5.Image = bigBitmap;
                    if (autoSaveImg) bigBitmap.Save(localPath + @"\export\seek_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss_fff") + ".png");
                }

                DrawHistogram();
            }

        }

    }
}
