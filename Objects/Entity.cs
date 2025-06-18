using System.Text.Json;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects {
    /// <summary>
    /// エンティティーを表す
    /// これは物理エンジンにおける最小単位です
    /// </summary>
    /// <param name="option">エンティティーの初期化クラス</param>
    public class Entity(EntityOption option) {
        public readonly string id = option.id ?? throw new ArgumentException(nameof(option.id));
        public Vector2 position = new Vector2(option.posX, option.posY);
        public Vector2 previousPosition = new Vector2(option.prePosX, option.prePosY);
        public Vector2 velocity = new Vector2(option.velocityX, option.velocityY);
        public double rotateAngle = option.rotateAngle;
        public double rotateSpeed = option.rotateSpeed;
        private double _diameter = CheckDiameterValue(option.diameter);
        private double _mass = CheckMassValue(option.mass);
        private double _stiffness = CheckStiffnessValue(option.stiffness);
        public readonly ConnectionManager connection = new ConnectionManager(option.targets);
        public string parentId = option.parentId ?? throw new ArgumentException(nameof(option.parentId));

        /// <summary>
        /// エンティティーの直径
        /// </summary>
        public double diameter {
            get {
                return _diameter;
            }
            set {
                _diameter = CheckDiameterValue(value);
            }
        }

        /// <summary>
        /// エンティティーの半径
        /// </summary>
        public double radius {
            get {
                return this._diameter / 2;
            }
        }

        /// <summary>
        /// エンティティーの質量
        /// </summary>
        public double mass {
            get {
                return _mass;
            }
            set {
                _mass = CheckMassValue(value);
            }
        }

        /// <summary>
        /// エンティティーの剛性
        /// 0超過かつ1以下の値のみをとる
        /// </summary>
        public double stiffness {
            get {
                return _stiffness;
            }
            set {
                _stiffness = CheckStiffnessValue(value);
            }
        }

        /// <summary>
        /// エンティティーの質量の逆数
        /// 質量が0の場合は0です
        /// </summary>
        public double invMass {
            get {
                if(this.mass == 0) return 0;

                return 1 / this.mass;
            }
        }

        /// <summary>
        /// エンティティーが静止しているかを判定します
        /// </summary>
        public bool isStop {
            get {
                return position.Equals(previousPosition);
            }
        }

        /// <summary>
        /// エンティティーの位置を保存します
        /// </summary>
        public void SavePosition() {
            this.previousPosition = this.position;
        }

        /// <summary>
        /// クラスのデータをJSON形式の文字列に変換します
        /// </summary>
        /// <returns>JSON形式の文字列</returns>
        public string ToJson() {
            return JsonSerializer.Serialize(this.ToOption());
        }

        /// <summary>
        /// クラスの引数に変換します
        /// </summary>
        /// <returns>クラスの引数</returns>
        public EntityOption ToOption() {
            return new EntityOption {
                id = this.id,
                posX = this.position.X,
                posY = this.position.Y,
                prePosX = this.previousPosition.X,
                prePosY = this.previousPosition.Y,
                mass = this.mass,
                stiffness = this.stiffness,
                diameter = this.diameter,
                velocityX = this.velocity.X,
                velocityY = this.velocity.Y,
                rotateAngle = this.rotateAngle,
                rotateSpeed = this.rotateSpeed,
                parentId = this.parentId,
                targets = this.connection.targets
            };
        }

        /// <summary>
        /// 剛性が正しい値かチェックします
        /// </summary>
        /// <param name="stiffness">剛性値</param>
        /// <returns>正しい剛性値</returns>
        /// <exception cref="Exception">0超過かつ1以下の値ではないときに例外</exception>
        private static double CheckStiffnessValue(double stiffness) {
            if(stiffness <= 0 || stiffness > 1) throw new Exception("剛性(stiffness)は0超過かつ1以下に設定する必要があります");

            return stiffness;
        }

        /// <summary>
        /// 直径が正しい値かチェックします
        /// </summary>
        /// <param name="diameter">直径</param>
        /// <returns>正しい直径</returns>
        /// <exception cref="Exception">0未満であったときに例外</exception>
        private static double CheckDiameterValue(double diameter) {
            if(diameter < 0) throw new Exception("直径(diameter)は0以上に設定する必要があります");

            return diameter;
        }

        /// <summary>
        /// 質量が正しい値かチェックします
        /// </summary>
        /// <param name="mass">質量</param>
        /// <returns>正しい質量</returns>
        /// <exception cref="Exception">0未満であったときに例外</exception>
        private static double CheckMassValue(double mass) {
            if(mass < 0) throw new Exception("質量(mass)は0以上に設定する必要があります");

            return mass;
        }
    }
}
