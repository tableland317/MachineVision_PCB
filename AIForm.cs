using MachineVision_PCB.Core;
using System;
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
                lblStatusValue.ForeColor = SystemColors.ControlText;
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

            AppendLog($"[{DateTime.Now:HH:mm:ss}] AI 검사 완료");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Image prev = pbResult.Image;
            pbResult.Image = null;
            prev?.Dispose();

            rtbLog.Clear();
            lblStatusValue.Text = "대기";
            lblStatusValue.ForeColor = SystemColors.ControlText;
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
