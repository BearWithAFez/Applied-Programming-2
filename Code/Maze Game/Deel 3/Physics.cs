using System;

namespace MazeGame
{
    static class Physics
    {
        public static (double displacementX, double displacementY, double velocityX, double velocityY, int moveDirX, int moveDirY) PhysicsLoop(BlockType block, double thetaX, double thetaY, double mass, int moveDirX, int moveDirY, double velocityX, double velocityY)
        {
            var returnable = (displacementX: 0.0, displacementY: 0.0, velocityX: velocityX, velocityY: velocityY, moveDirX: moveDirX, moveDirY: moveDirY);

            // Calculate the accelerations
            var aX = CalculateAcceleration(block, thetaX, mass, moveDirX);
            var aY = CalculateAcceleration(block, thetaY, mass, moveDirY);

            // X-Axis
            if (aX == 0)
            {
                // No movement in X
                returnable.displacementX = 0;
                returnable.velocityX = 0;
                returnable.moveDirX = 0;
            }
            else
            {
                var sX = CalculateDisplacement(velocityX, aX);
                var vX = CalculateVelocity(velocityX, aX);
                if (vX * moveDirX < 0)
                {
                    // Stop X
                    returnable.displacementX = 0;
                    returnable.velocityX = 0;
                    returnable.moveDirX = 0;
                }
                else
                {
                    returnable.displacementX = sX * Constants.DISPLACEMENT_SCALING;
                    returnable.velocityX = vX;
                    returnable.moveDirX = (vX < 0) ? -1 : +1;
                }
            }

            // No movement in Y
            if (aY == 0)
            {
                // No movement in Y
                returnable.displacementY = 0;
                returnable.velocityY = 0;
                returnable.moveDirY = 0;
            }
            else
            {
                var sY = CalculateDisplacement(velocityY, aY);
                var vY = CalculateVelocity(velocityY, aY);
                if (vY * moveDirY < 0)
                {
                    // Stop Y
                    returnable.displacementY = 0;
                    returnable.velocityY = 0;
                    returnable.moveDirY = 0;
                }
                else
                {
                    returnable.displacementY = sY * Constants.DISPLACEMENT_SCALING;
                    returnable.velocityY = vY;
                    returnable.moveDirY = (vY < 0) ? -1 : +1;
                }
            }

            Console.WriteLine("X-Axis:{ °-" + thetaX + "; a-" + aX + "; v-" + returnable.velocityX + "; dir-" + returnable.moveDirX + "; s-" + returnable.displacementX + "}");
            Console.WriteLine("Y-Axis:{ °-" + thetaY + "; a-" + aY + "; v-" + returnable.velocityY + "; dir-" + returnable.moveDirY + "; s-" + returnable.displacementY + "}");
            return returnable;
        }

        /// <summary>
        /// Calculates the Acceleration on the given axis
        /// </summary>
        /// <param name="block">The contact-block, used for friction</param>
        /// <param name="theta">The angle of the board-tilt in degrees</param>
        /// <param name="mass">The weight of the player in kilograms</param>
        /// <param name="moveDir">Axis movement "vector" in (-1/0/+1)</param>
        /// <returns>The acceleration in m/s/s</returns>
        public static double CalculateAcceleration(BlockType block, double theta, double mass, int moveDir)
        {
            // pre-work
            double theta_rad = Constants.ToRadians(theta);
            double Fg, Fe, FN, Ffr, Fres;

            Fg = mass * Constants.G;        // Grav-force
            FN = Fg * Math.Cos(theta_rad);  // Normal-force

            // No movement in this axis?
            if (moveDir == 0)
            {
                Ffr = FN * block.µs;
                Fe = Math.Abs(Fg * Math.Sin(theta_rad));
                if (Ffr >= Fe) return 0;    // Static Friction to much to cause movement
            }

            Ffr = FN * block.µk;
            Fe = Fg * Math.Sin(theta_rad);
            Fe = Math.Abs(Fe) - Ffr;

            if (theta_rad == 0) Fres = Fe * moveDir;
            else Fres = ((theta_rad < 0) ? -1 : 1) * Fe;

            return Fres / mass;
        }

        /// <summary>
        /// Calculates the Displacement on the given axis
        /// </summary>
        /// <param name="initialVelocity">The initial velocity</param>
        /// <param name="acceleration">The acceleration</param>
        /// <returns>The displacement</returns>
        public static double CalculateDisplacement(double initialVelocity, double acceleration) => initialVelocity * (1000 / Constants.FPS) + .5 * acceleration * Math.Pow((1000 / Constants.FPS), 2);

        /// <summary>
        /// Calculates the Velocity on the given axis
        /// </summary>
        /// <param name="initialVelocity">The initial velocity</param>
        /// <param name="acceleration">The acceleration</param>
        /// <returns>The final velocity</returns>
        public static double CalculateVelocity(double initialVelocity, double acceleration)
        {
            var v = initialVelocity + acceleration * (1000 / Constants.FPS);
            return (Constants.MAX_V < v) ? Constants.MAX_V : ((-Constants.MAX_V > v) ? -Constants.MAX_V : v);
        }
    }
}
