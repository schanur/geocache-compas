using System;
using System.Collections.Generic;
using System.Text;

namespace geoc
{
    public class NavigationCalculation
    {
        public class grad_minute_second
        {
            public double breite_grad;
            public double breite_min;
            public double breite_sec;
            public double laenge_grad;
            public double laenge_min;
            public double laenge_sec;

            public void Add(NavigationCalculation.grad_minute_second c_PositionToAdd)
            {
                breite_grad += c_PositionToAdd.breite_grad;
                breite_min += c_PositionToAdd.breite_min;
                breite_sec += c_PositionToAdd.breite_sec;
                laenge_grad += c_PositionToAdd.laenge_grad;
                laenge_min += c_PositionToAdd.laenge_min;
                laenge_sec += c_PositionToAdd.laenge_sec;
            }
            public void Sub(NavigationCalculation.grad_minute_second c_PositionToAdd)
            {
                breite_grad -= c_PositionToAdd.breite_grad;
                breite_min -= c_PositionToAdd.breite_min;
                breite_sec -= c_PositionToAdd.breite_sec;
                laenge_grad -= c_PositionToAdd.laenge_grad;
                laenge_min -= c_PositionToAdd.laenge_min;
                laenge_sec -= c_PositionToAdd.laenge_sec;
            }
            public void Mul(int i_MultiplicationFactor)
            {
                breite_grad *= i_MultiplicationFactor;
                breite_min *= i_MultiplicationFactor;
                breite_sec *= i_MultiplicationFactor;
                laenge_grad *= i_MultiplicationFactor;
                laenge_min *= i_MultiplicationFactor;
                laenge_sec *= i_MultiplicationFactor;
            }
            public void Div(int i_DivideFactor)
            {
                breite_grad /= i_DivideFactor;
                breite_min /= i_DivideFactor;
                breite_sec /= i_DivideFactor;
                laenge_grad /= i_DivideFactor;
                laenge_min /= i_DivideFactor;
                laenge_sec /= i_DivideFactor;
            }
        }

        private double entfernung;
        private double winkel;
        // x = Breite
        // y = Länge
        private double x_grad_erste;
        private double x_grad_zweite;
        private double x_minute_erste;
        private double x_minute_zweite;
        private double x_sekunde_erste;
        private double x_sekunde_zweite;
        public double x_dezgrad_erste;
        public double x_dezgrad_zweite;
        private double y_grad_erste;
        private double y_grad_zweite;
        private double y_minute_erste;
        private double y_minute_zweite;
        private double y_sekunde_erste;
        private double y_sekunde_zweite;
        public double y_dezgrad_erste;
        public double y_dezgrad_zweite;
        private double kurswinkel;


        public NavigationCalculation()
        {
            // alle Werte mit 0 initialisieren
            ResetAll();
        }

        // alle werte auf 0 setzen 
        public void ResetAll()
        {
            entfernung = 0;
            winkel = 0;
            x_grad_erste = 0;
            x_grad_zweite = 0;
            x_minute_erste = 0;
            x_minute_zweite = 0;
            x_sekunde_erste = 0;
            x_sekunde_zweite = 0;
            x_dezgrad_erste = 0;
            x_dezgrad_zweite = 0;
            y_grad_erste = 0;
            y_grad_zweite = 0;
            y_minute_erste = 0;
            y_minute_zweite = 0;
            y_sekunde_erste = 0;
            y_sekunde_zweite = 0;
            y_dezgrad_erste = 0;
            y_dezgrad_zweite = 0;
        }

        // GET Methoden
        public void SetPosStart(double breite_degrees, double breite_minutes, double laenge_degrees, double laenge_minutes)
        {
            x_grad_erste = breite_degrees;
            x_minute_erste = breite_minutes;
            x_dezgrad_erste = GradMinToDezigrad(breite_degrees, breite_minutes);
            y_grad_erste = laenge_degrees;
            y_minute_erste = laenge_minutes;
            y_dezgrad_erste = GradMinToDezigrad(laenge_degrees, laenge_minutes);
            x_sekunde_erste = 0;
            y_sekunde_erste = 0;
        }

