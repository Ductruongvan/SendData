using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net;
using System.Runtime.CompilerServices;

namespace SendFile
{
  public partial class Form1 : Form
  {
    private string _pathToWatch ="D:\\Isoft\\Project\\Posimat2";
    private string _user = "yourUsername"; // Thay thế bằng thông tin xác thực tài khoản máy chủ
    private string _password = "yourPassword"; // Thay thế bằng mật khẩu tài khoản máy chủ
    private string _apiUrl = "https://www.google.com/"; // Thay thế bằng API endpoint của máy chủ
    public Form1()
    { 
      InitializeComponent(); 
      // Tạo một FileSystemWatcher với đường dẫn cần theo dõi
      FileSystemWatcher watcher = new FileSystemWatcher();
      watcher.Path = _pathToWatch;
      watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName ;
      watcher.Filter = "*.*";
      watcher.Created += new FileSystemEventHandler(OnChanged);
      watcher.Changed += new FileSystemEventHandler(OnChanged);
      watcher.EnableRaisingEvents = true; 
    }

   

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
      // Xử lý sự kiện khi có file hoặc thư mục được thêm vào 
      if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created)
      {
        Console.WriteLine($"Đã thêm mới file hoặc thư mục: {e.Name} - Loại: {e.Name}");
        string filePath = Path.Combine(((FileSystemWatcher)sender).Path, e.Name);
        SendFileWebServer(filePath, _apiUrl);
      }
    }
    private void SendFileWebServer(string filePath,string apiUrl)
    {
      try
      {
        HttpWebRequest httpWebRequest = WebRequest.Create(apiUrl) as HttpWebRequest;
        httpWebRequest.KeepAlive = false;
        httpWebRequest.Method = "POST"; 
        httpWebRequest.PreAuthenticate = true;
        // Đọc dữ liệu từ tập tin Excel
        byte[] fileData;
        using (FileStream fs = File.OpenRead(filePath))
        {
          fileData = new byte[fs.Length];
          fs.Read(fileData, 0, fileData.Length);

        }
        // Thiết lập loại nội dung và độ dài của dữ liệu
        httpWebRequest.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; // Loại nội dung của tập tin Excel
        httpWebRequest.ContentLength = fileData.Length;
        // Gửi dữ liệu tập tin đến máy chủ
        using (Stream requestStream = httpWebRequest.GetRequestStream())
        {
          requestStream.Write(fileData, 0, fileData.Length);
        }
        // Nhận phản hồi từ máy chủ
        using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
        using (Stream responseStream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(responseStream))
        {
          string responseText = reader.ReadToEnd();
          Console.WriteLine("Phản hồi từ máy chủ: " + responseText);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Lỗi: " + ex.Message);
      }
    } 
  }
}
