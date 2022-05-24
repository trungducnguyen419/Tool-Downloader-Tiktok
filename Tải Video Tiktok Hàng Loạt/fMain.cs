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
                                var request = (HttpWebRequest)WebRequest.Create($"https://dntik.com/api?type=get-id-video&url={HttpUtility.UrlEncode(url)}");
                                var response = (HttpWebResponse)request.GetResponse();
                                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                                string id = Regex.Match(responseString, "\"id\":\"(.*?)\"").Groups[1].Value;
                                if (!string.IsNullOrEmpty(id))
                                {
                                    request = (HttpWebRequest)WebRequest.Create($"https://dntik.com/api?type=get-info-video&id={id}");
                                    request.UserAgent = "TrungDucNguyen";
                                    response = (HttpWebResponse)request.GetResponse();
                                    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                                    string mp4 = Regex.Match(responseString.Replace(" ", ""), "\"mp4\":\"(.*?)\"").Groups[1].Value;
                                    Uri uri = new Uri(mp4);
                                    string filename = System.IO.Path.GetFileName(uri.LocalPath);
                                    new WebClient().DownloadFile(mp4, Application.StartupPath + "\\video\\" + id + ".mp4");
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
