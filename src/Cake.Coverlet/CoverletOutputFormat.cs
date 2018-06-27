using System;

namespace Cake.Coverlet
{
    /// <summary>
    /// Maps to the output formats provided by Coverlet
    /// </summary>
    [Flags]
    public enum CoverletOutputFormat
    {
        /// <summary>
        /// Json
        /// </summary>
        json = 0x1,
        /// <summary>
        /// Lcov
        /// </summary>
        lcov = 0x2,
        /// <summary>
        /// Opencover
        /// </summary>
        opencover = 0x4,
        /// <summary>
        /// Cobertura
        /// </summary>
        cobertura = 0x8
    }
}
