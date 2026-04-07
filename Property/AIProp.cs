using MachineVision_PCB.Core;
using System;
using System.Windows.Forms;

namespace MachineVision_PCB.Property
{
    /// <summary>
    /// AI 검사 결과 후처리 필터 속성창.
    /// RangeTrackbar로 Min/Max Area를 조절하면 AI 엔진 재실행 없이
    /// 결과 오버레이를 실시간 갱신합니다.
    /// </summary>
    public partial class AIProp : UserControl
    {
        // SetProperty() 중 이벤트가 설정을 덮어쓰지 않도록 하는 플래그
        private bool _isSettingProperty = false;

        public AIProp()
        {
            InitializeComponent();

            // Area 필터 범위: 0 ~ 100,000 px²
            areaRangeTrackbar.Minimum = 0;
            areaRangeTrackbar.Maximum = 100000;
            areaRangeTrackbar.RangeChanged += AreaRangeTrackbar_RangeChanged;
        }

        // ──────────────────────────────────────────────────────────────
        // Public API
        // ──────────────────────────────────────────────────────────────

        /// <summary>설정값을 UI에 반영합니다 (모델 로드·속성창 열기 시 호출).</summary>
        public void SetProperty()
        {
            _isSettingProperty = true;
            try
            {
                var s = Setting.SettingXml.Inst;
                chkUseFilter.Checked         = s.AI_UseAreaFilter;
                areaRangeTrackbar.ValueLeft  = s.AI_MinDefectArea;
                areaRangeTrackbar.ValueRight = s.AI_MaxDefectArea;
                grpAreaFilter.Enabled        = s.AI_UseAreaFilter;
                nudCycleDelay.Value          = Math.Max(nudCycleDelay.Minimum,
                                               Math.Min(nudCycleDelay.Maximum, s.AI_CycleDelayMs));
                UpdateLabels();
            }
            finally
            {
                _isSettingProperty = false;
            }
        }

        // ──────────────────────────────────────────────────────────────
        // Private helpers
        // ──────────────────────────────────────────────────────────────

        /// <summary>UI 값을 전역 설정에 반영합니다.</summary>
        private void GetProperty()
        {
            var s = Setting.SettingXml.Inst;
            s.AI_UseAreaFilter  = chkUseFilter.Checked;
            s.AI_MinDefectArea  = areaRangeTrackbar.ValueLeft;
            s.AI_MaxDefectArea  = areaRangeTrackbar.ValueRight;
            s.AI_CycleDelayMs   = (int)nudCycleDelay.Value;
        }

        /// <summary>현재 Min/Max 값 레이블을 갱신합니다.</summary>
        private void UpdateLabels()
        {
            lblMinValue.Text = areaRangeTrackbar.ValueLeft.ToString("N0");
            lblMaxValue.Text = areaRangeTrackbar.ValueRight.ToString("N0");
        }

        /// <summary>AI 엔진 재실행 없이 현재 필터로 결과 오버레이를 재렌더링합니다.</summary>
        private void RefreshDisplay()
        {
            RunForm runForm = MainForm.GetDockForm<RunForm>();
            runForm?.RefreshAIDisplay();
        }

        // ──────────────────────────────────────────────────────────────
        // Event handlers
        // ──────────────────────────────────────────────────────────────

        private void AreaRangeTrackbar_RangeChanged(object sender, EventArgs e)
        {
            if (_isSettingProperty) return;
            GetProperty();
            UpdateLabels();
            RefreshDisplay();
        }

        private void chkUseFilter_CheckedChanged(object sender, EventArgs e)
        {
            grpAreaFilter.Enabled = chkUseFilter.Checked;
            if (_isSettingProperty) return;
            GetProperty();
            RefreshDisplay();
        }

        private void nudCycleDelay_ValueChanged(object sender, EventArgs e)
        {
            if (_isSettingProperty) return;
            GetProperty();
            // 딜레이는 다음 사이클부터 적용되므로 화면 갱신 불필요
        }
    }
}
