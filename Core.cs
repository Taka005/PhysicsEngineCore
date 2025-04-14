using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore{
    public class Core{
        public int pps;
        public double gravity;
        public double friction;
        private static readonly double CORRECTION_NUMBER = 0.00000001d;
        private static readonly double ROTATION_STRENGTH = 50d;
        private static readonly double MAX_ROTATION = 500d;

        public Core(int pps, double gravity, double friction){
            this.pps = pps;
            this.gravity = gravity;
            this.friction = friction;
        }

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

        protected void SolveGroundPosition(Entity entity,IGround ground){
            if(entity.mass == 0) return;

            Vector2 position = ground.SolvePosition(entity.position);

            Vector2 difference = position - entity.position;
            double distance = difference.Length();
            if(distance > entity.radius + ground.thickness/2) return;

            double move = (distance - (entity.radius + ground.thickness/2)) / (distance * entity.invMass + CORRECTION_NUMBER) * entity.stiffness;
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

        protected void SolveSpeed(Entity entity){
            double coefficient = entity.mass * entity.radius * this.friction;

            entity.velocity -= entity.velocity * coefficient * (1 / this.pps);
            entity.rotateSpeed -= entity.rotateSpeed * coefficient * (1 / this.pps);

            if(Math.Abs(entity.rotateSpeed) > MAX_ROTATION){
                entity.rotateSpeed = Math.Sign(entity.rotateSpeed) * MAX_ROTATION;
            }
        }

        protected void UpdateSpeed(Entity entity){
            entity.velocity.X = (entity.position.X - entity.previousPosition.X)/(1/this.pps);
            entity.velocity.Y = (entity.position.Y - entity.previousPosition.Y)/(1/this.pps);

            if(entity.mass != 0){
                entity.velocity.Y += this.gravity * (1/this.pps);
            }
        }

        protected void UpdatePosition(Entity entity){
            entity.SavePosition();

            entity.position.X += entity.velocity.X * (1/this.pps);
            entity.position.Y += entity.velocity.Y * (1/this.pps);
        }

        protected void UpdateRotate(Entity entity){
            entity.rotateAngle += entity.rotateSpeed * (1/this.pps);
        }
    }
}
