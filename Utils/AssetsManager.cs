using System.IO;

namespace PhysicsEngineCore.Utils {
    public class AssetsManager {
        public readonly List<Image> images = [];
        public readonly static string[] imageExtensions = [".png", ".jpg", ".jpeg"];

        /// <summary>
        /// 画像の追加
        /// </summary>
        /// <param name="filename">追加するファイル名</param>
        /// <param name="imageStream">追加するファイルストリーム</param>
        /// <exception cref="Exception">同じ画像がある場合エラー</exception>
        public void Add(string filename,Stream imageStream) {
            if(this.images.Any(image => image.filename == filename)) throw new Exception("同じ画像が既に存在します");

            Image image = new Image(filename,imageStream);

            this.images.Add(image);
        }

        /// <summary>
        /// 画像の削除
        /// </summary>
        /// <param name="filename">削除するファイル名</param>
        /// <exception cref="Exception">存在しない画像の場合エラー</exception>
        public void Remove(string filename) {
            if(!this.images.Any(image => image.filename == filename)) throw new Exception("指定された画像が存在しません");

            this.images.RemoveAll(image => image.filename == filename);
        }

        /// <summary>
        /// 画像を取得します
        /// </summary>
        /// <param name="filename">取得する画像ファイル名</param>
        /// <returns>取得した画像</returns>
        public Image? Get(string filename) {
            return this.images.FirstOrDefault(image => image.filename == filename);
        }

        /// <summary>
        /// 画像のファイル名のリストを取得します
        /// </summary>
        public List<string> fileNames {
            get {
                return [.. this.images.Select(image => image.filename)];
            }
        }

        /// <summary>
        /// 全ての画像をクリアします
        /// </summary>
        public void Clear() {
            this.images.Clear();
        }
    }
}
