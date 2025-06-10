using System.Windows.Media.Imaging;

namespace FileConverterApp.Models
{
    public class FileItem
    {
        private string _fileName;
        public string FileName
        {
            get { return this._fileName; }
            set { this._fileName = value; }
        }

        private BitmapImage _imagePreview;
        public BitmapImage ImagePreview
        {
            get { return this._imagePreview; }
            set { this._imagePreview = value; }
        }

        private string _filePath;
        public string FilePath
        {
            get { return this._filePath; }
            set { this._filePath = value; }
        }
    }
}
