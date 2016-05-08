using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsMobile.Samples.Location;

namespace geoc
{
    public partial class Form1 : Form
    {
        public string s_Content = "";
        //public string 
        public int i_ProgState = 0;
        public int i_OldProgState = -1;

        // pfeil test
        //private int temp__degrees = 0;
        //private bool start__arrowing = false;

        //pub
        public bool[] UpdateTab = {false, false, false, false};
        //public string[4] tabParameter;
        public int k = 0;

        private EventHandler updateDataHandler;
        //private EventHandler GuiUpdateHandler;

        public delegate void UpdateGuiDelegate();
        public event UpdateGuiDelegate UpdateGuiEvent;

        public PositionStabilisation posStabilisation;
              

        GpsDeviceState device = null;
        GpsPosition position = null;
        //string target = null;
        NavigationCalculation.grad_minute_second coord_gm_target;
        NavigationCalculation.grad_minute_second coord_gm_last_pos;

        
        bool b_ValidGPSData = false;
        bool b_HasTarget = false;
        bool b_FirstWebPage = true;
        bool b_FirstMovement = true;

        WebGetCache wgc = new WebGetCache();
        Gps gps = new Gps();
        WebBrowser browser = new WebBrowser();
        Navigation nav = new Navigation(60, 60);
        NavigationCalculation navcalc = new NavigationCalculation();
        NavigationCalculation movementcalc = new NavigationCalculation();
        double d_movementDirection = 0;

        Thread th_wgc_GetCacheList;
        Thread th_wgc_GetCacheSite;

        public int testtest2 = 0;

//###################################################################################

        // halbhohlniet
        private void UpdateStatus() 
        {
            textBox1.Text = s_Content;
            //textBox1.Show();
            UpdateTab[0] = false;
        }

        private void UpdateCacheList()
        {
            int j = 0;
                
            // einträge in die liste hinzufügen
            for (int i = 0; i < (wgc.l_CacheList.Count * 3); i+=3)
            {
                listView1.Items.Add(new ListViewItem());
                listView1.Items[i].Text = wgc.l_CacheList[j].s_Name.ToString();
                listView1.Items.Add(new ListViewItem());
                listView1.Items[i + 1].Text = wgc.l_CacheList[j].s_Description.ToString();
                listView1.Items.Add(new ListViewItem());
                listView1.Items[i + 2].Text = wgc.l_CacheList[j].s_Direction.ToString() + " " + wgc.l_CacheList[j].d_Distance.ToString() + " km " + wgc.l_CacheList[j].s_Type;
                j++;
            }

            // alle einträge abwechselnd grau weiss
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if ((i % 6) == 0)
                {
                    listView1.Items[i].BackColor = Color.FromArgb(200,200,200);
                    listView1.Items[i + 1].BackColor = Color.FromArgb(200, 200, 200);
                    listView1.Items[i + 2].BackColor = Color.FromArgb(200, 200, 200);
                } // FIXME neuaufbau beachten
            }
            UpdateTab[1] = false;
        }

        private void UpdateBrowser()
        {

            UpdateTab[2] = false;
            wgc.SetCacheID(wgc.l_CacheList[k].s_CacheID);
            
            if (wgc.i_HTTPLoadCacheSiteSuccess == 0)
            {
                th_wgc_GetCacheSite.Start();
                while (wgc.i_HTTPLoadCacheSiteSuccess == 0)
                {
                    Thread.Sleep(10);
                }
                th_wgc_GetCacheSite.Join();
            }              
            else
            {
                if (wgc.i_HTTPLoadCacheSiteSuccess != 1)
                {
                    if (wgc.i_HTTPLoadCacheSiteSuccess == 2)
                    {
                        s_Content += "\r\nGeocache-Liste konnte nicht geladen werden.";
                    }
                    else if (wgc.i_HTTPLoadCacheSiteSuccess == 3)
                    {
                        s_Content += "\r\nGeocache-Liste wurde geladen.";
                    }
                    else
                    {
                        s_Content += "\r\nSchwerer Ausnahmefehler!";
                    }
                }
            }




            if (b_FirstWebPage == true)
            {
                b_FirstWebPage = false;
                browser.Navigate(new Uri("http://www.opencaching.de/viewcache.php?cacheid=" + wgc.l_CacheList[k].s_CacheID));

                //browser.DocumentText = "baum";
                //browser.DocumentText = wgc.GetHTTPCacheSite();
                browser.Parent = tabPage3;
                browser.Dock = DockStyle.Fill;
                browser.Show();
            }
            else
            {
                browser.Navigate(new Uri("http://www.opencaching.de/viewcache.php?cacheid=" + wgc.l_CacheList[k].s_CacheID));
                browser.Show();
            }


            wgc.HTTPLoadCacheSite();

            coord_gm_target = navcalc.StringGradMinToDouble(wgc.ParseHTTPCacheSite());
            navcalc.SetPosFinished(coord_gm_target.breite_grad, coord_gm_target.breite_min, coord_gm_target.laenge_grad, coord_gm_target.laenge_min);

            UpdateTab[3] = true;

            b_HasTarget = true;

          //  wgc.i_HTTPLoadCacheSiteSuccess = 0;
            //while (browser.IsBusy)
            //{
                //Thread.Sleep(100);
                
            //}
            //wgc.ParseCacheDescription(browser.Text);
            
        }
        
