using System.IO;
using System.Windows.Media.Imaging;

namespace PhysicsEngineCore.Utils {
    public class Image {
        public string filename;
        public BitmapImage source;

        public Image(string filename,Stream imageStream) {
            this.filename = filename;
            this.source = new BitmapImage();

            this.source.BeginInit();
            this.source.StreamSource = imageStream;
            this.source.CacheOption = BitmapCacheOption.OnLoad;
            this.source.EndInit();
            this.source.Freeze();
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
                return this.source.PixelHeight;
            }
        }

        public int width {
            get {
                return this.source.PixelWidth;
            }
        }
    }
}
