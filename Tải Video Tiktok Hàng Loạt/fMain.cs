using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace Tải_Video_Tiktok_Hàng_Loạt
{
    public partial class fMain : Form
    {
        public fMain()
        {
            if (!Directory.Exists(Application.StartupPath + "\\video")) Directory.CreateDirectory(Application.StartupPath + "\\video");
            if (!Directory.Exists(Application.StartupPath + "\\image")) Directory.CreateDirectory(Application.StartupPath + "\\image");
            InitializeComponent();
            Label.CheckForIllegalCrossThreadCalls = false;
            Button.CheckForIllegalCrossThreadCalls = false;
            TextBox.CheckForIllegalCrossThreadCalls = false;
            Form.CheckForIllegalCrossThreadCalls = false;
        }
        bool isStop = false;
        private void btn_Click(object sender, EventArgs e)
        {
            if (btn.Text == "Tải")
            {
                tbUrl.Enabled = false;
                isStop = false;
                btn.Text = "Dừng";
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        var i = 0;
                        foreach (var url in tbUrl.Lines)
                        {
                            if (isStop) return;
                            i++;
                            if (!string.IsNullOrEmpty(url))
                            {
                                btn.Text = $"Dừng {i}/{tbUrl.Lines.Length}";
                                var request = (HttpWebRequest)WebRequest.Create($"https://dntik.ducnguyenfb.com/api?type=get-id-video&url={HttpUtility.UrlEncode(url)}");
                                var response = (HttpWebResponse)request.GetResponse();
                                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                                string id = Regex.Match(responseString, "\"id\":\"(.*?)\"").Groups[1].Value;
                                if (!string.IsNullOrEmpty(id))
                                {
                                    request = (HttpWebRequest)WebRequest.Create($"https://dntik.ducnguyenfb.com/api?type=get-information-video&id={id}");
                                    request.Method = "GET";
                                    request.UserAgent = "TrungDucNguyen";
                                    response = (HttpWebResponse)request.GetResponse();
                                    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                                    responseString = responseString.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                                    if (responseString.Contains("\"media\":null"))
                                    {
                                        string images = Regex.Match(responseString, "\"images\":\\[\"(.*?)\"\\]").Groups[1].Value;
                                        if (images.Contains("\",\""))
                                        {
                                            images = images.Replace("\",\"", "|");
                                            int j = 0;
                                            foreach (var img in images.Split('|'))
                                            {
                                                j++;
                                                new WebClient().DownloadFile(img, Application.StartupPath + "\\image\\" + id + $" ({j}).jpeg");
                                            }    
                                        }    
                                    }   
                                    else
                                    {
                                        string media = Regex.Match(responseString, "\"media\":\"(.*?)\"").Groups[1].Value;
                                        new WebClient().DownloadFile(media, Application.StartupPath + "\\video\\" + id + ".mp4");
                                    }    
                                }    
                            }
                        }
                        tbUrl.Enabled = true;
                        isStop = true;
                        btn.Text = "Tải";
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbUrl.Enabled = true;
                        isStop = true;
                        btn.Text = "Tải";
                        return;
                    }
                });
                thread.IsBackground = false;
                thread.Start();
            }   
            else
            {
                tbUrl.Enabled = true;
                isStop = true;
                btn.Text = "Tải";
            }    
        }
    }
}
