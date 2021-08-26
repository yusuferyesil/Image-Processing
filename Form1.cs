using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Odev_1
{
    public partial class Form1 : MetroForm
    {
        Rectangle rect;
        Point startLocation;
        Point endLocation;
        bool isMouseDown = false;
        bool paint = false;
        private Random rnd = new Random();
        int[] centroid;
        List<List<int>> matrix = new List<List<int>>();
        bool kflag;
        int[,] colorcentroid;
        List<List<string>> trix = new List<List<string>>();



        public Form1()
        {
            InitializeComponent();
        }

        public delegate void ReportProgressDelegate(int percentage);

        public int[,] initializehist(int[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = 0;
                }
            }
            return array;
        }
        public int[,] getHistogram(Bitmap bmp)
        {
            //Gri seviye için olabilir.
            int[,] histogram = new int[256, 3];
            histogram = initializehist(histogram);
            Color color;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    color = bmp.GetPixel(i, j);
                    histogram[color.R, 0]++;
                    histogram[color.G, 1]++;
                    histogram[color.B, 2]++;
                }
            }
            return histogram;
        }
        public string[] distc(int[,] lbl)
        {
            string[] strarr = new string[lbl.GetLength(0)];
            string[] distcstrarr = new string[lbl.GetLength(0)];
            string str;
            for (int i = 0; i < lbl.GetLength(0); i++)
            {
                str = "";
                str = lbl[i, 0] + "," + lbl[i, 1] + "," + lbl[i, 2];
                strarr[i] = str;
            }
            distcstrarr = strarr.Distinct().ToArray();
            return distcstrarr;
        }

        public int[,] stringToInt(List<string> str)
        {
            int[,] result = new int[str.Count, 3];
            int i = 0;
            foreach (var st in str)
            {
                string[] tmp = st.Split(',');
                result[i, 0] = Convert.ToInt16(tmp[0]);
                result[i, 1] = Convert.ToInt16(tmp[1]);
                result[i, 2] = Convert.ToInt16(tmp[2]);
                i++;
            }
            return result;
        }

        public int[,] SstringToInt(string[] str)
        {
            int[,] result = new int[str.Length, 3];
            for (int i = 0; i < str.Length; i++)
            {
                string[] tmp = str[i].Split(',');
                result[i, 0] = Convert.ToInt16(tmp[0]);
                result[i, 1] = Convert.ToInt16(tmp[1]);
                result[i, 2] = Convert.ToInt16(tmp[2]);
            }
            return result;
        }

        public void findCColorNearest(int[,] labelArray, int width, int height, int km)
        {
            string denem = "";
            string str = null;
            int min, tmp, k = 0, counter = 0;
            double deneme;
            for (int i = 0; i < labelArray.GetLength(0); i++)
            {
                denem = "";
                int[,] denemelabel = new int[labelArray.GetLength(0), 3];
                min = 99999;
                int ri = labelArray[i, 0];
                int gi = labelArray[i, 1];
                int bi = labelArray[i, 2];

                for (int j = 0; j < colorcentroid.GetLength(0); j++)
                {
                    int rc = colorcentroid[j, 0];
                    int gc = colorcentroid[j, 1];
                    int bc = colorcentroid[j, 2];
                    deneme = Math.Sqrt(((ri - rc) * (ri - rc)) + ((gi - gc) * (gi - gc)) + ((bi - bc) * (bi - bc)));
                    tmp = Convert.ToInt16(deneme);
                    if (min >= tmp)
                    {
                        denem = "";
                        denem = labelArray[i, 0] + "," + labelArray[i, 1] + "," + labelArray[i, 2];
                        trix[j].Remove(denem);
                        min = tmp;
                        k = j;
                    }
                }
                denem = labelArray[i, 0] + "," + labelArray[i, 1] + "," + labelArray[i, 2];
                trix[k].Add(denem);
            }
            if (counter > colorcentroid.GetLength(0))
            {
                kflag = true;
            }
        }

        public void findCCentroid(int[,] array, int centroidNumber)
        {
            int cnt = 0;
            int totalR = 0;
            int totalG = 0;
            int totalB = 0;
            int tmp = 0;
            cnt = 0;
            for (int i = 0; i < array.GetLength(0); i++)//sublistteki her elemanı topla
            {
                totalR += array[i, 0];
                totalG += array[i, 1];
                totalB += array[i, 2];
                tmp++;
            }
            if (totalR != 0)
            {

                totalR /= tmp;
                totalG /= tmp;
                totalB /= tmp;

                if (colorcentroid[centroidNumber, 0] != totalR)
                {
                    colorcentroid[centroidNumber, 0] = totalR;
                    cnt++;
                }
                if (colorcentroid[centroidNumber, 1] != totalG)
                {
                    colorcentroid[centroidNumber, 1] = totalG;
                    cnt++;
                }
                if (colorcentroid[centroidNumber, 2] != totalB)
                {
                    colorcentroid[centroidNumber, 2] = totalB;
                    cnt++;
                }
            }
            if (cnt > 0)
            {
                kflag = true;
            }
            else
            {
                kflag = false;
            }
        }
        public void ck_means(int[,] label, int k, int width, int height)
        {
            int[] centroid = new int[k + 1];
            int c = 0, i = 0, j = 0, count = 0;

            string[] distcolor = distc(label);
            int number = distcolor.Length / k;
            int temp = 0;

            for (j = 0; j < k; j++)//matrixe küme elemanları ekle
            {
                List<string> sublist = new List<string>();
                for (i = 0; i < number; i++)
                {
                    sublist.Add(distcolor[temp]);
                    temp++;
                }
                trix.Add(sublist);
            }
            int centroidNumber = 0;
            centroidNumber = 0;
            foreach (var a in trix)//a=matrxdeki listler yani sublist
            {
                count = 0;
                foreach (var sublist in a)
                {
                    count++;
                }

                findCCentroid(stringToInt(a), centroidNumber);
                centroidNumber++;
            }
            do
            {
                kflag = false;
                findCColorNearest(SstringToInt(distcolor), width, height, k);
                centroidNumber = 0;
                foreach (var a in trix)
                {
                    count = 0;
                    foreach (var sublist in a)
                    {
                        count++;
                    }
                    findCCentroid(stringToInt(a), centroidNumber);
                    centroidNumber++;
                    /*if (ct == 1000)
                    {
                        kflag = false;
                    }
                    ct++;*/
                }
            } while (kflag);
            centroidNumber = 0;
        }
        public void randomCentroid(int k)
        {
            for (int i = 0; i < k; i++)
            {
                colorcentroid[i, 0] = rnd.Next(256);
                colorcentroid[i, 1] = rnd.Next(256);
                colorcentroid[i, 2] = rnd.Next(256);
            }
        }
        public Bitmap colorDetect(Bitmap bmp, int k)
        {
            int i, j;
            int[,] label = new int[bmp.Width * bmp.Height, 3];
            int tmp = 0;
            Color color;

            for (i = 0; i < bmp.Width; i++)//label gri renk değerleri ile doldur
            {
                for (j = 0; j < bmp.Height; j++)
                {
                    color = bmp.GetPixel(i, j);
                    label[tmp, 0] = color.R;
                    label[tmp, 1] = color.G;
                    label[tmp, 2] = color.B;
                    tmp++;
                }
            }
            string[] temp = distc(label);
            randomCentroid(k);
            Bitmap result = new Bitmap(bmp.Width, bmp.Height);
            int r, g, b;
            int colornum = k + 1;
            Color[] color2 = new Color[colornum];
            for (j = 0; j < colornum; j++)
            {
                r = rnd.Next(256);
                g = rnd.Next(256);
                b = rnd.Next(256);
                color2[j] = Color.FromArgb(r, g, b);
            }

            ck_means(label, k, bmp.Width, bmp.Height);//int[,] label, int k, int width, int height
            int c = 0;
            int[,] lbl = new int[bmp.Width, bmp.Height];
            string[] den;
            foreach (var sublist in trix)
            {
                foreach (var value in sublist)
                {
                    den = value.Split(',');
                    tmp = 0;
                    for (i = 0; i < bmp.Width; i++)
                    {
                        for (j = 0; j < bmp.Height; j++)
                        {
                            if (label[tmp, 0] == Convert.ToInt16(den[0]) && label[tmp, 1] == Convert.ToInt16(den[1]) && label[tmp, 2] == Convert.ToInt16(den[2]))
                            {
                                result.SetPixel(i, j, color2[c]);
                            }
                            tmp++;
                        }
                    }
                }
                c++;
            }
            trix.Clear();
            return result;
        }
        public void findNearest(int[] labelArray)
        {
            int min, tmp, k = 0, counter = 0;
            double deneme;
            for (int i = 0; i < labelArray.Length; i++)
            {
                min = 99999;
                for (int j = 0; j < centroid.Length; j++)
                {
                    deneme = Math.Sqrt((centroid[j] - labelArray[i]) * (centroid[j] - labelArray[i]));
                    tmp = Convert.ToInt16(deneme);
                    //tmp = Math.Abs(centroid[j] - labelArray[i]);
                    if (min >= tmp)
                    {
                        matrix[j].Remove(labelArray[i]);
                        min = tmp;
                        k = j;
                        counter++;
                    }
                }
                matrix[k].Add(labelArray[i]);// matrix[]=centroidlerin kümelenmiş hali
            }
            if (counter > centroid.Length)
            {
                Console.WriteLine("" + counter);
                kflag = true;
            }
        }
        public void findCentroid(List<int> array, int centroidNumber, int lenght)
        {
            int total = 0;
            foreach (int i in array)//sublistteki her elemanı topla
            {
                total += i;//i nın ne oldupuna bak dizi mi dizinin içerisindeki bilgimi
                           //MessageBox.Show(i+"");
            }
            if (total != 0)
            {
                //MessageBox.Show(centroidNumber + "");
                centroid[centroidNumber] = total /= lenght;
            }

        }

        public void k_means(int[,] label, int k, int width, int height)
        {
            int[] centroid = new int[k + 1];
            int c = 0, i = 0, j = 0, count = 0;
            int[] labelArray = new int[width * height];
            c = 0;
            for (i = 0; i < width; i++)//labelarrayı label değerleri ile doldur
            {
                for (j = 0; j < height; j++)
                {
                    labelArray[c] = label[i, j];
                    c++;
                }
            }
            Array.Sort(labelArray);
            int[] distcolor = labelArray.Distinct().ToArray();
            int number = distcolor.Length / k;
            int temp = 0;

            for (j = 0; j < k; j++)//matrixe küme elemanları ekle
            {
                List<int> sublist = new List<int>();
                for (i = 0; i < number; i++)
                {
                    sublist.Add(distcolor[temp]);
                    temp++;
                }
                matrix.Add(sublist);
            }

            /*foreach (var sublist in matrix)
            {
                foreach (var value in sublist)
                {
                    Console.Write(value);
                    Console.Write(' ');
                }
                Console.WriteLine();
                Console.WriteLine(sublist.Count);
                Console.WriteLine();
            }*/
            int centroidNumber = 0;
            centroidNumber = 0;
            foreach (var a in matrix)//a=matrxdeki listler yani sublist
            {
                count = 0;
                foreach (var sublist in a)
                {
                    count++;
                }

                findCentroid(a, centroidNumber, count);
                centroidNumber++;
            }
            int ct = 0;
            do
            {
                kflag = false;
                //findNearest;
                //findCentroid;
                findNearest(distcolor);
                centroidNumber = 0;
                foreach (var a in matrix)
                {
                    count = 0;
                    foreach (var sublist in a)
                    {
                        count++;
                    }
                    findCentroid(a, centroidNumber, count);
                    centroidNumber++;
                    if (ct == 1000)
                    {
                        kflag = false;
                    }
                    ct++;
                }
            } while (kflag);
            centroidNumber = 0;
        }

        public Bitmap grayDetect(Bitmap bmp, int k)
        {
            int i, j;
            int[,] label = new int[bmp.Width, bmp.Height];
            int tmp = 0;
            Color color;

            for (i = 0; i < bmp.Width; i++)//label gri renk değerleri ile doldur
            {
                for (j = 0; j < bmp.Height; j++)
                {
                    color = bmp.GetPixel(i, j);
                    label[i, j] = color.R;
                }
            }

            Bitmap result = new Bitmap(bmp.Width, bmp.Height);
            int r, g, b;
            int colornum = k + 1;
            Color[] color2 = new Color[colornum];
            for (j = 0; j < colornum; j++)
            {
                r = rnd.Next(256);
                g = rnd.Next(256);
                b = rnd.Next(256);
                color2[j] = Color.FromArgb(r, g, b);
            }
            k_means(label, k, bmp.Width, bmp.Height);//int[,] label, int k, int width, int height
            int c = 0;
            foreach (var sublist in matrix)
            {
                foreach (var value in sublist)
                {
                    for (i = 0; i < bmp.Width; i++)
                    {
                        for (j = 0; j < bmp.Height; j++)
                        {
                            if (label[i, j] == value)
                            {
                                result.SetPixel(i, j, color2[c]);
                            }
                        }
                    }
                }
                c++;
            }
            foreach (var sublist in matrix)
            {
                foreach (var value in sublist)
                {
                    Console.Write(value);
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
            matrix.Clear();
            return result;
        }

        public Bitmap labeling(Bitmap bmp, float[] Matris)
        {
            Bitmap result = new Bitmap(bmp.Width, bmp.Height);
            int minLabel = 0;
            int pixelNumber = bmp.Width * bmp.Height;
            int x, y, i, j, label = 0;
            int[,] LabelNumber = new int[bmp.Width, bmp.Height];
            for (x = 0; x < bmp.Width; x++)
            {
                for (y = 0; y < bmp.Height; y++)
                {
                    LabelNumber[x, y] = 0;
                }
            }
            int first = 0, last = 0;
            bool flag = false;

            do
            {
                flag = false;
                for (y = 1; y < bmp.Height - 1; y++)
                {
                    for (x = 1; x < bmp.Width - 1; x++)
                    {
                        if (bmp.GetPixel(x, y).R == 255)
                        {
                            first = LabelNumber[x, y];
                            minLabel = 0;
                            int temp = 0;
                            for (j = -1; j <= 1; j++)
                            {
                                for (i = -1; i <= 1; i++)
                                {
                                    if (Matris[temp] == 255)
                                    {
                                        if (LabelNumber[x + i, y + j] != 0 && minLabel == 0)
                                        {
                                            minLabel = LabelNumber[x + i, y + j];
                                        }
                                        else if (LabelNumber[x + i, y + j] < minLabel && LabelNumber[x + i, y + j] != 0 && minLabel != 0)
                                        {
                                            minLabel = LabelNumber[x + i, y + j];
                                        }
                                    }
                                    temp++;
                                }
                            }
                            if (minLabel != 0)
                            {
                                LabelNumber[x, y] = minLabel;
                            }
                            else if (minLabel == 0)
                            {
                                label = label + 1;
                                LabelNumber[x, y] = label;
                            }
                            last = LabelNumber[x, y];
                            if (first != last)
                            {
                                flag = true;
                            }
                        }
                    }
                }
            } while (flag);

            int[] labelArray = new int[pixelNumber];
            i = 0;
            for (x = 1; x < bmp.Width - 1; x++)
            {
                for (y = 1; y < bmp.Height - 1; y++)
                {
                    i++;
                    labelArray[i] = LabelNumber[x, y];
                }
            }
            Array.Sort(labelArray);

            int[] distcolor = labelArray.Distinct().ToArray();
            int[] colors = new int[distcolor.Length];

            for (j = 0; j < distcolor.Length; j++)
            {
                colors[j] = distcolor[j];
            }

            int colornum = colors.Length;
            Color[] colors2 = new Color[colornum];
            int r, g, b;
            for (j = 0; j < colornum; j++)
            {
                r = rnd.Next(256);
                g = rnd.Next(256);
                b = rnd.Next(256);
                colors2[j] = Color.FromArgb(r, g, b);
            }

            for (x = 1; x < bmp.Width - 1; x++)
            {
                for (y = 1; y < bmp.Height - 1; y++)
                {
                    Console.WriteLine(LabelNumber[x, y] + "");
                    int colorsort = Array.IndexOf(colors, LabelNumber[x, y]);
                    if (bmp.GetPixel(x, y).R == 0)
                    {
                        result.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    }
                    else
                    {
                        result.SetPixel(x, y, colors2[colorsort]);
                    }
                }
            }

            return result;
        }
        public Bitmap erosion(Bitmap bmp, float[] Matris)
        {
            int mSize = Convert.ToInt32(Math.Sqrt(Matris.Length));
            float totalMatris = 0;
            Color color;
            Bitmap result = new Bitmap(bmp.Width, bmp.Height);
            int x, y;
            int R;
            for (int i = 0; i < Matris.Length; i++)
            {
                totalMatris += Matris[i];
            }
            for (x = (mSize - 1) / 2; x < bmp.Width - (mSize - 1) / 2; x++)
            {
                for (y = (mSize - 1) / 2; y < bmp.Height - (mSize - 1) / 2; y++)
                {
                    int counter = 0;
                    int tmp = 0;
                    for (int i = -((mSize - 1) / 2); i <= (mSize - 1) / 2; i++)
                    {
                        for (int j = -((mSize - 1) / 2); j <= (mSize - 1) / 2; j++)
                        {

                            color = bmp.GetPixel(x + i, y + j);
                            if (color.R == Matris[tmp] && Matris[tmp] == 255)
                            {
                                counter++;
                            }
                            tmp++;
                        }
                    }

                    if (counter == 5)
                    {
                        R = bmp.GetPixel(x, y).R;
                        result.SetPixel(x, y, Color.FromArgb(R, R, R));
                    }
                    else
                    {
                        result.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    }
                }
            }
            return result;
        }

        public Bitmap dilation(Bitmap bmp, float[] Matris)
        {
            int mSize = Convert.ToInt32(Math.Sqrt(Matris.Length));
            float totalMatris = 0;
            Color color, color2;
            Bitmap result = new Bitmap(bmp.Width, bmp.Height);
            int x, y;
            for (int i = 0; i < Matris.Length; i++)
            {
                totalMatris += Matris[i];
            }
            for (x = (mSize - 1) / 2; x < bmp.Width - (mSize - 1) / 2; x++)
            {
                for (y = (mSize - 1) / 2; y < bmp.Height - (mSize - 1) / 2; y++)
                {
                    color2 = bmp.GetPixel(x, y);
                    if (color2.R == 255)
                    {
                        int tmp = 0;
                        for (int i = -((mSize - 1) / 2); i <= (mSize - 1) / 2; i++)
                        {
                            for (int j = -((mSize - 1) / 2); j <= (mSize - 1) / 2; j++)
                            {

                                color = bmp.GetPixel(x + i, y + j);
                                if (color.R == 0 && Matris[tmp] == 255)
                                {
                                    result.SetPixel(x + i, y + j, Color.FromArgb(255, 255, 255));
                                }
                                else if (color.R == 255)
                                {
                                    result.SetPixel(x + i, y + j, Color.FromArgb(255, 255, 255));
                                }
                                else
                                {
                                    result.SetPixel(x + i, y + j, Color.FromArgb(0, 0, 0));
                                }


                                tmp++;
                            }
                        }
                    }
                    else
                    {
                        result.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    }

                }
            }
            return result;
        }
        public Bitmap blackWhite(Bitmap bmp)
        {
            int[] arr = new int[225];
            int i = 0;
            Color p;
            Bitmap result = new Bitmap(bmp.Width, bmp.Height);

            //Grayscale
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    p = bmp.GetPixel(x, y);
                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;
                    int avg = (r + g + b) / 3;
                    avg = avg < 128 ? 0 : 255;     // Converting gray pixels to either pure black or pure white
                    result.SetPixel(x, y, Color.FromArgb(a, avg, avg, avg));
                }
            }
            return result;
        }

        public Bitmap Sobel(Bitmap bmp, float[] Matris1, float[] Matris2)
        {
            int mSize = Convert.ToInt32(Math.Sqrt(Matris1.Length));
            float totalMatris = 0;
            Color color;
            Bitmap result = new Bitmap(bmp.Width, bmp.Height);
            int x, y;
            float totalR1, totalR2;
            int R;
            for (int i = 0; i < Matris1.Length; i++)
            {
                totalMatris += Matris1[i];
            }
            for (x = (mSize - 1) / 2; x < bmp.Width - (mSize - 1) / 2; x++)
            {
                for (y = (mSize - 1) / 2; y < bmp.Height - (mSize - 1) / 2; y++)
                {
                    totalR1 = 0;
                    totalR2 = 0;

                    int tmp = 0;
                    for (int i = -((mSize - 1) / 2); i <= (mSize - 1) / 2; i++)
                    {
                        for (int j = -((mSize - 1) / 2); j <= (mSize - 1) / 2; j++)
                        {
                            color = bmp.GetPixel(x + i, y + j);
                            totalR1 += color.R * Matris1[tmp];
                            totalR2 += color.R * Matris2[tmp];

                            R = (int)(Math.Abs(totalR1) + Math.Abs(totalR2));

                            if (R > 255)
                            {
                                R = 255;
                            }
                            if (R < 0)
                            {
                                R = 0;
                            }
                            result.SetPixel(x, y, Color.FromArgb(R, R, R));
                            tmp++;
                        }
                    }
                }
            }
            return result;
        }
        public Bitmap Convolution(Bitmap bmp, float[] Matris)
        {
            int mSize = Convert.ToInt32(Math.Sqrt(Matris.Length));
            float totalMatris = 0;
            Color color;
            Bitmap result = new Bitmap(bmp.Width, bmp.Height);
            int x, y;
            float totalR, totalG, totalB;
            int R, G, B;
            for (int i = 0; i < Matris.Length; i++)
            {
                totalMatris += Matris[i];
            }
            for (x = (mSize - 1) / 2; x < bmp.Width - (mSize - 1) / 2; x++)
            {
                for (y = (mSize - 1) / 2; y < bmp.Height - (mSize - 1) / 2; y++)
                {
                    totalR = 0;
                    totalG = 0;
                    totalB = 0;

                    int tmp = 0;
                    for (int i = -((mSize - 1) / 2); i <= (mSize - 1) / 2; i++)
                    {
                        for (int j = -((mSize - 1) / 2); j <= (mSize - 1) / 2; j++)
                        {
                            color = bmp.GetPixel(x + i, y + j);
                            totalR += color.R * Matris[tmp];
                            totalG += color.G * Matris[tmp];
                            totalB += color.B * Matris[tmp];

                            R = Convert.ToInt16(totalR / totalMatris);
                            G = Convert.ToInt16(totalG / totalMatris);
                            B = Convert.ToInt16(totalB / totalMatris);

                            if (R > 255)
                            {
                                R = 255;
                            }
                            if (G > 255)
                            {
                                G = 255;
                            }
                            if (B > 255)
                            {
                                B = 255;
                            }
                            if (R < 0)
                            {
                                R = 0;
                            }
                            if (G < 0)
                            {
                                G = 0;
                            }
                            if (B < 0)
                            {
                                B = 0;
                            }
                            result.SetPixel(x, y, Color.FromArgb(R, G, B));
                            tmp++;
                        }
                    }
                }
            }
            return result;
        }
        public Bitmap MedianFiltering(Bitmap bm, int size, ReportProgressDelegate reportProgress)
        {
            List<byte> termsList = new List<byte>();
            double width = bm.Width;
            double percentage = 100 / width;
            percentage = Math.Round(percentage, 2);
            bm = convertGray(bm);
            for (int i = 0; i <= bm.Width - size; i++)
            {
                for (int j = 0; j <= bm.Height - size; j++)
                {
                    for (int x = i; x <= i + (size - 1); x++)
                        for (int y = j; y <= j + (size - 1); y++)
                        {
                            termsList.Add(bm.GetPixel(x, y).R);
                        }
                    byte[] terms = termsList.ToArray();
                    termsList.Clear();
                    Array.Sort<byte>(terms);
                    Array.Reverse(terms);
                    byte color = terms[(size * size) / 2];
                    bm.SetPixel(i + 1, j + 1, Color.FromArgb(color, color, color));
                }
                double temp = percentage * i;
                backgroundWorker1.ReportProgress(Convert.ToInt32(temp));
            }
            return bm;
        }
        public Bitmap convertGray(Bitmap bmp)
        {
            Bitmap result = new Bitmap(bmp.Width, bmp.Height);
            int temp;
            Color color;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    color = bmp.GetPixel(i, j);
                    temp = Convert.ToInt16(color.R * 0.3) + Convert.ToInt16(color.G * 0.59) + Convert.ToInt16(color.B * 0.11);
                    color = Color.FromArgb(temp, temp, temp);
                    result.SetPixel(i, j, color);
                }
            }
            return result;
        }
        public void forwardbtn()
        {
            tabControl.SelectedIndex = tabControl.SelectedIndex + 1;
        }
        public void backbtn()
        {
            tabControl.SelectedIndex = tabControl.SelectedIndex - 1;
        }
        private void forwardBtn_Click(object sender, EventArgs e)
        {
            preImg.Image = openImg.Image;
            forwardbtn();
        }

        private void allowRad_CheckedChanged(object sender, EventArgs e)
        {
            if (allowRad.Checked)
            {
                preList.Enabled = true;
            }
        }

        private void disallowRad_CheckedChanged(object sender, EventArgs e)
        {
            if (disallowRad.Checked)
            {
                preList.Enabled = false;
            }
        }

        private void forwardBtn2_Click(object sender, EventArgs e)
        {
            filterImg.Image = preImg.Image;
            forwardbtn();
        }

        private void backBtn1_Click(object sender, EventArgs e)
        {
            backbtn();
        }

        private void forwardBtn3_Click(object sender, EventArgs e)
        {
            morpImg.Image = filterImg.Image;
            forwardbtn();
        }

        private void BackBtn2_Click(object sender, EventArgs e)
        {
            backbtn();
        }

        private void filterDisRad_CheckedChanged(object sender, EventArgs e)
        {
            if (filterDisRad.Checked)
            {
                filterList.Enabled = false;
            }
        }

        private void filterRad_CheckedChanged(object sender, EventArgs e)
        {
            if (filterRad.Checked)
            {
                filterList.Enabled = true;
            }
        }

        private void morpRad_CheckedChanged(object sender, EventArgs e)
        {
            if (morpRad.Checked)
            {
                morpList.Enabled = true;
            }
        }

        private void morpDisRad_CheckedChanged(object sender, EventArgs e)
        {
            if (morpDisRad.Checked)
            {
                morpList.Enabled = false;
            }
        }

        private void forwardBtn4_Click(object sender, EventArgs e)
        {
            segImg.Image = morpImg.Image;
            forwardbtn();
        }

        private void backBtn3_Click(object sender, EventArgs e)
        {
            backbtn();
        }

        private void segRad_CheckedChanged(object sender, EventArgs e)
        {
            if (segRad.Checked)
            {
                segList.Enabled = true;
            }
        }

        private void segDisRad_CheckedChanged(object sender, EventArgs e)
        {
            if (segDisRad.Checked)
            {
                segList.Enabled = false;
            }
        }

        private void forwardBtn5_Click(object sender, EventArgs e)
        {
            saveImg.Image = segImg.Image;
            forwardbtn();
        }

        private void backBtn4_Click(object sender, EventArgs e)
        {
            backbtn();
        }

        private void backBtn5_Click(object sender, EventArgs e)
        {
            backbtn();
        }

        private void preImg_Paint(object sender, PaintEventArgs e)
        {
            Pen greenPen = new Pen(Color.Red, 5);
            if (rect != null && paint)
            {
                e.Graphics.DrawRectangle(greenPen, GetRect());
            }
        }
        private Rectangle GetRect()
        {
            rect = new Rectangle();
            rect.X = Math.Min(startLocation.X, endLocation.X);
            rect.Y = Math.Min(startLocation.Y, endLocation.Y);
            rect.Width = Math.Abs(startLocation.X - endLocation.X);
            rect.Height = Math.Abs(startLocation.Y - endLocation.Y);
            return rect;
        }

        private void preImg_MouseDown(object sender, MouseEventArgs e)
        {
            paint = true;
            isMouseDown = true;
            startLocation = e.Location;

        }

        private void preImg_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown == true)
            {
                endLocation = e.Location;
                Refresh();
            }
        }

        private void preImg_MouseUp(object sender, MouseEventArgs e)
        {
            if (isMouseDown == true)
            {
                endLocation = e.Location;
                isMouseDown = false;
            }
        }

        private void openBtn_Click(object sender, EventArgs e)
        {
            //Create a new instance of openFileDialog
            OpenFileDialog res = new OpenFileDialog();

            //Filter
            res.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...";

            //When the user select the file
            if (res.ShowDialog() == DialogResult.OK)
            {
                //Get the file's path
                var filePath = res.FileName;
                //Do something
                openImg.Image = new Bitmap(res.FileName);
            }
        }

        private void forwardBtn6_Click(object sender, EventArgs e)
        {
            ImageFormat format = ImageFormat.Png;
            string ext = saveList.SelectedItem.ToString();
            switch (ext)
            {
                case ".jpg":
                    format = ImageFormat.Jpeg;
                    break;
                case ".bmp":
                    format = ImageFormat.Bmp;
                    break;
                case ".png":
                    format = ImageFormat.Png;
                    break;
            }
            saveImg.Image.Save(@"C:\Users\yerye\source\repos\Odev_1\image." + format.ToString());
        }

        private void preList_SelectedIndexChanged(object sender, EventArgs e)
        {
            histText.Visible = false;
            cropBtn.Visible = false;
            int i = preList.SelectedIndex;
            switch (i)
            {
                case 0:
                    preImg.Image = convertGray((Bitmap)preImg.Image);
                    break;
                case 1:
                    preImg.Enabled = true;
                    cropBtn.Visible = true;
                    paint = true;
                    break;
                case 2:
                    //int[,] hist = new int[256, 3];
                    var hist = getHistogram((Bitmap)preImg.Image);
                    histText.Visible = true;
                    for (int a = 0; a < hist.GetLength(0); a++)
                    {
                        histText.AppendText("R:" + hist[a, 0] + ", G:" + hist[a, 1] + " B:" + hist[a, 2]);
                        histText.AppendText("\n");
                    }
                    break;
            }
        }

        private void cropBtn_Click(object sender, EventArgs e)
        {
            preImg.Image = crop((Bitmap)preImg.Image);
            paint = false;
        }
        public Bitmap crop(Bitmap bmp)
        {
            Bitmap tmp = new Bitmap(Math.Abs(startLocation.X - endLocation.X), Math.Abs(startLocation.Y - endLocation.Y));
            int tmpi, tmpj;
            tmpi = 0;
            tmpj = 0;
            for (int i = startLocation.X; i < endLocation.X; i++)
            {

                for (int j = startLocation.Y; j < endLocation.Y; j++)
                {
                    tmp.SetPixel(tmpi, tmpj, bmp.GetPixel(i, j));
                    tmpj++;
                }
                tmpi++;
                tmpj = 0;
            }
            Refresh();
            return tmp;
        }

        private void filterList_SelectedIndexChanged(object sender, EventArgs e)
        {
            medianSizeLbl.Visible = false;
            medianSizeTxt.Visible = false;
            medianBtn.Visible = false;
            medianLabel.Visible = false;
            medianProg.Visible = false;
            int i = filterList.SelectedIndex;
            switch (i)
            {
                case 0:
                    medianSizeLbl.Visible = true;
                    medianSizeTxt.Visible = true;
                    medianBtn.Visible = true;
                    medianLabel.Visible = true;
                    medianProg.Visible = true;
                    /*float[] mMatris = { 0.11F, 0.11F, 0.11F, 0.11F, 0.11F, 0.11F, 0.11F, 0.11F, 0.11F };//konvolüsyon ile yumuşatma yapılıştır efendim
                    filterImg.Image = Convolution((Bitmap)filterImg.Image, mMatris);*/
                    break;
                case 1:
                    //filterImg.Image = sharpen((Bitmap)filterImg.Image);
                    //filterImg.Image = imgSharp((Bitmap)filterImg.Image);
                    float[] Matris = { 0, -2, 0, -2, 11, -2, 0, -2, 0 };
                    filterImg.Image = Convolution((Bitmap)filterImg.Image, Matris);
                    break;
                case 2:
                    float[] sobelMatris1 = { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
                    float[] sobelMatris2 = { 1, 2, 1, 0, 0, 0, -1, -2, -1 };
                    filterImg.Image = Sobel((Bitmap)filterImg.Image, sobelMatris1, sobelMatris2);
                    break;
            }
        }

        private void medianBtn_Click(object sender, EventArgs e)
        {
            backgroundWorker1.WorkerReportsProgress = true;
            //backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            //backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            // backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.RunWorkerAsync();
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //BackgroundWorker backgroundWorker1 = sender as BackgroundWorker;
            filterImg.Image = MedianFiltering((Bitmap)preImg.Image, Convert.ToInt16(medianSizeTxt.Text), backgroundWorker1.ReportProgress);
        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            medianProg.Value = e.ProgressPercentage;
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("İşleminiz Tamamlandı");
        }

        private void morpList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = morpList.SelectedIndex;
            switch (i)
            {
                case 0:
                    float[] mMatris = { 255, 255, 255, 255, 255, 255, 255, 255, 255 };
                    morpImg.Image = dilation(blackWhite(convertGray((Bitmap)morpImg.Image)), mMatris);
                    break;
                case 1:
                    float[] Matris = { 0, 255, 0, 255, 255, 255, 0, 255, 0 };
                    morpImg.Image = erosion(blackWhite(convertGray((Bitmap)morpImg.Image)), Matris);
                    break;
            }
        }

        private void segList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = segList.SelectedIndex;
            switch (i)
            {
                case 0:
                    float[] Matris = { 255, 255, 255, 255, 255, 255, 255, 255, 255 };
                    segImg.Image = labeling(blackWhite((Bitmap)segImg.Image), Matris);
                    break;
                case 1:
                    matrix.Add(new List<int>());
                    int k = Convert.ToInt16(textBox1.Text);
                    k = k + 1;
                    centroid = new int[k + 1];
                    segImg.Image = grayDetect(convertGray((Bitmap)segImg.Image), k);
                    break;
                case 2:
                    int t = Convert.ToInt16(textBox1.Text);
                    t = t + 1;
                    colorcentroid = new int[t, 3];
                    segImg.Image = colorDetect((Bitmap)segImg.Image, t);
                    break;
            }
        }
    }
}