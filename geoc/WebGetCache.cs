#region Using-Direktiven

using System;
using System.Drawing;
using System.Threading;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using Microsoft.WindowsMobile.Samples.Location;
using System.Windows.Forms;
using Microsoft.WindowsMobile.Forms;
using Microsoft.WindowsMobile.PocketOutlook;
//using System.Collections.Generic;
using System.ComponentModel;

#endregion

namespace geoc
{
    public class WebGetCache
    {
        // Threading
        
        // 0 = noch nicht angefangen
        // 1 = laeuft
        // 2 = fehler
        // 3 = korrekt abgearbeitet
        public volatile int i_HTTPLoadCacheListSuccess = 0;
        public volatile int i_HTTPLoadCacheSiteSuccess = 0;
        // Threading


        private volatile int i_MaxCacheDistance = 0;
        private volatile int i_MaxResults = 0;

        private volatile string s_SelectedCacheID = "";
        private volatile bool b_SelectedCacheIDChanged = false;
        private volatile string s_HTTPCacheSiteResponse;

        private volatile GpsPosition c_GPSPosition = null;

        private volatile string s_HTTPCacheListRequestString;
        //private volatile string s_HTTPCacheListResponse;

        public class sct_CacheListNode
        {
            public volatile string s_CacheID;
            public volatile string s_UserID;
            public volatile string s_UserName;

            public volatile string s_Type;
            //public volatile string s_Author;
            public volatile string s_Name;
            public volatile string s_Description;
            //public volatile DateTime dt_LastLog;

            public volatile string s_Direction;
            public double d_Distance;
            public double d_Difficulty;
            public double d_Country;

        }
        private volatile List<string> l_HTTPCacheListResponse = new List<string>();

        public volatile List<sct_CacheListNode> l_CacheList = new List<sct_CacheListNode>();
        
        //private int i_ResultCount = 0;

        public WebGetCache()
        {
        }
        ~WebGetCache()
        {
        }

        public void SetGPSPosition(GpsPosition l_GPSPosition)
        {
            c_GPSPosition = l_GPSPosition;

        }

        public void SetMaxCacheDistance(int l_MaxCacheDistance)
        {
            if (l_MaxCacheDistance < 1)
            {
                l_MaxCacheDistance = 1;
            }
            i_MaxCacheDistance = l_MaxCacheDistance;
        }

        public void SetMaxResults(int l_MaxResults)
        {
            i_MaxResults = l_MaxResults;
        }
        
        public bool BuildHTTPCacheListRequestString()
        {
            if (c_GPSPosition != null)
            {
                if (i_MaxCacheDistance > 0)
                {
                    string s_GPSSecondsLatitudeCleaned = Convert.ToInt64(c_GPSPosition.LatitudeInDegreesMinutesSeconds.Seconds * 1000).ToString();
                    string s_GPSSecondsLongitudeCleaned = Convert.ToInt64(c_GPSPosition.LongitudeInDegreesMinutesSeconds.Seconds * 1000).ToString();

                    //s_HTTPCacheListRequestString = "http://opencaching.de/search.php?searchto=searchbydistance&showresult=1&expert=0&output=HTML&sort=bydistance&f_userowner=0&f_userfound=0&f_inactive=1&country=DE&cachetype=&cache_attribs=&cache_attribs_not=7&latNS=N&lat_h=51&lat_min=20.53286&lonEW=E&lon_h=11&lon_min=58.48612&distance=100&unit=km";
                    s_HTTPCacheListRequestString = "http://opencaching.de/" +
                            "search.php?" +
                            "searchto=" + "searchbydistance" +
                            "&showresult=" + "1" +
                            "&expert=" + "0" +
                            "&output=" + "HTML" +
                            "&sort=" + "bydistance" +
                            "&f_userowner=" + "0" +
                            "&f_userfound=" + "0" +
                            "&f_inactive=" + "1" +
                            "&country=" + "DE" +
                            "&cachetype=" + "" +
                            "&cache_attribs=" + "" +
                            "&cache_attribs_not=" + "7" +
                            "&latNS=" + "N" +
                            "&lat_h=" + c_GPSPosition.LatitudeInDegreesMinutesSeconds.Degrees.ToString() +
                            "&lat_min=" + c_GPSPosition.LatitudeInDegreesMinutesSeconds.Minutes.ToString() + "." +
                            s_GPSSecondsLatitudeCleaned +
                            "&lonEW=" + "E" +
                            "&lon_h=" + c_GPSPosition.LongitudeInDegreesMinutesSeconds.Degrees.ToString() +
                            "&lon_min=" + c_GPSPosition.LongitudeInDegreesMinutesSeconds.Minutes.ToString() + "." +
                            s_GPSSecondsLongitudeCleaned +
                            "&distance=" + i_MaxCacheDistance.ToString() +
                            "&unit=" + "km";

                    return (true);
                }
            }
            return (false);
        }

        public string GetHTTPCacheListRequestString()
        {
            return s_HTTPCacheListRequestString;
        }

