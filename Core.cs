using System.Numerics;
using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore{
    class Core{
        private int pps;
        private float gravity;
        private float friction;
        private static readonly float CORRECTION_NUMBER = 0.000001f;
        private static readonly float ROTATION_STRENGTH = 50f;
        private static readonly float MAX_ROTATION = 500f;

        protected void SolvePosition(Entity source, Entity target){
            float totalMass = source.invMass + target.invMass;
            if(totalMass == 0) return;

            Vector2 difference = target.position - source.position;

            if(
                Math.Abs(difference.X) >= source.radius + target.radius ||
                Math.Abs(difference.Y) >= source.radius + target.radius
            ) return;

            float distance = difference.Length();
            if(distance > source.radius + target.radius) return;

            float move = (distance - (source.radius + target.radius)) / (distance * totalMass + CORRECTION_NUMBER) * source.stiffness;
            Vector2 correction = difference * move;

            source.position += correction * source.invMass;
            target.position -= correction * target.invMass;

            float angle = -difference.X * source.velocity.Y + difference.X * source.velocity.X;

            float rotate = (float)(Math.Acos(Vector2.Dot(difference,source.velocity) / (distance * source.velocity.Length())) * (180 / Math.PI));

            if(angle > 0){
                source.rotateSpeed -= rotate / ROTATION_STRENGTH;
                target.rotateSpeed += rotate / ROTATION_STRENGTH;
            }else if(angle < 0){
                source.rotateSpeed += rotate / ROTATION_STRENGTH;
                target.rotateSpeed -= rotate / ROTATION_STRENGTH;
            }
        }

        protected void SolveGroundPosition(Entity entity,IGround ground){
            if(entity.mass == 0) return;

            Vector2 position = ground.SolvePosition(entity.position);

            Vector2 difference = position - entity.position;
            float distance = difference.Length();
            if(distance > entity.radius + ground.thickness/2) return;

            float move = (distance - (entity.radius + ground.thickness/2)) / (distance * entity.invMass + CORRECTION_NUMBER) * entity.stiffness;
            Vector2 correction = difference * move;

            entity.position += correction * entity.invMass;

            float angle = -difference.X * entity.velocity.Y + difference.X * entity.velocity.X;

            float rotate = (float)(Math.Acos(Vector2.Dot(difference, entity.velocity) / (distance * entity.velocity.Length())) * (180 / Math.PI));

            if(angle > 0){
                entity.rotateSpeed -= rotate / ROTATION_STRENGTH;
            }else if(angle < 0){
                entity.rotateSpeed += rotate / ROTATION_STRENGTH;
            }
        }

        protected void SolveSpeed(Entity entity){
            float coefficient = entity.mass * entity.radius * this.friction;

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
            entity.rotate += entity.rotateSpeed * (1/this.pps);
        }
    }
}
