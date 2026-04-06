using MachineVision_PCB.Core;
using MachineVision_PCB.Setting;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MachineVision_PCB
{
    public partial class RunForm : DockContent
    {
        public RunForm()
        {
            InitializeComponent();
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