        private void UpdateMap()
        {
            //int degrees = ?;
            //nav.SetDegrees()
            UpdateTab[3] = false;
        }

        private void UpdateGui()
        {
            if (UpdateTab[0] == true)
            {
                UpdateStatus();
            }
            if (UpdateTab[1] == true)
            {
                UpdateCacheList();
            }
            if (UpdateTab[2] == true)
            {
                UpdateBrowser();
            }
            if (UpdateTab[3] == true)
            {
                UpdateMap();
                //start__arrowing = true;
            }

            /*if (start__arrowing = true)
            {
                temp__degrees = (temp__degrees + 5) % 360;
                nav.SetDegrees(temp__degrees);
                
                pictureBox1.Image = nav.GetNavigationArrow();
            }*/
        }
        
        public void GetNewProgState()
        {
            /*if (i_ProgState == i_OldProgState)
            {
                return;
            }
            else
            {
                i_OldProgState = i_ProgState;
                UpdateTab[0] = true;
            }*/
            UpdateTab[0] = true;

            if (i_ProgState == 0)
            {
                // funktion wurde das erste mal aufgerufen
                // mache nichts weiter
                i_ProgState++;
                s_Content = "Programm wird gestartet...";
            }
            else if (i_ProgState == 1)
            {
                i_ProgState++;
                s_Content += "\r\nWarte auf GPS-Daten";
            }
            else if (i_ProgState == 2)
            {
                //b_ValidGPSData = true; //
                if (b_ValidGPSData == true)
                {
                    i_ProgState++;
                }
            }
            else if (i_ProgState == 3)
            {
                // bastel den string und schau
                // auch ob das klappt
                wgc.SetMaxCacheDistance(100);
                wgc.SetMaxResults(10);
                wgc.SetGPSPosition(position);
                if (wgc.BuildHTTPCacheListRequestString() == true)
                {
                    i_ProgState++;
                }
                //s_Content += "\r\nbuilding cache list string";
            }
            else if (i_ProgState == 4)
            {
                // hier spaeter noch mal pruefen ob ueberhaupt internet geht
                // wird jetzt einfach vorrasgesetzt und deshalb sofort inkrementiert
                i_ProgState++; // FIXME
                s_Content += "\r\nVersuche die Geocache-Seite zu laden...";
            }
            else if (i_ProgState == 5)
            {
                if (wgc.i_HTTPLoadCacheListSuccess == 0)
                {
                    th_wgc_GetCacheList.Start();
                    while (wgc.i_HTTPLoadCacheListSuccess == 0)
                    {
                        Thread.Sleep(10);
                    }

                    //th_wgc_GetCacheList.
                    th_wgc_GetCacheList.Join();
                    //i_ProgState++;
                }
                else
                {
                    if (wgc.i_HTTPLoadCacheListSuccess != 1)
                    {
                        if (wgc.i_HTTPLoadCacheListSuccess == 2)
                        {
                            s_Content += "\r\nGeocache-Liste konnte nicht geladen werden.\r\nVermutlich besteht ein Probelm mit Ihrer Internetverbindung.";
                        }
                        else if (wgc.i_HTTPLoadCacheListSuccess == 3)
                        {
                            s_Content += "\r\nSeite wurde geladen.";
                            i_ProgState++;
                        }
                        else
                        {
                            s_Content += "\r\nSchwerer Ausnahmefehler!";
                        }
                    }
                }
            }
            else if (i_ProgState == 6)
            {
                // pruefe fleissig ob die website auf dem string liegt
                //if (th_WebGetCache[1].Join(1))
                //{
                //th_WebGetCache[1].Join();
                // thread ist fertig
                i_ProgState++;
                //}
                //s_Content += "\r\nwait for web cache list";
            }
            else if (i_ProgState == 7)
            {
                s_Content += "\r\nExtrahiere benötigte Informationen aus der Internet-Seite";
                wgc.ParseHTTPCacheList();
                i_ProgState++;
            }
            else if (i_ProgState == 8)
            {
                s_Content += "\r\n\r\nStartvorgang beendet.";
                i_ProgState++;

            }
            else if (i_ProgState == 9)
            {
                UpdateTab[1] = true;
                i_ProgState++;
            }
            else if (i_ProgState == 10)
            {
               // UpdateTab[1] = true;
               // i_ProgState++;
            }
        }
        //###################################################################################

