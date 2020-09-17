using Mapster;

namespace Isotope.Code.Utils
{
    /// <summary>
    /// Interface for DTO types that provide mapping from models.
    /// </summary>
    public interface IMapped
    {
        /// <summary>
        /// Creates a mapping for this type.
        /// </summary>
        void Configure(TypeAdapterConfig config);
    }
}