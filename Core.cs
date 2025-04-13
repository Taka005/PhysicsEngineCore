using System.Numerics;
using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore{
    class Core{
        private static readonly float CORRECTION_NUMBER = 0.000001f;
        private static readonly float ROTATION_STRENGTH = 50;

        protected void solvePosition(Entity source, Entity target){
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
    }
}
