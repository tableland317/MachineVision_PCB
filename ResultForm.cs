using BrightIdeasSoftware;
using MachineVision_PCB.Inspect;
using MachineVision_PCB.Teach;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MachineVision_PCB
{
    public partial class ResultForm : DockContent
    {
        //검사 결과를 보여주기 위한 컨트롤 추가
        private SplitContainer _splitContainer;
        private TreeListView _treeListView;
        private TextBox _txtDetails;

        public ResultForm()
        {
            InitializeComponent();

            //컨트롤 초기화, 아래 함수 구현할것
            InitTreeListView();
        }

        private void InitTreeListView()
        {
            // SplitContainer 사용하여 상하 분할 레이아웃 구성
            _splitContainer = new SplitContainer()
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 120,
                Panel1MinSize = 70,
                Panel2MinSize = 70
            };

            //TreeListView 검사 결과 트리 생성
            _treeListView = new TreeListView()
            {
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                ShowGroups = false,
                UseFiltering = true,
                OwnerDraw = true,
                MultiSelect = false,
                GridLines = true
            };
            _treeListView.SelectionChanged += TreeListView_SelectionChanged;

            // AIClassRecord(클래스)만 확장 가능, AIImageEntry/InspWindow는 리프 노드
            _treeListView.CanExpandGetter = x => x is AIClassRecord || x is InspWindow;

            _treeListView.ChildrenGetter = x =>
            {
                if (x is AIClassRecord cls) return cls.ImageEntries;
                if (x is InspWindow w)      return w.InspResultList;
                return new List<object>();
            };

            // 컬럼: 클래스명 / 파일명
            var colUID = new OLVColumn("클래스 / 파일명", "")
            {
                Width = 180,
                IsEditable = false,
                AspectGetter = obj =>
                {
                    if (obj is AIClassRecord cls)  return cls.ClassName;
                    if (obj is AIImageEntry entry) return entry.FileName;
                    if (obj is InspWindow win)     return win.UID;
                    if (obj is InspResult res)     return res.InspType == Core.InspectType.InspAIModule
                                                          ? res.ObjectID
                                                          : res.InspType.ToString();
                    return "";
                }
            };

            var colStatus = new OLVColumn("판정", "")
            {
                Width = 60,
                TextAlign = HorizontalAlignment.Center,
                AspectGetter = obj =>
                {
                    if (obj is AIClassRecord cls)  return cls.IsDefect ? "NG" : "OK";
                    if (obj is AIImageEntry entry) return entry.IsDefect ? "NG" : "OK";
                    if (obj is InspResult res)     return res.IsDefect ? "NG" : "OK";
                    return "";
                }
            };

            var colValue = new OLVColumn("검출 수", "")
            {
                Width = 70,
                TextAlign = HorizontalAlignment.Center,
                AspectGetter = obj =>
                {
                    if (obj is AIClassRecord cls)  return cls.TotalCount.ToString();
                    if (obj is AIImageEntry entry) return entry.Count.ToString();
                    if (obj is InspResult res)     return res.ResultValue;
                    return "";
                }
            };

            var colPath = new OLVColumn("경로", "")
            {
                Width = 250,
                IsEditable = false,
                AspectGetter = obj =>
                {
                    if (obj is AIImageEntry entry) return entry.ImagePath;
                    return "";
                }
            };

            // 컬럼 추가
            _treeListView.Columns.AddRange(new OLVColumn[] { colUID, colStatus, colValue, colPath });


            // 검사 상세 정보 텍스트박스 생성
            _txtDetails = new TextBox()
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Arial", 10),
                ReadOnly = true
            };

            // 컨테이너에 컨트롤 추가
            _splitContainer.Panel1.Controls.Add(_treeListView);
            _splitContainer.Panel2.Controls.Add(_txtDetails);
            Controls.Add(_splitContainer);
        }

        public void AddModelResult(Model curModel)
        {
            if (curModel is null)
                return;

            _treeListView.SetObjects(curModel.InspWindowList);

            foreach (var window in curModel.InspWindowList)
            {
                _treeListView.Expand(window);
            }
        }

        public void AddWindowResult(InspWindow inspWindow)
        {
            if (inspWindow is null)
                return;

            _treeListView.SetObjects(new List<InspWindow> { inspWindow });
            _treeListView.Expand(inspWindow);

            if (inspWindow.InspResultList.Count > 0)
            {
                InspResult inspResult = inspWindow.InspResultList[0];
                ShowDedtail(inspResult);
            }
        }

        // AI 누적 검사 결과를 클래스(Crack/Scratch) 단위로 ResultForm에 표시
        public void ShowAIRecords(List<AIClassRecord> classRecords)
        {
            _treeListView.ClearObjects();

            if (classRecords == null || classRecords.Count == 0)
            {
                _txtDetails.Text = "검사 결과가 없습니다.";
                return;
            }

            _treeListView.SetObjects(classRecords);

            // 모든 클래스 노드 펼치기
            foreach (var cls in classRecords)
                _treeListView.Expand(cls);

            // 요약 텍스트
            _txtDetails.Text = "[AI 검사 결과]\r\n" +
                string.Join("\r\n", classRecords.Select(c =>
                    $"  [{c.ClassName}] 총 {c.ImageEntries.Count}장, 검출 합계: {c.TotalCount}개 " +
                    $"({(c.IsDefect ? "NG" : "OK")})"));
        }

        //실제 검사가 되었을때, 검사 결과를 추가하는 함수
        public void AddInspResult(InspResult inspResult)
        {
            if (inspResult is null)
                return;

            // 현재 트리에 있는 객체 리스트 가져오기
            var existingResults = _treeListView.Objects as List<InspResult>;

            if (existingResults == null)
                existingResults = new List<InspResult>();

            // 기존 검사 결과에서 같은 BodyID를 가진 부모 찾기
            var parentResult = existingResults.FirstOrDefault(r => r.GroupID == inspResult.GroupID);

            existingResults.Add(inspResult);

            // TreeListView 업데이트
            _treeListView.SetObjects(existingResults);
        }

        //해당 트리 리스트 뷰 선택시, 상세 정보 텍스트 박스에 표시
        private void TreeListView_SelectionChanged(object sender, EventArgs e)
        {
            if (_treeListView.SelectedObject == null)
            {
                _txtDetails.Text = string.Empty;
                return;
            }

            if (_treeListView.SelectedObject is AIClassRecord cls)
            {
                _txtDetails.Text = $"[{cls.ClassName}]  총 {cls.ImageEntries.Count}장  검출 합계: {cls.TotalCount}개\r\n" +
                    string.Join("\r\n", cls.ImageEntries.Select(x =>
                        $"  {x.FileName}: {x.Count}개 ({(x.IsDefect ? "NG" : "OK")})"));
            }
            else if (_treeListView.SelectedObject is AIImageEntry entry)
            {
                _txtDetails.Text = $"{entry.ImagePath}\r\n검출 수: {entry.Count}개  판정: {(entry.IsDefect ? "NG" : "OK")}";
            }
            else if (_treeListView.SelectedObject is InspResult result)
            {
                ShowDedtail(result);
            }
            else if (_treeListView.SelectedObject is InspWindow window)
            {
                var infos = window.InspResultList.Select(r => $" -{r.ObjectID}: {r.ResultInfos}").ToList();
                _txtDetails.Text = $"{window.UID}\r\n" +
                    string.Join("\r\n", infos);
            }
        }

        private void ShowDedtail(InspResult result)
        {
            if (result is null)
                return;

            _txtDetails.Text = result.ResultInfos.ToString();

            if (result.ResultRectList != null)
            {
                CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
                if (cameraForm != null)
                {
                    cameraForm.AddRect(result.ResultRectList);
                }
            }
        }
    }
}
