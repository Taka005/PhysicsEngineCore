using System.IO;

namespace PhysicsEngineCore.Utils {
    public class AssetsManager {
        private readonly List<Image> images = [];
        public readonly static string[] imageExtensions = [".png", ".jpg", ".jpeg"];

        public void Add(string filename,Stream imageStream) {
            if(this.images.Any(image => image.filename == filename)) throw new Exception("同じ画像が既に存在します");

            Image image = new Image(filename,imageStream);

            this.images.Add(image);
        }

        public void Remove(string filename) {
            if(!this.images.Any(image => image.filename == filename)) throw new Exception("指定された画像が存在しません");

            this.images.RemoveAll(image => image.filename == filename);
        }

        public Image? Get(string filename) {
            return this.images.FirstOrDefault(image => image.filename == filename);
        }

        public List<string> paths {
            get {
                return [.. this.images.Select(image => image.filename)];
            }
        }

        public void Clear() {
            this.images.Clear();
        }
    }
}
