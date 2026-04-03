namespace MachineVision_PCB
{
    partial class AIForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.grpModel = new System.Windows.Forms.GroupBox();
            this.btnLoadModel = new System.Windows.Forms.Button();
            this.btnSelectModel = new System.Windows.Forms.Button();
            this.txtModelPath = new System.Windows.Forms.TextBox();
            this.lblModelPath = new System.Windows.Forms.Label();
            this.cbEngineType = new System.Windows.Forms.ComboBox();
            this.lblEngineType = new System.Windows.Forms.Label();
            this.grpInspect = new System.Windows.Forms.GroupBox();
            this.lblStatusValue = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnInspect = new System.Windows.Forms.Button();
            this.grpResult = new System.Windows.Forms.GroupBox();
            this.pbResult = new System.Windows.Forms.PictureBox();
            this.grpModel.SuspendLayout();
            this.grpInspect.SuspendLayout();
            this.grpResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbResult)).BeginInit();
            this.SuspendLayout();
            //
            // grpModel
            //
            this.grpModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpModel.Controls.Add(this.btnLoadModel);
            this.grpModel.Controls.Add(this.btnSelectModel);
            this.grpModel.Controls.Add(this.txtModelPath);
            this.grpModel.Controls.Add(this.lblModelPath);
            this.grpModel.Controls.Add(this.cbEngineType);
            this.grpModel.Controls.Add(this.lblEngineType);
            this.grpModel.Location = new System.Drawing.Point(6, 6);
            this.grpModel.Name = "grpModel";
            this.grpModel.Size = new System.Drawing.Size(268, 175);
            this.grpModel.TabIndex = 0;
            this.grpModel.TabStop = false;
            this.grpModel.Text = "모델 설정";
            //
            // lblEngineType
            //
            this.lblEngineType.AutoSize = true;
            this.lblEngineType.Location = new System.Drawing.Point(8, 22);
            this.lblEngineType.Name = "lblEngineType";
            this.lblEngineType.Size = new System.Drawing.Size(52, 12);
            this.lblEngineType.TabIndex = 0;
            this.lblEngineType.Text = "엔진 타입:";
            //
            // cbEngineType
            //
            this.cbEngineType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cbEngineType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEngineType.FormattingEnabled = true;
            this.cbEngineType.Location = new System.Drawing.Point(8, 38);
            this.cbEngineType.Name = "cbEngineType";
            this.cbEngineType.Size = new System.Drawing.Size(250, 20);
            this.cbEngineType.TabIndex = 1;
            this.cbEngineType.SelectedIndexChanged += new System.EventHandler(this.cbEngineType_SelectedIndexChanged);
            //
            // lblModelPath
            //
            this.lblModelPath.AutoSize = true;
            this.lblModelPath.Location = new System.Drawing.Point(8, 68);
            this.lblModelPath.Name = "lblModelPath";
            this.lblModelPath.Size = new System.Drawing.Size(52, 12);
            this.lblModelPath.TabIndex = 2;
            this.lblModelPath.Text = "모델 경로:";
            //
            // txtModelPath
            //
            this.txtModelPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModelPath.Location = new System.Drawing.Point(8, 84);
            this.txtModelPath.Name = "txtModelPath";
            this.txtModelPath.ReadOnly = true;
            this.txtModelPath.Size = new System.Drawing.Size(250, 21);
            this.txtModelPath.TabIndex = 3;
            //
            // btnSelectModel
            //
            this.btnSelectModel.Location = new System.Drawing.Point(8, 112);
            this.btnSelectModel.Name = "btnSelectModel";
            this.btnSelectModel.Size = new System.Drawing.Size(90, 26);
            this.btnSelectModel.TabIndex = 4;
            this.btnSelectModel.Text = "파일 선택...";
            this.btnSelectModel.UseVisualStyleBackColor = true;
            this.btnSelectModel.Click += new System.EventHandler(this.btnSelectModel_Click);
            //
            // btnLoadModel
            //
            this.btnLoadModel.Location = new System.Drawing.Point(104, 112);
            this.btnLoadModel.Name = "btnLoadModel";
            this.btnLoadModel.Size = new System.Drawing.Size(90, 26);
            this.btnLoadModel.TabIndex = 5;
            this.btnLoadModel.Text = "모델 로딩";
            this.btnLoadModel.UseVisualStyleBackColor = true;
            this.btnLoadModel.Click += new System.EventHandler(this.btnLoadModel_Click);
            //
            // grpInspect
            //
            this.grpInspect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpInspect.Controls.Add(this.lblStatusValue);
            this.grpInspect.Controls.Add(this.lblStatus);
            this.grpInspect.Controls.Add(this.btnClear);
            this.grpInspect.Controls.Add(this.btnInspect);
            this.grpInspect.Location = new System.Drawing.Point(6, 188);
            this.grpInspect.Name = "grpInspect";
            this.grpInspect.Size = new System.Drawing.Size(268, 90);
            this.grpInspect.TabIndex = 1;
            this.grpInspect.TabStop = false;
            this.grpInspect.Text = "검사 실행";
            //
            // btnInspect
            //
            this.btnInspect.Location = new System.Drawing.Point(8, 22);
            this.btnInspect.Name = "btnInspect";
            this.btnInspect.Size = new System.Drawing.Size(110, 30);
            this.btnInspect.TabIndex = 0;
            this.btnInspect.Text = "AI 검사 실행";
            this.btnInspect.UseVisualStyleBackColor = true;
            this.btnInspect.Click += new System.EventHandler(this.btnInspect_Click);
            //
            // btnClear
            //
            this.btnClear.Location = new System.Drawing.Point(124, 22);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(90, 30);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "결과 초기화";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(8, 62);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(30, 12);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "상태:";
            //
            // lblStatusValue
            //
            this.lblStatusValue.AutoSize = true;
            this.lblStatusValue.Location = new System.Drawing.Point(44, 62);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Size = new System.Drawing.Size(24, 12);
            this.lblStatusValue.TabIndex = 3;
            this.lblStatusValue.Text = "대기";
            // 
            // grpResult
            // 
            this.grpResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpResult.Controls.Add(this.pbResult);
            this.grpResult.Location = new System.Drawing.Point(6, 285);
            this.grpResult.Name = "grpResult";
            this.grpResult.Size = new System.Drawing.Size(268, 379);
            this.grpResult.TabIndex = 2;
            this.grpResult.TabStop = false;
            this.grpResult.Text = "결과 이미지";
            // 
            // pbResult
            // 
            this.pbResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pbResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbResult.Location = new System.Drawing.Point(8, 18);
            this.pbResult.Name = "pbResult";
            this.pbResult.Size = new System.Drawing.Size(250, 351);
            this.pbResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbResult.TabIndex = 0;
            this.pbResult.TabStop = false;
            // 
            // AIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 670);
            this.Controls.Add(this.grpResult);
            this.Controls.Add(this.grpInspect);
            this.Controls.Add(this.grpModel);
            this.Name = "AIForm";
            this.Text = "AI 검사";
            this.grpModel.ResumeLayout(false);
            this.grpModel.PerformLayout();
            this.grpInspect.ResumeLayout(false);
            this.grpInspect.PerformLayout();
            this.grpResult.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbResult)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpModel;
        private System.Windows.Forms.Label lblEngineType;
        private System.Windows.Forms.ComboBox cbEngineType;
        private System.Windows.Forms.Label lblModelPath;
        private System.Windows.Forms.TextBox txtModelPath;
        private System.Windows.Forms.Button btnSelectModel;
        private System.Windows.Forms.Button btnLoadModel;
        private System.Windows.Forms.GroupBox grpInspect;
        private System.Windows.Forms.Button btnInspect;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblStatusValue;
        private System.Windows.Forms.GroupBox grpResult;
        private System.Windows.Forms.PictureBox pbResult;
    }
}
