using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Windows.Forms;

namespace MachineVision_PCB.UIControl
{
    /// <summary>
    /// 결과 미리보기: NG일 때 Properties.Resources의 <c>NG_Image</c> 타일 배경(없으면 기본 NG 패턴) +
    /// Zoom 영역 위쪽 여백에 빨간 <c>NG</c> 텍스트.
    /// </summary>
    public class NgResultPictureBox : PictureBox
    {
        static readonly object TileLock = new object();
        static Bitmap _proceduralNgTile;
        static Bitmap _cachedResourceTile;
        static bool _resourceLoadAttempted;

        /// <summary>불량 결과 표시 시 배경 타일 + 상단 NG 라벨 사용.</summary>
        public bool ShowNgOverlay { get; set; }

        public NgResultPictureBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;

            if (Image == null)
            {
                g.Clear(BackColor);
                return;
            }

            Rectangle box = ClientRectangle;
            Rectangle imageRect = GetZoomRectangle(box.Size, Image.Size);

            if (ShowNgOverlay)
                DrawNgBackground(g, box);
            else
            {
                using (SolidBrush br = new SolidBrush(BackColor))
                    g.FillRectangle(br, box);
            }

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(Image, imageRect);

            if (ShowNgOverlay && imageRect.Top > 4)
            {
                int bandH = imageRect.Top;
                Rectangle labelBounds = new Rectangle(0, 0, Width, bandH);
                float emSize = Math.Max(16f, Math.Min(28f, bandH * 0.55f));
                using (Font f = new Font(FontFamily.GenericSansSerif, emSize, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    TextRenderer.DrawText(g, "NG", f, labelBounds, Color.FromArgb(255, 220, 40, 40),
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                        TextFormatFlags.NoPadding | TextFormatFlags.NoClipping);
                }
            }
        }

        static Rectangle GetZoomRectangle(Size box, Size image)
        {
            if (image.Width < 1 || image.Height < 1 || box.Width < 1 || box.Height < 1)
                return Rectangle.Empty;

            float ratio = Math.Min((float)box.Width / image.Width, (float)box.Height / image.Height);
            int w = (int)Math.Round(image.Width * ratio);
            int h = (int)Math.Round(image.Height * ratio);
            int x = (box.Width - w) / 2;
            int y = (box.Height - h) / 2;
            return new Rectangle(x, y, w, h);
        }

        void DrawNgBackground(Graphics g, Rectangle clip)
        {
            using (Bitmap tile = CreateTileClone())
            using (TextureBrush tb = new TextureBrush(tile, WrapMode.Tile))
            {
                g.FillRectangle(tb, clip);
            }
        }

        Bitmap CreateTileClone()
        {
            Bitmap fromRes = GetOrLoadResourceTile();
            if (fromRes != null)
                return (Bitmap)fromRes.Clone();

            lock (TileLock)
            {
                if (_proceduralNgTile == null)
                {
                    const int n = 64;
                    Bitmap bmp = new Bitmap(n, n);
                    using (Graphics gx = Graphics.FromImage(bmp))
                    {
                        gx.Clear(Color.FromArgb(16, 16, 20));
                        using (Pen gridPen = new Pen(Color.FromArgb(38, 32, 32), 1f))
                        {
                            for (int i = -n; i < n * 2; i += 10)
                                gx.DrawLine(gridPen, i, 0, i + n, n);
                        }
                        using (Font f = new Font(FontFamily.GenericSansSerif, 11f, FontStyle.Bold))
                        using (SolidBrush b = new SolidBrush(Color.FromArgb(32, 24, 24)))
                        {
                            gx.DrawString("NG", f, b, 8, 20);
                        }
                    }
                    _proceduralNgTile = bmp;
                }
                return (Bitmap)_proceduralNgTile.Clone();
            }
        }

        static Bitmap GetOrLoadResourceTile()
        {
            lock (TileLock)
            {
                if (_cachedResourceTile != null)
                    return _cachedResourceTile;
                if (_resourceLoadAttempted)
                    return null;
                _resourceLoadAttempted = true;

                try
                {
                    object o = Properties.Resources.ResourceManager.GetObject("NG_Image", Properties.Resources.Culture);
                    Bitmap src = o as Bitmap;
                    if (src == null)
                        return null;
                    _cachedResourceTile = (Bitmap)src.Clone();
                    return _cachedResourceTile;
                }
                catch (MissingManifestResourceException)
                {
                    return null;
                }
            }
        }
    }
}
