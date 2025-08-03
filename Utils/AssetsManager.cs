namespace PhysicsEngineCore.Utils {
    public class AssetsManager {
        private readonly List<Image> images = [];

        public void Add(string path) {
            if(this.images.Any(image => image.path == path)) throw new Exception("同じパスの画像が既に存在します");

            Image image = new Image(path);

            this.images.Add(image);
        }

        public void Remove(string path) {
            if(!this.images.Any(image => image.path == path)) throw new Exception("指定されたパスの画像が存在しません");

            this.images.RemoveAll(image => image.path == path);
        }

        public Image? Get(string path) {
            return this.images.FirstOrDefault(image => image.path == path);
        }

        public List<string> paths {
            get {
                return [.. this.images.Select(image => image.path)];
            }
        }
    }
}
