using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Web.Hosting;
using System.IO.Compression;
using System.Runtime.InteropServices.ComTypes;
using System.IO;
using System.Security.Policy;

namespace GetCIDbyVN
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            this.textBox2.Clear();
            string iid = Clipboard.GetText().Replace(" ", "").Replace("-", "");
            if (Regex.IsMatch(iid, "[\\d]{63}"))
            {
                this.textBox1.Text = Regex.Match(iid, "[\\d]{63}").Value;
                GetCID(this.textBox1.Text);
            }
            else if (Regex.IsMatch(iid, "[\\d]{54}"))
            {
                this.textBox1.Text = Regex.Match(iid, "[\\d]{54}").Value;
                GetCID(this.textBox1.Text);
            }
            else
            {
                this.textBox1.Text = "安装ID有误";
            }
        }

        private async void GetCID(string iid)
        {
           await  Task.Factory.StartNew(() => {
               this.label3.Invoke(new Action(() =>
               {
                   this.label3.Text = "正在获取，请耐心等待下。。。。";
                   this.label3.Visible = true;
               }));
                
                string crtiid = string.Empty;
                using (Aes aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.IV = Encoding.UTF8.GetBytes("7061759328313233");
                    aes.Key = Encoding.UTF8.GetBytes("ditObg4239Ajdk@d");
                    ICryptoTransform ct = aes.CreateEncryptor(aes.Key, aes.IV);
                    byte[] encrypted;
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, ct, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(iid);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                    crtiid = Convert.ToBase64String(encrypted);

                }
                string url1 = $"https://0xc004c008.com";
                string url2 = $"https://0xc004c008.com/ajax/get_cid?iids={System.Web.HttpUtility.UrlEncode(crtiid)}";
                string cookie = GetCookie(url1);
                HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(url2);
                hwr.Method = "GET";
                hwr.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7\r\n";
                hwr.KeepAlive = true;
                hwr.AllowAutoRedirect = true;
                hwr.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36";
                hwr.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                hwr.Headers.Add("accept-encoding", "gzip, deflate");//, br
                hwr.Headers.Add("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,ru;q=0.7,de;q=0.6");
                //hwr.Headers.Add("Cookie", "userlogin=usersettings=6Y6J10PXuqzKxk6kYtsmQNb8QZLN41TB3BfFXKBOhw/nWUqqE5UVkpzMe8jRCDP92w7zAl/PnPTRHax/O50kDdZNzG1OoCSDmKDYc1YQFquyiFly3LLp/9lg+thKbYeiBpNrHGKyOkBsicVNQUSKtEBOjqW9eAIkVyuWUrqLIGeCOAPW4EreXXA/FJeexYStlkga8shEG7YhZo2q7fHrP+JU3eEEwkongW6MUDbTSWBAgj7v7lDjUSwtGdKVrT05u8ZjBOKPrXO2XHMC/jsspOKDQ0fit6vMA0ut2Lh1pNS/lZhcD2L1JLoQyYi9hTiGmAAFygjBW+Ner8555gsVTg==\r\n\r\n");
                hwr.Headers.Add("Cookie", cookie);
                HttpWebResponse hws;
                string outhtml = string.Empty;
                string plaintext = string.Empty;
                try
                {
                    using (hws = (HttpWebResponse)hwr.GetResponse())
                    {
                        if (hws.ContentEncoding.ToLower().Contains("gzip"))
                        {
                            outhtml = new StreamReader(new GZipStream(hws.GetResponseStream(), CompressionMode.Decompress)).ReadToEnd();
                        }
                        else if (hws.ContentEncoding.ToLower().Contains("deflate"))
                        {
                            outhtml = new StreamReader(new DeflateStream(hws.GetResponseStream(), CompressionMode.Decompress)).ReadToEnd();
                        }
                        //else if (hws.ContentEncoding.ToLower().Contains("br"))
                        //{
                        //    //outhtml = new StreamReader(new brstream(hws.GetResponseStream(), CompressionMode.Decompress)).ReadToEnd();
                        //}
                        else
                        {
                            outhtml = new StreamReader(hws.GetResponseStream()).ReadToEnd();
                        }
                        if (!string.IsNullOrEmpty(outhtml))
                        {
                            using (Aes aes = Aes.Create())
                            {
                                aes.Mode = CipherMode.CBC;
                                aes.Padding = PaddingMode.PKCS7;
                                aes.IV = Encoding.UTF8.GetBytes("7061759328313233");
                                aes.Key = Encoding.UTF8.GetBytes("ditObg4239Ajdk@d");
                                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(outhtml)))
                                {
                                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                                    {
                                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                                        {
                                            plaintext = srDecrypt.ReadToEnd();
                                        }
                                    }
                                }

                            }
                        }
                    }
                    System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                    Dictionary<string, string> dy = js.Deserialize<Dictionary<string, string>>(plaintext);
                    if (dy["result"].Equals("Successfully"))
                    {
                       this.textBox2.Invoke(new Action(() =>
                       {
                           this.textBox2.Text = dy["confirmationid"];
                       }));
                       
                    }
                    else
                    {
                       this.textBox2.Invoke(new Action(() =>
                       {
                           this.textBox2.Text = dy["short_result"];
                       }));
                       
                    }

                   Invoke((Action)(() => { Clipboard.SetText("安装ID：" + this.textBox1.Text + "\r\n" + "确认ID：" + this.textBox2.Text); }));
                   
                   this.label3.Invoke(new Action(() =>
                   {
                       this.label3.Text = "已获取并复制到了剪切板";
                   }));
                   
                }
                catch (Exception ex)
                {

                    throw;
                }

            });
            
        }
        private string GetCookie(string url)
        {
            HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(url);
            hwr.Method = "GET";
            hwr.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7\r\n";
            hwr.KeepAlive = true;
            hwr.AllowAutoRedirect = true;
            hwr.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36";
            hwr.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            hwr.Headers.Add("accept-encoding", "gzip, deflate, br");
            hwr.Headers.Add("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,ru;q=0.7,de;q=0.6");
            //hwr.Headers.Add("Cookie", "userlogin=usersettings=6Y6J10PXuqzKxk6kYtsmQNb8QZLN41TB3BfFXKBOhw/nWUqqE5UVkpzMe8jRCDP92w7zAl/PnPTRHax/O50kDdZNzG1OoCSDmKDYc1YQFquyiFly3LLp/9lg+thKbYeiBpNrHGKyOkBsicVNQUSKtEBOjqW9eAIkVyuWUrqLIGeCOAPW4EreXXA/FJeexYStlkga8shEG7YhZo2q7fHrP+JU3eEEwkongW6MUDbTSWBAgj7v7lDjUSwtGdKVrT05u8ZjBOKPrXO2XHMC/jsspOKDQ0fit6vMA0ut2Lh1pNS/lZhcD2L1JLoQyYi9hTiGmAAFygjBW+Ner8555gsVTg==\r\n\r\n");
            HttpWebResponse hws;
            string outhtml = string.Empty;
            try
            {
                using (hws = (HttpWebResponse)hwr.GetResponse())
                {
                    //return hws.Headers.Get("Set-Cookie");
                    return hws.Headers[HttpResponseHeader.SetCookie];
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.textBox2.Text);
        }
    }
}
