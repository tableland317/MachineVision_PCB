using MachineVision_PCB.Core;
using MachineVision_PCB.Inspect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        public AIForm()
        {
            InitializeComponent();

            cbEngineType.DataSource = Enum.GetValues(typeof(AIEngineType))
                                         .Cast<AIEngineType>()
                                         .ToList();
            cbEngineType.SelectedIndex = 0;
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
                AppendLog($"[{DateTime.Now:HH:mm:ss}] 모델 로딩 완료: {System.IO.Path.GetFileName(_modelPath)}");
            }
            catch (Exception ex)
            {
                lblStatusValue.Text = "모델 로딩 실패";
                lblStatusValue.ForeColor = Color.Red;
                AppendLog($"[{DateTime.Now:HH:mm:ss}] 모델 로딩 실패: {ex.Message}");
            }
        }

        private void btnInspect_Click(object sender, EventArgs e)
        {
            if (_saigeAI == null)
            {
                MessageBox.Show("AI 모듈이 초기화되지 않았습니다. 모델을 먼저 로딩하세요.", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Bitmap bitmap = Global.Inst.InspStage.GetBitmap();
            if (bitmap == null)
            {
                MessageBox.Show("현재 이미지가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool result = _saigeAI.InspAIModule(bitmap);
            if (!result)
                return;

            Bitmap resultImage = _saigeAI.GetResultImage();
            if (resultImage != null)
            {
                Image prev = pbResult.Image;
                pbResult.Image = (Bitmap)resultImage.Clone();
                prev?.Dispose();

                Global.Inst.InspStage.UpdateDisplay(resultImage);
            }

            // 현재 이미지 경로 기준으로 클래스별 결과 누적
            string imagePath = Global.Inst.InspStage.CurModel?.InspectImagePath ?? "";
            var aiResults = _saigeAI.GetAIInspResults();

            foreach (var _result in aiResults)
            {
                string className = _result.ObjectID; // Crack / Scratch 등
                int.TryParse(_result.ResultValue, out int count);

                // 해당 클래스 레코드 찾거나 새로 생성
                var classRec = _classRecords.FirstOrDefault(c => c.ClassName == className);
                if (classRec == null)
                {
                    classRec = new AIClassRecord { ClassName = className };
                    _classRecords.Add(classRec);
                }

                // 같은 이미지 경로가 이미 있으면 덮어쓰기, 없으면 추가
                var imgEntry = classRec.ImageEntries.FirstOrDefault(x => x.ImagePath == imagePath);
                if (imgEntry != null)
                {
                    imgEntry.Count = count;
                }
                else
                {
                    classRec.ImageEntries.Add(new AIImageEntry { ImagePath = imagePath, Count = count });
                }
            }

            // ResultForm에 누적 결과 전달
            ResultForm resultForm = MainForm.GetDockForm<ResultForm>();
            resultForm?.ShowAIRecords(_classRecords);

            bool isDefect = aiResults.Any(r => r.IsDefect);
            lblStatusValue.Text = isDefect ? "NG" : "OK";
            lblStatusValue.ForeColor = isDefect ? Color.Red : Color.Green;

            AppendLog($"[{DateTime.Now:HH:mm:ss}] AI 검사 완료 → {(isDefect ? "NG" : "OK")} " +
                      $"({string.Join(", ", aiResults.Select(r => $"{r.ObjectID}:{r.ResultValue}개"))})");

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Image prev = pbResult.Image;
            pbResult.Image = null;
            prev?.Dispose();

            _classRecords.Clear();
            ResultForm resultForm = MainForm.GetDockForm<ResultForm>();
            resultForm?.ShowAIRecords(_classRecords);

            // CameraForm에 박스가 구워진 이미지 대신 원본 이미지로 복원
            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            cameraForm?.UpdateDisplay();

            rtbLog.Clear();
            lblStatusValue.Text = "대기";
            lblStatusValue.ForeColor = UiTheme.TextPrimary;
        }

        private void AppendLog(string message)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new Action<string>(AppendLog), message);
                return;
            }

            rtbLog.AppendText(message + Environment.NewLine);
            rtbLog.ScrollToCaret();
        }
    }
}
