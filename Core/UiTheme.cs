using MachineVision_PCB.UIControl;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MachineVision_PCB
{
    /// <summary>
    /// 산업용 머신비전 UI: 다크 배경 + 라임/그린 액센트 (#8CC63F).
    /// </summary>
    public static class UiTheme
    {
        public static readonly Color Background = Color.FromArgb(0x1A, 0x1A, 0x1A);
        public static readonly Color Panel = Color.FromArgb(0x26, 0x2A, 0x22);
        public static readonly Color PanelDeep = Color.FromArgb(0x12, 0x14, 0x10);
        public static readonly Color Border = Color.FromArgb(0x42, 0x52, 0x38);
        public static readonly Color Accent = Color.FromArgb(0x8C, 0xC6, 0x3F);
        /// <summary>도킹 비활성 탭·패널용 다크 올리브 그린.</summary>
        public static readonly Color AccentInactive = Color.FromArgb(0x34, 0x3E, 0x2C);
        public static readonly Color AccentHover = Color.FromArgb(0x5A, 0x72, 0x3A);
        public static readonly Color AccentStrip = Color.FromArgb(0x1C, 0x22, 0x18);
        public static readonly Color TextPrimary = Color.FromArgb(0xE8, 0xE8, 0xE8);
        public static readonly Color TextMuted = Color.FromArgb(0xA8, 0xB0, 0x9C);
        public static readonly Color TextOnAccent = Color.FromArgb(0x10, 0x10, 0x10);
        public static readonly Color InputBackground = Color.FromArgb(0x12, 0x14, 0x10);
        public static readonly Color ListSelection = Color.FromArgb(0x45, 0x58, 0x32);

        private static readonly VisionColorTable ColorTable = new VisionColorTable();
        private static readonly ToolStripRenderer StripRenderer = new ToolStripProfessionalRenderer(ColorTable);
        private static readonly ToolStripRenderer MainMenuLimeRenderer = new ToolStripProfessionalRenderer(new MainMenuLimeColorTable());

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
                c.BackColor = PanelDeep;
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
            }
            else if (c is GroupBox gb)
            {
                gb.BackColor = Panel;
                gb.ForeColor = Accent;
                gb.Paint -= OnGroupBoxChromePaint;
                gb.Paint += OnGroupBoxChromePaint;
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
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = Accent;
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.MouseOverBackColor = AccentHover;
                btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(0x6A, 0x8C, 0x40);
                btn.BackColor = Panel;
                btn.ForeColor = TextPrimary;
                btn.UseVisualStyleBackColor = false;
            }
            else if (c is TextBox tb)
            {
                tb.BackColor = InputBackground;
                tb.ForeColor = TextPrimary;
                tb.BorderStyle = BorderStyle.FixedSingle;
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
                pb.BackColor = PanelDeep;
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

            Color back = selected ? Accent : Panel;
            Color fore = selected ? TextOnAccent : TextMuted;

            using (var br = new SolidBrush(back))
                e.Graphics.FillRectangle(br, r);

            using (var pen = new Pen(Border))
                e.Graphics.DrawRectangle(pen, new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1));

            if (selected)
            {
                using (var glow = new Pen(Color.FromArgb(0xAA, Accent), 2))
                    e.Graphics.DrawLine(glow, r.Left + 1, r.Bottom - 1, r.Right - 2, r.Bottom - 1);
            }

            TextRenderer.DrawText(e.Graphics, page.Text, tc.Font, r, fore,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static void OnGroupBoxChromePaint(object sender, PaintEventArgs e)
        {
            GroupBox gb = sender as GroupBox;
            if (gb == null) return;

            Rectangle bounds = gb.ClientRectangle;
            int topInset = 8;
            if (!string.IsNullOrEmpty(gb.Text))
            {
                Size ts = TextRenderer.MeasureText(gb.Text, gb.Font);
                topInset = Math.Max(ts.Height / 2, 8);
            }

            using (Pen pen = new Pen(Accent, 1))
            {
                e.Graphics.DrawRectangle(pen, 0, topInset, bounds.Width - 1, bounds.Height - topInset - 1);
            }
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
        public static void StyleMainMenuLime(MenuStrip menu)
        {
            if (menu == null) return;
            menu.RenderMode = ToolStripRenderMode.Professional;
            menu.Renderer = MainMenuLimeRenderer;
            menu.BackColor = Accent;
            menu.ForeColor = TextOnAccent;
            foreach (ToolStripItem t in menu.Items)
                t.ForeColor = TextOnAccent;
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
            mi.DropDown.BackColor = Color.FromArgb(0xEE, 0xF5, 0xDC);
            StyleDropDownItemsBlack(mi.DropDownItems);
        }

        private static void StyleDropDownItemsBlack(ToolStripItemCollection items)
        {
            foreach (ToolStripItem t in items)
            {
                t.ForeColor = Color.Black;
                ToolStripMenuItem sub = t as ToolStripMenuItem;
                if (sub != null)
                    StyleDropDownItemsBlack(sub.DropDownItems);
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
            public override Color ToolStripBorder => Accent;
            public override Color ToolStripGradientBegin => AccentStrip;
            public override Color ToolStripGradientMiddle => Panel;
            public override Color ToolStripGradientEnd => AccentStrip;
            public override Color ToolStripContentPanelGradientBegin => Background;
            public override Color ToolStripContentPanelGradientEnd => Background;
            public override Color ToolStripPanelGradientBegin => Background;
            public override Color ToolStripPanelGradientEnd => Background;
            public override Color MenuStripGradientBegin => Background;
            public override Color MenuStripGradientEnd => Background;
            public override Color MenuBorder => Border;
            public override Color MenuItemBorder => Accent;
            public override Color MenuItemSelected => Accent;
            public override Color MenuItemSelectedGradientBegin => Accent;
            public override Color MenuItemSelectedGradientEnd => Accent;
            public override Color MenuItemPressedGradientBegin => Color.FromArgb(0x7A, 0xB0, 0x35);
            public override Color MenuItemPressedGradientEnd => Color.FromArgb(0x7A, 0xB0, 0x35);
            public override Color ImageMarginGradientBegin => Panel;
            public override Color ImageMarginGradientMiddle => Panel;
            public override Color ImageMarginGradientEnd => Panel;
            public override Color SeparatorDark => Border;
            public override Color SeparatorLight => Border;
            public override Color ButtonSelectedBorder => Accent;
            public override Color ButtonSelectedGradientBegin => AccentHover;
            public override Color ButtonSelectedGradientMiddle => AccentHover;
            public override Color ButtonSelectedGradientEnd => AccentHover;
            public override Color ButtonPressedGradientBegin => Accent;
            public override Color ButtonPressedGradientMiddle => Accent;
            public override Color ButtonPressedGradientEnd => Accent;
            public override Color OverflowButtonGradientBegin => Panel;
            public override Color OverflowButtonGradientMiddle => Panel;
            public override Color OverflowButtonGradientEnd => Panel;
        }

        /// <summary>상단 MenuStrip 전용 — 라임 바·호버·드롭다운 톤.</summary>
        private sealed class MainMenuLimeColorTable : ProfessionalColorTable
        {
            private static readonly Color LimeDark = Color.FromArgb(0x6A, 0x9A, 0x30);
            private static readonly Color DropLight = Color.FromArgb(0xEE, 0xF5, 0xDC);
            private static readonly Color DropHover = Color.FromArgb(0xD0, 0xE8, 0x98);

            public override Color ToolStripBorder => LimeDark;
            public override Color ToolStripGradientBegin => Accent;
            public override Color ToolStripGradientMiddle => Accent;
            public override Color ToolStripGradientEnd => Accent;
            public override Color ToolStripContentPanelGradientBegin => Accent;
            public override Color ToolStripContentPanelGradientEnd => Accent;
            public override Color ToolStripPanelGradientBegin => Accent;
            public override Color ToolStripPanelGradientEnd => Accent;
            public override Color MenuStripGradientBegin => Accent;
            public override Color MenuStripGradientEnd => Accent;
            public override Color MenuBorder => LimeDark;
            public override Color MenuItemBorder => LimeDark;
            public override Color MenuItemSelected => DropHover;
            public override Color MenuItemSelectedGradientBegin => DropHover;
            public override Color MenuItemSelectedGradientEnd => DropHover;
            public override Color MenuItemPressedGradientBegin => Color.FromArgb(0x7A, 0xB0, 0x35);
            public override Color MenuItemPressedGradientEnd => Color.FromArgb(0x7A, 0xB0, 0x35);
            public override Color ImageMarginGradientBegin => DropLight;
            public override Color ImageMarginGradientMiddle => DropLight;
            public override Color ImageMarginGradientEnd => DropLight;
            public override Color SeparatorDark => LimeDark;
            public override Color SeparatorLight => Color.FromArgb(0xB8, 0xE0, 0x70);
            public override Color ButtonSelectedBorder => LimeDark;
            public override Color ButtonSelectedGradientBegin => AccentHover;
            public override Color ButtonSelectedGradientMiddle => AccentHover;
            public override Color ButtonSelectedGradientEnd => AccentHover;
            public override Color ButtonPressedGradientBegin => Color.FromArgb(0x7A, 0xB0, 0x35);
            public override Color ButtonPressedGradientMiddle => Color.FromArgb(0x7A, 0xB0, 0x35);
            public override Color ButtonPressedGradientEnd => Color.FromArgb(0x7A, 0xB0, 0x35);
            public override Color OverflowButtonGradientBegin => Accent;
            public override Color OverflowButtonGradientMiddle => Accent;
            public override Color OverflowButtonGradientEnd => Accent;
        }
    }
}