        public void HTTPLoadCacheList()
        {
            i_HTTPLoadCacheListSuccess = 1;
            string s_HTTPLine;
            //WebRequest request = WebRequest.Create("http://www.heise.de/index.html");
            WebRequest request = WebRequest.Create(s_HTTPCacheListRequestString);
            //If required by the server, set the credentials.
            //request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                //return (false);
                i_HTTPLoadCacheListSuccess = 2;
                return;
            }

                // Display the status.
            string s_CacheList = response.StatusDescription;
            // Get the stream containing content returned by the server.
            Stream str_CacheList = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader strr_CacheList = new StreamReader(str_CacheList);
            // Read the content.
            //s_HTTPCacheListResponse = strr_CacheList.ReadToEnd();
            while (strr_CacheList.EndOfStream == false)
            {
                s_HTTPLine = strr_CacheList.ReadLine();
                l_HTTPCacheListResponse.Add(s_HTTPLine);
            }
            strr_CacheList.Close();
            str_CacheList.Close();
            response.Close();
            //return (true);
            i_HTTPLoadCacheListSuccess = 3;
            
            //return;
        }

        public void ParseCacheDescription(string HTMLText)
        {
            //int i;
        }
        
        private void ParseHTTPCacheListBlock(int i_ListPos)
        {
            sct_CacheListNode sct_NewEntry = new sct_CacheListNode();
            int i_LeftPos = 0;
            int i_RightPos = 0;
            
            // distance
            i_LeftPos = l_HTTPCacheListResponse[i_ListPos + 1].IndexOf(">") + 1;
            i_RightPos = l_HTTPCacheListResponse[i_ListPos + 1].IndexOf("&", i_LeftPos);
            sct_NewEntry.d_Distance = (Convert.ToDouble(l_HTTPCacheListResponse[i_ListPos + 1].Substring(i_LeftPos, i_RightPos - i_LeftPos))) / 10;

            // type
            i_LeftPos = l_HTTPCacheListResponse[i_ListPos + 2].IndexOf("title=") + 7;
            i_RightPos = l_HTTPCacheListResponse[i_ListPos + 2].IndexOf(">", i_LeftPos) - 1;
            sct_NewEntry.s_Type = l_HTTPCacheListResponse[i_ListPos + 2].Substring(i_LeftPos, i_RightPos - i_LeftPos);

            // difficulty
            i_LeftPos = l_HTTPCacheListResponse[i_ListPos + 3].IndexOf("Schwierigkeit: ") + 15;
            i_RightPos = l_HTTPCacheListResponse[i_ListPos + 3].IndexOf(" von", i_LeftPos);
            sct_NewEntry.d_Difficulty = (Convert.ToDouble(l_HTTPCacheListResponse[i_ListPos + 3].Substring(i_LeftPos, i_RightPos - i_LeftPos))) / 10;

            // country
            i_LeftPos = l_HTTPCacheListResponse[i_ListPos + 3].IndexOf("Gel&auml;nde: ") + 14;
            i_RightPos = l_HTTPCacheListResponse[i_ListPos + 3].IndexOf(" von", i_LeftPos);
            sct_NewEntry.d_Country = (Convert.ToDouble(l_HTTPCacheListResponse[i_ListPos + 3].Substring(i_LeftPos, i_RightPos - i_LeftPos))) / 10;

            // cache id
            i_LeftPos = l_HTTPCacheListResponse[i_ListPos + 4].IndexOf("cacheid=") + 8;
            i_RightPos = l_HTTPCacheListResponse[i_ListPos + 4].IndexOf(">", i_LeftPos) - 2;
            sct_NewEntry.s_CacheID = l_HTTPCacheListResponse[i_ListPos + 4].Substring(i_LeftPos, i_RightPos - i_LeftPos + 1);

            // name
            i_LeftPos = i_RightPos + 3;
            i_RightPos = l_HTTPCacheListResponse[i_ListPos + 4].IndexOf("</a>", i_LeftPos);
            sct_NewEntry.s_Name = l_HTTPCacheListResponse[i_ListPos + 4].Substring(i_LeftPos, i_RightPos - i_LeftPos);
            
            // user id
            i_LeftPos = l_HTTPCacheListResponse[i_ListPos + 4].IndexOf("userid=", i_RightPos) + 7;
            i_RightPos = l_HTTPCacheListResponse[i_ListPos + 4].IndexOf(">", i_LeftPos) - 2;
            sct_NewEntry.s_UserID = l_HTTPCacheListResponse[i_ListPos + 4].Substring(i_LeftPos, i_RightPos - i_LeftPos + 1);

            // username
            i_LeftPos = i_RightPos + 3;
            i_RightPos = l_HTTPCacheListResponse[i_ListPos + 4].IndexOf("</a>", i_LeftPos);
            sct_NewEntry.s_UserName = l_HTTPCacheListResponse[i_ListPos + 4].Substring(i_LeftPos, i_RightPos - i_LeftPos);

            // direction
            i_LeftPos = l_HTTPCacheListResponse[i_ListPos + 9].IndexOf(">") + 1;
            i_RightPos = l_HTTPCacheListResponse[i_ListPos + 9].IndexOf("&", i_LeftPos);
            sct_NewEntry.s_Direction = l_HTTPCacheListResponse[i_ListPos + 9].Substring(i_LeftPos, i_RightPos - i_LeftPos);

            // description
            i_LeftPos = l_HTTPCacheListResponse[i_ListPos + 10].IndexOf("</b></a> ") + 10;
            i_RightPos = l_HTTPCacheListResponse[i_ListPos + 10].IndexOf(" &nbsp", i_LeftPos);
            if (i_RightPos > 0) // tollwitz hack
            {
                sct_NewEntry.s_Description = l_HTTPCacheListResponse[i_ListPos + 10].Substring(i_LeftPos, i_RightPos - i_LeftPos);
            }
            else
            {
                sct_NewEntry.s_Description = "";
            }
            l_CacheList.Add(sct_NewEntry);
        }


