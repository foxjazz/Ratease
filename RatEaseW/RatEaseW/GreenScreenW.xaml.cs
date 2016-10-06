using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace RatEaseW
{
    /// <summary>
    /// Interaction logic for GreenScreenW.xaml
    /// </summary>
    public partial class GreenScreenW : Window
    {
        public GreenScreenW()
        {
            InitializeComponent();
            
            cd = CurrentData.Instance;
        }
        CurrentData cd;
        private bool IsRecSelectOn { get; set; }
        private System.Drawing.Point StartPoint { get; set; }
        private System.Drawing.Point EndPoint { get; set; }
        private Rectangle rec { get; set; }
        private Rectangle recStart { get; set; }
        System.Drawing.Graphics graphics { get; set; }
        private bool RecSettingEnable { get; set; }
        private int width { get; set; }
        private int height { get; set; }
        public bool UseTitle { get; set; }
        public bool mouseDown { get; set; }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
            var pos = e.GetPosition(this);

            if ( e.RightButton== MouseButtonState.Pressed)
            {
                graphics.Clear(System.Drawing.Color.PaleGreen);
                return;
            }

            if (UseTitle)
            {
                cd.pBottomRight = new System.Drawing.Point((int)pos.X, (int)pos.Y);
                cd.SetTitleImage(false);
            }
            if (cd.SelectedDraw == "SetVertical")
            {
                cd.pBottomRight = new System.Drawing.Point((int)pos.X, (int) pos.Y);
                cd.SetVerticalImage(false);
                cd.recselect = false;
                cd.foundRed = false;
            }


            if (cd.SelectedDraw == "foundRed")
            {
                rec = new System.Drawing.Rectangle(cd.redPixel.X, cd.redPixel.Y, 6, 6);
                graphics.DrawRectangle(System.Drawing.Pens.DarkRed, rec);
            }
            if (cd.SelectedDraw  == "recselect")
            {
                width = cd.pBottomRight.X - cd.pTopleft.X; height = cd.pBottomRight.Y - cd.pTopleft.Y;
                if (width > 1 && height > 1)
                {
                    rec = new System.Drawing.Rectangle(cd.pTopleft.X, cd.pTopleft.Y, width, height);
                    graphics.DrawRectangle(System.Drawing.Pens.Black, rec);
                }
            }
            this.Hide();

        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {

        }


    }
}
