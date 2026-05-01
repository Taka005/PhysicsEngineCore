using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Utils;
using System.Windows;
using System.Windows.Media;

namespace PhysicsEngineCore.Renders{
    public class ObjectRender{
        public static void DrawCircle(DrawingContext context,Circle circle){
            if (circle.image == null){
                Brush brush = ParseColor.StringToBrush(circle.color);

                Pen pen = new Pen(brush, 1);

                context.DrawEllipse(
                    brush,
                    pen,
                    new Point(circle.position.X, circle.position.Y),
                    circle.radius - 0.5,
                    circle.radius - 0.5
                );
            }else{
                TransformGroup transformGroup = new TransformGroup();

                double angleRad = Math.Atan2(circle.velocity.Y, -circle.velocity.X);

                transformGroup.Children.Add(new RotateTransform(angleRad * 180 / Math.PI, circle.position.X, circle.position.Y));

                context.PushTransform(transformGroup);
                context.PushClip(new EllipseGeometry(new Point(circle.position.X, circle.position.Y), circle.radius, circle.radius));

                context.DrawImage(
                    circle.image.source,
                    new Rect(
                        circle.position.X - circle.radius,
                        circle.position.Y - circle.radius,
                        circle.diameter,
                        circle.diameter
                    )
                );

                context.Pop();
                context.Pop();
            }
        }

        public static void DrawRope(DrawingContext context,Rope rope){
            if (rope.image == null){
                Brush brush = ParseColor.StringToBrush(rope.color);

                Pen pen = new Pen(brush, rope.width);

                Entity? target = null;

                rope.entities.ForEach(entity =>{
                    if (target != null){
                        context.DrawLine(
                            pen,
                            new Point(entity.position.X, entity.position.Y),
                            new Point(target.position.X, target.position.Y)
                        );

                        context.DrawEllipse(
                            brush,
                            null,
                            new Point(entity.position.X, entity.position.Y),
                            entity.radius,
                            entity.radius
                        );
                    }else{
                        context.DrawEllipse(
                            brush,
                            null,
                            new Point(entity.position.X, entity.position.Y),
                            entity.radius,
                            entity.radius
                        );
                    }

                    if (rope.entities.IndexOf(entity) == rope.entities.Count - 1){
                        context.DrawEllipse(
                            brush,
                            null,
                            new Point(entity.position.X, entity.position.Y),
                            entity.radius,
                            entity.radius
                        );
                    }

                    target = entity;
                });
            }else{
                rope.entities.ForEach(entity=>{
                    TransformGroup transformGroup = new TransformGroup();

                    double angleRad = Math.Atan2(rope.velocity.Y, -rope.velocity.X);

                    transformGroup.Children.Add(new RotateTransform(angleRad * 180 / Math.PI, entity.position.X, entity.position.Y));

                    context.PushTransform(transformGroup);
                    context.PushClip(new EllipseGeometry(new Point(entity.position.X, entity.position.Y), entity.radius, entity.radius));

                    context.DrawImage(
                        rope.image.source,
                        new Rect(
                            entity.position.X - entity.radius,
                            entity.position.Y - entity.radius,
                            entity.diameter,
                            entity.diameter
                        )
                    );

                    context.Pop();
                });

                context.Pop();
            }
        }

        public static void DrawSquare(DrawingContext context,Square square){
            if (square.image == null){
                Brush brush = ParseColor.StringToBrush(square.color);

                Pen pen = new Pen(brush, square.size / 2);

                square.entities.ForEach(source => {
                    context.DrawEllipse(
                            brush,
                            null,
                            new Point(source.position.X, source.position.Y),
                            source.radius,
                            source.radius
                        );

                    square.entities.ForEach(target => {
                        if (source.id == target.id) return;

                        context.DrawLine(
                            pen,
                            new Point(source.position.X, source.position.Y),
                            new Point(target.position.X, target.position.Y)
                        );
                    });
                });
            }else{
                TransformGroup transformGroup = new TransformGroup();

                Entity start = square.entities[0];
                Entity end = square.entities[2];

                double angle = (start.position - end.position).Angle();

                angle += Math.PI;

                transformGroup.Children.Add(new RotateTransform(angle * 180 / Math.PI, square.position.X, square.position.Y));

                context.PushTransform(transformGroup);

                context.DrawImage(
                    square.image.source,
                    new Rect(
                        square.position.X - square.size / 2,
                        square.position.Y - square.size / 2,
                        square.size,
                        square.size
                    )
                );

                context.Pop();
            }
        }

        public static void DrawTriangle(DrawingContext context,Triangle triangle){
            if (triangle.image == null){
                Brush brush = ParseColor.StringToBrush(triangle.color);

                Pen pen = new Pen(brush, triangle.entityDiameter);

                triangle.entities.ForEach(source => {
                    context.DrawEllipse(
                            brush,
                            null,
                            new Point(source.position.X, source.position.Y),
                            source.radius,
                            source.radius
                        );

                    triangle.entities.ForEach(target => {
                        if (source.id == target.id) return;

                        context.DrawLine(
                            pen,
                            new Point(source.position.X, source.position.Y),
                            new Point(target.position.X, target.position.Y)
                        );
                    });
                });
            }else{
                TransformGroup transformGroup = new TransformGroup();

                Entity start = triangle.entities[1];
                Entity end = triangle.entities[2];

                double angle = (start.position - end.position).Angle();

                angle += Math.PI;

                transformGroup.Children.Add(new RotateTransform(angle * 180 / Math.PI, triangle.position.X, triangle.position.Y));

                context.PushTransform(transformGroup);

                context.DrawImage(
                    triangle.image.source,
                    new Rect(
                        triangle.position.X - triangle.size / 2,
                        triangle.position.Y - triangle.size / 2,
                        triangle.size,
                        triangle.size
                    )
                );

                context.Pop();
            }
        }

        public static void DrawVector(DrawingContext context,IObject obj){
            if (obj.velocity.IsZero()) return;

            Pen vectorPen = new Pen(Brushes.Black, 1);

            Point startPoint = new Point(obj.position.X, obj.position.Y);
            Point endPoint = new Point(obj.position.X + obj.velocity.X, obj.position.Y + obj.velocity.Y);

            context.DrawLine(vectorPen, startPoint, endPoint);
        }
    }
}