        public void ParseHTTPCacheList()
        {
            if (l_CacheList.Count != 0) {
                l_CacheList.Clear();
                //i_ResultCount = 0;
            }
            
            //IEnumerator<string> ie_HTTPCacheList = l_HTTPCacheListResponse.GetEnumerator();

            int i;
            int i_ListPosNextCache = 0;
            for (i = 0; i < l_HTTPCacheListResponse.Count; i++)
            {
                if (l_HTTPCacheListResponse[i].IndexOf("km") > 10)
                {
                    if (l_HTTPCacheListResponse[i + 1].IndexOf("Typ") > 10)
                    {
                        if (l_HTTPCacheListResponse[i + 2].IndexOf("D/T") > 10)
                        {
                            if (l_HTTPCacheListResponse[i + 3].IndexOf("Name") > 10)
                            {
                                if (l_HTTPCacheListResponse[i + 4].IndexOf("Letzte Logs") > 10)
                                {
                                    i_ListPosNextCache = i + 7;
                                    while (l_HTTPCacheListResponse[i_ListPosNextCache].Substring(0, 11) == "  <td width")
                                    {
                                        ParseHTTPCacheListBlock(i_ListPosNextCache);
                                        i_ListPosNextCache += 13;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SetCacheID(string s_NewCacheID)
        {
            if (s_SelectedCacheID != s_NewCacheID)
            {
                b_SelectedCacheIDChanged = true;
                s_SelectedCacheID = s_NewCacheID;
            }
        }
        public void HTTPLoadCacheSite()
        {
            i_HTTPLoadCacheSiteSuccess = 1;
            if (s_SelectedCacheID == "")
            {
                i_HTTPLoadCacheSiteSuccess = 2;
                return;
            }
            if (b_SelectedCacheIDChanged == false)
            {
                i_HTTPLoadCacheSiteSuccess = 3;
                return;
            }
          
            //WebRequest request = WebRequest.Create("http://www.heise.de/index.html");
            WebRequest request = WebRequest.Create("http://www.opencaching.de/viewcache.php?cacheid=" + s_SelectedCacheID);
            //If required by the server, set the credentials.
            //request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                //return (false);
                i_HTTPLoadCacheSiteSuccess = 2;
                return;
            }

            // Display the status.
            string s_CacheSite = response.StatusDescription;
            // Get the stream containing content returned by the server.
            Stream str_CacheSite = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader strr_CacheSite = new StreamReader(str_CacheSite);
            // Read the content.
            //s_HTTPCacheSiteResponse = strr_CacheSite.ReadToEnd();

            String tempstr;
            s_HTTPCacheSiteResponse = "";
            while (strr_CacheSite.EndOfStream == false)
            {
                tempstr = strr_CacheSite.ReadLine();
                //tempstr.TrimStart('\t');
                tempstr.Trim();
                s_HTTPCacheSiteResponse += tempstr;
            }

            //s_HTTPCacheSiteResponse = s_HTTPCacheSiteResponse.Replace("\t", null);
            //s_HTTPCacheSiteResponse.Replace("\r", "5");
            //s_HTTPCacheSiteResponse.Replace("\n", "trivial");
            //s_HTTPCacheSiteResponse.

            strr_CacheSite.Close();;
            //return; //FIXME
            str_CacheSite.Close();
            response.Close();
            b_SelectedCacheIDChanged = false;
            //return (true);
            i_HTTPLoadCacheSiteSuccess = 3;
        }

        public string GetHTTPCacheSite()
        {
            return (s_HTTPCacheSiteResponse);
        }

        public string ParseHTTPCacheSite()
        {
            int i_left_pos = s_HTTPCacheSiteResponse.IndexOf("</span><font size=\"3\"><b>") + 25;
            int i_right_pos = s_HTTPCacheSiteResponse.IndexOf("<", i_left_pos, 300);
            return (s_HTTPCacheSiteResponse.Substring(i_left_pos, i_right_pos - i_left_pos).Replace("&nbsp;", " "));
        }
    }
}
