using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Views;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace PhysicsEngineCore {
    /// <summary>
    /// 物理エンジンのレンダークラス
    /// </summary>
    public class Render : FrameworkElement {
        public bool _isDebugMode = false;
        public bool _isDevMode = false;
        public bool _isDisplayGrid = true;
        public double offsetX = 0;
        public double offsetY = 0;
        public double scale = 1;
        public double gridInterval = 50;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private TimeSpan _lastFpsUpdateTime = TimeSpan.Zero;
        private int _frameCount = 0;
        private double _fps = 0;
        private readonly VisualCollection visuals;
        private readonly Dictionary<string, DrawingVisual> objectVisuals = [];
        private readonly Dictionary<string, DrawingVisual> groundVisuals = [];
        private readonly Dictionary<string, DrawingVisual> effectVisuals = [];
        private readonly Dictionary<string, DrawingVisual> trackingVisuals = [];
        private readonly VectorVisual vectorVisual = new VectorVisual();
        private readonly DebugVisual debugVisual = new DebugVisual();
        private readonly GridVisual gridVisual = new GridVisual();

        public Render() {
            this.visuals = new VisualCollection(this) {
                this.gridVisual,
                this.vectorVisual,
                this.debugVisual
            };

            this.stopwatch.Start();
        }

        /// <summary>
        /// デバッグモードの切り替え
        /// </summary>
        public bool isDebugMode {
            get {
                return this._isDebugMode;
            }
            set {
                this._isDebugMode = value;

                this.vectorVisual.Clear();
            }
        }

        /// <summary>
        /// 開発者モードの切り替え
        /// </summary>
        public bool isDevMode {
            get{
                return this._isDevMode;
            }
            set {
                this._isDevMode = value;

                this.debugVisual.Clear();
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
        /// 基本的なレンダリングの処理を行います
        /// このメソッドはUIスレッドで呼び出される必要があります
        /// </summary>
        public void Update() {
            TransformGroup newTranslateTransform = this.CreateTransformGroup();

            foreach(DrawingVisual visual in this.visuals) {
                if(visual is DebugVisual || visual is GridVisual) continue;

                visual.Transform = newTranslateTransform;
            }

            TimeSpan currentTime = this.stopwatch.Elapsed;

             this._frameCount++;

            if((currentTime - this._lastFpsUpdateTime).TotalSeconds >= 1){
                this._fps = _frameCount / (currentTime - this._lastFpsUpdateTime).TotalSeconds;
                this._lastFpsUpdateTime = currentTime;
                this._frameCount = 0;
            }

            if(this.isDevMode) {
                this.debugVisual.Draw(this.fps);
            }

            if(this.isDisplayGrid) {
                this.gridVisual.Draw(this.gridInterval,this.scale,this.offsetX,this.offsetY);
            }
        }

        /// <summary>
        /// 物理エンジンのオブジェクトデータを受け取り、描画を更新します
        /// このメソッドはUIスレッドで呼び出される必要があります
        /// </summary>
        /// <param name="objects">描画するオブジェクトのリスト</param>
        public void DrawObject(List<IObject> objects) {
            HashSet<string> currentObjectIds = [.. objects.Select(o => o.trackingId)];
            List<string>? visualsToRemove = [.. this.objectVisuals.Keys.Where(id => !currentObjectIds.Contains(id))];
            List<VectorData> vectors = [];

            foreach(string id in visualsToRemove) {
                this.visuals.Remove(this.objectVisuals[id]);
                this.objectVisuals.Remove(id);
            }

            foreach(IObject obj in objects) {
                if(this.objectVisuals.TryGetValue(obj.trackingId, out DrawingVisual? visual)) {
                    if(visual is IObjectVisual objectVisual) {
                        //IObject oldObj = objectVisual.GetObjectData();

                        //if(!obj.Equals(oldObj)){
                        //    if(!obj.position.Equals(oldObj.position)){
                        //        visual.Offset = new Vector(obj.position.X,obj.position.Y);

                        //        if(this.isDebugMode) {
                        //            vectors.Add(new VectorData(
                        //                obj.position,
                        //                obj.velocity
                        //            ));
                        //        }
                        //    }else{
                                objectVisual.Draw();
                        //    }

                        //    objectVisual.SetObjectData(obj.Clone());
                        //}

                        vectors.Add(new VectorData(
                            obj.position,
                            obj.velocity
                        ));
                    }
                }else{
                    DrawingVisual? newVisual = this.CreateObjectVisual(obj);

                    if(newVisual != null) {
                        this.objectVisuals.Add(obj.trackingId, newVisual);
                        this.visuals.Insert(0, newVisual);
                        newVisual.Transform = this.CreateTransformGroup();

                        if(newVisual is IObjectVisual objectVisual) {
                            objectVisual.Draw();
                        }
                    }
                }
            }

            if(this.isDebugMode) {
                this.vectorVisual.Draw(vectors);
            }
        }

        /// <summary>
        /// 物理エンジンのオブジェクトデータを受け取り、描画を更新します
        /// このメソッドはUIスレッドで呼び出される必要があります
        /// </summary>
        /// <param name="grounds">描画する地面のリスト</param>
        public void DrawGround(List<IGround> grounds) {
            HashSet<string> currentGrounds = [.. grounds.Select(o => o.trackingId)];
            List<string>? visualsToRemove = [.. this.groundVisuals.Keys.Where(id => !currentGrounds.Contains(id))];

            foreach(string id in visualsToRemove) {
                this.visuals.Remove(this.groundVisuals[id]);
                this.groundVisuals.Remove(id);
            }

            foreach(IGround ground in grounds) {
                if(this.groundVisuals.TryGetValue(ground.trackingId, out DrawingVisual? visual)) {
                    if(visual is IGroundVisual groundVisual) {
                        //IGround oldGround = groundVisual.GetGroundData();

                        //if(!ground.Equals(oldGround)) {
                            groundVisual.Draw();

                        //    groundVisual.SetGroundData(ground.Clone());
                        //}
                    }
                } else {
                    DrawingVisual? newVisual = this.CreateGroundVisual(ground);

                    if(newVisual != null) {
                        this.groundVisuals.Add(ground.trackingId, newVisual);
                        this.visuals.Insert(0, newVisual);
                        newVisual.Transform = this.CreateTransformGroup();

                        if(newVisual is IGroundVisual groundVisual) {
                            groundVisual.Draw();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 物理エンジンのエフェクトデータを受け取り、描画を更新します
        /// このメソッドはUIスレッドで呼び出される必要があります
        /// </summary>
        /// <param name="effects">描画する地面のエフェクト</param>
        public void DrawEffect(List<IEffect> effects) {
            HashSet<string> currentEffectIds = [.. effects.Select(o => o.trackingId)];
            List<string>? visualsToRemove = [.. this.effectVisuals.Keys.Where(id => !currentEffectIds.Contains(id))];

            foreach(string id in visualsToRemove) {
                this.visuals.Remove(this.effectVisuals[id]);
                this.effectVisuals.Remove(id);
            }

            foreach(IEffect effect in effects) {
                if(this.effectVisuals.TryGetValue(effect.trackingId, out DrawingVisual? visual)) {
                    if(visual is IEffectVisual effectVisual) {
                        effectVisual.Draw();
                    }
                } else {
                    DrawingVisual? newVisual = this.CreateEffectVisual(effect);

                    if(newVisual != null) {
                        this.effectVisuals.Add(effect.trackingId, newVisual);
                        this.visuals.Insert(0, newVisual);
                        newVisual.Transform = this.CreateTransformGroup();

                        if(newVisual is IEffectVisual effectVisual) {
                            effectVisual.Draw();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 物理エンジンのオブジェクトデータを受け取り、描画します
        /// このメソッドはUIスレッドで呼び出される必要があります
        /// </summary>
        /// <param name="tracks">描画するオブジェクトのリスト</param>
        public void DrawTracking(List<IObject> tracks) {
            HashSet<string> currentObjectIds = [.. tracks.Select(o => o.trackingId)];
            List<string>? visualsToRemove = [.. this.trackingVisuals.Keys.Where(id => !currentObjectIds.Contains(id))];
            List<VectorData> vectors = [];

            foreach(string id in visualsToRemove) {
                this.visuals.Remove(this.trackingVisuals[id]);
                this.trackingVisuals.Remove(id);
            }

            foreach(IObject obj in tracks) {
                if(!this.trackingVisuals.TryGetValue(obj.trackingId, out DrawingVisual? visual)) {
                    DrawingVisual? newVisual = this.CreateObjectVisual(obj);

                    if(newVisual != null) {
                        this.trackingVisuals.Add(obj.trackingId, newVisual);
                        this.visuals.Insert(0, newVisual);
                        newVisual.Transform = this.CreateTransformGroup();

                        if(newVisual is IObjectVisual trackingVisual) {
                            trackingVisual.opacity = 0.2f;
                            trackingVisual.Draw();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 追跡中のオブジェクトの描画をクリアします
        /// </summary>
        public void ClearTracking() {
            List<string>? visualsToRemove = [.. this.trackingVisuals.Keys];

            foreach(string id in visualsToRemove) {
                this.visuals.Remove(this.trackingVisuals[id]);
                this.trackingVisuals.Remove(id);
            }
        }

        /// <summary>
        /// オブジェクトの種類に基づいて適切なDrawingVisualを作成
        /// </summary>
        private DrawingVisual? CreateObjectVisual(IObject obj) {
            if(obj is Circle circle) {
                return new CircleVisual(circle);
            } else if(obj is Rope rope) {
                return new RopeVisual(rope);
            } else if(obj is Square square) {
                return new SquareVisual(square);
            } else if(obj is Triangle triangle) {
                return new TriangleVisual(triangle);
            }

            return null;
        }

        /// <summary>
        /// 地面の種類に基づいて適切なDrawingVisualを作成
        /// </summary>
        private DrawingVisual? CreateGroundVisual(IGround obj) {
            if(obj is Line line) {
                return new LineVisual(line);
            } else if(obj is Curve curve) {
                return new CurveVisual(curve);
            }

            return null;
        }

        /// <summary>
        /// エフェクトの種類に基づいて適切なDrawingVisualを作成
        /// </summary>
        /// <returns></returns>
        private DrawingVisual? CreateEffectVisual(IEffect effect) {
            if(effect is Booster booster) {
                return new BoosterVisual(booster);
            }

            return null;
        }

        /// <summary>
        /// オフセットとスケールを適用するTransformGroupを作成します
        /// </summary>
        /// <returns>作成されたTransformGroup</returns>
        private TransformGroup CreateTransformGroup() {
            TransformGroup transformGroup = new TransformGroup();

            transformGroup.Children.Add(new ScaleTransform(this.scale,this.scale));
            transformGroup.Children.Add(new TranslateTransform(this.offsetX, this.offsetY));

            return transformGroup;
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
