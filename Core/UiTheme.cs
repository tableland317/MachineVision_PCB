using MachineVision_PCB.UIControl;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MachineVision_PCB
{
    /// <summary>
    /// 다크 크롬 + 에메랄드 액센트 + 패널/버튼 입체감 (Nuvo류 하이테크 기기 UI 톤).
    /// </summary>
    public static class UiTheme
    {
        public static readonly Color Background = Color.FromArgb(0x12, 0x14, 0x18);
        public static readonly Color Panel = Color.FromArgb(0x24, 0x28, 0x30);
        public static readonly Color PanelDeep = Color.FromArgb(0x18, 0x1A, 0x22);
        /// <summary>이미지·디스플레이 영역용 블루 틴트 베이스.</summary>
        public static readonly Color DisplaySurface = Color.FromArgb(0x12, 0x1E, 0x34);
        public static readonly Color Border = Color.FromArgb(0x3A, 0x40, 0x4A);
        public static readonly Color BorderHighlight = Color.FromArgb(0x4A, 0x52, 0x5E);
        public static readonly Color Accent = Color.FromArgb(0x2E, 0xD4, 0x7A);
        public static readonly Color AccentInactive = Color.FromArgb(0x28, 0x34, 0x30);
        public static readonly Color AccentHover = Color.FromArgb(0x4E, 0xE8, 0x9A);
        public static readonly Color AccentStrip = Color.FromArgb(0x1A, 0x1E, 0x26);
        public static readonly Color TextPrimary = Color.FromArgb(0xEE, 0xF0, 0xF4);
        public static readonly Color TextMuted = Color.FromArgb(0x94, 0xA0, 0xB0);
        public static readonly Color TextOnAccent = Color.FromArgb(0x08, 0x12, 0x0C);
        public static readonly Color InputBackground = Color.FromArgb(0x14, 0x1A, 0x26);
        public static readonly Color ListSelection = Color.FromArgb(0x28, 0x48, 0x3C);

        private const int ButtonCornerRadius = 7;
        private const int TabCornerRadius = 4;

        private static readonly VisionColorTable ColorTable = new VisionColorTable();
        private static readonly ToolStripRenderer StripRenderer = new ToolStripProfessionalRenderer(ColorTable);

        public static void ApplyTo(Control root)
        {
            if (root == null) return;
            StyleRecursive(root);
        }

        /// <summary>
        /// VS2015 다크 도킹 탭의 파란 하이라이트를 라임/그린 톤으로 바꿉니다. (Skin API는 DLL 버전에 따라 reflection 사용)
        /// </summary>
        public static void ApplyDockPanelAccent(DockPanel panel)
        {
            if (panel == null) return;
            try
            {
                object skin = GetPropertyValue(panel, "Skin");
                if (skin == null) return;
                object stripSkin = GetPropertyValue(skin, "DockPaneStripSkin");
                if (stripSkin == null) return;

                object docGrad = GetPropertyValue(stripSkin, "DocumentGradient");
                if (docGrad != null)
                {
                    ApplyTabGradientSkin(GetPropertyValue(docGrad, "ActiveTabGradient"), Accent, Accent, TextOnAccent);
                    ApplyTabGradientSkin(GetPropertyValue(docGrad, "InactiveTabGradient"), AccentInactive, AccentInactive, TextMuted);
                    ApplyTabGradientSkin(GetPropertyValue(docGrad, "HoverTabGradient"), AccentHover, AccentHover, TextPrimary);
                    object dockStrip = GetPropertyValue(docGrad, "DockStripGradient");
                    SetGradientColors(dockStrip, AccentStrip, AccentStrip);
                }

                object twGrad = GetPropertyValue(stripSkin, "ToolWindowGradient");
                if (twGrad != null)
                {
                    ApplyTabGradientSkin(GetPropertyValue(twGrad, "ActiveTabGradient"), Accent, Accent, TextOnAccent);
                    ApplyTabGradientSkin(GetPropertyValue(twGrad, "InactiveTabGradient"), AccentInactive, AccentInactive, TextMuted);
                    ApplyTabGradientSkin(GetPropertyValue(twGrad, "HoverTabGradient"), AccentHover, AccentHover, TextPrimary);
                    object dockStripTw = GetPropertyValue(twGrad, "DockStripGradient");
                    SetGradientColors(dockStripTw, AccentStrip, AccentStrip);
                }

                DarkenDockGradientBorderColors(docGrad);
                DarkenDockGradientBorderColors(twGrad);

                panel.Refresh();
            }
            catch
            {
            }
        }

        private static object GetPropertyValue(object target, string name)
        {
            if (target == null) return null;
            PropertyInfo p = target.GetType().GetProperty(name);
            return p != null ? p.GetValue(target, null) : null;
        }

        private static void SetPropertyValue(object target, string name, object value)
        {
            if (target == null) return;
            PropertyInfo p = target.GetType().GetProperty(name);
            if (p != null && p.CanWrite)
                p.SetValue(target, value, null);
        }

        private static void ApplyTabGradientSkin(object tab, Color start, Color end, Color text)
        {
            if (tab == null) return;
            SetPropertyValue(tab, "StartColor", start);
            SetPropertyValue(tab, "EndColor", end);
            SetPropertyValue(tab, "TextColor", text);
        }

        private static void SetGradientColors(object gradient, Color start, Color end)
        {
            if (gradient == null) return;
            SetPropertyValue(gradient, "StartColor", start);
            SetPropertyValue(gradient, "EndColor", end);
        }

        /// <summary>도킹 스킨에 Border/Separator 색이 있으면 밝은 테두리 대신 어둡게 맞춥니다.</summary>
        private static void DarkenDockGradientBorderColors(object gradientHost)
        {
            if (gradientHost == null) return;
            Color dark = Color.FromArgb(0x28, 0x2C, 0x34);
            try
            {
                foreach (PropertyInfo p in gradientHost.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (!p.CanWrite || p.PropertyType != typeof(Color))
                        continue;
                    string n = p.Name;
                    if (n.IndexOf("Border", StringComparison.OrdinalIgnoreCase) < 0 &&
                        n.IndexOf("Separator", StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                    p.SetValue(gradientHost, dark, null);
                }
            }
            catch
            {
            }
        }

        private static GraphicsPath CreateRoundRectPath(Rectangle bounds, int radius)
        {
            int d = Math.Min(radius * 2, Math.Min(Math.Max(1, bounds.Width), Math.Max(1, bounds.Height)));
            var path = new GraphicsPath();
            if (d <= 1)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void EnableButtonUserPaint(Button btn)
        {
            if (btn == null) return;
            try
            {
                const BindingFlags bf = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
                typeof(Control).InvokeMember("SetStyle", bf, null, btn,
                    new object[] { ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque, true });
            }
            catch
            {
            }
        }

        private static void ThemedButtonInvalidate(object sender, EventArgs e)
        {
            (sender as Control)?.Invalidate();
        }

        private static Image GetButtonImage(Button btn)
        {
            if (btn == null) return null;
            if (btn.ImageList != null && btn.ImageIndex >= 0 && btn.ImageIndex < btn.ImageList.Images.Count)
                return btn.ImageList.Images[btn.ImageIndex];
            return btn.Image;
        }

        private static void DrawImageCenteredIn(Graphics g, Image img, Rectangle cr)
        {
            if (img == null || cr.Width < 1 || cr.Height < 1) return;
            int iw = img.Width;
            int ih = img.Height;
            if (iw < 1 || ih < 1) return;
            double scale = Math.Min(1.0, Math.Min((double)cr.Width / iw, (double)cr.Height / ih));
            int w = Math.Max(1, (int)(iw * scale));
            int h = Math.Max(1, (int)(ih * scale));
            int x = cr.X + (cr.Width - w) / 2;
            int y = cr.Y + (cr.Height - h) / 2;
            g.DrawImage(img, x, y, w, h);
        }

        private static void DrawImageAboveText(Graphics g, Button btn, Image img, Rectangle cr, Color textColor)
        {
            string t = btn.Text ?? "";
            Size textSize = TextRenderer.MeasureText(string.IsNullOrEmpty(t) ? " " : t, btn.Font,
                new Size(cr.Width, cr.Height), TextFormatFlags.WordBreak);
            const int sp = 4;
            int maxImgH = Math.Max(1, cr.Height - textSize.Height - sp);
            int iw = img.Width;
            int ih = img.Height;
            if (iw < 1 || ih < 1) return;
            double scale = Math.Min(1.0, Math.Min((double)cr.Width / iw, (double)maxImgH / ih));
            iw = Math.Max(1, (int)(iw * scale));
            ih = Math.Max(1, (int)(ih * scale));
            int imgX = cr.X + (cr.Width - iw) / 2;
            int blockH = ih + sp + textSize.Height;
            int y0 = cr.Y + Math.Max(0, (cr.Height - blockH) / 2);
            g.DrawImage(img, imgX, y0, iw, ih);
            if (!string.IsNullOrEmpty(t))
            {
                Rectangle tr = new Rectangle(cr.X, y0 + ih + sp, cr.Width, Math.Max(1, cr.Bottom - (y0 + ih + sp)));
                TextRenderer.DrawText(g, t, btn.Font, tr, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.Top |
                    TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
            }
        }

        private static void OnThemedButtonPaint(object sender, PaintEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 기본 Button 렌더 후 Paint 이벤트가 호출되어 모서리·포커스 점선이 남을 수 있음 → 패널색으로 덮고 아이콘·글자를 다시 그림.
            using (var wipe = new SolidBrush(Panel))
                g.FillRectangle(wipe, btn.ClientRectangle);

            Rectangle r = btn.ClientRectangle;
            r.Inflate(-2, -2);
            if (r.Width < 4 || r.Height < 4)
                return;

            bool hover = btn.Enabled && r.Contains(btn.PointToClient(Control.MousePosition));
            bool pressed = hover && (Control.MouseButtons & MouseButtons.Left) != 0;

            Color top = Color.FromArgb(0x5C, 0x64, 0x70);
            Color bottom = Color.FromArgb(0x32, 0x38, 0x42);
            if (!btn.Enabled)
            {
                top = Color.FromArgb(0x40, 0x44, 0x4C);
                bottom = Color.FromArgb(0x28, 0x2C, 0x32);
            }
            else if (pressed)
            {
                top = Color.FromArgb(0x38, 0x42, 0x4E);
                bottom = Color.FromArgb(0x22, 0x28, 0x32);
            }
            else if (hover)
            {
                top = Color.FromArgb(0x6E, 0x7A, 0x88);
                bottom = Color.FromArgb(0x3C, 0x46, 0x52);
            }

            using (GraphicsPath path = CreateRoundRectPath(r, ButtonCornerRadius))
            {
                using (var br = new LinearGradientBrush(r, top, bottom, LinearGradientMode.Vertical))
                    g.FillPath(br, path);

                Color edge = Border;
                if (btn.Enabled && hover)
                    edge = Color.FromArgb(0x9A, Accent.R, Accent.G, Accent.B);
                if (!btn.Enabled)
                    edge = Color.FromArgb(0x38, 0x3C, 0x44);
                using (var rim = new Pen(edge, 1f))
                    g.DrawPath(rim, path);
            }

            Color textColor = btn.Enabled ? btn.ForeColor : TextMuted;
            Image img = GetButtonImage(btn);
            Rectangle content = btn.ClientRectangle;
            content.Inflate(-6, -6);

            if (img != null && string.IsNullOrEmpty(btn.Text))
            {
                DrawImageCenteredIn(g, img, content);
            }
            else if (img != null && btn.TextImageRelation == TextImageRelation.ImageAboveText)
            {
                DrawImageAboveText(g, btn, img, content, textColor);
            }
            else if (img != null)
            {
                int imgW = Math.Min(Math.Max(1, content.Width / 2), img.Width);
                int imgH = (int)(img.Height * (imgW / (double)Math.Max(1, img.Width)));
                imgH = Math.Min(content.Height, imgH);
                int iy = content.Y + (content.Height - imgH) / 2;
                g.DrawImage(img, content.X, iy, imgW, imgH);
                var tr = new Rectangle(content.X + imgW + 4, content.Y, Math.Max(1, content.Width - imgW - 4), content.Height);
                TextRenderer.DrawText(g, btn.Text, btn.Font, tr, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
            }
            else
            {
                TextRenderer.DrawText(g, btn.Text, btn.Font, btn.ClientRectangle, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
            }
        }

        // 스타일.
        private static void StyleRecursive(Control c)
        {
            if (c == null) return;

            if (c is ToolStrip strip)
                StyleToolStrip(strip);

            ApplyControlColors(c);

            foreach (Control child in c.Controls)
                StyleRecursive(child);
        }

        private static void ApplyControlColors(Control c)
        {
            Type t = c.GetType();

            if (c is TabControl tabCtrl)
            {
                tabCtrl.DrawMode = TabDrawMode.OwnerDrawFixed;
                tabCtrl.ItemSize = new Size(Math.Max(tabCtrl.ItemSize.Width, 72), Math.Max(tabCtrl.ItemSize.Height, 22));
                tabCtrl.SizeMode = TabSizeMode.Fixed;
                tabCtrl.BackColor = Background;
                tabCtrl.ForeColor = TextPrimary;
                tabCtrl.DrawItem -= OnTabControlDrawItem;
                tabCtrl.DrawItem += OnTabControlDrawItem;
            }
            else if (c is ImageViewCtrl)
            {
                c.BackColor = DisplaySurface;
                c.ForeColor = TextPrimary;
            }
            else if (c is RangeTrackbar)
            {
                c.BackColor = Panel;
                c.ForeColor = TextPrimary;
            }
            else if (c is Form f)
            {
                f.BackColor = Background;
                f.ForeColor = TextPrimary;
            }
            else if (c is UserControl)
            {
                c.BackColor = Panel;
                c.ForeColor = TextPrimary;
            }
            else if (c is Panel || c is SplitterPanel || c is FlowLayoutPanel || c is TableLayoutPanel)
            {
                c.BackColor = Panel;
                c.ForeColor = TextPrimary;
            }
            else if (c is SplitContainer sc)
            {
                sc.BackColor = Background;
                sc.BorderStyle = BorderStyle.None;
            }
            else if (c is GroupBox gb)
            {
                gb.BackColor = Panel;
                gb.ForeColor = TextMuted;
                gb.FlatStyle = FlatStyle.Flat;
            }
            else if (c is Label lab)
            {
                lab.BackColor = Color.Transparent;
                lab.ForeColor = string.Equals(lab.Name, "lblResultNg", StringComparison.Ordinal)
                    ? Color.Red
                    : TextPrimary;
            }
            else if (c is CheckBox cb)
            {
                cb.BackColor = Color.Transparent;
                cb.ForeColor = TextPrimary;
                cb.FlatStyle = FlatStyle.Flat;
                cb.UseVisualStyleBackColor = false;
            }
            else if (c is RadioButton rb)
            {
                rb.BackColor = Color.Transparent;
                rb.ForeColor = TextPrimary;
                rb.FlatStyle = FlatStyle.Flat;
                rb.UseVisualStyleBackColor = false;
            }
            else if (c is Button btn)
            {
                EnableButtonUserPaint(btn);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                // ButtonBase는 BorderColor에 Transparent를 허용하지 않음(NotSupportedException).
                btn.FlatAppearance.BorderColor = Background;
                btn.FlatAppearance.MouseOverBackColor = Background;
                btn.FlatAppearance.MouseDownBackColor = Background;
                btn.BackColor = Color.Transparent;
                btn.ForeColor = TextPrimary;
                btn.UseVisualStyleBackColor = false;
                btn.Paint -= OnThemedButtonPaint;
                btn.Paint += OnThemedButtonPaint;
                btn.MouseEnter -= ThemedButtonInvalidate;
                btn.MouseLeave -= ThemedButtonInvalidate;
                btn.MouseDown -= ThemedButtonInvalidate;
                btn.MouseUp -= ThemedButtonInvalidate;
                btn.EnabledChanged -= ThemedButtonInvalidate;
                btn.MouseEnter += ThemedButtonInvalidate;
                btn.MouseLeave += ThemedButtonInvalidate;
                btn.MouseDown += ThemedButtonInvalidate;
                btn.MouseUp += ThemedButtonInvalidate;
                btn.EnabledChanged += ThemedButtonInvalidate;
            }
            else if (c is TextBox tb)
            {
                tb.BackColor = InputBackground;
                tb.ForeColor = TextPrimary;
                tb.BorderStyle = BorderStyle.None;
            }
            else if (c is ComboBox combo)
            {
                combo.BackColor = InputBackground;
                combo.ForeColor = TextPrimary;
                combo.FlatStyle = FlatStyle.Flat;
            }
            else if (c is ListBox lb)
            {
                lb.BackColor = InputBackground;
                lb.ForeColor = TextPrimary;
            }
            else if (c is TreeView tv)
            {
                tv.BackColor = InputBackground;
                tv.ForeColor = TextPrimary;
                tv.LineColor = Accent;
                tv.HotTracking = true;
            }
            else if (c is ListView lv)
            {
                lv.BackColor = InputBackground;
                lv.ForeColor = TextPrimary;
            }
            else if (c is NumericUpDown nud)
            {
                nud.BackColor = InputBackground;
                nud.ForeColor = TextPrimary;
            }
            else if (c is PictureBox pb)
            {
                pb.BackColor = DisplaySurface;
            }
            else if (c is TabPage tp)
            {
                tp.BackColor = Panel;
                tp.ForeColor = TextPrimary;
            }
            else if (t.FullName != null && t.FullName.StartsWith("BrightIdeasSoftware.", StringComparison.Ordinal))
            {
                c.BackColor = InputBackground;
                c.ForeColor = TextPrimary;
            }
            else if (c is Splitter splitter)
            {
                splitter.BackColor = AccentInactive;
            }
            else if (c is ScrollableControl && c.Controls.Count > 0)
            {
                c.ForeColor = TextPrimary;
            }
        }

        private static void OnTabControlDrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tc = sender as TabControl;
            if (tc == null || e.Index < 0 || e.Index >= tc.TabPages.Count)
                return;

            TabPage page = tc.TabPages[e.Index];
            bool selected = tc.SelectedIndex == e.Index;
            Rectangle r = e.Bounds;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color top = selected ? AccentHover : Color.FromArgb(0x3A, 0x42, 0x4E);
            Color bottom = selected ? Accent : Color.FromArgb(0x26, 0x2C, 0x36);
            Color fore = selected ? TextOnAccent : TextMuted;

            using (GraphicsPath path = CreateRoundRectPath(r, TabCornerRadius))
            using (var br = new LinearGradientBrush(r, top, bottom, LinearGradientMode.Vertical))
                g.FillPath(br, path);

            using (GraphicsPath path = CreateRoundRectPath(r, TabCornerRadius))
            using (var pen = new Pen(selected ? Color.FromArgb(0x80, Accent) : Color.FromArgb(0x50, Border), 1f))
                g.DrawPath(pen, path);

            TextRenderer.DrawText(g, page.Text, tc.Font, r, fore,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        public static void StyleContextMenu(ContextMenuStrip menu)
        {
            if (menu == null) return;
            menu.BackColor = Panel;
            menu.ForeColor = TextPrimary;
            menu.RenderMode = ToolStripRenderMode.Professional;
            menu.Renderer = StripRenderer;
            StyleToolStripItems(menu.Items);
        }

        /// <summary>
        /// 메인폼 상단 MenuStrip: 라임 배경 + 어두운 글자, 드롭다운은 밝은 연녹 배경.
        /// </summary>
        /// <summary>상단 메뉴: 다크 크롬 바 + 밝은 글자 (참고 UI 헤더 톤).</summary>
        public static void StyleMainMenuLime(MenuStrip menu)
        {
            if (menu == null) return;
            menu.RenderMode = ToolStripRenderMode.Professional;
            menu.Renderer = new ToolStripProfessionalRenderer(new MainMenuChromeColorTable());
            menu.BackColor = Color.FromArgb(0x28, 0x2C, 0x36);
            menu.ForeColor = TextPrimary;
            foreach (ToolStripItem t in menu.Items)
                t.ForeColor = TextPrimary;
            HookMainMenuDropDowns(menu.Items);
        }

        private static void HookMainMenuDropDowns(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                ToolStripMenuItem mi = item as ToolStripMenuItem;
                if (mi == null) continue;
                if (mi.HasDropDownItems)
                {
                    mi.DropDownOpening -= MainMenuDropDown_Opening;
                    mi.DropDownOpening += MainMenuDropDown_Opening;
                    HookMainMenuDropDowns(mi.DropDownItems);
                }
            }
        }

        private static void MainMenuDropDown_Opening(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = sender as ToolStripMenuItem;
            if (mi == null || mi.DropDown == null) return;
            mi.DropDown.BackColor = InputBackground;
            StyleDropDownItemsLight(mi.DropDownItems);
        }

        private static void StyleDropDownItemsLight(ToolStripItemCollection items)
        {
            foreach (ToolStripItem t in items)
            {
                t.ForeColor = TextPrimary;
                ToolStripMenuItem sub = t as ToolStripMenuItem;
                if (sub != null)
                    StyleDropDownItemsLight(sub.DropDownItems);
            }
        }

        private static void StyleToolStrip(ToolStrip strip)
        {
            strip.BackColor = Background;
            strip.ForeColor = TextPrimary;
            strip.RenderMode = ToolStripRenderMode.Professional;
            strip.Renderer = StripRenderer;
            StyleToolStripItems(strip.Items);
        }

        private static void StyleToolStripItems(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                item.ForeColor = TextPrimary;
                if (item is ToolStripMenuItem mi && mi.HasDropDownItems)
                    StyleToolStripItems(mi.DropDownItems);
                else if (item is ToolStripDropDownButton dd && dd.DropDownItems.Count > 0)
                    StyleToolStripItems(dd.DropDownItems);
            }
        }

        private sealed class VisionColorTable : ProfessionalColorTable
        {
            private static readonly Color MenuBar = Color.FromArgb(0x28, 0x2C, 0x36);
            private static readonly Color Pressed = Color.FromArgb(0x22, 0x9E, 0x58);

            public override Color ToolStripBorder => Border;
            public override Color ToolStripGradientBegin => AccentStrip;
            public override Color ToolStripGradientMiddle => Panel;
            public override Color ToolStripGradientEnd => AccentStrip;
            public override Color ToolStripContentPanelGradientBegin => Background;
            public override Color ToolStripContentPanelGradientEnd => Background;
            public override Color ToolStripPanelGradientBegin => Background;
            public override Color ToolStripPanelGradientEnd => Background;
            public override Color MenuStripGradientBegin => MenuBar;
            public override Color MenuStripGradientEnd => MenuBar;
            public override Color MenuBorder => Border;
            public override Color MenuItemBorder => AccentInactive;
            public override Color MenuItemSelected => Accent;
            public override Color MenuItemSelectedGradientBegin => AccentHover;
            public override Color MenuItemSelectedGradientEnd => Accent;
            public override Color MenuItemPressedGradientBegin => Pressed;
            public override Color MenuItemPressedGradientEnd => Pressed;
            public override Color ImageMarginGradientBegin => PanelDeep;
            public override Color ImageMarginGradientMiddle => PanelDeep;
            public override Color ImageMarginGradientEnd => PanelDeep;
            public override Color SeparatorDark => Border;
            public override Color SeparatorLight => Border;
            public override Color ButtonSelectedBorder => Accent;
            public override Color ButtonSelectedGradientBegin => AccentHover;
            public override Color ButtonSelectedGradientMiddle => AccentHover;
            public override Color ButtonSelectedGradientEnd => AccentHover;
            public override Color ButtonPressedGradientBegin => Pressed;
            public override Color ButtonPressedGradientMiddle => Pressed;
            public override Color ButtonPressedGradientEnd => Pressed;
            public override Color OverflowButtonGradientBegin => Panel;
            public override Color OverflowButtonGradientMiddle => Panel;
            public override Color OverflowButtonGradientEnd => Panel;
        }

        /// <summary>상단 MenuStrip — 다크 바, 호버 시 에메랄드 하이라이트.</summary>
        private sealed class MainMenuChromeColorTable : ProfessionalColorTable
        {
            private static readonly Color BarTop = Color.FromArgb(0x32, 0x38, 0x44);
            private static readonly Color BarBot = Color.FromArgb(0x22, 0x26, 0x2E);
            private static readonly Color HoverGlow = Color.FromArgb(0x3A, 0x52, 0x44);
            private static readonly Color Pressed = Color.FromArgb(0x26, 0x9A, 0x5E);

            public override Color ToolStripBorder => Border;
            public override Color ToolStripGradientBegin => BarTop;
            public override Color ToolStripGradientMiddle => BarBot;
            public override Color ToolStripGradientEnd => BarBot;
            public override Color ToolStripContentPanelGradientBegin => BarBot;
            public override Color ToolStripContentPanelGradientEnd => BarBot;
            public override Color ToolStripPanelGradientBegin => BarBot;
            public override Color ToolStripPanelGradientEnd => BarBot;
            public override Color MenuStripGradientBegin => BarTop;
            public override Color MenuStripGradientEnd => BarBot;
            public override Color MenuBorder => Border;
            public override Color MenuItemBorder => Border;
            public override Color MenuItemSelected => HoverGlow;
            public override Color MenuItemSelectedGradientBegin => HoverGlow;
            public override Color MenuItemSelectedGradientEnd => Color.FromArgb(0x34, 0x48, 0x3C);
            public override Color MenuItemPressedGradientBegin => Pressed;
            public override Color MenuItemPressedGradientEnd => Pressed;
            public override Color ImageMarginGradientBegin => InputBackground;
            public override Color ImageMarginGradientMiddle => InputBackground;
            public override Color ImageMarginGradientEnd => InputBackground;
            public override Color SeparatorDark => Border;
            public override Color SeparatorLight => Border;
            public override Color ButtonSelectedBorder => Accent;
            public override Color ButtonSelectedGradientBegin => HoverGlow;
            public override Color ButtonSelectedGradientMiddle => HoverGlow;
            public override Color ButtonSelectedGradientEnd => HoverGlow;
            public override Color ButtonPressedGradientBegin => Pressed;
            public override Color ButtonPressedGradientMiddle => Pressed;
            public override Color ButtonPressedGradientEnd => Pressed;
            public override Color OverflowButtonGradientBegin => BarBot;
            public override Color OverflowButtonGradientMiddle => BarBot;
            public override Color OverflowButtonGradientEnd => BarBot;
        }
    }
}
