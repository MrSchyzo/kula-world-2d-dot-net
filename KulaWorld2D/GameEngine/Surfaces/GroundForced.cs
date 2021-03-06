﻿using GameEngine.Enumerations;
using GameEngine.Utils;
using LevelsStructure;
using System.Drawing;

namespace GameEngine.Surfaces
{
    public class GroundForced : ContactSurface
    {
        public GroundForced(int idxX, int idxY, KulaLevel.Orientation o)
        {
            surfacesPhysicInit(Constants.BlockWidth, 8, -RotationUtilities.getAngleFromDownOrientation(o));
            surfColor = Color.FromArgb(200, Color.Violet);
            type = SurfType.Forced;
            PointF newCenter = suggestedCenter(idxX, idxY, o);
            startingX = newCenter.X;
            startingY = newCenter.Y;
            currentX = startingX;
            currentY = startingY;
            isLandingNeeded = true;
        }
    }
}
