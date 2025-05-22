using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Objects;
using System.Diagnostics;

namespace PhysicsEngineCore{
    public class Core{
        public int pps;
        public double gravity;
        public double friction;
        private static readonly double CORRECTION_NUMBER = 0.0000001d;
        private static readonly double ROTATION_STRENGTH = 50d;
        private static readonly double FRICTION_STRENGTH = 0.1d;
        private static readonly double MAX_ROTATION = 500d;

        public Core(int pps, double gravity, double friction){
            this.pps = pps;
            this.gravity = gravity;
            this.friction = friction;
        }

        protected double deltaTime{
            get{
                return 1 / (double)this.pps;
            }
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

        protected void SolveSpeed(Entity entity){
            double coefficient = entity.mass * entity.radius * this.friction*FRICTION_STRENGTH;

            entity.velocity -= entity.velocity * coefficient * this.deltaTime;
            entity.rotateSpeed -= entity.rotateSpeed * coefficient * this.deltaTime;

            if(Math.Abs(entity.rotateSpeed) > MAX_ROTATION){
                entity.rotateSpeed = Math.Sign(entity.rotateSpeed) * MAX_ROTATION;
            }
        }

        protected void UpdateSpeed(Entity entity){
            entity.velocity.X = (entity.position.X - entity.previousPosition.X)/ this.deltaTime;
            entity.velocity.Y = (entity.position.Y - entity.previousPosition.Y)/ this.deltaTime;

            if(entity.mass != 0){
                entity.velocity.Y += this.gravity * this.deltaTime;
            }
        }

        protected void UpdatePosition(Entity entity){
            entity.SavePosition();

            entity.position.X += entity.velocity.X * this.deltaTime;
            entity.position.Y += entity.velocity.Y * this.deltaTime;
        }

        protected void UpdateRotate(Entity entity){
            entity.rotateAngle += entity.rotateSpeed * this.deltaTime;
        }
    }
}
