namespace MachineVision_PCB
{
    partial class RunForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunForm));
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.btnGrab = new System.Windows.Forms.Button();
            this.runImageList = new System.Windows.Forms.ImageList(this.components);
            this.btnLive = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.tblAux = new System.Windows.Forms.TableLayoutPanel();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tblMain.SuspendLayout();
            this.tblAux.SuspendLayout();
            this.SuspendLayout();
            //
            // tblMain
            //
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.btnGrab, 0, 0);
            this.tblMain.Controls.Add(this.btnLive, 0, 1);
            this.tblMain.Controls.Add(this.btnStart, 0, 2);
            this.tblMain.Controls.Add(this.btnStop, 0, 3);
            this.tblMain.Controls.Add(this.tblAux, 0, 4);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Name = "tblMain";
            this.tblMain.Padding = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.tblMain.RowCount = 5;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tblMain.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.None;
            this.tblMain.Size = new System.Drawing.Size(220, 520);
            this.tblMain.TabIndex = 0;
            //
            // btnGrab
            //
            this.btnGrab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGrab.ImageIndex = 0;
            this.btnGrab.ImageList = this.runImageList;
            this.btnGrab.Location = new System.Drawing.Point(9, 11);
            this.btnGrab.Margin = new System.Windows.Forms.Padding(3, 3, 3, 4);
            this.btnGrab.Name = "btnGrab";
            this.btnGrab.Size = new System.Drawing.Size(202, 105);
            this.btnGrab.TabIndex = 0;
            this.btnGrab.Text = "촬영";
            this.btnGrab.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnGrab.UseVisualStyleBackColor = true;
            this.btnGrab.Click += new System.EventHandler(this.btnGrab_Click);
            //
            // runImageList
            //
            this.runImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("runImageList.ImageStream")));
            this.runImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.runImageList.Images.SetKeyName(0, "capture.png");
            this.runImageList.Images.SetKeyName(1, "live.png");
            this.runImageList.Images.SetKeyName(2, "inspection.png");
            this.runImageList.Images.SetKeyName(3, "stop.png");
            this.runImageList.Images.SetKeyName(4, "arrow-rotate-left-solid-full.png");
            this.runImageList.Images.SetKeyName(5, "angles-left-solid-full.png");
            this.runImageList.Images.SetKeyName(6, "angles-right-solid-full.png");
            //
            // btnLive
            //
            this.btnLive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLive.ImageIndex = 1;
            this.btnLive.ImageList = this.runImageList;
            this.btnLive.Location = new System.Drawing.Point(9, 123);
            this.btnLive.Margin = new System.Windows.Forms.Padding(3, 3, 3, 4);
            this.btnLive.Name = "btnLive";
            this.btnLive.Size = new System.Drawing.Size(202, 105);
            this.btnLive.TabIndex = 1;
            this.btnLive.Text = "라이브";
            this.btnLive.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnLive.UseVisualStyleBackColor = true;
            this.btnLive.Click += new System.EventHandler(this.btnLive_Click);
            //
            // btnStart
            //
            this.btnStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStart.ImageIndex = 2;
            this.btnStart.ImageList = this.runImageList;
            this.btnStart.Location = new System.Drawing.Point(9, 235);
            this.btnStart.Margin = new System.Windows.Forms.Padding(3, 3, 3, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(202, 105);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "검사";
            this.btnStart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            //
            // btnStop
            //
            this.btnStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStop.ImageIndex = 3;
            this.btnStop.ImageList = this.runImageList;
            this.btnStop.Location = new System.Drawing.Point(9, 347);
            this.btnStop.Margin = new System.Windows.Forms.Padding(3, 3, 3, 4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(202, 105);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "중지";
            this.btnStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            //
            // tblAux
            //
            this.tblAux.ColumnCount = 3;
            this.tblAux.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tblAux.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tblAux.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tblAux.Controls.Add(this.btnPrev, 0, 0);
            this.tblAux.Controls.Add(this.btnNext, 1, 0);
            this.tblAux.Controls.Add(this.btnReset, 2, 0);
            this.tblAux.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblAux.Location = new System.Drawing.Point(9, 459);
            this.tblAux.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.tblAux.Name = "tblAux";
            this.tblAux.RowCount = 1;
            this.tblAux.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblAux.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.None;
            this.tblAux.Size = new System.Drawing.Size(202, 53);
            this.tblAux.TabIndex = 4;
            //
            // btnPrev
            //
            this.btnPrev.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPrev.ImageIndex = 5;
            this.btnPrev.ImageList = this.runImageList;
            this.btnPrev.Location = new System.Drawing.Point(3, 3);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(61, 47);
            this.btnPrev.TabIndex = 0;
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            //
            // btnNext
            //
            this.btnNext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNext.ImageIndex = 6;
            this.btnNext.ImageList = this.runImageList;
            this.btnNext.Location = new System.Drawing.Point(70, 3);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(61, 47);
            this.btnNext.TabIndex = 1;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            //
            // btnReset
            //
            this.btnReset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReset.ImageIndex = 4;
            this.btnReset.ImageList = this.runImageList;
            this.btnReset.Location = new System.Drawing.Point(137, 3);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(62, 47);
            this.btnReset.TabIndex = 2;
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            //
            // toolTip
            //
            this.toolTip.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip_Popup);
            //
            // RunForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 520);
            this.Controls.Add(this.tblMain);
            this.Name = "RunForm";
            this.Text = "실행";
            this.Load += new System.EventHandler(this.RunForm_Load);
            this.tblMain.ResumeLayout(false);
            this.tblAux.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.TableLayoutPanel tblAux;
        private System.Windows.Forms.Button btnGrab;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnLive;
        private System.Windows.Forms.ImageList runImageList;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
