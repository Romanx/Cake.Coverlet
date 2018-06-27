using System;

namespace Cake.Coverlet
{
    /// <summary>
    /// The type of threshold to apply the limit to
    /// </summary>
    [Flags]
    public enum ThresholdType
    {
        /// <summary>
        /// Not Set
        /// </summary>
        NotSet = 0x0,
        /// <summary>
        /// Line
        /// </summary>
        Line = 0x1,
        /// <summary>
        /// Branch
        /// </summary>
        Branch = 0x2,
        /// <summary>
        /// Method
        /// </summary>
        Method = 0x4
    }
}
