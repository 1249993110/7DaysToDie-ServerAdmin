using System.Diagnostics.CodeAnalysis;

namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// Position
    /// </summary>
    public struct Position
    {
        public Position() { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        [SetsRequiredMembers]
        public Position(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        [SetsRequiredMembers]
        public Position(float x, float y, float z)
        {
            X = (int)x;
            Y = (int)y;
            Z = (int)z;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        [SetsRequiredMembers]
        public Position(double x, double y, double z)
        {
            X = (int)x;
            Y = (int)y;
            Z = (int)z;
        }

        /// <summary>
        /// X
        /// </summary>
        public required int X { get; set; }

        /// <summary>
        /// Y
        /// </summary>
        public required int Y { get; set; }

        /// <summary>
        /// Z
        /// </summary>
        public required int Z { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override readonly string ToString()
        {
            return $"{X} {Y} {Z}";
        }
    }
}