        public void TimedFunction(object State) {
            GetNewProgState();

            for (int i = 0; i < 4; i++)
            {
                if (UpdateTab[i] == true)
                {
                    Invoke(UpdateGuiEvent);
                    break;
                }
            }
        }
            
        public Form1()
        {
           
            InitializeComponent();
            this.UpdateGuiEvent += new UpdateGuiDelegate(this.UpdateGui);
            System.Threading.Timer bla = new System.Threading.Timer(new TimerCallback(TimedFunction), null, 0,2000);
            gps.Open();
            //th_WebGetCache[1] = new Thread(new ThreadStart(wgc.HTTPLoadCacheList));
            //Thread workerThread = new Thread(workerObject.DoWork);
            th_wgc_GetCacheList = new Thread(wgc.HTTPLoadCacheList);
            th_wgc_GetCacheSite = new Thread(wgc.HTTPLoadCacheSite);

            posStabilisation = new PositionStabilisation(10);
        }
        
     
        private void Form1_Load(object sender, System.EventArgs e)
        {
            updateDataHandler = new EventHandler(UpdateData);
            gps.DeviceStateChanged += new DeviceStateChangedEventHandler(gps_DeviceStateChanged);
            gps.LocationChanged += new LocationChangedEventHandler(gps_LocationChanged);
        }

        protected void gps_LocationChanged(object sender, LocationChangedEventArgs args)
        {
            position = args.Position;

            // call the UpdateData method via the updateDataHandler so that we
            // update the UI on the UI thread
            Invoke(updateDataHandler);
        }

        
        void gps_DeviceStateChanged(object sender, DeviceStateChangedEventArgs args)
        {
            device = args.DeviceState;

            // call the UpdateData method via the updateDataHandler so that we
            // update the UI on the UI thread
            Invoke(updateDataHandler);
        }

