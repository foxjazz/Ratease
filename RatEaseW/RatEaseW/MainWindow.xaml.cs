using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;
using Microsoft.Win32;
namespace RatEaseW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = CurrentData.Instance;
            dtimer = new DispatcherTimer();
            dtimer.Interval = new TimeSpan(0,0,0,0,250);
            dtimer.Tick += Dtimer_Tick;
            msg = CurrentData.Instance.RedData;
            openFileDialog1 = new OpenFileDialog();
            player = new System.Media.SoundPlayer();
            gcw = new GreenScreenW();
        }
        public GreenScreenW gcw { get; set; }
        
        public ScreenCapture sc { get; set; }
        Message msg;
        FileDialog openFileDialog1;
        byte red = 130;
        private int redV;
        private List<int> RedStartList;
        private List<RedTopHeight> listRedTopHeight;
        public int cnt { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public System.Drawing.Point pTopleft { get; set; }
        public System.Drawing.Point pBottomRight { get; set; }
        public int iteration { get; set; }
        private void Dtimer_Tick(object sender, EventArgs e)
        {
            dtimer.Stop();
            if (subred == null)
                subred = new SmaRedis();
            if (!subred.Connected)
            {
                MessageBox.Show("Service is not active.");
                return;
            }
            //subred.sublist = listAlerts;
            subred.Subscribe("fade", CurrentData.Instance);
            //lblDetectedRed.Text = "Started";
            cnt = 0;
            width = pBottomRight.X - pTopleft.X;
            height = pBottomRight.Y - pTopleft.Y;

            pBottomRight = new System.Drawing.Point { X = VRec.X + VRec.Width, Y = VRec.Y + VRec.Height };
            iteration = 0;
            dtimer.Start();
        }
        public TimeSpan duration { get; set; }
        public DateTime ts { get; set; }
        public System.Drawing.Point redPixel { get; set; }
        System.Drawing.Rectangle VRec;
        Bitmap curBitmap;
        public bool foundRed { get; set; }
        private int CheckRed()
        {

            RedStartList = new List<int>();
            bool inRed = false;
            RedCount = 0;
            int ySectionStart = 0;
            listRedTopHeight.Clear();
            RedStartList.Clear();
            for (int x = 0; x < VRec.Width; x++)
            {
                for (int y = 0; y < VRec.Height; y++)
                {
                    var pixel = curBitmap.GetPixel(x, y);
                    if (pixel.R > red && pixel.B < 16 && pixel.G < 15)
                    {
                        dtimer.Stop();
                        //lblTopLeft.Text = x.ToString() + " - " + y.ToString();
                        redPixel = new System.Drawing.Point(VRec.X + x, VRec.Y + y);
                        foundRed = true;
                        //lblDetectedRed.Text = "lblDetected Red";
                        IsClear = false;
                        if (inRed == false)
                        {
                            RedStartList.Add(y);
                            ySectionStart = y;
                            RedCount++;
                            redV = 1;
                            inRed = true;
                        }
                        else
                        {
                            redV++;
                        }

                    }
                    else {
                        if (inRed)
                        {
                            inRed = false;
                            listRedTopHeight.Add(new RedTopHeight { Top = ySectionStart, Height = redV + 1 });
                        }
                    }
                }
                if (RedCount > 0)
                    return RedCount;

                duration = DateTime.Now.Subtract(ts);

                //if (duration.Seconds > 1)
                //    lblDetectedRed.Text = "lagging scan by" + duration.ToString();
            }
            return 0;
        }
        DispatcherTimer dtimer;
        System.Media.SoundPlayer player;
        public bool IsClear { get; set; }
        SmaRedis subred;
        public int RedCount { get; set; }
        private void PlaySound()
        {

            try
            {
                
                if (IsClear)
                {
                    player.SoundLocation = Properties.Settings.Default.AllClearSoundFile;
                }
                else
                {

                    player.SoundLocation = Properties.Settings.Default.AlertSoundFile;

                }
                
                if (File.Exists(player.SoundLocation))
                {
                    player.Load();
                    player.Play();
                }
                if (!IsClear)
                {
                    //subred.publish(tbSysName.Text + " vertical=" + redV);
                    var msg = new Message();
                    msg.Redcount = RedCount;
                    msg.SystemText = tbSystemName.Text;
                    //msg.System = TitleImage;
                    var rth = listRedTopHeight.FirstOrDefault();
                    if (rth != null)
                        msg.FirstRed = sc.CaptureRed(VRec, rth);
                    subred.publish(msg);

                }
                if (subred != null && IsClear)
                    subred.publish(tbSystemName.Text + " Clear ");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " could not find sound file:" + Properties.Settings.Default.AlertSoundFile);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dtimer.Start();
            BtnStart.Content = "Started";

        }

        private void SetAlertSound_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog1.Title = "Pick sound alert file";
            openFileDialog1.ShowDialog();
            Properties.Settings.Default.AlertSoundFile = openFileDialog1.FileName;
            Properties.Settings.Default.Save();
            bool hold = IsClear;
            IsClear = false;
            PlaySound();
            IsClear = hold;
        }

        private void SetClearSound_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog1.Title = "Pick sound all clear file";
            openFileDialog1.ShowDialog();
            Properties.Settings.Default.AllClearSoundFile = openFileDialog1.FileName;
            Properties.Settings.Default.Save();
            bool hold = IsClear;
            IsClear = true;
            PlaySound();
            IsClear = hold;
        }
    }
}
