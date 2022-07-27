﻿

using BEPUPhysics1int;
namespace BEPUPhysics1int.BroadPhaseSystems
{
    ///<summary>
    /// Requires that a class have a BoundingBox.
    ///</summary>
    public interface IBoundingBoxOwner
    {
        ///<summary>
        /// Gets the bounding box of the object.
        ///</summary>
        BoundingBox BoundingBox { get; }
    }
}
