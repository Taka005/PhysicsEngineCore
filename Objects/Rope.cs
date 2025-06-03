using System.Text.Json;
using System.Windows.Media;
using PhysicsEngineCore.Options;

namespace PhysicsEngineCore.Objects{

    /// <summary>
    /// ロープを表すクラス
    /// </summary>
    public class Rope: BaseObject, IObject{
        private readonly string _id;
        public double width;
        private string _color;
        private readonly DrawingVisual _visual = new DrawingVisual();

        public Rope(RopeOption option): base(option.entities){
            this._id = option.id;
            this.width = option.width;
            this._color = option.color;

            if(option.entities.Count != 0) {
                this.AddAllEntities(option.entities);
            } else {
                double width = option.endX - option.startX;
                double height = option.endY - option.startY;

                double count = Math.Floor(Math.Sqrt(width * width + height * height) / (this.width * 2));

                Entity? entity = null;
                for(int i = 0;i <= count;i++) {
                    double posX = option.startX + i * (width / count);
                    double posY = option.startY + i * (height / count);

                    EntityOption entityOption = new EntityOption {
                        posX = posX,
                        posY = posY,
                        diameter = option.width,
                        mass = option.mass,
                        stiffness = option.stiffness,
                        velocityX = option.velocityX,
                        velocityY = option.velocityY,
                        parentId = this.id
                    };

                    Entity target = this.AddEntity(entityOption);

                    if(entity != null){
                        entity.connection.Add(target, this.width * 2, option.stiffness);
                        target.connection.Add(entity, this.width * 2, option.stiffness);
                    }

                    entity = target;
                }
            }
        }

        /// <summary>
        /// オブジェクトの固有ID
        /// </summary>
        public string id{
            get{
                return _id;
            }
        }

        /// <summary>
        /// 描画インスタンス
        /// </summary>
        public DrawingVisual visual{
            get{
                return _visual;
            }
        }

        public string color{
            get{
                return this._color;
            }
            set{
                this._color = value;
            }
        }

        /// <summary>
        /// クラスのデータをJSON形式の文字列に変換します
        /// </summary>
        /// <returns>JSON形式の文字列</returns>
        public string ToJson(){
            return JsonSerializer.Serialize(this.ToOption());
        }

        /// <summary>
        /// 同じ状態のクラスを複製します
        /// </summary>
        /// <returns>複製されたクラス</returns>
        public IObject Clone(){
            return new Rope(this.ToOption());
        }

        /// <summary>
        /// クラスの引数に変換します
        /// </summary>
        /// <returns>クラスの引数</returns>
        public RopeOption ToOption(){
            return new RopeOption {
                id = this.id,
                startX = this.entities.First().position.X,
                startY = this.entities.First().position.Y,
                endX = this.entities.Last().position.X,
                endY = this.entities.Last().position.Y,
                velocityX = this.velocity.X,
                velocityY = this.velocity.Y,
                width = this.width,
                mass = this.mass,
                stiffness = this.stiffness,
                color = this.color,
                entities = [..this.entities.Select(entity =>entity.ToOption())]
            };
        }
    }
}
