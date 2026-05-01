using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Utils;
using System.Windows;
using System.Windows.Media;

namespace PhysicsEngineCore.Renders{
    public class EffectRender{
        public static void DrawBooster(DrawingContext context,Booster booster){
            if (booster.image == null){
                Brush brush = ParseColor.StringToBrush(booster.color);
                Pen pen = new Pen(brush, 3);

                double minX = Math.Min(booster.start.X, booster.end.X);
                double minY = Math.Min(booster.start.Y, booster.end.Y);
                double maxX = Math.Max(booster.start.X, booster.end.X);
                double maxY = Math.Max(booster.start.Y, booster.end.Y);

                context.DrawRectangle(null, pen, new Rect(minX, minY, maxX - minX, maxY - minY));
            }else{
                double minX = Math.Min(booster.start.X, booster.end.X);
                double minY = Math.Min(booster.start.Y, booster.end.Y);
                double maxX = Math.Max(booster.start.X, booster.end.X);
                double maxY = Math.Max(booster.start.Y, booster.end.Y);

                context.DrawImage(booster.image.source, new Rect(minX, minY, maxX - minX, maxY - minY));
            }
        }

        public static void DrawEdge(DrawingContext context, IEffect effect){
            Pen edgePen = new Pen(Brushes.Black, 1);

            if (effect is Booster booster){
                context.DrawEllipse(
                    null,
                    edgePen,
                    new Point(booster.start.X, booster.start.Y),
                    1.5 - 0.5,
                    1.5 - 0.5
                );

                context.DrawEllipse(
                    null,
                    edgePen,
                    new Point(booster.end.X, booster.end.Y),
                    1.5 - 0.5,
                    1.5 - 0.5
                );
            }
        }
    }
}
