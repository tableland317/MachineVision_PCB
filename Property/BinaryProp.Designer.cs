namespace MachineVision_PCB.Property
{
    partial class BinaryProp
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (binRangeTrackbar != null)
                    binRangeTrackbar.RangeChanged -= Range_RangeChanged;

                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpBinary = new System.Windows.Forms.GroupBox();
            this.cbChannel = new System.Windows.Forms.ComboBox();
            this.cbHighlight = new System.Windows.Forms.ComboBox();
            this.lbChannel = new System.Windows.Forms.Label();
            this.lbHighlight = new System.Windows.Forms.Label();
            this.chkUse = new System.Windows.Forms.CheckBox();
            this.chkRotatedRect = new System.Windows.Forms.CheckBox();
            this.cbBinMethod = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewFilter = new System.Windows.Forms.DataGridView();
            this.binRangeTrackbar = new MachineVision_PCB.UIControl.RangeTrackbar();
            this.grpBinary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFilter)).BeginInit();
            this.SuspendLayout();
            // 
            // grpBinary
            // 
            this.grpBinary.Controls.Add(this.cbChannel);
            this.grpBinary.Controls.Add(this.cbHighlight);
            this.grpBinary.Controls.Add(this.lbChannel);
            this.grpBinary.Controls.Add(this.lbHighlight);
            this.grpBinary.Controls.Add(this.binRangeTrackbar);
            this.grpBinary.Location = new System.Drawing.Point(16, 54);
            this.grpBinary.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpBinary.Name = "grpBinary";
            this.grpBinary.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpBinary.Size = new System.Drawing.Size(357, 182);
            this.grpBinary.TabIndex = 0;
            this.grpBinary.TabStop = false;
            this.grpBinary.Text = "이진화";
            // 
            // cbChannel
            // 
            this.cbChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbChannel.FormattingEnabled = true;
            this.cbChannel.Location = new System.Drawing.Point(110, 80);
            this.cbChannel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbChannel.Name = "cbChannel";
            this.cbChannel.Size = new System.Drawing.Size(165, 26);
            this.cbChannel.TabIndex = 2;
            this.cbChannel.SelectedIndexChanged += new System.EventHandler(this.cbChannel_SelectedIndexChanged);
            // 
            // cbHighlight
            // 
            this.cbHighlight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHighlight.FormattingEnabled = true;
            this.cbHighlight.Location = new System.Drawing.Point(110, 132);
            this.cbHighlight.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbHighlight.Name = "cbHighlight";
            this.cbHighlight.Size = new System.Drawing.Size(165, 26);
            this.cbHighlight.TabIndex = 2;
            this.cbHighlight.SelectedIndexChanged += new System.EventHandler(this.cbHighlight_SelectedIndexChanged);
            // 
            // lbChannel
            // 
            this.lbChannel.AutoSize = true;
            this.lbChannel.Location = new System.Drawing.Point(9, 88);
            this.lbChannel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbChannel.Name = "lbChannel";
            this.lbChannel.Size = new System.Drawing.Size(104, 18);
            this.lbChannel.TabIndex = 7;
            this.lbChannel.Text = "이미지 채널";
            // 
            // lbHighlight
            // 
            this.lbHighlight.AutoSize = true;
            this.lbHighlight.Location = new System.Drawing.Point(9, 141);
            this.lbHighlight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbHighlight.Name = "lbHighlight";
            this.lbHighlight.Size = new System.Drawing.Size(98, 18);
            this.lbHighlight.TabIndex = 7;
            this.lbHighlight.Text = "하이라이트";
            // 
            // chkUse
            // 
            this.chkUse.AutoSize = true;
            this.chkUse.Location = new System.Drawing.Point(16, 21);
            this.chkUse.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkUse.Name = "chkUse";
            this.chkUse.Size = new System.Drawing.Size(70, 22);
            this.chkUse.TabIndex = 4;
            this.chkUse.Text = "검사";
            this.chkUse.UseVisualStyleBackColor = true;
            this.chkUse.CheckedChanged += new System.EventHandler(this.chkUse_CheckedChanged);
            // 
            // chkRotatedRect
            // 
            this.chkRotatedRect.AutoSize = true;
            this.chkRotatedRect.Checked = true;
            this.chkRotatedRect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRotatedRect.Location = new System.Drawing.Point(27, 478);
            this.chkRotatedRect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkRotatedRect.Name = "chkRotatedRect";
            this.chkRotatedRect.Size = new System.Drawing.Size(124, 22);
            this.chkRotatedRect.TabIndex = 14;
            this.chkRotatedRect.Text = "회전사각형";
            this.chkRotatedRect.UseVisualStyleBackColor = true;
            this.chkRotatedRect.CheckedChanged += new System.EventHandler(this.chkRotatedRect_CheckedChanged);
            // 
            // cbBinMethod
            // 
            this.cbBinMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBinMethod.FormattingEnabled = true;
            this.cbBinMethod.Location = new System.Drawing.Point(124, 244);
            this.cbBinMethod.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbBinMethod.Name = "cbBinMethod";
            this.cbBinMethod.Size = new System.Drawing.Size(167, 26);
            this.cbBinMethod.TabIndex = 12;
            this.cbBinMethod.SelectedIndexChanged += new System.EventHandler(this.cbBinMethod_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 250);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 18);
            this.label1.TabIndex = 13;
            this.label1.Text = "검사 타입";
            // 
            // dataGridViewFilter
            // 
            this.dataGridViewFilter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFilter.Location = new System.Drawing.Point(16, 292);
            this.dataGridViewFilter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dataGridViewFilter.Name = "dataGridViewFilter";
            this.dataGridViewFilter.RowHeadersWidth = 62;
            this.dataGridViewFilter.RowTemplate.Height = 23;
            this.dataGridViewFilter.Size = new System.Drawing.Size(357, 177);
            this.dataGridViewFilter.TabIndex = 11;
            this.dataGridViewFilter.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewFilter_CellValueChanged);
            this.dataGridViewFilter.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridViewFilter_CurrentCellDirtyStateChanged);
            // 
            // binRangeTrackbar
            // 
            this.binRangeTrackbar.Location = new System.Drawing.Point(10, 18);
            this.binRangeTrackbar.Margin = new System.Windows.Forms.Padding(4);
            this.binRangeTrackbar.Name = "binRangeTrackbar";
            this.binRangeTrackbar.Size = new System.Drawing.Size(339, 66);
            this.binRangeTrackbar.TabIndex = 6;
            this.binRangeTrackbar.ValueLeft = 0;
            this.binRangeTrackbar.ValueRight = 128;
            // 
            // BinaryProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkRotatedRect);
            this.Controls.Add(this.cbBinMethod);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewFilter);
            this.Controls.Add(this.chkUse);
            this.Controls.Add(this.grpBinary);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "BinaryProp";
            this.Size = new System.Drawing.Size(389, 606);
            this.grpBinary.ResumeLayout(false);
            this.grpBinary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFilter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpBinary;
        private UIControl.RangeTrackbar binRangeTrackbar;
        private System.Windows.Forms.ComboBox cbHighlight;
        private System.Windows.Forms.Label lbHighlight;
        private System.Windows.Forms.CheckBox chkUse;
        private System.Windows.Forms.CheckBox chkRotatedRect;
        private System.Windows.Forms.ComboBox cbBinMethod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridViewFilter;
        private System.Windows.Forms.ComboBox cbChannel;
        private System.Windows.Forms.Label lbChannel;
    }
}
