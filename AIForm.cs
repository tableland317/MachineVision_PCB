using MachineVision_PCB.Core;
using MachineVision_PCB.Inspect;
using MachineVision_PCB.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MachineVision_PCB
{
    public partial class AIForm : DockContent
    {
        private SaigeAI _saigeAI;
        private string _modelPath = string.Empty;
        private AIEngineType _engineType;

        // 클래스(Crack/Scratch)별 누적 검사 결과
        private readonly List<AIClassRecord> _classRecords = new List<AIClassRecord>();

        private bool _isCycleRunning = false;
        private CancellationTokenSource _cycleCts;

        private bool _syncingAiLayout;

        public AIForm()
        {
            InitializeComponent();

            cbEngineType.DataSource = Enum.GetValues(typeof(AIEngineType))
                                         .Cast<AIEngineType>()
                                         .ToList();
            cbEngineType.SelectedIndex = 0;

            WireAiFormLayout();
            SyncAiFormClientWidths();
        }

        /// <summary>도킹 폭이 좁아도 콤보·텍스트·버튼이 그룹박스 안에 머물도록 합니다.</summary>
        private void WireAiFormLayout()
        {
            grpModel.Padding = new Padding(10, 8, 10, 10);
            grpInspect.Padding = new Padding(10, 8, 10, 10);
            cbEngineType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtModelPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnSelectModel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnLoadModel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnInspect.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnClear.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            Resize += (_, __) => SyncAiFormClientWidths();
        }

        private void SyncAiFormClientWidths()
        {
            if (_syncingAiLayout || grpModel == null || grpInspect == null)
                return;
            _syncingAiLayout = true;
            try
            {
                const int gap = 10;
                int padL = grpModel.Padding.Left;
                int padR = grpModel.Padding.Right;
                int w = grpModel.ClientSize.Width - padL - padR;
                if (w >= 40)
                {
                    cbEngineType.SetBounds(padL, cbEngineType.Top, w, cbEngineType.Height);
                    txtModelPath.SetBounds(padL, txtModelPath.Top, w, txtModelPath.Height);
                    int half = Math.Max(72, (w - gap) / 2);
                    int total = half * 2 + gap;
                    if (total > w)
                        half = Math.Max(60, (w - gap) / 2);
                    btnSelectModel.SetBounds(padL, btnSelectModel.Top, half, btnSelectModel.Height);
                    btnLoadModel.SetBounds(padL + half + gap, btnLoadModel.Top, half, btnLoadModel.Height);
                }

                int ix = grpInspect.Padding.Left;
                int ir = grpInspect.Padding.Right;
                int iw = grpInspect.ClientSize.Width - ix - ir;
                if (iw >= 40)
                {
                    int hW = Math.Max(72, (iw - gap) / 2);
                    if (hW * 2 + gap > iw)
                        hW = Math.Max(60, (iw - gap) / 2);
                    btnInspect.SetBounds(ix, btnInspect.Top, hW, btnInspect.Height);
                    btnClear.SetBounds(ix + hW + gap, btnClear.Top, hW, btnClear.Height);
                }
            }
            finally
            {
                _syncingAiLayout = false;
            }
        }

        private void cbEngineType_SelectedIndexChanged(object sender, EventArgs e)
        {
            AIEngineType selected = (AIEngineType)cbEngineType.SelectedItem;

            if (selected != _engineType)
            {
                _saigeAI?.Dispose();
                _saigeAI = null;
                lblStatusValue.Text = "대기";
                lblStatusValue.ForeColor = UiTheme.TextPrimary;
            }

            _engineType = selected;
        }

        private void btnSelectModel_Click(object sender, EventArgs e)
        {
            string filter;
            switch (_engineType)
            {
                case AIEngineType.AnomalyDetection:
                    filter = "Anomaly Detection Files|*.saigeiad;";
                    break;
                case AIEngineType.Segmentation:
                    filter = "Segmentation Files|*.saigeseg;";
                    break;
                case AIEngineType.Detection:
                    filter = "Detection Files|*.saigedet;";
                    break;
                default:
                    filter = "AI Files|*.*;";
                    break;
            }

            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "AI 모델 파일 선택";
                dlg.Filter = filter;
                dlg.Multiselect = false;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _modelPath = dlg.FileName;
                    txtModelPath.Text = _modelPath;
                }
            }
        }

        private void btnLoadModel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_modelPath))
            {
                MessageBox.Show("모델 파일을 선택해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (_saigeAI == null)
                    _saigeAI = Global.Inst.InspStage.AIModule;

                _saigeAI.LoadEngine(_modelPath, _engineType);

                lblStatusValue.Text = "모델 로딩 완료";
                lblStatusValue.ForeColor = Color.Green;
                AppendLog($"모델 로딩 완료: {System.IO.Path.GetFileName(_modelPath)}");
            }
            catch (Exception ex)
            {
                lblStatusValue.Text = "모델 로딩 실패";
                lblStatusValue.ForeColor = Color.Red;
                AppendLog($"모델 로딩 실패: {ex.Message}", SLogger.LogType.Error);
            }
        }

        private async void btnInspect_Click(object sender, EventArgs e)
        {
            // 사이클 모드 OFF → 단일 검사
            if (!Setting.SettingXml.Inst.CycleMode)
            {
                RunOneInspection();
                return;
            }

            // 사이클 모드 ON → 실행 중이면 중지, 아니면 시작
            if (_isCycleRunning)
            {
                _cycleCts?.Cancel();
                return;
            }

            _isCycleRunning = true;
            btnInspect.Text = "■ 중지";
            _cycleCts = new CancellationTokenSource();
            var token = _cycleCts.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    // 현재 이미지 검사
                    if (!RunOneInspection())
                        break;

                    // 다음 검사 전 대기
                    await Task.Delay(1000, token);

                    // 파일 모드: 다음 이미지로 이동, 마지막 이미지면 종료
                    if (!Global.Inst.InspStage.UseCamera)
                    {
                        bool moved = Global.Inst.InspStage.MoveNextImage();
                        if (!moved)
                            break;
                    }
                    // 카메라 모드: Live Grab으로 ImageSpace가 자동 갱신되므로 별도 처리 불필요
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                _isCycleRunning = false;
                btnInspect.Text = "AI 검사 실행";
            }
        }

        /// <summary>현재 이미지에 대해 AI 검사 1회 실행. 성공 시 true 반환.</summary>
        private bool RunOneInspection()
        {
            if (_saigeAI == null)
            {
                MessageBox.Show("AI 모듈이 초기화되지 않았습니다. 모델을 먼저 로딩하세요.", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Bitmap bitmap = Global.Inst.InspStage.GetBitmap();
            if (bitmap == null)
            {
                MessageBox.Show("현재 이미지가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            bool result = _saigeAI.InspAIModule(bitmap);
            if (!result)
                return false;

            Bitmap resultImage = _saigeAI.GetResultImage();
            string imagePath = Global.Inst.InspStage.CurModel?.InspectImagePath ?? "";
            var aiResults = _saigeAI.GetAIInspResults();
            bool isDefect = aiResults.Any(r => r.IsDefect);

            RunForm runForm = MainForm.GetDockForm<RunForm>();
            if (resultImage != null)
            {
                runForm?.ShowAIInspectionOverlay(resultImage, _saigeAI, isDefect);
                Global.Inst.InspStage.UpdateDisplay(resultImage);
            }
            else
                runForm?.ClearAIInspectionOverlay();

            ResultForm resultForm = MainForm.GetDockForm<ResultForm>();

            foreach (var _result in aiResults)
            {
                string className = _result.ObjectID;
                int.TryParse(_result.ResultValue, out int count);

                var classRec = _classRecords.FirstOrDefault(c => c.ClassName == className);
                if (classRec == null)
                {
                    classRec = new AIClassRecord { ClassName = className };
                    _classRecords.Add(classRec);
                }

                var imgEntry = classRec.ImageEntries.FirstOrDefault(x => x.ImagePath == imagePath);
                if (imgEntry != null)
                    imgEntry.Count = count;
                else
                    classRec.ImageEntries.Add(new AIImageEntry { ImagePath = imagePath, Count = count });
            }

            resultForm?.ShowAIRecords(_classRecords);

            lblStatusValue.Text = isDefect ? "NG" : "OK";
            lblStatusValue.ForeColor = isDefect ? Color.Red : Color.Green;

            AppendLog($"AI 검사 완료 → {(isDefect ? "NG" : "OK")} " +
                      $"({string.Join(", ", aiResults.Select(r => $"{r.ObjectID}:{r.ResultValue}개"))})");

            return true;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _classRecords.Clear();
            RunForm runForm = MainForm.GetDockForm<RunForm>();
            runForm?.ClearAIInspectionOverlay();
            ResultForm resultForm = MainForm.GetDockForm<ResultForm>();
            resultForm?.ShowAIRecords(_classRecords);

            // CameraForm에 박스가 구워진 이미지 대신 원본 이미지로 복원
            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            cameraForm?.UpdateDisplay();

            lblStatusValue.Text = "대기";
            lblStatusValue.ForeColor = UiTheme.TextPrimary;
        }

        /// <summary>LogForm 리스트 및 log4net 파일과 공유 (SLogger).</summary>
        private void AppendLog(string message, SLogger.LogType type = SLogger.LogType.Info)
        {
            SLogger.Write("[AI] " + message, type);
        }
    }
}
