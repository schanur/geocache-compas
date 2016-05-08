using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace geoc
{
    class Navigation
    {
        private int xResolution;
        private int yResolution;
        private int smallestResolution;
        //private int borderSize;

        private int degrees = -1;
        private int degreesPainted = -1;
        //private int 

        private bool statusChanged = false;

        //Image navigationArrowImg;
        Bitmap navigationMasterBmp;
        Bitmap navigationArrowBmp;

        /*private double[,] arrowMasterCoordinates = new double[8, 4] {{   0,  65,  50,  20}, 
                                                                     {  50,  20,  20,  20},
                                                                     {  20,  20,  20, -55},
                                                                     {  20, -55,   0, -30},
                                                                     {   0, -30, -20, -55},
                                                                     { -20, -55, -20,  20},
                                                                     { -20,  20, -50,  20},
                                                                     { -50,  20,   0,  65}};*/
        private double[,] arrowMasterCoordinates = new double[8, 4] {{   0,  26,  20,   8}, 
                                                                     {  20,   8,   8,   8},
                                                                     {   8,   8,   8, -22},
                                                                     {   8, -22,   0, -12},
                                                                     {   0, -12,  -8, -22},
                                                                     {  -8, -22,  -8,   8},
                                                                     {  -8,   8, -20,   8},
                                                                     { -20,   8,   0,  26}};
        private double[,] arrowRotCoordinates = new double[8, 4];
                                            

        private void initArrowCoordinates() 
        {
            //arrowMasterCoordinates[1] = {50, 75,100, 75};
        }

        public Navigation(int xInitResolution, int yInitResolution)
        {
            int i;
            int j;
            
            xResolution = xInitResolution;
            yResolution = yInitResolution;

            if (xResolution > yResolution)
            {
                smallestResolution = yResolution;
            }
            else
            {
                smallestResolution = xResolution;
            }
            //borderSize = 3;

            navigationMasterBmp = new Bitmap(xResolution, yResolution);
            navigationArrowBmp = new Bitmap(xResolution, yResolution);

            for (i = 0; i < xResolution; i++)
            {
                for (j = 0; j < yResolution; j++)
                {
                    navigationMasterBmp.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                }
            }
            navigationArrowBmp = navigationMasterBmp;
        }

        public bool StatusChanged() {
            return statusChanged;
        }

        public void SetDegrees(int newDegrees) 
        {
            if (newDegrees != degrees)
            {
                degrees = newDegrees;
                statusChanged = true;
            }
        }

        private void swap(ref int val1, ref int val2)
        {
            int swap = val1;
            val1 = val2;
            val2 = swap;
        }

        private void sort(ref int val1, ref int val2)
        {
            if (val2 < val1)
            {
                swap(ref val1, ref val2);
            }
        }

        private void ClearBitmap()
        {
            int i;
            int j;
            Color col = Color.FromArgb(255, 0, 0);
            for (i = 0; i < yResolution; i++)
            {
                for (j = 0; j < xResolution; j++)
                {
                    navigationArrowBmp.SetPixel(i, j, col);
                }
            }
        }

        private void DrawLine(int x1, int y1, int x2, int y2, Color lineColor)
        {
            int i;
            
            int xDiff = x2 - x1;
            int yDiff = y2 - y1;
            int xAbsDiff = System.Math.Abs(xDiff);
            int yAbsDiff = System.Math.Abs(yDiff);
           
            if (xDiff == 0) 
            {
                sort(ref y1, ref y2);
                for (i = y1; i <= y2; i++)
                {
                    navigationArrowBmp.SetPixel(x1, i, lineColor);
                }
            }
            else if (yDiff == 0)
            {
                sort(ref x1, ref x2);
                for (i = x1; i <= x2; i++)
                {
                    navigationArrowBmp.SetPixel(i, y1, lineColor);
                }
            }
            else if (xAbsDiff < yAbsDiff)
            {
                if (y2 < y1) 
                {
                    swap(ref x1, ref x2);
                    swap(ref y1, ref y2);
                }

                for (i = 0; i <= yAbsDiff; i++)
                {
                    navigationArrowBmp.SetPixel(x1 + ((xDiff * i) / yDiff), i + y1, lineColor);
                }
            }
            else
            {
                if (x2 < x1)
                {
                    swap(ref x1, ref x2);
                    swap(ref y1, ref y2);
                }

                for (i = 0; i <= xAbsDiff; i++)
                {
                    navigationArrowBmp.SetPixel(i + x1, y1 + ((yDiff * i) / xDiff), lineColor); 
                }
            }
        }

        private void RotateCoord(ref double x, ref double y, int l_degrees)
        {
            double mult = System.Math.PI / 180;
            double newX = x * System.Math.Cos(l_degrees * mult) - y * System.Math.Sin(l_degrees * mult);
            y = y * System.Math.Cos(l_degrees * mult) + x * System.Math.Sin(l_degrees * mult);
            x = newX;
        }

        private void Coord2Point(double x, double y, ref int px, ref int py)
        {
            px = Convert.ToInt32(x + xResolution / 2);
            py = Convert.ToInt32(y + yResolution / 2);
            
        }

        private void FillForm()
        {
        }

        private void DrawArrow(int l_degrees, Color arrowColor)
        {
            
            double x1rot;
            double x2rot;
            double y1rot;
            double y2rot;
            int px1 = 0;
            int px2 = 0;
            int py1 = 0;
            int py2 = 0;

            int i;
            for (i = 0; i <= 7; i++)
            {
                x1rot = arrowMasterCoordinates[i, 0];
                y1rot = arrowMasterCoordinates[i, 1];
                x2rot = arrowMasterCoordinates[i, 2];
                y2rot = arrowMasterCoordinates[i, 3];
                RotateCoord(ref x1rot, ref y1rot, (l_degrees + 180) % 360);
                RotateCoord(ref x2rot, ref y2rot, (l_degrees + 180) % 360);
                Coord2Point(x1rot, y1rot, ref px1, ref py1);
                Coord2Point(x2rot, y2rot, ref px2, ref py2);
                
                
                DrawLine(px1, py1, px2, py2, arrowColor);
            }
            DrawLine(0, 0, 59, 0, Color.FromArgb(0, 0, 0));
            DrawLine(59, 0, 59, 59, Color.FromArgb(0, 0, 0));
            DrawLine(59, 59, 0, 59, Color.FromArgb(0, 0, 0));
            DrawLine(0, 59, 0, 0, Color.FromArgb(0, 0, 0));
        }

        public Image GetNavigationArrow()
        {
            if (statusChanged == true) 
            {
                // setze Bild zurueck auf den Zustand des Masters;
                DrawArrow(degreesPainted, Color.FromArgb(255, 255, 255));
                // Zeichnet neuen Pfeil
                DrawArrow(degrees, Color.FromArgb(0, 0, 0));
                degreesPainted = degrees;
            }

            return (Image)navigationArrowBmp;
        }

        public string FormatDistance(double d_dist_in_meters)
        {
            if (d_dist_in_meters >= 1000)
            {
                // show in kilometers
                d_dist_in_meters /= 10;
                int i_dist = Convert.ToInt32(d_dist_in_meters);
                string s_dist = i_dist.ToString();

                s_dist = s_dist.Substring(0, s_dist.Length - 2) + "," + s_dist.Substring(s_dist.Length - 2);
                return (s_dist + " km");
            }
            else
            {
                int i_dist = Convert.ToInt32(d_dist_in_meters);
                return (i_dist.ToString() + " m");
            }
        }
    }
}