        public void SetPosStart(double breite_degrees, double breite_minutes, double breite_seconds, double laenge_degrees, double laenge_minutes, double laenge_seconds)
        {
            x_grad_erste = breite_degrees;
            x_minute_erste = breite_minutes;
            x_sekunde_erste = breite_seconds;
            x_dezgrad_erste = GradMinToDezigrad(breite_degrees, breite_minutes, breite_seconds);
            y_grad_erste = laenge_degrees;
            y_minute_erste = laenge_minutes;
            y_sekunde_erste = laenge_seconds;
            y_dezgrad_erste = GradMinToDezigrad(laenge_degrees, laenge_minutes, laenge_seconds);
        }

        public void SetPosFinished(double breite_degrees, double breite_minutes, double laenge_degrees, double laenge_minutes)
        {
            x_grad_zweite = breite_degrees;
            x_minute_zweite = breite_minutes;
            x_dezgrad_zweite = GradMinToDezigrad(breite_degrees, breite_minutes);
            y_grad_zweite = laenge_degrees;
            y_minute_zweite = laenge_minutes;
            y_dezgrad_zweite = GradMinToDezigrad(laenge_degrees, laenge_minutes);
            x_sekunde_zweite = 0;
            y_sekunde_zweite = 0;
        }

        public void SetPosFinished(double breite_degrees, double breite_minutes, double breite_seconds, double laenge_degrees, double laenge_minutes, double laenge_seconds)
        {
            x_grad_zweite = breite_degrees;
            x_minute_zweite = breite_minutes;
            x_sekunde_zweite = breite_seconds;
            x_dezgrad_zweite = GradMinToDezigrad(breite_degrees, breite_minutes, breite_seconds);
            y_grad_zweite = laenge_degrees;
            y_minute_zweite = laenge_minutes;
            y_sekunde_zweite = laenge_seconds;
            y_dezgrad_zweite = GradMinToDezigrad(laenge_degrees, laenge_minutes, laenge_seconds);
        }

        // GET Methoden
        // gibt die Entfernung in Metern zurück
        public double GetDistance()
        {
            BerechneWinkel(x_dezgrad_erste, y_dezgrad_erste, x_dezgrad_zweite, y_dezgrad_zweite);
            BerechneEntfernung();

            return entfernung;
        }

        // gibt den Winkel in Grad zurück
        public double GetDirection()
        {
            BerechneWinkel(x_dezgrad_erste, y_dezgrad_erste, x_dezgrad_zweite, y_dezgrad_zweite);
            kurswinkel = (System.Math.Sin(x_dezgrad_zweite) - System.Math.Sin(x_dezgrad_erste) * System.Math.Cos(winkel)) / (System.Math.Cos(x_dezgrad_erste) * System.Math.Sin(winkel));

            double degrees = (System.Math.Acos(kurswinkel) * (360 / (2 * System.Math.PI)));
            if (y_dezgrad_erste < y_dezgrad_zweite)
            {
                return (degrees);
            }
            else 
            {
                return (360 - degrees);
            }
            
        }

        //parst die double Werte aus dem String "N 51° 21.363' E 012° 20.772'"
        public grad_minute_second StringGradMinToDouble(string s_toparse)
        {
            s_toparse = s_toparse.Replace(".", ",");
            grad_minute_second struct_gms = new grad_minute_second();
            string[] gesplittet = s_toparse.Split(' ');
            if (gesplittet[0].Equals("N"))
            {
                struct_gms.breite_grad = Convert.ToDouble(gesplittet[1].Substring(0, gesplittet[1].Length - 1));
            }
            if (gesplittet[0].Equals("S"))
            {
                // wenn man sich auf der Südhalbkugel befindet
                struct_gms.breite_grad = Convert.ToDouble(gesplittet[1].Substring(0, gesplittet[1].Length - 1)) * -1;
            }

            struct_gms.breite_min = Convert.ToDouble(gesplittet[2].Substring(0, gesplittet[2].Length - 1));
            if (gesplittet[3].Equals("E"))
            {
                struct_gms.laenge_grad = Convert.ToDouble(gesplittet[4].Substring(0, gesplittet[4].Length - 1));
            }
            if (gesplittet[3].Equals("W"))
            {
                // wenn Koordinaten mit "West" gegeben sind
                struct_gms.laenge_grad = Convert.ToDouble(gesplittet[4].Substring(0, gesplittet[4].Length - 1)) * -1;
            }
            struct_gms.laenge_min = Convert.ToDouble(gesplittet[5].Substring(0, gesplittet[5].Length - 1));
            struct_gms.breite_sec = 0;
            struct_gms.laenge_sec = 0;
            return struct_gms;
        }

