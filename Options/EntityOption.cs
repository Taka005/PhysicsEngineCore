﻿using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Options{
    public class EntityOption() : IOption {
        public string? id { get; set; }
        public double posX { get; set; }
        public double posY { get; set; }
        public double prePosX { get; set; } = 0;
        public double prePosY { get; set; } = 0;
        public double diameter { get; set; }
        public double mass { get; set; }
        public double stiffness { get; set; }
        public string? parentId { get; set; }
        public double velocityX { get; set; } = 0;
        public double velocityY { get; set; } = 0;
        public List<Target> targets { get; set; } = [];
    }
}