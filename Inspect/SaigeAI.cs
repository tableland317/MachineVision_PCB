using SaigeVision.Net.V2;
using SaigeVision.Net.V2.Detection;
using SaigeVision.Net.V2.IAD;
using SaigeVision.Net.V2.Segmentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MachineVision_PCB
{
    /*
    #5_SAIGE_SDK# - <<<Saige SDK 모듈 적용>>> 
    1) SaigeVision.Net.Core.V2 참조 추가
    2) SaigeVision.Net.V2 참조 추가
    3) 솔루션 플렛폼 x64 추가
    4) class SaigeAI 코드 구현
    5) 전체 검사를 관리하는 InspStage 클래스 구현
    6) 싱글톤 패턴을 이용하여 전역적으로 접근할 수 있도록 Global 클래스 구현
    7) AIModuleProp UserControl 구현
    8) PropertiesForm에 AIModuleProp UserControl 추가
    */

    public enum AIEngineType
    {
        [Description("Anomaly Detection")]
        AnomalyDetection = 0,
        [Description("Segmentation")]
        Segmentation,
        [Description("Detection")]
        Detection
    }

    public class SaigeAI : IDisposable
    {


        AIEngineType _engineType;
        IADEngine _iADEngine = null;
        IADResult _iADResult = null;
        SegmentationEngine _segEngine = null;
        SegmentationResult _segResult = null;
        DetectionEngine _detEngine = null;
        DetectionResult _detResult = null;

        Bitmap _inspImage = null;

        /// <summary>NG 박스 외곽선 — 모델 클래스 색보다 진한 고대비 블루.</summary>
        private static readonly Color AiDefectBoxOutline = Color.FromArgb(255, 0, 45, 165);

        /// <summary>스크래치 등 밝은 픽셀에 씌우는 강조색.</summary>
        private static readonly Color ScratchHighlightColor = Color.FromArgb(255, 0, 220, 90);

        /// <summary>박스(또는 마스크) 내부에서 스크래치처럼 밝은 픽셀만 강조색으로 밀어붙이는 기준 (0–255).</summary>
        private const int ScratchHighlightLuminanceThreshold = 188;

        private const float AiDefectBoxPenWidth = 4f;

        public SaigeAI()
        {
        }

        // 엔진을 로드하는 메서드입니다.
        public void LoadEngine(string modelPath, AIEngineType engineType)
        {
            //GPU에 여러개 모델을 넣을 경우, 메모리가 부족할 수 있으므로, 해제
            DisposeMode();

            _engineType = engineType;

            switch (_engineType)
            {
                case AIEngineType.AnomalyDetection:
                    LoadIADEngine(modelPath);
                    break;
                case AIEngineType.Segmentation:
                    LoadSegEngine(modelPath);
                    break;
                case AIEngineType.Detection:
                    LoadDetEngine(modelPath);
                    break;
                default:
                    throw new NotSupportedException("지원하지 않는 엔진 타입입니다.");
            }
        }

        public void LoadIADEngine(string modelPath)
        {
            // 검사하기 위한 엔진에 대한 객체를 생성합니다.
            // 인스턴스 생성 시 모데파일 정보와 GPU Index를 입력해줍니다.
            // 필요에 따라 batch size를 입력합니다
            _iADEngine = new IADEngine(modelPath, 0);

            // 검사 전 option에 대한 설정을 가져옵니다
            IADOption option = _iADEngine.GetInferenceOption();

            option.CalcScoremap = false;

            // 검사 결과에 대한 heatmap 이미지를 가져올 지 선택합니다
            // 약간의 속도차이로 불필요할 경우 false 로 설정합니다
            option.CalcHeatmap = false;

            // 검사 결과에 대한 mask이미지를 가져올 지 선택합니다
            // 약간의 속도차이로 불필요할 경우 false 로 설정합니다
            option.CalcMask = false;

            // 검사 결과에 대한 segmencted object (contour) 에 대한 정보를 가져올 지 선택합니다
            // 약간의 속도차이로 불필요할 경우 false 로 설정합니다
            option.CalcObject = true;

            // Segmented object의 면적이 object area threshold 보다 작으면 최종 결과에서 제외됩니다.
            option.CalcObjectAreaAndApplyThreshold = true;

            // Segmented object의 면적이 object score threshold 보다 작으면 최종 결과에서 제외됩니다.
            option.CalcObjectScoreAndApplyThreshold = true;

            // 추론 API 실행에 소요되는 시간을 세분화하여 출력할지 결정합니다.
            // `true`로 설정하면 이미지를 읽는 시간, 순수 딥러닝 추론 시간, 후처리 시간을 각각 확인할 수 있습니다.
            // `false`로 설정하면 추론 API 실행에 소요된 총 시간만을 확인할 수 있습니다.
            // `true`로 설정하면 전체 추론 시간이 느려질 수 있습니다. 실제 검사 시에는 `false`로 설정하는 것을 권장합니다.
            option.CalcTime = true;

            // option을 적용하여 검사에 대한 조건을 변경할 수 있습니다.
            // 필요에 따라 writeModelFile parameter를 이용하여 모델파일에 정보를 영구적으로 변경할 수 있습니다.
            _iADEngine.SetInferenceOption(option);
        }

        public void LoadSegEngine(string modelPath)
        {
            // 검사하기 위한 엔진에 대한 객체를 생성합니다.
            // 인스턴스 생성 시 모데파일 정보와 GPU Index를 입력해줍니다.
            // 필요에 따라 batch size를 입력합니다
            _segEngine = new SegmentationEngine(modelPath, 0);

            // 검사 전 option에 대한 설정을 가져옵니다
            SegmentationOption option = _segEngine.GetInferenceOption();

            /// 추론 API 실행에 소요되는 시간을 세분화하여 출력할지 결정합니다.
            /// `true`로 설정하면 이미지를 읽는 시간, 순수 딥러닝 추론 시간, 후처리 시간을 각각 확인할 수 있습니다.
            /// `false`로 설정하면 추론 API 실행에 소요된 총 시간만을 확인할 수 있습니다.
            /// `true`로 설정하면 전체 추론 시간이 느려질 수 있습니다. 실제 검사 시에는 `false`로 설정하는 것을 권장합니다.
            option.CalcTime = true;
            option.CalcObject = true;
            option.CalcScoremap = false;
            option.CalcMask = false;
            option.CalcObjectAreaAndApplyThreshold = true;
            option.CalcObjectScoreAndApplyThreshold = true;
            option.OversizedImageHandling = OverSizeImageFlags.resize_to_fit;

            //option.ObjectScoreThresholdPerClass[1] = 0;
            //option.ObjectScoreThresholdPerClass[2] = 0;

            //option.ObjectAreaThresholdPerClass[1] = 0;
            //option.ObjectAreaThresholdPerClass[2] = 0;

            // option을 적용하여 검사에 대한 조건을 변경할 수 있습니다.
            // 필요에 따라 writeModelFile parameter를 이용하여 모델파일에 정보를 영구적으로 변경할 수 있습니다.
            _segEngine.SetInferenceOption(option);
        }

        public void LoadDetEngine(string modelPath)
        {
            // 검사하기 위한 엔진에 대한 객체를 생성합니다.
            // 인스턴스 생성 시 모데파일 정보와 GPU Index를 입력해줍니다.
            // 필요에 따라 batch size, optimaize 사용 여부를 입력합니다.
            _detEngine = new DetectionEngine(modelPath, 0);

            // 검사 전 option에 대한 설정을 가져옵니다
            DetectionOption option = _detEngine.GetInferenceOption();

            option.CalcTime = true;

            //option.ObjectScoreThresholdPerClass[1] = 50;
            //option.ObjectScoreThresholdPerClass[2] = 50;

            //option.ObjectAreaThresholdPerClass[1] = 0;
            //option.ObjectAreaThresholdPerClass[2] = 0;

            //option.MaxNumOfDetectedObjects[1] = -1;
            //option.MaxNumOfDetectedObjects[2] = -1;

            // option을 적용하여 검사에 대한 조건을 변경할 수 있습니다.
            // 필요에 따라 writeModelFile parameter를 이용하여 모델파일에 정보를 영구적으로 변경할 수 있습니다.
            _detEngine.SetInferenceOption(option);
        }


        // 입력된 이미지에서 검사 진행
        public bool InspAIModule(Bitmap bmpImage)
        {
            if (bmpImage is null)
            {
                MessageBox.Show("이미지가 없습니다. 유효한 이미지를 입력해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            _inspImage = bmpImage;

            //int targetWidth = 2594;  // Saige 모델 학습 시 설정한 Width
            //int targetHeight = 1944; // Saige 모델 학습 시 설정한 Height

            //Bitmap resizedBmp = new Bitmap(bmpImage, targetWidth, targetHeight);

            SrImage srImage = new SrImage(bmpImage);

            Stopwatch sw = Stopwatch.StartNew();

            switch (_engineType)
            {
                case AIEngineType.AnomalyDetection:
                    // IAD 엔진을 이용하여 검사합니다.
                    if (_iADEngine == null)
                    {
                        MessageBox.Show("엔진이 초기화되지 않았습니다. LoadEngine 메서드를 호출하여 엔진을 초기화하세요.");
                        return false;
                    }

                    _iADResult = _iADEngine.Inspection(srImage);
                    break;
                case AIEngineType.Segmentation:
                    if (_segEngine == null)
                    {
                        MessageBox.Show("엔진이 초기화되지 않았습니다. LoadEngine 메서드를 호출하여 엔진을 초기화하세요.");
                        return false;
                    }
                    _segResult = _segEngine.Inspection(srImage);
                    break;
                case AIEngineType.Detection:
                    if (_detEngine == null)
                    {
                        MessageBox.Show("엔진이 초기화되지 않았습니다. LoadEngine 메서드를 호출하여 엔진을 초기화하세요.");
                        return false;
                    }
                    // Detection 엔진을 이용하여 검사합니다.
                    _detResult = _detEngine.Inspection(srImage);
                    break;
            }

            //txt_InspectionTime.Text = sw.ElapsedMilliseconds.ToString();
            sw.Stop();

            return true;
        }

        /// <summary>박스(또는 세그 마스크) 안에서 밝은 영역(스크래치)만 강조색(초록)으로 칠합니다.</summary>
        private static void HighlightBrightScratchPixels(Bitmap bmp, Rectangle bounds, GraphicsPath clipPath)
        {
            bounds.Intersect(new Rectangle(0, 0, bmp.Width, bmp.Height));
            if (bounds.Width < 1 || bounds.Height < 1)
                return;

            if (clipPath != null)
            {
                using (Region region = new Region(clipPath))
                    HighlightBrightScratchPixelsCore(bmp, bounds, region);
            }
            else
                HighlightBrightScratchPixelsCore(bmp, bounds, null);
        }

        private static void HighlightBrightScratchPixelsCore(Bitmap bmp, Rectangle bounds, Region clipRegion)
        {
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            try
            {
                int stride = bd.Stride;
                IntPtr scan0 = bd.Scan0;
                int th = ScratchHighlightLuminanceThreshold;

                for (int y = bounds.Top; y < bounds.Bottom; y++)
                {
                    for (int x = bounds.Left; x < bounds.Right; x++)
                    {
                        if (clipRegion != null && !clipRegion.IsVisible(new Point(x, y)))
                            continue;

                        int idx = y * stride + x * 3;
                        byte blue = Marshal.ReadByte(scan0, idx);
                        byte green = Marshal.ReadByte(scan0, idx + 1);
                        byte red = Marshal.ReadByte(scan0, idx + 2);
                        int lum = (red * 30 + green * 59 + blue * 11) / 100;
                        if (lum < th)
                            continue;

                        Marshal.WriteByte(scan0, idx, ScratchHighlightColor.B);
                        Marshal.WriteByte(scan0, idx + 1, ScratchHighlightColor.G);
                        Marshal.WriteByte(scan0, idx + 2, ScratchHighlightColor.R);
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(bd);
            }
        }

        private static void BuildContourPath(SegmentedObject prediction, GraphicsPath gp)
        {
            gp.Reset();
            if (prediction.Contour.Value == null || prediction.Contour.Value.Count < 3)
                return;
            gp.AddPolygon(prediction.Contour.Value.ToArray());
            if (prediction.Contour.InnerValue == null)
                return;
            foreach (var innerValue in prediction.Contour.InnerValue)
            {
                if (innerValue != null && innerValue.Count >= 3)
                    gp.AddPolygon(innerValue.ToArray());
            }
        }

        /// <summary>클래스 이름을 기반으로 색상을 반환합니다. Scratch=Yellow, Crack=Red, 기타=Magenta.</summary>
        private static Color GetClassColor(string className)
        {
            switch (className)
            {
                case "Scratch": return Color.Red;
                case "Crack":   return Color.Yellow;
                default:        return Color.Magenta;
            }
        }

        // IAD / Segmentation: 클래스명 기반 색상으로 폴리곤 내부 반투명 채우기 + 뚜렷한 외곽선.
        private void DrawSegResult(SegmentedObject[] segmentedObjects, Bitmap bmp)
        {
            if (segmentedObjects == null)
                return;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                foreach (var prediction in segmentedObjects)
                {
                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        BuildContourPath(prediction, gp);
                        if (gp.PointCount == 0)
                            continue;

                        // 클래스명으로 색상 결정: Scratch=Yellow, Crack=Red, 기타=Magenta
                        Color baseColor = GetClassColor(prediction.ClassInfo.Name);

                        // 내부 반투명 채우기 (알파 76 ≈ 30%)
                        using (SolidBrush fillBrush = new SolidBrush(Color.FromArgb(76, baseColor)))
                            g.FillPath(fillBrush, gp);

                        // 외곽선 (알파 255, 두께 2)
                        using (Pen outlinePen = new Pen(Color.FromArgb(255, baseColor), 2f))
                        {
                            outlinePen.LineJoin = LineJoin.Round;
                            g.DrawPath(outlinePen, gp);
                        }
                    }
                }
            }
        }

        // Detection: 박스 내부 밝은 픽셀을 초록 강조 후 진한 블루 사각형.
        private void DrawDetectionResult(DetectionResult result, Bitmap bmp)
        {
            if (result?.DetectedObjects == null)
                return;

            foreach (var prediction in result.DetectedObjects)
            {
                var bb = prediction.BoundingBox;
                int x = (int)Math.Floor(bb.X);
                int y = (int)Math.Floor(bb.Y);
                int w = Math.Max(1, (int)Math.Ceiling(bb.Width));
                int h = Math.Max(1, (int)Math.Ceiling(bb.Height));
                var region = new Rectangle(x, y, w, h);
                HighlightBrightScratchPixels(bmp, region, null);
            }

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(AiDefectBoxOutline, AiDefectBoxPenWidth))
                {
                    pen.LineJoin = LineJoin.Round;
                    foreach (var prediction in result.DetectedObjects)
                    {
                        var bb = prediction.BoundingBox;
                        float fx = (float)bb.X;
                        float fy = (float)bb.Y;
                        float fw = Math.Max(1f, (float)bb.Width);
                        float fh = Math.Max(1f, (float)bb.Height);
                        g.DrawRectangle(pen, fx, fy, fw, fh);
                    }
                }
            }
        }

        public Bitmap GetResultImage()
        {
            if (_inspImage is null)
                return null;

            Bitmap resultImage = _inspImage.Clone(new Rectangle(0, 0, _inspImage.Width, _inspImage.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            switch (_engineType)
            {
                case AIEngineType.AnomalyDetection:
                    if (_iADResult == null)
                        return resultImage;
                    DrawSegResult(_iADResult.SegmentedObjects, resultImage);
                    break;
                case AIEngineType.Segmentation:
                    if (_segResult == null)
                        return resultImage;
                    DrawSegResult(_segResult.SegmentedObjects, resultImage);
                    break;
                case AIEngineType.Detection:
                    if (_detResult == null)
                        return resultImage;
                    DrawDetectionResult(_detResult, resultImage);
                    break;
            }

            return resultImage;
        }

        /// <summary>
        /// NG(결함) 영역의 바운딩을 합친 사각형. 없으면 <see cref="Rectangle.Empty"/>.
        /// </summary>
        public Rectangle GetDefectUnionBounds()
        {
            Rectangle union = Rectangle.Empty;

            switch (_engineType)
            {
                case AIEngineType.AnomalyDetection:
                    if (_iADResult?.SegmentedObjects == null)
                        return union;
                    foreach (var o in _iADResult.SegmentedObjects)
                    {
                        Rectangle r = BoundsFromSegmentedObject(o);
                        if (!r.IsEmpty)
                            union = union.IsEmpty ? r : Rectangle.Union(union, r);
                    }
                    break;

                case AIEngineType.Segmentation:
                    if (_segResult?.SegmentedObjects == null)
                        return union;
                    foreach (var o in _segResult.SegmentedObjects)
                    {
                        Rectangle r = BoundsFromSegmentedObject(o);
                        if (!r.IsEmpty)
                            union = union.IsEmpty ? r : Rectangle.Union(union, r);
                    }
                    break;

                case AIEngineType.Detection:
                    if (_detResult?.DetectedObjects == null)
                        return union;
                    foreach (var o in _detResult.DetectedObjects)
                    {
                        var bb = o.BoundingBox;
                        var r = new Rectangle(
                            (int)Math.Floor(bb.X),
                            (int)Math.Floor(bb.Y),
                            (int)Math.Ceiling(bb.Width),
                            (int)Math.Ceiling(bb.Height));
                        if (r.Width > 0 && r.Height > 0)
                            union = union.IsEmpty ? r : Rectangle.Union(union, r);
                    }
                    break;
            }

            return union;
        }

        /// <summary>
        /// 전체 결과 오버레이 이미지에서 결함 부분만 잘라, 작은 영역은 확대합니다. AIForm 결과 미리보기용.
        /// </summary>
        public Bitmap ExtractDefectZoomPreview(Bitmap fullAnnotatedImage, int margin = 20, int minShortEdge = 100, int maxLongEdge = 720)
        {
            if (fullAnnotatedImage == null)
                return null;

            Rectangle bounds = GetDefectUnionBounds();
            if (bounds.IsEmpty)
                return null;

            bounds.Inflate(margin, margin);
            Rectangle imgRect = new Rectangle(0, 0, fullAnnotatedImage.Width, fullAnnotatedImage.Height);
            bounds.Intersect(imgRect);
            if (bounds.Width < 1 || bounds.Height < 1)
                return null;

            Bitmap cropped;
            try
            {
                cropped = fullAnnotatedImage.Clone(bounds, fullAnnotatedImage.PixelFormat);
            }
            catch (ArgumentException)
            {
                return null;
            }

            int w = cropped.Width;
            int h = cropped.Height;
            int shortEdge = Math.Min(w, h);
            int longEdge = Math.Max(w, h);

            float scale = 1f;
            if (shortEdge < minShortEdge && shortEdge > 0)
                scale = minShortEdge / (float)shortEdge;

            int nw = Math.Max(1, (int)Math.Round(w * scale));
            int nh = Math.Max(1, (int)Math.Round(h * scale));
            if (Math.Max(nw, nh) > maxLongEdge)
            {
                float shrink = maxLongEdge / (float)Math.Max(nw, nh);
                nw = Math.Max(1, (int)Math.Round(nw * shrink));
                nh = Math.Max(1, (int)Math.Round(nh * shrink));
            }

            if (nw == w && nh == h)
                return cropped;

            Bitmap scaled = new Bitmap(nw, nh, fullAnnotatedImage.PixelFormat);
            using (Graphics g = Graphics.FromImage(scaled))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(cropped, 0, 0, nw, nh);
            }
            cropped.Dispose();
            return scaled;
        }

        private static Rectangle BoundsFromSegmentedObject(SegmentedObject prediction)
        {
            try
            {
                if (prediction.Contour.Value == null || prediction.Contour.Value.Count < 1)
                    return Rectangle.Empty;

                using (GraphicsPath gp = new GraphicsPath())
                {
                    if (prediction.Contour.Value.Count >= 3)
                        gp.AddPolygon(prediction.Contour.Value.ToArray());
                    else
                    {
                        foreach (var p in prediction.Contour.Value)
                        {
                            float x = Convert.ToSingle(p.X);
                            float y = Convert.ToSingle(p.Y);
                            gp.AddRectangle(new RectangleF(x - 3f, y - 3f, 6f, 6f));
                        }
                    }

                    if (prediction.Contour.InnerValue != null)
                    {
                        foreach (var innerValue in prediction.Contour.InnerValue)
                        {
                            if (innerValue == null || innerValue.Count < 3)
                                continue;
                            gp.AddPolygon(innerValue.ToArray());
                        }
                    }

                    RectangleF b = gp.GetBounds();
                    if (b.Width < 0.5f || b.Height < 0.5f)
                        return Rectangle.Empty;

                    return Rectangle.FromLTRB(
                        (int)Math.Floor(b.Left),
                        (int)Math.Floor(b.Top),
                        (int)Math.Ceiling(b.Right),
                        (int)Math.Ceiling(b.Bottom));
                }
            }
            catch
            {
                return Rectangle.Empty;
            }
        }

        // 검사 결과를 클래스명별로 묶어 InspResult 리스트로 반환
        public List<Inspect.InspResult> GetAIInspResults()
        {
            var results = new List<Inspect.InspResult>();

            SegmentedObject[] segObjects = null;

            switch (_engineType)
            {
                case AIEngineType.AnomalyDetection:
                    if (_iADResult == null) return results;
                    segObjects = _iADResult.SegmentedObjects;
                    break;
                case AIEngineType.Segmentation:
                    if (_segResult == null) return results;
                    segObjects = _segResult.SegmentedObjects;
                    break;
                case AIEngineType.Detection:
                    if (_detResult == null) return results;
                    // Detection은 DetectedObject 기준으로 처리
                    var detGroups = _detResult.DetectedObjects
                        .GroupBy(o => o.ClassInfo.Name);
                    foreach (var group in detGroups)
                    {
                        string className = group.Key;
                        int count = group.Count();
                        string details = string.Join("\r\n", group.Select((o, i) =>
                            $"  [{i + 1}] Score:{o.Score:F2}  " +
                            $"Rect:({o.BoundingBox.X},{o.BoundingBox.Y},{o.BoundingBox.Width},{o.BoundingBox.Height})"));

                        var result = new Inspect.InspResult
                        {
                            ObjectID  = className,
                            InspType  = Core.InspectType.InspAIModule,
                            IsDefect  = count > 0,
                            ResultValue  = count.ToString(),
                            ResultInfos  = $"[{className}] 검출 수: {count}\r\n{details}"
                        };
                        results.Add(result);
                    }
                    return results;
            }

            if (segObjects == null) return results;

            // IAD / Segmentation 공통 처리 (클래스명 기준 그룹핑)
            var groups = segObjects.GroupBy(o => o.ClassInfo.Name);
            foreach (var group in groups)
            {
                string className = group.Key;
                int count = group.Count();
                string details = string.Join("\r\n", group.Select((o, i) =>
                    $"  [{i + 1}] Score:{o.Score:F2}  Area:{o.Area}"));

                var result = new Inspect.InspResult
                {
                    ObjectID    = className,
                    InspType    = Core.InspectType.InspAIModule,
                    IsDefect    = count > 0,
                    ResultValue = count.ToString(),
                    ResultInfos = $"[{className}] 검출 수: {count}\r\n{details}"
                };
                results.Add(result);
            }

            return results;
        }

        private void DisposeMode()
        {
            //GPU에 여러개 모델을 넣을 경우, 메모리가 부족할 수 있으므로, 해제
            if (_iADEngine != null)
                _iADEngine.Dispose();

            if (_segEngine != null)
                _segEngine.Dispose();

            if (_detEngine != null)
                _detEngine.Dispose();
        }

        #region Disposable

        private bool disposed = false; // to detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.

                    // 검사완료 후 메모리 해제를 합니다.
                    // 엔진 사용이 완료되면 꼭 dispose 해주세요
                    DisposeMode();
                }

                // Dispose unmanaged managed resources.

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion //Disposable
    }
}
