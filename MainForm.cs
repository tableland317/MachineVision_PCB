using MachineVision_PCB.Core;
using MachineVision_PCB.Setting;
using MachineVision_PCB.Teach;
using MachineVision_PCB.Util;
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
    /*
    #2_DOCKPANEL# - <<<MainForm에 연동할 Form 도킹>>> 
    도킹에 필요한 참조를 추가하고, MainForm에 Form을 도킹
    1) ..\ExternalLib\Dll\Docking\WeifenLuo.WinFormsUI.Docking.dll 참조 추가
    2) ..\ExternalLib\Dll\Docking\WeifenLuo.WinFormsUI.Docking.ThemeVS2015.dll 참조 추가
    */

    /*
    #3_CAMERAVIEW_PROPERTY# - <<<카메라뷰와 속성창 기본 구현>>> 
    카메라뷰에 Pane과 PictureBox를 추가하고, 이미지 로딩 기능 구현
    UserControl과 TabControl을 이용해 속성창 구현
    1) CameraForm에 PictureBox와 Pane 추가
    2) 풀다운 메뉴에 ImageOpen 메뉴 추가
    3) #3_CAMERAVIEW_PROPERTY#1 ~ 2 이미지 로딩 기능 구현
    4) Property 폴더를 솔루션탐색기에 추가
    5) PropertiesForm에 ImageFilterProp UserControl과 BinaryProp UserControl 추가
    6) PropertiesForm에 TabControl 추가
    3) #3_CAMERAVIEW_PROPERTY#3 ~ 탭 콘트롤 연동 기능 구현
    */

    public partial class MainForm : Form
    {
        //#2_DOCKPANEL#1 DockPanel을 전역으로 선언
        private static DockPanel _dockPanel;

        // 도킹 비율 (UI 레퍼런스 스크린샷 기준: 좌 ~18%, 우 ~20%, 하단 높이 ~28%, 하단 가로 검사결과|AI 반반)
        private const double LayoutLeftRunPortion = 0.18;
        private const double LayoutRightDockPortion = 0.20;
        private const double LayoutLogOfRightPortion = 0.30;
        private const double LayoutBottomDockPortion = 0.28;
        private const double LayoutBottomAiOfPanePortion = 0.50;

        public MainForm()
        {
            InitializeComponent();

            var menuAccentLine = new Panel
            {
                Dock = DockStyle.Top,
                Height = 2,
                BackColor = UiTheme.Accent
            };
            Controls.Add(menuAccentLine);

            //#2_DOCKPANEL#2 DockPanel 초기화
            _dockPanel = new DockPanel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Background,
                DockBackColor = UiTheme.Background
            };
            Controls.Add(_dockPanel);

            // Visual Studio 2015 테마 적용
            _dockPanel.Theme = new VS2015DarkTheme();

            //#2_DOCKPANEL#6 도킹 윈도우 로드 메서드 호출
            LoadDockingWindows();

            //#6_INSP_STAGE#1 전역 인스턴스 초기화
            Global.Inst.Initialize();

            // 시작 시 자동 모델 로드 후 타이틀 갱신
            Model startModel = Global.Inst.InspStage.CurModel;
            if (startModel != null)
                this.Text = GetMdoelTitle(startModel);

            //#15_INSP_WORKER#2 연속 검사 모드 설정값 로딩
            LoadSetting();

            UiTheme.ApplyTo(this);
            UiTheme.ApplyDockPanelAccent(_dockPanel);
            UiTheme.StyleMainMenuLime(mainMenu);
            ForeColor = UiTheme.TextPrimary;
        }

        //#2_DOCKPANEL#5 도킹 윈도우를 로드하는 메서드
        private void LoadDockingWindows()
        {
            _dockPanel.AllowEndUserDocking = false;
            // Teaching: DockState.Hidden은 Show에 사용 불가 → Document로 등록 후 즉시 Hide(Contents 유지). 트리는 ResultForm으로 옮김.
            var modelTreeWindow = new ModelTreeForm();
            modelTreeWindow.Show(_dockPanel, DockState.Document);
            modelTreeWindow.Hide();

            // 중앙: 검사 이미지(전체 높이, 결과 미리보기 분리로 영역 확대)
            var cameraWindow = new CameraForm();
            cameraWindow.Show(_dockPanel, DockState.Document);

            var runWindow = new RunForm();
            runWindow.Show(cameraWindow.Pane, DockAlignment.Left, LayoutLeftRunPortion);

            // 우측: 속성 + 로그
            var propWindow = new PropertiesForm();
            propWindow.Show(_dockPanel, DockState.DockRight);
            _dockPanel.DockRightPortion = LayoutRightDockPortion;

            var logWindow = new LogForm();
            logWindow.Show(propWindow.Pane, DockAlignment.Bottom, LayoutLogOfRightPortion);

            // 하단: 검사 결과(+Teaching) | AI 폼
            _dockPanel.DockBottomPortion = LayoutBottomDockPortion;
            var resultWindow = new ResultForm();
            resultWindow.Show(_dockPanel, DockState.DockBottom);

            var aiWindow = new AIForm();
            aiWindow.Show(resultWindow.Pane, DockAlignment.Right, LayoutBottomAiOfPanePortion);

            resultWindow.AttachTeachingTree(modelTreeWindow);
        }
        private void LoadSetting()
        {
            cycleModeMenuItem.Checked = SettingXml.Inst.CycleMode;
        }

        //#2_DOCKPANEL#6 쉽게 도킹패널에 접근하기 위한 정적 함수
        //제네릭 함수 사용를 이용해 입력된 타입의 폼 객체 얻기
        public static T GetDockForm<T>() where T : DockContent
        {
            var findForm = _dockPanel.Contents.OfType<T>().FirstOrDefault();
            return findForm;
        }

        //#3_CAMERAVIEW_PROPERTY#2 풀다운 메뉴에서 이미지 열기 기능 구현
        private void imageOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CameraForm cameraForm = GetDockForm<CameraForm>();
            if (cameraForm is null)
                return;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "이미지 파일 선택";
                openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //#13_SET_IMAGE_BUFFER#2 이미지에 맞게 버퍼를 먼저 설정하도록 변경
                    string filePath = openFileDialog.FileName;
                    Global.Inst.InspStage.SetImageBuffer(filePath);
                }
            }
        }

        //#9_SETUP#1 환경설정창 실행
        private void SetupMenuItem_Click(object sender, EventArgs e)
        {
            SLogger.Write($"환경설정창 열기");
            SetupForm setupForm = new SetupForm();
            setupForm.ShowDialog();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Global.Inst.Dispose();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left)
            {
                Global.Inst.InspStage.MovePrevImage();
                return true;
            }
            if (keyData == Keys.Right)
            {
                Global.Inst.InspStage.MoveNextImage();
                return true;
            }
            if (keyData == (Keys.Control | Keys.O))
            {
                imageOpenToolStripMenuItem_Click(this, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        //#12_MODEL SAVE#3 모델 파일 열기,저장, 다른 이름으로 저장 기능 구현
        private string GetMdoelTitle(Model curModel)
        {
            if (curModel is null)
                return "";

            string modelName = curModel.ModelName;
            return $"{Define.PROGRAM_NAME} - MODEL : {modelName}";
        }

        private void modelNewMenuItem_Click(object sender, EventArgs e)
        {
            //신규 모델 추가를 위한 모델 정보를 받기 위한 창 띄우기
            NewModel newModel = new NewModel();
            newModel.ShowDialog();

            Model curModel = Global.Inst.InspStage.CurModel;
            if (curModel != null)
            {
                this.Text = GetMdoelTitle(curModel);
            }
        }

        private void modelOpenMenuItem_Click(object sender, EventArgs e)
        {
            //모델 파일 열기
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "모델 파일 선택";
                openFileDialog.Filter = "Model Files|*.xml;";
                openFileDialog.Multiselect = false;
                openFileDialog.InitialDirectory = SettingXml.Inst.ModelDir;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    if (Global.Inst.InspStage.LoadModel(filePath))
                    {
                        Model curModel = Global.Inst.InspStage.CurModel;
                        if (curModel != null)
                        {
                            this.Text = GetMdoelTitle(curModel);
                        }
                    }
                }
            }
        }

        private void modelSaveMenuItem_Click(object sender, EventArgs e)
        {
            //모델 파일 저장
            Global.Inst.InspStage.SaveModel("");
        }

        private void modelSaveAsMenuItem_Click(object sender, EventArgs e)
        {
            //다른이름으로 모델 파일 저장
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = SettingXml.Inst.ModelDir;
                saveFileDialog.Title = "모델 파일 선택";
                saveFileDialog.Filter = "Model Files|*.xml;";
                saveFileDialog.DefaultExt = "xml";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    Global.Inst.InspStage.SaveModel(filePath);
                }
            }
        }

        //#15_INSP_WORKER#3 Cycle 모드 설정
        private void cycleModeMenuItem_Click(object sender, EventArgs e)
        {
            // 현재 체크 상태 확인
            bool isChecked = cycleModeMenuItem.Checked;
            SettingXml.Inst.CycleMode = isChecked;
        }

       
    }
}
