using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVision_PCB.Util
{
    public class ImageLoader : IDisposable
    {
        private List<string> _sortedImages;
        private int _grabIndex = -1;

        public bool CyclicMode { get; set; } = true;
        public string LoadedDirectory { get; private set; } = "";

        public ImageLoader() { }

        public bool LoadImages(string imageDir)
        {
            if (!Directory.Exists(imageDir))
                return false;

            _sortedImages = ImageFileSorter.GetSortedImages(imageDir);
            if (_sortedImages.Count() <= 0)
                return false;

            _grabIndex = -1;
            LoadedDirectory = imageDir;

            return true;
        }

        public bool IsLoadedImages()
        {
            if (_sortedImages is null)
                return false;

            if (_sortedImages.Count() <= 0)
                return false;

            return true;
        }

        public bool Reset()
        {
            _grabIndex = -1;
            return true;
        }

        // CycleInspect용 - 다음 GetNextImagePath() 호출 시 해당 이미지 반환 (idx-1 세팅)
        public void SetStartImage(string imagePath)
        {
            if (_sortedImages is null)
                return;

            int idx = _sortedImages.IndexOf(imagePath);
            _grabIndex = (idx >= 0) ? idx - 1 : -1;
        }

        // Prev/Next 탐색용 - 현재 위치를 idx로 고정하여 Next→idx+1, Prev→idx-1 이동
        public void SetCurrentImage(string imagePath)
        {
            if (_sortedImages is null)
                return;

            int idx = _sortedImages.IndexOf(imagePath);
            _grabIndex = (idx >= 0) ? idx : 0;
        }

        public string GetImagePath()
        {
            if (_sortedImages is null)
                return "";

            _grabIndex++;

            if (_grabIndex >= _sortedImages.Count)
            {
                if (CyclicMode == false)
                    return "";

                _grabIndex = 0;
            }

            return _sortedImages[_grabIndex];
        }

        public string GetNextImagePath(bool reset = false)
        {
            if (reset)
                Reset();

            return GetImagePath();
        }

        public string GetPrevImagePath()
        {
            if (_sortedImages is null)
                return "";

            _grabIndex--;

            if (_grabIndex < 0)
            {
                if (CyclicMode == false)
                {
                    _grabIndex = 0;
                    return "";
                }

                _grabIndex = _sortedImages.Count - 1;
            }

            return _sortedImages[_grabIndex];
        }


        #region Dispose

        private bool _disposed = false; // to detect redundant calls

        protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;
        }
        ~ImageLoader()
        {
            Dispose(disposing: false);
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
