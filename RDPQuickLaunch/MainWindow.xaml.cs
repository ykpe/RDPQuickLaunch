using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;

namespace RDPQuickLaunch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Default.cfg");

        public MainWindow()
        {
            InitializeComponent();
            LoadDefaultText();
        }
        private void LoadDefaultText()
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                string? targetLine = lines.FirstOrDefault(line => line.StartsWith("full address"));
                if (targetLine != null)
                {
                    inputIPBox.Text = targetLine[15..].Split(':')[0];
                }
            }
            else
            {
                inputIPBox.Text = "找不到檔案：" + filePath;
            }
        }
        private string GetIPv4Address()
        {
            try
            {
                string url = inputAddBox.Text;
                IPHostEntry hostEntry = Dns.GetHostEntry(url);
                foreach (IPAddress ip in hostEntry.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "No IPv4 address found";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(inputAddBox.Text != "URL")
            {
                inputIPBox.Text = GetIPv4Address();
            }

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("full address"))
                    {
                        lines[i] = "full address:s:" + inputIPBox.Text + ":443"; 
                        break;
                    }
                }

                File.WriteAllLines(filePath, lines);  

                string command = $"mstsc \"{filePath}\"";
                Process process = new()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c {command}",  
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true  
                    }
                };
                this.Hide();
                process.Start();

                Environment.Exit(0);
            }
            else
            {
                inputIPBox.Text = "找不到檔案：{filePath}";
            }

        }
    }
}