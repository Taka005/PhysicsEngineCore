using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Renders;
using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Views;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace PhysicsEngineCore {
    /// <summary>
    /// 物理エンジンのレンダークラス
    /// </summary>
    public class Render : FrameworkElement {
        private bool _isDisplayVector = false;
        private bool _isDisplayGrid = false;
        private bool _isDebugMode = false;
        private double _offsetX = 0;
        private double _offsetY = 0;
        private double _scale = 1;
        private double _gridInterval = 50;
        private readonly TransformGroup transformGroup;
        private readonly TranslateTransform translateTransform;
        private readonly ScaleTransform scaleTransform;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private TimeSpan _lastFpsUpdateTime = TimeSpan.Zero;
        private int _frameCount = 0;
        private double _fps = 0;
        private readonly VisualCollection visuals;
        private readonly DrawingVisual mainSceneVisual = new DrawingVisual();
        private readonly DrawingVisual trackingVisual = new DrawingVisual();
        private readonly DebugVisual debugVisual = new DebugVisual();
        private readonly GridVisual gridVisual = new GridVisual();
        public Vector2 currentPosition = new Vector2(0, 0);

        public Render(RenderOption? option) {
            if(option != null) {
                this._offsetX = option.offsetX;
                this._offsetY = option.offsetY;
                this._scale = option.scale;
            }

            this.visuals = new VisualCollection(this) {
                this.gridVisual,
                this.trackingVisual,
                this.mainSceneVisual,
                this.debugVisual
            };

            this.transformGroup = new TransformGroup();
            this.translateTransform = new TranslateTransform(this.offsetX, this.offsetY);
            this.scaleTransform = new ScaleTransform(this.scale, this.scale);

            this.transformGroup.Children.Add(this.scaleTransform);
            this.transformGroup.Children.Add(this.translateTransform);

            this.stopwatch.Start();
        }

        /// <summary>
        /// オフセットX座標
        /// </summary>
        public double offsetX {
            get {
                return this._offsetX;
            }
            set {
                this._offsetX = value;

                this.translateTransform.X = value;
            }
        }

        /// <summary>
        /// オフセットY座標
        /// </summary>
        public double offsetY {
            get {
                return this._offsetY;
            }
            set {
                this._offsetY = value;

                this.translateTransform.Y = value;
            }
        }

        /// <summary>
        /// スケール値
        /// </summary>
        public double scale {
            get {
                return this._scale;
            }
            set {
                if(value <= 0) throw new ArgumentOutOfRangeException(nameof(value), "scaleを0以下には設定できません");

                this._scale = value;

                this.scaleTransform.ScaleX = value;
                this.scaleTransform.ScaleY = value;
            }
        }

        /// <summary>
        /// グリッドの間隔
        /// </summary>
        public double gridInterval {
            get {
                return this._gridInterval;
            }
            set {
                if(value <= 0) throw new ArgumentOutOfRangeException(nameof(value), "gridIntervalを0以下には設定できません");

                this._gridInterval = value;
            }
        }

        /// <summary>
        /// デバッグモードの切り替え
        /// </summary>
        public bool isDisplayVector {
            get {
                return this._isDisplayVector;
            }
            set {
                this._isDisplayVector = value;
            }
        }

        /// <summary>
        /// グリッドの表示切り替え
        /// </summary>
        public bool isDisplayGrid {
            get {
                return this._isDisplayGrid;
            }
            set {
                this._isDisplayGrid = value;

                if (!value){
                    this.gridVisual.Clear();
                }
            }
        }

        /// <summary>
        /// デバッグモードの切り替え
        /// </summary>
        public bool isDebugMode {
            get{
                return this._isDebugMode;
            }
            set {
                this._isDebugMode = value;

                if(!value){
                    this.debugVisual.Clear();
                }
            }
        }

        /// <summary>
        /// 現在のFPS
        /// </summary>
        public int fps{
            get{
                return (int)this._fps;
            }
        }


        /// <summary>
        /// 物体の描画を更新します
        /// UIスレッドで呼び出す必要があります
        /// </summary>
        /// <param name="objects">物体のリスト</param>
        /// <param name="grounds">地面のリスト</param>
        /// <param name="effects">エフェクトのリスト</param>
        public void Update(List<IObject> objects, List<IGround> grounds, List<IEffect> effects){
            UpdateFps();

            using (DrawingContext context = this.mainSceneVisual.RenderOpen()){
                context.PushTransform(this.transformGroup);

                DrawGrounds(context, grounds);
                DrawEffects(context, effects);
                DrawObjects(context, objects);

                context.Pop();
            }

            if (this.isDebugMode){
                this.debugVisual.Draw(this.fps, this.currentPosition, objects.Count, grounds.Count, effects.Count);
            }

            if (this.isDisplayGrid){
                this.gridVisual.Draw(this.gridInterval, this.scale, this.offsetX, this.offsetY);
            }
        }
        
        /// <summary>
        /// 追跡中のオブジェクトの描画を更新します
        /// UIスレッドで呼び出す必要があります
        /// </summary>
        /// <param name="tracks">追跡中のオブジェクトのリスト</param>
        public void DrawTracking(List<IObject> tracks){
            using (DrawingContext context = this.trackingVisual.RenderOpen()){
                context.PushTransform(this.transformGroup);
                context.PushOpacity(0.2);

                DrawObjects(context, tracks);

                context.Pop();
                context.Pop();
            }
        }

        private void DrawObjects(DrawingContext context,List<IObject> objects){
            foreach (IObject obj in objects){
                if(obj is Circle circle){
                    ObjectRender.DrawCircle(context, circle);
                } else if(obj is Square square){
                    ObjectRender.DrawSquare(context, square);
                } else if(obj is Triangle triangle){
                    ObjectRender.DrawTriangle(context, triangle);
                } else if(obj is Rope rope){
                    ObjectRender.DrawRope(context, rope);
                }

                if (this.isDisplayVector){
                    ObjectRender.DrawVector(context, obj);
                }
            }
        }

        private void DrawGrounds(DrawingContext context, List<IGround> grounds){
            foreach(IGround ground in grounds) {
                if(ground is Line line) {
                    GroundRender.DrawLine(context, line);
                } else if(ground is Curve curve) {
                    GroundRender.DrawCurve(context, curve);
                }

                if (this.isDebugMode){
                    GroundRender.DrawEdge(context, ground);
                }
            }
        }

        private void DrawEffects(DrawingContext context, List<IEffect> effects){
            foreach(IEffect effect in effects) {
                if(effect is Booster booster) {
                    EffectRender.DrawBooster(context, booster);
                }

                if (this.isDebugMode){
                    EffectRender.DrawEdge(context, effect);
                }
            }
        }

        /// <summary>
        /// FPSを更新します
        /// </summary>
        private void UpdateFps(){
            TimeSpan currentTime = this.stopwatch.Elapsed;

            this._frameCount++;

            if ((currentTime - this._lastFpsUpdateTime).TotalSeconds >= 1){
                this._fps = _frameCount / (currentTime - this._lastFpsUpdateTime).TotalSeconds;
                this._lastFpsUpdateTime = currentTime;
                this._frameCount = 0;
            }
        }

        /// <summary>
        /// 追跡中のオブジェクトの描画をクリアします
        /// </summary>
        public void ClearTracking() {
            this.trackingVisual.RenderOpen().Close();
        }

        /// <summary>
        /// 現在のオフセットとスケールをリセットします
        /// </summary>
        public void ResetTransform() {
            this.offsetX = 0;
            this.offsetY = 0;
            this.scale = 1;
        }

        /// <summary>
        /// この要素の子ビジュアルの数を取得します
        /// </summary>
        protected override int VisualChildrenCount {
            get {
                return this.visuals.Count;
            }
        }

        /// <summary>
        /// 指定されたインデックスの子ビジュアルを取得します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index) {
            return this.visuals[index];
        }
    }
}
