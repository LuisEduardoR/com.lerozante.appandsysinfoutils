/*
* Author: Luís Eduardo Rozante
*/

using UnityEngine;

namespace Lerozante.AppAndSysInfoUtils
{

    /// <summary>
    /// Helps calculate aspect ratios froms creen resolutions.
    /// </summary>
    public static class AspectRatioUtils {

        /// <summary>
        /// Returns a string representing an aspect ration for the resolution provided.
        /// </summary>
        /// <param "resolution"=Resolution to calculate the aspect ration from.</param>
        /// <returns>String with the aspect ration between width and heigt in the format "W:H".</returns>
        public static string ResolutionToAspectRatioString(Resolution resolution) {
            return ResolutionToAspectRatioString(resolution.width, resolution.height);
        }

        /// <summary>
        /// Returns a string representing an aspect ration for the resolution provided.
        /// </summary>
        /// <param "width"=Witdh of the screen.</param>
        /// <param "heigh"=Height of the screen.</param>
        /// <returns>String with the aspect ration between width and heigt in the format "W:H".</returns>
        public static string ResolutionToAspectRatioString(int width, int height) {
            int gcd = GCD(width, height);
            return $"{width / gcd}:{height / gcd}";
        }

        /// <summary>
        /// Calculates the GCD between two numbers. (Are you serious that neither UnityEngine.Mathf nor System.Math has a GCD function?)
        /// </summary>
        /// <param "a"=The first number.</param>
        /// <param "b"=The second number.</param>
        /// <returns> The GCD of a and b. </returns>
        static int GCD(int a, int b) {
            while (a != 0 && b != 0) {
                if (a > b) {
                    a %= b;
                }
                else {
                    b %= a;
                }
            }
            return a | b;
        }
    }

}
