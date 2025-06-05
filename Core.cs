using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore{
    /// <summary>
    /// 物理エンジンのコアクラス
    /// </summary>
    public class Core{
        /// <summary>
        /// 1秒間あたりの処理回数
        /// </summary>
        public int pps;

        /// <summary>
        /// 重力加速度
        /// </summary>
        public double gravity;

        /// <summary>
        /// 摩擦係数
        /// </summary>
        public double friction;

        /// <summary>
        /// 0除算を防止するための値
        /// </summary>
        private static readonly double CORRECTION_NUMBER = 0.0000001d;

        /// <summary>
        /// 回転の強さ
        /// </summary>
        private static readonly double ROTATION_STRENGTH = 50d;

        /// <summary>
        /// 摩擦の強さ
        /// </summary>
        private static readonly double FRICTION_STRENGTH = 0.1d;

        /// <summary>
        /// 最大回転数
        /// </summary>
        private static readonly double MAX_ROTATION = 500d;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="pps">1秒間あたりの処理回数</param>
        /// <param name="gravity">重力加速度</param>
        /// <param name="friction">摩擦係数</param>
        public Core(int pps, double gravity, double friction){
            this.pps = pps;
            this.gravity = gravity;
            this.friction = friction;
        }

        /// <summary>
        /// 処理する間隔
        /// </summary>
        protected double deltaTime{
            get{
                return 1 / (double)this.pps;
            }
        }

        /// <summary>
        /// エンティティー同士の位置の計算
        /// </summary>
        /// <param name="source">対象のエンティティー</param>
        /// <param name="target">対象のエンティティー</param>
        protected void SolvePosition(Entity source, Entity target){
            double totalMass = source.invMass + target.invMass;
            if(totalMass == 0) return;

            Vector2 difference = target.position - source.position;

            if(
                Math.Abs(difference.X) >= source.radius + target.radius ||
                Math.Abs(difference.Y) >= source.radius + target.radius
            ) return;

            double distance = difference.Length();
            if(distance > source.radius + target.radius) return;

            double move = (distance - (source.radius + target.radius)) / (distance * totalMass + CORRECTION_NUMBER) * source.stiffness;
            Vector2 correction = difference * move;

            source.position += correction * source.invMass;
            target.position -= correction * target.invMass;

            double angle = -difference.X * source.velocity.Y + difference.X * source.velocity.X;

            double rotateAngle = (Math.Acos(Vector2.Dot(difference,source.velocity) / (distance * source.velocity.Length())) * (180 / Math.PI));

            if(angle > 0){
                source.rotateSpeed -= rotateAngle / ROTATION_STRENGTH;
                target.rotateSpeed += rotateAngle / ROTATION_STRENGTH;
            }else if(angle < 0){
                source.rotateSpeed += rotateAngle / ROTATION_STRENGTH;
                target.rotateSpeed -= rotateAngle / ROTATION_STRENGTH;
            }
        }

        /// <summary>
        /// 地面とエンティティーの位置の計算
        /// </summary>
        /// <param name="entity">対象のエンティティー</param>
        /// <param name="ground">対象のグラウンド</param>
        protected void SolveGroundPosition(Entity entity,IGround ground){
            if(entity.mass == 0) return;

            Vector2 position = ground.SolvePosition(entity.position);

            Vector2 difference = position - entity.position;
            double distance = difference.Length();
            if(distance > entity.radius + ground.width/2) return;

            double move = (distance - (entity.radius + ground.width/2)) / (distance * entity.invMass + CORRECTION_NUMBER) * entity.stiffness;
            Vector2 correction = difference * move;

            entity.position += correction * entity.invMass;

            double angle = -difference.X * entity.velocity.Y + difference.X * entity.velocity.X;

            double rotateAngle = (Math.Acos(Vector2.Dot(difference, entity.velocity) / (distance * entity.velocity.Length())) * (180 / Math.PI));

            if(angle > 0){
                entity.rotateSpeed -= rotateAngle / ROTATION_STRENGTH;
            }else if(angle < 0){
                entity.rotateSpeed += rotateAngle / ROTATION_STRENGTH;
            }
        }

        /// <summary>
        /// エンティティー同士の接続の計算
        /// </summary>
        /// <param name="source">対象のエンティティー</param>
        /// <param name="target">対象のエンティティー</param>
        /// <param name="connectDistance">接続する距離</param>
        /// <param name="connectStiffness">接続する剛性</param>
        protected void SolveConnection(Entity source, Entity target,double connectDistance, double connectStiffness){
            double totalMass = source.invMass + target.invMass;
            if(totalMass == 0) return;

            Vector2 difference = target.position - source.position;

            double distance = difference.Length();

            double move = (distance - connectDistance) / (distance * totalMass + CORRECTION_NUMBER) * connectStiffness;
            Vector2 correction = difference * move;

            source.position += correction * source.invMass;
            target.position -= correction * target.invMass;

            double angle = -difference.X * source.velocity.Y + difference.X * source.velocity.X;

            double rotateAngle = (Math.Acos(Vector2.Dot(difference, source.velocity) / (distance * source.velocity.Length())) * (180 / Math.PI));

            if(angle > 0){
                source.rotateSpeed -= rotateAngle / ROTATION_STRENGTH;
                target.rotateSpeed += rotateAngle / ROTATION_STRENGTH;
            }else if(angle < 0){
                source.rotateSpeed += rotateAngle / ROTATION_STRENGTH;
                target.rotateSpeed -= rotateAngle / ROTATION_STRENGTH;
            }
        }

        /// <summary>
        /// エンティティーの速度の計算
        /// </summary>
        /// <param name="entity">対象のエンティティー</param>
        protected void SolveSpeed(Entity entity){
            double coefficient = entity.mass * entity.radius * this.friction*FRICTION_STRENGTH;

            entity.velocity -= entity.velocity * coefficient * this.deltaTime;
            entity.rotateSpeed -= entity.rotateSpeed * coefficient * this.deltaTime;

            if(Math.Abs(entity.rotateSpeed) > MAX_ROTATION){
                entity.rotateSpeed = Math.Sign(entity.rotateSpeed) * MAX_ROTATION;
            }
        }

        /// <summary>
        /// エンティティーの速度の更新
        /// </summary>
        /// <param name="entity">対象のエンティティー</param>
        protected void UpdateSpeed(Entity entity){
            entity.velocity = (entity.position - entity.previousPosition) / this.deltaTime;

            if(entity.mass != 0){
                entity.velocity.Y += this.gravity * this.deltaTime;
            }
        }

        /// <summary>
        /// エンティティーの位置の更新
        /// </summary>
        /// <param name="entity">対象のエンティティー</param>
        protected void UpdatePosition(Entity entity){
            entity.SavePosition();

            entity.position += entity.velocity * this.deltaTime;
        }

        /// <summary>
        /// エンティティーの回転の更新
        /// </summary>
        /// <param name="entity">対象のエンティティー</param>
        protected void UpdateRotate(Entity entity){
            entity.rotateAngle += entity.rotateSpeed * this.deltaTime;
        }
    }
}
