using System.IO;
using System.Windows.Media.Imaging;

namespace PhysicsEngineCore.Utils {
    public class Image {
        public string filename;
        public BitmapImage data;

        public Image(string filename,Stream imageStream) {
            this.filename = filename;
            this.data = new BitmapImage();

            this.data.BeginInit();
            this.data.StreamSource = imageStream;
            this.data.CacheOption = BitmapCacheOption.OnLoad;
            this.data.EndInit();
            this.data.Freeze();
        }

        public string name {
            get {
                return Path.GetFileNameWithoutExtension(this.filename);
            }
        }

        public string extension {
            get {
                return Path.GetExtension(this.filename).ToLower();
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
