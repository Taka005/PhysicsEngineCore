using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Views;
using System.Windows;
using System.Windows.Media;

namespace PhysicsEngineCore {
    /// <summary>
    /// 物理エンジンのレンダークラス
    /// </summary>
    public class Render : FrameworkElement {
        public bool isDebugMode = false;
        private readonly VisualCollection visuals;
        private readonly Dictionary<string, DrawingVisual> objectVisuals = [];
        private readonly Dictionary<string, DrawingVisual> groundVisuals = [];
        private readonly Dictionary<string, DrawingVisual> trackingVisuals = [];
        private readonly OverlayVisual overlayVisual = new OverlayVisual();

        public Render() {
            this.visuals = new VisualCollection(this) {
                this.overlayVisual
            };
        }

        /// <summary>
        /// 物理エンジンのオブジェクトデータを受け取り、描画を更新します
        /// このメソッドはUIスレッドで呼び出される必要があります
        /// </summary>
        /// <param name="objects">描画するオブジェクトのリスト</param>
        public void DrawObject(List<IObject> objects) {
            HashSet<string> currentObjectIds = [.. objects.Select(o => o.id)];
            List<string>? visualsToRemove = [.. this.objectVisuals.Keys.Where(id => !currentObjectIds.Contains(id))];
            List<VectorData> vectors = [];

            foreach(string id in visualsToRemove) {
                this.visuals.Remove(this.objectVisuals[id]);
                this.objectVisuals.Remove(id);
            }

            foreach(IObject obj in objects) {
                if(this.objectVisuals.TryGetValue(obj.id, out DrawingVisual? visual)) {
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
                    DrawingVisual? newVisual = this.CreateVisualForObject(obj);

                    if(newVisual != null) {
                        this.objectVisuals.Add(obj.id, newVisual);
                        this.visuals.Insert(0, newVisual);

                        if(newVisual is IObjectVisual objectVisual) {
                            objectVisual.Draw();
                        }
                    }
                }
            }

            if(this.isDebugMode) {
                this.overlayVisual.UpdateVectors(vectors);
            } else {
                this.overlayVisual.Clear();
            }
        }

        /// <summary>
        /// 物理エンジンのオブジェクトデータを受け取り、描画を更新します
        /// このメソッドはUIスレッドで呼び出される必要があります
        /// </summary>
        /// <param name="grounds">描画する地面のリスト</param>
        public void DrawGround(List<IGround> grounds) {
            HashSet<string> currentGrounds = [.. grounds.Select(o => o.id)];
            List<string>? visualsToRemove = [.. this.groundVisuals.Keys.Where(id => !currentGrounds.Contains(id))];

            foreach(string id in visualsToRemove) {
                this.visuals.Remove(this.groundVisuals[id]);
                this.groundVisuals.Remove(id);
            }

            foreach(IGround ground in grounds) {
                if(this.groundVisuals.TryGetValue(ground.id, out DrawingVisual? visual)) {
                    if(visual is IGroundVisual groundVisual) {
                        //IGround oldGround = groundVisual.GetGroundData();

                        //if(!ground.Equals(oldGround)) {
                            groundVisual.Draw();

                        //    groundVisual.SetGroundData(ground.Clone());
                        //}
                    }
                } else {
                    DrawingVisual? newVisual = this.CreateVisualForGround(ground);

                    if(newVisual != null) {
                        this.groundVisuals.Add(ground.id, newVisual);
                        this.visuals.Insert(0, newVisual);

                        if(newVisual is IGroundVisual groundVisual) {
                            groundVisual.Draw();
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
                    DrawingVisual? newVisual = this.CreateVisualForObject(obj);

                    if(newVisual != null) {
                        this.trackingVisuals.Add(obj.trackingId, newVisual);
                        this.visuals.Insert(0, newVisual);

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
        private DrawingVisual? CreateVisualForObject(IObject obj) {
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
        private DrawingVisual? CreateVisualForGround(IGround obj) {
            if(obj is Line line) {
                return new LineVisual(line);
            } else if(obj is Curve curve) {
                return new CurveVisual(curve);
            }

            return null;
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