        //parst die double Werte aus dem String "N 51° 21' 444" E 012° 20' 222""
        public grad_minute_second StringGradMinSecToDouble(string s_toparse)
        {
            grad_minute_second struct_gms = new grad_minute_second();
            string[] gesplittet = s_toparse.Split(' ');
            if (gesplittet[0].Equals("N"))
            {
                struct_gms.breite_grad = Convert.ToDouble(gesplittet[1].Substring(0, gesplittet[1].Length - 1));
            }
            if (gesplittet[0].Equals("S"))
            {
                // wenn man sich auf der Südhalbkugel befindet
                struct_gms.breite_grad = Convert.ToDouble(gesplittet[1].Substring(0, gesplittet[1].Length - 1)) * -1;
            }

            struct_gms.breite_min = Convert.ToDouble(gesplittet[2].Substring(0, gesplittet[2].Length - 1));
            struct_gms.breite_sec = Convert.ToDouble(gesplittet[3].Substring(0, gesplittet[3].Length - 1));

            if (gesplittet[3].Equals("E"))
            {
                struct_gms.laenge_grad = Convert.ToDouble(gesplittet[4].Substring(0, gesplittet[4].Length - 1));
            }
            if (gesplittet[3].Equals("W"))
            {
                // wenn Koordinaten mit "West" gegeben sind
                struct_gms.laenge_grad = Convert.ToDouble(gesplittet[4].Substring(0, gesplittet[4].Length - 1)) * -1;
            }
            struct_gms.laenge_min = Convert.ToDouble(gesplittet[5].Substring(0, gesplittet[5].Length - 1));
            struct_gms.laenge_sec = Convert.ToDouble(gesplittet[6].Substring(0, gesplittet[6].Length - 1));
            return struct_gms;
        }

        // 51d 20' 53,0039999999826\"
        public grad_minute_second StringDMSToDouble(string s_toparse)
        {
            //s_toparse = s_toparse.Replace(",", ".");
            grad_minute_second struct_gms = new grad_minute_second();
            string[] gesplittet = s_toparse.Split(' ');
            struct_gms.breite_grad = Convert.ToDouble(gesplittet[0].Substring(0, gesplittet[0].Length - 1));
            struct_gms.breite_min = Convert.ToDouble(gesplittet[1].Substring(0, gesplittet[1].Length - 1));
            struct_gms.breite_sec = Convert.ToDouble(gesplittet[2].Substring(0, gesplittet[2].Length - 1));
            struct_gms.laenge_grad = Convert.ToDouble(gesplittet[3].Substring(0, gesplittet[3].Length - 1));
            struct_gms.laenge_min = Convert.ToDouble(gesplittet[4].Substring(0, gesplittet[4].Length - 1));
            struct_gms.laenge_sec = Convert.ToDouble(gesplittet[5].Substring(0, gesplittet[5].Length - 1));
            //Convert.
            return struct_gms;
        }

        // Umrechnung von Grad Minute Sekunde in Dezimalgrad
        private double GradMinToDezigrad(double grad, double min)
        {
            return (grad + (min / 60));
        }

        private double GradMinToDezigrad(double grad, double min, double sec)
        {
            return (grad + (min / 60) + (sec / 3600));
        }

        // Berechnung des Winkels
        // cos g = sin (Breite 1) · sin (Breite 2) + cos (Breite 1) · cos (Breite 2) 
        // · cos (Länge 2 - Länge 1)
        private void BerechneWinkel(double breite1, double laenge1, double breite2, double laenge2)
        {

            breite1 = (breite1 * (2 * System.Math.PI) / 360);
            breite2 = (breite2 * (2 * System.Math.PI) / 360);
            laenge1 = (laenge1 * (2 * System.Math.PI) / 360);
            laenge2 = (laenge2 * (2 * System.Math.PI) / 360);
            double coswinkel = (System.Math.Sin(breite1) * System.Math.Sin(breite2)) +
            (System.Math.Cos(breite1) * System.Math.Cos(breite2) * System.Math.Cos((laenge2 - laenge1)));

            winkel = System.Math.Acos(coswinkel) * (360 / ( 2 * System.Math.PI));
        }

        //Berechnung der Entfernung in Metern
        private void BerechneEntfernung()
        {
            entfernung = ((winkel * (System.Math.PI * 2) / 360)) * 6371000;

        }
    }
}