        void UpdateData(object sender, System.EventArgs args)
        {
            int testtest = 0;
            if (gps.Opened)
            {
                string str = "";
                if (device != null)
                {
                    str = device.FriendlyName + " " + device.ServiceState + ", " + device.DeviceState + "\r\n";
                }

                if (position != null)
                {

                    if (position.LatitudeValid)
                    {
                        str += "Latitude (DD):\r\n   " + position.Latitude + "\r\n";
                        str += "Latitude (D,M,S):\r\n   " + position.LatitudeInDegreesMinutesSeconds + "\r\n";
                        testtest++;
                    }

                    if (position.LongitudeValid)
                    {
                        str += "Longitude (DD):\r\n   " + position.Longitude + "\r\n";
                        str += "Longitude (D,M,S):\r\n   " + position.LongitudeInDegreesMinutesSeconds + "\r\n";
                        testtest++;
                    }

                    if (position.SatellitesInSolutionValid &&
                        position.SatellitesInViewValid &&
                        position.SatelliteCountValid)
                    {
                        str += "Satellite Count:\r\n   " + position.GetSatellitesInSolution().Length + "/" +
                            position.GetSatellitesInView().Length + " (" +
                            position.SatelliteCount + ")\r\n";
                    }

                    if (position.TimeValid)
                    {
                        str += "Time:\r\n   " + position.Time.ToString() + "\r\n";
                    }

                    if (testtest == 2)
                    {
                        b_ValidGPSData = true;
                        if (b_HasTarget == true)
                        {
                            string l_pos = position.LatitudeInDegreesMinutesSeconds.ToString() + " " + position.LongitudeInDegreesMinutesSeconds;
                            
                            NavigationCalculation.grad_minute_second l_pos_gms = navcalc.StringDMSToDouble(l_pos);
                            
                            ///// bauarbeiten
                            //posStabilisation.addPosition(l_pos_gms);
                            //l_pos_gms = posStabilisation.getAveragePosition();
                            //if (b_FirstMovement == true)
                            //{
                                //b_FirstMovement = false;
                                //coord_gm_last_pos = l_pos_gms;
                            //}

                            //movementcalc.SetPosStart(coord_gm_last_pos.breite_grad, coord_gm_last_pos.breite_min, coord_gm_last_pos.breite_sec, coord_gm_last_pos.laenge_grad, coord_gm_last_pos.laenge_min, coord_gm_last_pos.laenge_sec);
                            //movementcalc.SetPosFinished(l_pos_gms.breite_grad, l_pos_gms.breite_min, l_pos_gms.breite_sec, l_pos_gms.laenge_grad, l_pos_gms.laenge_min, l_pos_gms.laenge_sec);
                            //d_movementDirection = movementcalc.GetDirection();
                            //navMovementDirection.Text = "Bewegungsrichtung: " + d_movementDirection.ToString();
                            ///// bauarbeiten

                            navcalc.SetPosStart(l_pos_gms.breite_grad, l_pos_gms.breite_min, l_pos_gms.breite_sec, l_pos_gms.laenge_grad, l_pos_gms.laenge_min, l_pos_gms.laenge_sec);
                            double l_dist = navcalc.GetDistance();
                            double d_direction = navcalc.GetDirection();
                            navDistance.Text = "Distanz: " + nav.FormatDistance(l_dist);
                            string temp_pos1 = d_direction.ToString();
                            if (temp_pos1.Length > 7)
                            {
                                temp_pos1 = temp_pos1.Substring(0, 7);
                            }
                            navDirection.Text = "Richtung: " + temp_pos1;
                            
                            string temp_pos2;
                            temp_pos1 = navcalc.x_dezgrad_zweite.ToString();
                            temp_pos2 = navcalc.y_dezgrad_zweite.ToString();
                            if (temp_pos1.Length > 10)
                            {
                                temp_pos1 = temp_pos1.Substring(0, 10);
                            }
                            if (temp_pos2.Length > 10)
                            {
                                temp_pos2 = temp_pos2.Substring(0, 10);
                            }
                            navTargetPosition.Text = "Zielposition:\r\nN " 
                                    + temp_pos1 
                                    + "\r\nE " + temp_pos2;
                            
                            temp_pos1 = navcalc.x_dezgrad_erste.ToString();
                            temp_pos2 = navcalc.y_dezgrad_erste.ToString();
                            if (temp_pos1.Length > 10)
                            {
                                temp_pos1 = temp_pos1.Substring(0, 10);
                            }
                            if (temp_pos2.Length > 10)
                            {
                                temp_pos2 = temp_pos2.Substring(0, 10);
                            }
                            navActualPosition.Text = "aktuelle Position:\r\nN " 
                                    + temp_pos1
                                    + "\r\nE " + temp_pos2;
                            nav.SetDegrees(Convert.ToInt32(d_direction));
                            navPicArrow.Image = nav.GetNavigationArrow();
                        }
                    }
                    else
                    {
                        b_ValidGPSData = false;
                    }
                }
            }
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            b_HasTarget = false;
            
            // index des markierten eintrages errechnen und auf k legen
            k = listView1.FocusedItem.Index / 3;

            // alle einträge abwechselnd grau weiss
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if ((i % 6) == 0)
                {
                    listView1.Items[i].BackColor = Color.FromArgb(200, 200, 200);
                    listView1.Items[i + 1].BackColor = Color.FromArgb(200, 200, 200);
                    listView1.Items[i + 2].BackColor = Color.FromArgb(200, 200, 200);
                }
                if ((i % 6) == 3)
                {
                    listView1.Items[i].BackColor = Color.FromArgb(255, 255, 255);
                    listView1.Items[i + 1].BackColor = Color.FromArgb(255, 255, 255);
                    listView1.Items[i + 2].BackColor = Color.FromArgb(255, 255, 255);
                }
            }

            // markierten eintrag rötlich hinterlegen
            if ((listView1.FocusedItem.Index % 6) == 0)
            {
                listView1.Items[k * 3].BackColor = Color.FromArgb(200, 150, 150);
                listView1.Items[k * 3 + 1].BackColor = Color.FromArgb(200, 150, 150);
                listView1.Items[k * 3 + 2].BackColor = Color.FromArgb(200, 150, 150);
            }
            else
            {
                listView1.Items[k * 3].BackColor = Color.FromArgb(255, 205, 205); ;
                listView1.Items[k * 3 + 1].BackColor = Color.FromArgb(255, 205, 205);
                listView1.Items[k * 3 + 2].BackColor = Color.FromArgb(255, 205, 205);
            }

            // geo caching homepage mit markiertem eintrag aufrufen
            //Thread.Sleep(10000);
            UpdateTab[2] = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Up))
            {
                // Up
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Down))
            {
                // Down
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Left))
            {
                // Left
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                // Right
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                // Enter
            }

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }
    }
}