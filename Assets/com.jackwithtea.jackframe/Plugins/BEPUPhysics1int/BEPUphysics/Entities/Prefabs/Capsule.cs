using BEPUPhysics1int.BroadPhaseEntries.MobileCollidables;
using BEPUPhysics1int.EntityStateManagement;
 
using BEPUPhysics1int.CollisionShapes.ConvexShapes;
using BEPUPhysics1int;
using FixMath.NET;

namespace BEPUPhysics1int
{
    /// <summary>
    /// Pill-shaped object that can collide and move.  After making an entity, add it to a Space so that the engine can manage it.
    /// </summary>
    public class Capsule : Entity<ConvexCollidable<CapsuleShape>>
    {
        /// <summary>
        /// Gets or sets the length of the capsule.
        /// </summary>
        public Fixed64 Length
        {
            get
            {
                return CollisionInformation.Shape.Length;
            }
            set
            {
                CollisionInformation.Shape.Length = value;
            }
        }

        /// <summary>
        /// Gets or sets the radius of the capsule.
        /// </summary>
        public Fixed64 Radius
        {
            get
            {
                return CollisionInformation.Shape.Radius;
            }
            set
            {
                CollisionInformation.Shape.Radius = value;
            }
        }

        private Capsule(Fixed64 len, Fixed64 rad)
            : base(new ConvexCollidable<CapsuleShape>(new CapsuleShape(len, rad)))
        {
        }

        private Capsule(Fixed64 len, Fixed64 rad, Fixed64 mass)
            : base(new ConvexCollidable<CapsuleShape>(new CapsuleShape(len, rad)), mass)
        {
        }



        ///<summary>
        /// Computes an orientation and length from a line segment.
        ///</summary>
        ///<param name="start">Starting point of the line segment.</param>
        ///<param name="end">Endpoint of the line segment.</param>
        ///<param name="orientation">Orientation of a line that fits the line segment.</param>
        ///<param name="length">Length of the line segment.</param>
        public static void GetCapsuleInformation(ref FixedV3 start, ref FixedV3 end, out FixedQuaternion orientation, out Fixed64 length)
        {
            FixedV3 segmentDirection;
            FixedV3.Subtract(ref end, ref start, out segmentDirection);
            length = segmentDirection.Length();
            if (length > F64.C0)
            {
                FixedV3.Divide(ref segmentDirection, length, out segmentDirection);
                FixedQuaternion.GetQuaternionBetweenNormalizedVectors(ref Toolbox.UpVector, ref segmentDirection, out orientation);
            }
            else
                orientation = FixedQuaternion.Identity;
        }

        ///<summary>
        /// Constructs a new kinematic capsule.
        ///</summary>
        ///<param name="start">Line segment start point.</param>
        ///<param name="end">Line segment end point.</param>
        ///<param name="radius">Radius of the capsule to expand the line segment by.</param>
        public Capsule(FixedV3 start, FixedV3 end, Fixed64 radius)
            : this((end - start).Length(), radius)
        {
            Fixed64 length;
            FixedQuaternion orientation;
            GetCapsuleInformation(ref start, ref end, out orientation, out length);
            this.Orientation = orientation;
            FixedV3 position;
            FixedV3.Add(ref start, ref end, out position);
            FixedV3.Multiply(ref position, F64.C0p5, out position);
            this.Position = position;
        }


        ///<summary>
        /// Constructs a new dynamic capsule.
        ///</summary>
        ///<param name="start">Line segment start point.</param>
        ///<param name="end">Line segment end point.</param>
        ///<param name="radius">Radius of the capsule to expand the line segment by.</param>
        /// <param name="mass">Mass of the entity.</param>
        public Capsule(FixedV3 start, FixedV3 end, Fixed64 radius, Fixed64 mass)
            : this((end - start).Length(), radius, mass)
        {
            Fixed64 length;
            FixedQuaternion orientation;
            GetCapsuleInformation(ref start, ref end, out orientation, out length);
            this.Orientation = orientation;
            FixedV3 position;
            FixedV3.Add(ref start, ref end, out position);
            FixedV3.Multiply(ref position, F64.C0p5, out position);
            this.Position = position;
        }

        /// <summary>
        /// Constructs a physically simulated capsule.
        /// </summary>
        /// <param name="position">Position of the capsule.</param>
        /// <param name="length">Length of the capsule.</param>
        /// <param name="radius">Radius of the capsule.</param>
        /// <param name="mass">Mass of the object.</param>
        public Capsule(FixedV3 position, Fixed64 length, Fixed64 radius, Fixed64 mass)
            : this(length, radius, mass)
        {
            Position = position;
        }

        /// <summary>
        /// Constructs a nondynamic capsule.
        /// </summary>
        /// <param name="position">Position of the capsule.</param>
        /// <param name="length">Length of the capsule.</param>
        /// <param name="radius">Radius of the capsule.</param>
        public Capsule(FixedV3 position, Fixed64 length, Fixed64 radius)
            : this(length, radius)
        {
            Position = position;
        }

        /// <summary>
        /// Constructs a dynamic capsule.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="length">Length of the capsule.</param>
        /// <param name="radius">Radius of the capsule.</param>
        /// <param name="mass">Mass of the object.</param>
        public Capsule(MotionState motionState, Fixed64 length, Fixed64 radius, Fixed64 mass)
            : this(length, radius, mass)
        {
            MotionState = motionState;
        }

        /// <summary>
        /// Constructs a nondynamic capsule.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="length">Length of the capsule.</param>
        /// <param name="radius">Radius of the capsule.</param>
        public Capsule(MotionState motionState, Fixed64 length, Fixed64 radius)
            : this(length, radius)
        {
            MotionState = motionState;
        }

    }
}