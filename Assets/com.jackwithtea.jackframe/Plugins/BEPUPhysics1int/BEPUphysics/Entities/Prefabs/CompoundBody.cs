using System;
using System.Collections.Generic;
using BEPUPhysics1int.BroadPhaseEntries.MobileCollidables;
using BEPUPhysics1int.CollisionShapes;
using BEPUPhysics1int;
using BEPUPhysics1int.DataStructures;
using FixMath.NET;

namespace BEPUPhysics1int
{
    /// <summary>
    /// Acts as a grouping of multiple other objects.  Can be used to form physically simulated concave shapes.
    /// </summary>
    public class CompoundBody : Entity<CompoundCollidable>
    {
        ///<summary>
        /// Gets the list of shapes in the compound.
        ///</summary>
        public ReadOnlyList<CompoundShapeEntry> Shapes
        {
            get
            {
                return CollisionInformation.Shape.Shapes;
            }
        }


        /// <summary>
        /// Creates a new kinematic CompoundBody with the given subbodies.
        /// </summary>
        /// <param name="bodies">List of entities to use as subbodies of the compound body.</param>
        /// <exception cref="InvalidOperationException">Thrown when the bodies list is empty or there is a mix of kinematic and dynamic entities in the body list.</exception>
        public CompoundBody(IList<CompoundShapeEntry> bodies)
        {
            FixedV3 center;
            var shape = new CompoundShape(bodies, out center);
            Initialize(new CompoundCollidable(shape));
            Position = center;
        }


        /// <summary>
        /// Creates a new dynamic CompoundBody with the given subbodies.
        /// </summary>
        /// <param name="bodies">List of entities to use as subbodies of the compound body.</param>
        /// <param name="mass">Mass of the compound.</param>
        /// <exception cref="InvalidOperationException">Thrown when the bodies list is empty or there is a mix of kinematic and dynamic entities in the body list.</exception>
        public CompoundBody(IList<CompoundShapeEntry> bodies, Fixed64 mass)
        {
            FixedV3 center;
            var shape = new CompoundShape(bodies, out center);
            Initialize(new CompoundCollidable(shape), mass);
            Position = center;
        }


        ///<summary>
        /// Constructs a kinematic compound body from the children data.
        ///</summary>
        ///<param name="children">Children data to construct the compound from.</param>
        public CompoundBody(IList<CompoundChildData> children)
        {

            FixedV3 center;
            var collidable = new CompoundCollidable(children, out center);
            Initialize(collidable);
            Position = center;
        }

        ///<summary>
        /// Constructs a dynamic compound body from the children data.
        ///</summary>
        ///<param name="children">Children data to construct the compound from.</param>
        ///<param name="mass">Mass of the compound body.</param>
        public CompoundBody(IList<CompoundChildData> children, Fixed64 mass)
        {
            FixedV3 center;
            var collidable = new CompoundCollidable(children, out center);
            Initialize(collidable, mass);
            Position = center;
        }



    }


}