using MachineVision_PCB.Algorithm;
using MachineVision_PCB.Core;
using MachineVision_PCB.Teach;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVision_PCB.Inspect
{
    public class InspectBoard
    {
        public InspectBoard()
        {
        }

        public bool Inspect(InspWindow window)
        {
            if (window is null)
                return false;

            if (!InspectWindow(window))
                return false;

            return true;
        }

        private bool InspectWindow(InspWindow window)
        {
            window.ResetInspResult();
            foreach (InspAlgorithm algo in window.AlgorithmList)
            {
                if (algo.IsUse == false)
                    continue;

                // 개별 알고리즘 실패 시 해당 알고리즘만 건너뛰고 나머지는 계속 실행
                if (!algo.DoInspect())
                    continue;

                string resultInfo = string.Join("\r\n", algo.ResultString);

                InspResult inspResult = new InspResult
                {
                    ObjectID = window.UID,
                    InspType = algo.InspectType,
                    IsDefect = algo.IsDefect,
                    ResultInfos = resultInfo
                };

                switch (algo.InspectType)
                {
                    case InspectType.InspMatch:
                        MatchAlgorithm matchAlgo = algo as MatchAlgorithm;
                        inspResult.ResultValue = $"{matchAlgo.OutScore}";
                        break;
                    case InspectType.InspBinary:
                        BlobAlgorithm blobAlgo = algo as BlobAlgorithm;
                        BlobFilter countFilter = blobAlgo.BlobFilters[blobAlgo.FILTER_COUNT];
                        int min = countFilter.min;
                        int max = countFilter.max;
                        int found = blobAlgo.OutBlobCount;

                        inspResult.ResultValue = $"{found}/{min}~{max}";

                        if (algo.IsDefect)
                        {
                            if (countFilter.isUse)
                            {
                                string reason    = found < min ? "핀 누락" : "핀 초과";
                                string criterion = found < min
                                    ? $"기준: 최소 {min}개"
                                    : $"기준: 최대 {max}개";
                                inspResult.ResultInfos =
                                    $"[{window.Name} 영역 검사 실패 - {reason}]\r\n" +
                                    $"- 핀 검출 수: {found}개  ({criterion}) → NG!";
                            }
                            else
                            {
                                inspResult.ResultInfos =
                                    $"[{window.Name} 영역 검사 실패]\r\n" +
                                    $"- 예상치 못한 블롭 검출: {found}개 → NG!";
                            }
                        }
                        else
                        {
                            string range = countFilter.isUse ? $"{min}~{max}개" : "제한 없음";
                            inspResult.ResultInfos =
                                $"[{window.Name} 영역 검사 완료 - OK]\r\n" +
                                $"- 핀 검출 수: {found}개  (기준: {range})";
                        }
                        break;
                }

                List<DrawInspectInfo> resultArea = new List<DrawInspectInfo>();
                int resultCnt = algo.GetResultRect(out resultArea);
                inspResult.ResultRectList = resultArea;

                window.AddInspResult(inspResult);
            }

            return true;
        }

        public bool InspectWindowList(List<InspWindow> windowList)
        {
            if (windowList.Count <= 0)
                return false;

            //ID 윈도우가 매칭알고리즘이 있고, 검사가 되었다면, 오프셋을 얻는다.
            Point alignOffset = new Point(0, 0);
            InspWindow idWindow = windowList.Find(w => w.InspWindowType == Core.InspWindowType.ID);
            if (idWindow != null)
            {
                MatchAlgorithm matchAlgo = (MatchAlgorithm)idWindow.FindInspAlgorithm(InspectType.InspMatch);
                if (matchAlgo != null && matchAlgo.IsUse)
                {
                    if (!InspectWindow(idWindow))
                        return false;

                    if (matchAlgo.IsInspected)
                    {
                        alignOffset = matchAlgo.GetOffset();
                        idWindow.InspArea = idWindow.WindowArea + alignOffset;
                    }
                }
            }

            foreach (InspWindow window in windowList)
            {
                //모든 윈도우에 오프셋 반영
                window.SetInspOffset(alignOffset);
                if (!InspectWindow(window))
                    return false;
            }

            return true;
        }
    }
}
