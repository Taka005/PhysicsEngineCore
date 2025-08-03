using System.IO;
using System.Windows.Media.Imaging;

namespace PhysicsEngineCore.Utils {
    public class Image {
        public  string path;
        public BitmapImage data;

        public Image(string path) {
            this.path = path;
            this.data = new BitmapImage();

            this.data.BeginInit();
            this.data.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            this.data.CacheOption = BitmapCacheOption.OnLoad;
            this.data.EndInit();
            this.data.Freeze();
        }

        public string filename {
            get {
                return Path.GetFileName(this.path);
            }
        }

        public string name {
            get {
                return Path.GetFileNameWithoutExtension(this.path);
            }
        }

        public string extension {
            get {
                return Path.GetExtension(this.path).ToLower();
            }
        }

        public int height {
            get {
                return this.data.PixelHeight;
            }
        }

        public int width {
            get {
                return this.data.PixelWidth;
            }
        }
    }
}
