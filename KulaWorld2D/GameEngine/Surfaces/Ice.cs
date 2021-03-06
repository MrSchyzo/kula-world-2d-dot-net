﻿using GameEngine.Enumerations;
using GameEngine.Utils;
using LevelsStructure;
using System.Drawing;

namespace GameEngine.Surfaces
{
    public class Ice : ContactSurface
    {
        public Ice(int idxX, int idxY, KulaLevel.Orientation o)
        {
            type = SurfType.Ice;
            surfacesPhysicInit(Constants.BlockWidth, 8, -RotationUtilities.getAngleFromDownOrientation(o));
            surfColor = Color.FromArgb(200, Color.Azure);
            PointF newCenter = suggestedCenter(idxX, idxY, o);
            startingX = newCenter.X;
            startingY = newCenter.Y;
            currentX = startingX;
            currentY = startingY;
            isLandingNeeded = true;
        }
    }
}
