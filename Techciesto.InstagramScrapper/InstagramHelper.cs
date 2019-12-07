using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Techciesto.InstagramScrapper
{
    public static class InstagramHelper
    {
        private static string INSTA_URL = "https://www.instagram.com/{0}";

        public static InstaProfile GetProfile(string username)
        {
            var result = FetchProfile(username);

            if (!string.IsNullOrEmpty(result))
            {
                result = ParseProfile(result);

                var profile = JObject.Parse(result);

                if (profile != null && profile["entry_data"] != null && profile["entry_data"]["ProfilePage"] != null && profile["entry_data"]["ProfilePage"][0] != null && profile["entry_data"]["ProfilePage"][0]["graphql"] != null && profile["entry_data"]["ProfilePage"][0]["graphql"]["user"] != null)
                {
                    var user = profile["entry_data"]["ProfilePage"][0]["graphql"]["user"];

                    InstaProfile model = new InstaProfile();

                    model.Name = user["full_name"] != null ? user.Value<string>("full_name") : string.Empty;
                    model.Username = user["username"] != null ? user.Value<string>("username") : string.Empty;
                    model.ProfilePicture = user["profile_pic_url"] != null ? user.Value<string>("profile_pic_url") : string.Empty;
                    model.FollowerCount = user["edge_followed_by"] != null && user["edge_followed_by"]["count"] != null ? user["edge_followed_by"].Value<int>("count") : 0;
                    model.FollowCount = user["edge_follow"] != null && user["edge_follow"]["count"] != null ? user["edge_follow"].Value<int>("count") : 0;
                    model.PostCount = user["edge_owner_to_timeline_media"] != null && user["edge_owner_to_timeline_media"]["count"] != null ? user["edge_owner_to_timeline_media"].Value<int>("count") : 0;

                    model.PostThumnails = new List<string>();

                    if (user["edge_owner_to_timeline_media"] != null && user["edge_owner_to_timeline_media"]["edges"] != null)
                    {
                        foreach (var item in user["edge_owner_to_timeline_media"]["edges"])
                        {
                            var thumb = item["node"] != null && item["node"]["thumbnail_src"] != null ? item["node"].Value<string>("thumbnail_src") : string.Empty;
                            if (!string.IsNullOrEmpty(thumb))
                            {
                                model.PostThumnails.Add(thumb);
                            }
                        }
                    }

                    return model;

                }
            }

            return null;
        }

        private static string FetchProfile(string username)
        {
            string retVal = string.Empty;
            HttpWebRequest request = null;
            try
            {
                string url = string.Format(INSTA_URL, username);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    retVal = reader.ReadToEnd();
                }
                return retVal;

            }
            catch (WebException ex)
            {
                using (HttpWebResponse response = ex.Response as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    retVal = reader.ReadToEnd();
                }
                return null;
            }
            finally
            {
                retVal = null;
                request = null;
            }
        }

        private static string ParseProfile(string response)
        {
            var start = response.IndexOf("window._sharedData = ") + 21;
            var end = response.IndexOf(";</script>");

            return response.Substring(start, end - start);
        }
    }
}
