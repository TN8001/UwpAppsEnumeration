using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UwpAppsEnumeration;

namespace Demo.Forms
{
    // DataGridViewをListBoxに似せて使用 できるだけwpfに寄せた
    // デザイナコードがあるが2019/9/19 現在まだ使えないので
    // Frameworkプロジェクトで作ってコピペした
    public partial class Form1 : Form
    {
        private static readonly Color lightBlue = ColorTranslator.FromHtml("#cbe8f6");
        private static readonly Color aliceBlue = ColorTranslator.FromHtml("#e5f3fb");
        private static readonly Color windowGlass = GetWindowColorizationColor(true);

        private static readonly DataGridViewCellStyle show = new DataGridViewCellStyle
        {
            Font = new Font("メイリオ", 12F, FontStyle.Regular, GraphicsUnit.Point, 128),
            Padding = new Padding(4),
        };
        private static readonly DataGridViewCellStyle hide = new DataGridViewCellStyle
        {
            Font = new Font("メイリオ", 12F, FontStyle.Regular, GraphicsUnit.Point, 128),
            Padding = new Padding(100, 4, 4, 4),
        };

        public Form1()
        {
            InitializeComponent();

            dataGridView1.DefaultCellStyle.SelectionBackColor = lightBlue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
        }


        private void Form1_Load(object sender, EventArgs e) => dataGridView1.ClearSelection();

        private async void Button1_Click(object sender, EventArgs e)
        {
            if(sender is Button button)
            {
                button.Enabled = false;
                appBindingSource.Clear();

                await foreach(var entry in UwpApps.EnumerateAsync(toImageSource))
                {
                    appBindingSource.Add(new UwpAppWrapper(entry));
                    Debug.WriteLine($"{entry.DisplayInfo.DisplayName}, {entry.DisplayInfo.Logo?.Width}");
                }

                button.Enabled = true;
            }

            static Image toImageSource(Stream stream, string backgroundColor)
            {
                var img = Image.FromStream(stream, false, false);
                var resize = new Bitmap(32, 32);
                var g = Graphics.FromImage(resize);

                var color = ColorTranslator.FromHtml(backgroundColor);
                if(color == Color.Transparent) color = windowGlass;
                var brush = new SolidBrush(color);

                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.FillRectangle(brush, new Rectangle(0, 0, 32, 32));
                g.DrawImage(img, 4, 4, 24, 24);
                g.Dispose();
                img.Dispose();
                brush.Dispose();

                return resize;
            }
        }

        private void DataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if(sender is DataGridView dgv)
            {
                var b = (DataGridViewButtonCell)dgv.Rows[e.RowIndex].Cells[2];
                b.Style = hide;
            }
        }

        private void DataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if(sender is DataGridView dgv)
            {
                dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = aliceBlue;
                var b = (DataGridViewButtonCell)dgv.Rows[e.RowIndex].Cells[2];
                b.Style = show;
            }
        }

        private void DataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if(sender is DataGridView dgv)
            {
                dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                var b = (DataGridViewButtonCell)dgv.Rows[e.RowIndex].Cells[2];
                b.Style = hide;
            }
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(sender is DataGridView dgv)
            {
                if(dgv.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
                {
                    var app = (UwpAppWrapper)appBindingSource[e.RowIndex];
                    _ = app.LaunchAsync();
                }
            }
        }



        [DllImport("dwmapi.dll", EntryPoint = "#127", PreserveSig = false)]
        private static extern void DwmGetColorizationParameters(out DWM_COLORIZATION_PARAMS parameters);
        private struct DWM_COLORIZATION_PARAMS
        {
            public uint clrColor;
            public uint clrAfterGlow;
            public uint nIntensity;
            public uint clrAfterGlowBalance;
            public uint clrBlurBalance;
            public uint clrGlassReflectionIntensity;
            public bool fOpaque;
        }
        private static Color GetWindowColorizationColor(bool opaque)
        {
            DwmGetColorizationParameters(out var p);

            return Color.FromArgb(
                (byte)(opaque ? 255 : p.clrColor >> 24),
                (byte)(p.clrColor >> 16),
                (byte)(p.clrColor >> 8),
                (byte)(p.clrColor));
        }
    }
}
