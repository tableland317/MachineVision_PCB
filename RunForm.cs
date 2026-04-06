using MachineVision_PCB.Core;
using MachineVision_PCB.Inspect;
using MachineVision_PCB.Setting;
using MachineVision_PCB.UIControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MachineVision_PCB
{
    public partial class RunForm : DockContent
    {
        /// <summary>실행 버튼 영역 고정 높이(px). 나머지는 결과 이미지.</summary>
        private const int LayoutRunButtonsAreaHeight = 288;

        private SplitContainer _splitRun;
        private GroupBox _grpAiResult;
        private NgResultPictureBox _pbAiResult;
        private Label _lblResultNg;
        private bool _syncingRunSplit;

        public RunForm()
        {
            InitializeComponent();
            BuildRunLayoutWithAiPreview();
        }

        private void BuildRunLayoutWithAiPreview()
        {
            _splitRun = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                BorderStyle = BorderStyle.None,
                SplitterWidth = 5,
                Panel1MinSize = 160,
                Panel2MinSize = 100,
                FixedPanel = FixedPanel.Panel1
            };
            _splitRun.Panel2.Padding = new Padding(2, 0, 2, 2);
            Controls.Remove(tblMain);
            tblMain.Dock = DockStyle.Fill;
            _splitRun.Panel1.Controls.Add(tblMain);

            _grpAiResult = new GroupBox
            {
                Text = "결과 이미지",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 2, 0, 0),
                Padding = new Padding(10, 8, 10, 10)
            };
            var tblAi = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            tblAi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblAi.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblAi.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _lblResultNg = new Label
            {
                Name = "lblResultNg",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Point, 129),
                ForeColor = Color.Red,
                Margin = new Padding(0, 0, 0, 4),
                Text = "NG",
                Visible = false
            };
            _pbAiResult = new NgResultPictureBox
            {
                BackColor = Color.Black,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = Padding.Empty
            };
            tblAi.Controls.Add(_lblResultNg, 0, 0);
            tblAi.Controls.Add(_pbAiResult, 0, 1);
            _grpAiResult.Controls.Add(tblAi);
            _splitRun.Panel2.Controls.Add(_grpAiResult);
            Controls.Add(_splitRun);
            _splitRun.Resize += (_, __) => ApplyRunButtonsImageSplitRatio();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ApplyRunButtonsImageSplitRatio();
        }

        private void ApplyRunButtonsImageSplitRatio()
        {
            if (_splitRun == null || _syncingRunSplit)
                return;
            int h = _splitRun.Height;
            int sw = _splitRun.SplitterWidth;
            if (h <= _splitRun.Panel1MinSize + _splitRun.Panel2MinSize + sw)
                return;
            int want = LayoutRunButtonsAreaHeight;
            want = Math.Max(_splitRun.Panel1MinSize, want);
            want = Math.Min(want, h - _splitRun.Panel2MinSize - sw);
            if (want < 0 || want == _splitRun.SplitterDistance)
                return;
            _syncingRunSplit = true;
            try
            {
                _splitRun.SplitterDistance = want;
            }
            finally
            {
                _syncingRunSplit = false;
            }
        }

        /// <summary>AI 검사 결과 오버레이 이미지를 실행 열 하단에 표시합니다.</summary>
        public void ShowAIInspectionOverlay(Bitmap resultImage, SaigeAI saigeAI, bool isDefect)
        {
            if (resultImage == null || _pbAiResult == null)
                return;

            Image prev = _pbAiResult.Image;
            Bitmap pbBmp = isDefect
                ? (saigeAI.ExtractDefectZoomPreview(resultImage) ?? (Bitmap)resultImage.Clone())
                : (Bitmap)resultImage.Clone();
            _pbAiResult.Image = pbBmp;
            _pbAiResult.ShowNgOverlay = isDefect;
            if (_lblResultNg != null)
            {
                _lblResultNg.ForeColor = Color.Red;
                _lblResultNg.Visible = isDefect;
            }
            _pbAiResult.Invalidate();
            prev?.Dispose();
        }

        public void ClearAIInspectionOverlay()
        {
            if (_pbAiResult == null)
                return;
            Image prev = _pbAiResult.Image;
            _pbAiResult.Image = null;
            _pbAiResult.ShowNgOverlay = false;
            if (_lblResultNg != null)
                _lblResultNg.Visible = false;
            _pbAiResult.Invalidate();
            prev?.Dispose();
        }

        private void btnGrab_Click(object sender, EventArgs e)
        {
            //#13_SET_IMAGE_BUFFER#3 그랩시 이미지 버퍼를 먼저 설정하도록 변경
            Global.Inst.InspStage.CheckImageBuffer();
            Global.Inst.InspStage.Grab(0);
        }

        //#8_INSPECT_BINARY#20 검사 시작 버튼을 디자인창에서 만들고, 검사 함수 호출
        private void btnStart_Click(object sender, EventArgs e)
        {
            //#15_INSP_WORKER#10 카메라 타입에 따라 자동 검사 모드 설정

            string serialID = $"{DateTime.Now:MM-dd HH:mm:ss}";
            Global.Inst.InspStage.InspectReady("LOT_NUMBER", serialID);

            if (SettingXml.Inst.CamType == Grab.CameraType.None)
            {
                bool cycleMode = SettingXml.Inst.CycleMode;
                Global.Inst.InspStage.CycleInspect(cycleMode);
            }
            else if(SettingXml.Inst.CamType == Grab.CameraType.HikRobotCam)
            {
                bool cycleMode = SettingXml.Inst.CycleMode;
                Global.Inst.InspStage.CycleInspect(cycleMode);
            }
            else
            {
                Global.Inst.InspStage.StartAutoRun();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Global.Inst.InspStage.StopCycle();
        }

        //#8_LIVE#3 라이브 모드 버튼 추가
        private void btnLive_Click(object sender, EventArgs e)
        {
            Global.Inst.InspStage.LiveMode = !Global.Inst.InspStage.LiveMode;

            //#17_WORKING_STATE#6 LIVE 상태 화면 표시
            if (Global.Inst.InspStage.LiveMode)
            {
                Global.Inst.InspStage.SetWorkingState(WorkingState.LIVE);

                //#13_SET_IMAGE_BUFFER#4 그랩시 이미지 버퍼를 먼저 설정하도록 변경
                Global.Inst.InspStage.CheckImageBuffer();
                Global.Inst.InspStage.Grab(0);
            }
            else
            {
                Global.Inst.InspStage.SetWorkingState(WorkingState.NONE);
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            Global.Inst.InspStage.MovePrevImage();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Global.Inst.InspStage.MoveNextImage();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "ROI와 이미지를 모두 초기화하시겠습니까?",
                "초기화 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            Global.Inst.InspStage.ResetAll();
        }

        private void toolTip_Popup(object sender, PopupEventArgs e)
        {

        }

        private void RunForm_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
                UiTheme.ApplyTo(this);

            toolTip.SetToolTip(btnPrev, "이전 이미지");     // << 버튼
            toolTip.SetToolTip(btnNext, "다음 이미지");     // >> 버튼
            toolTip.SetToolTip(btnGrab, "이미지 촬상"); // 촬영 버튼
            toolTip.SetToolTip(btnLive, "라이브");      // LIVE 버튼
            toolTip.SetToolTip(btnStart, "검사 시작");      // 검사 버튼
            toolTip.SetToolTip(btnStop, "검사 중지");      // 중지 버튼
            toolTip.SetToolTip(btnReset, "초기화");      // 초기화 버튼
        }


    }
}
