namespace MachineVision_PCB.Property
{
    partial class AIProp
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (areaRangeTrackbar != null)
                    areaRangeTrackbar.RangeChanged -= AreaRangeTrackbar_RangeChanged;
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.chkUseFilter      = new System.Windows.Forms.CheckBox();
            this.grpAreaFilter     = new System.Windows.Forms.GroupBox();
            this.areaRangeTrackbar = new MachineVision_PCB.UIControl.RangeTrackbar();
            this.lblMinLabel       = new System.Windows.Forms.Label();
            this.lblMinValue       = new System.Windows.Forms.Label();
            this.lblMaxLabel       = new System.Windows.Forms.Label();
            this.lblMaxValue       = new System.Windows.Forms.Label();
            this.grpCycleDelay     = new System.Windows.Forms.GroupBox();
            this.lblCycleDelayUnit = new System.Windows.Forms.Label();
            this.nudCycleDelay     = new System.Windows.Forms.NumericUpDown();
            this.lblCycleDelayDesc = new System.Windows.Forms.Label();

            this.grpAreaFilter.SuspendLayout();
            this.grpCycleDelay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCycleDelay)).BeginInit();
            this.SuspendLayout();

            // ── chkUseFilter ──────────────────────────────────────────
            this.chkUseFilter.AutoSize = true;
            this.chkUseFilter.Location = new System.Drawing.Point(8, 8);
            this.chkUseFilter.Name     = "chkUseFilter";
            this.chkUseFilter.Size     = new System.Drawing.Size(110, 22);
            this.chkUseFilter.TabIndex = 0;
            this.chkUseFilter.Text     = "Area 필터 사용";
            this.chkUseFilter.UseVisualStyleBackColor = true;
            this.chkUseFilter.CheckedChanged += new System.EventHandler(this.chkUseFilter_CheckedChanged);

            // ── grpAreaFilter ─────────────────────────────────────────
            this.grpAreaFilter.Controls.Add(this.areaRangeTrackbar);
            this.grpAreaFilter.Controls.Add(this.lblMinLabel);
            this.grpAreaFilter.Controls.Add(this.lblMinValue);
            this.grpAreaFilter.Controls.Add(this.lblMaxLabel);
            this.grpAreaFilter.Controls.Add(this.lblMaxValue);
            this.grpAreaFilter.Enabled  = false;
            this.grpAreaFilter.Location = new System.Drawing.Point(8, 36);
            this.grpAreaFilter.Name     = "grpAreaFilter";
            this.grpAreaFilter.Padding  = new System.Windows.Forms.Padding(4);
            this.grpAreaFilter.Size     = new System.Drawing.Size(360, 118);
            this.grpAreaFilter.TabIndex = 1;
            this.grpAreaFilter.TabStop  = false;
            this.grpAreaFilter.Text     = "Area 필터 범위 (px²)";

            // ── areaRangeTrackbar ─────────────────────────────────────
            this.areaRangeTrackbar.Location   = new System.Drawing.Point(8, 18);
            this.areaRangeTrackbar.Margin     = new System.Windows.Forms.Padding(4);
            this.areaRangeTrackbar.Name       = "areaRangeTrackbar";
            this.areaRangeTrackbar.Size       = new System.Drawing.Size(340, 56);
            this.areaRangeTrackbar.TabIndex   = 0;
            this.areaRangeTrackbar.ValueLeft  = 0;
            this.areaRangeTrackbar.ValueRight = 100000;

            // ── lblMinLabel ───────────────────────────────────────────
            this.lblMinLabel.AutoSize = true;
            this.lblMinLabel.Location = new System.Drawing.Point(8, 86);
            this.lblMinLabel.Name     = "lblMinLabel";
            this.lblMinLabel.TabIndex = 1;
            this.lblMinLabel.Text     = "최소:";

            // ── lblMinValue ───────────────────────────────────────────
            this.lblMinValue.AutoSize = true;
            this.lblMinValue.Location = new System.Drawing.Point(52, 86);
            this.lblMinValue.Name     = "lblMinValue";
            this.lblMinValue.TabIndex = 2;
            this.lblMinValue.Text     = "0";

            // ── lblMaxLabel ───────────────────────────────────────────
            this.lblMaxLabel.AutoSize = true;
            this.lblMaxLabel.Location = new System.Drawing.Point(190, 86);
            this.lblMaxLabel.Name     = "lblMaxLabel";
            this.lblMaxLabel.TabIndex = 3;
            this.lblMaxLabel.Text     = "최대:";

            // ── lblMaxValue ───────────────────────────────────────────
            this.lblMaxValue.AutoSize = true;
            this.lblMaxValue.Location = new System.Drawing.Point(234, 86);
            this.lblMaxValue.Name     = "lblMaxValue";
            this.lblMaxValue.TabIndex = 4;
            this.lblMaxValue.Text     = "100,000";

            // ── grpCycleDelay ─────────────────────────────────────────
            this.grpCycleDelay.Controls.Add(this.nudCycleDelay);
            this.grpCycleDelay.Controls.Add(this.lblCycleDelayUnit);
            this.grpCycleDelay.Controls.Add(this.lblCycleDelayDesc);
            this.grpCycleDelay.Location = new System.Drawing.Point(8, 164);
            this.grpCycleDelay.Name     = "grpCycleDelay";
            this.grpCycleDelay.Size     = new System.Drawing.Size(360, 64);
            this.grpCycleDelay.TabIndex = 2;
            this.grpCycleDelay.TabStop  = false;
            this.grpCycleDelay.Text     = "AI AutoRun 사이클 딜레이";

            // ── lblCycleDelayDesc ─────────────────────────────────────
            this.lblCycleDelayDesc.AutoSize = true;
            this.lblCycleDelayDesc.Location = new System.Drawing.Point(8, 28);
            this.lblCycleDelayDesc.Name     = "lblCycleDelayDesc";
            this.lblCycleDelayDesc.TabIndex = 0;
            this.lblCycleDelayDesc.Text     = "검사 후 대기:";

            // ── nudCycleDelay ─────────────────────────────────────────
            this.nudCycleDelay.Location  = new System.Drawing.Point(88, 24);
            this.nudCycleDelay.Minimum   = 100;
            this.nudCycleDelay.Maximum   = 30000;
            this.nudCycleDelay.Increment = 100;
            this.nudCycleDelay.Value     = 2000;
            this.nudCycleDelay.Name      = "nudCycleDelay";
            this.nudCycleDelay.Size      = new System.Drawing.Size(90, 21);
            this.nudCycleDelay.TabIndex  = 1;
            this.nudCycleDelay.ValueChanged += new System.EventHandler(this.nudCycleDelay_ValueChanged);

            // ── lblCycleDelayUnit ─────────────────────────────────────
            this.lblCycleDelayUnit.AutoSize = true;
            this.lblCycleDelayUnit.Location = new System.Drawing.Point(184, 28);
            this.lblCycleDelayUnit.Name     = "lblCycleDelayUnit";
            this.lblCycleDelayUnit.TabIndex = 2;
            this.lblCycleDelayUnit.Text     = "ms  (100 ~ 30000)";

            // ── AIProp (UserControl) ──────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkUseFilter);
            this.Controls.Add(this.grpAreaFilter);
            this.Controls.Add(this.grpCycleDelay);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name   = "AIProp";
            this.Size   = new System.Drawing.Size(376, 240);

            this.grpAreaFilter.ResumeLayout(false);
            this.grpAreaFilter.PerformLayout();
            this.grpCycleDelay.ResumeLayout(false);
            this.grpCycleDelay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCycleDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox          chkUseFilter;
        private System.Windows.Forms.GroupBox          grpAreaFilter;
        private UIControl.RangeTrackbar                areaRangeTrackbar;
        private System.Windows.Forms.Label             lblMinLabel;
        private System.Windows.Forms.Label             lblMinValue;
        private System.Windows.Forms.Label             lblMaxLabel;
        private System.Windows.Forms.Label             lblMaxValue;
        private System.Windows.Forms.GroupBox          grpCycleDelay;
        private System.Windows.Forms.NumericUpDown     nudCycleDelay;
        private System.Windows.Forms.Label             lblCycleDelayUnit;
        private System.Windows.Forms.Label             lblCycleDelayDesc;
    }
}
