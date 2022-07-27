﻿using System;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUutilities;
using BEPUutilities.DataStructures;
using FixMath.NET;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms
{
    ///<summary>
    /// Persistent tester that compares triangles against convex objects.
    ///</summary>
    public sealed class TriangleSpherePairTester : TrianglePairTester
    {
        internal SphereShape sphere;

        private VoronoiRegion lastRegion;


        //Relies on the triangle being located in the local space of the convex object.  The convex transform is used to transform the
        //contact points back from the convex's local space into world space.
        ///<summary>
        /// Generates a contact between the triangle and convex.
        ///</summary>
        ///<param name="contactList">Contact between the shapes, if any.</param>
        ///<returns>Whether or not the shapes are colliding.</returns>
        public override bool GenerateContactCandidates(TriangleShape triangle, out TinyStructList<ContactData> contactList)
        {
            contactList = new TinyStructList<ContactData>();


            FixedV3 ab, ac;
            FixedV3.Subtract(ref triangle.vB, ref triangle.vA, out ab);
            FixedV3.Subtract(ref triangle.vC, ref triangle.vA, out ac);
            FixedV3 triangleNormal;
            FixedV3.Cross(ref ab, ref ac, out triangleNormal);
            if (triangleNormal.LengthSquared() < Toolbox.Epsilon * F64.C0p01)
            {
                //If the triangle is degenerate, use the offset between its center and the sphere.
                FixedV3.Add(ref triangle.vA, ref triangle.vB, out triangleNormal);
                FixedV3.Add(ref triangleNormal, ref triangle.vC, out triangleNormal);
                FixedV3.Multiply(ref triangleNormal, F64.OneThird, out triangleNormal);
                if (triangleNormal.LengthSquared() < Toolbox.Epsilon * F64.C0p01)
                    triangleNormal = Toolbox.UpVector; //Alrighty then! Pick a random direction.
                    
            }

            
            Fixed64 dot;
            FixedV3.Dot(ref triangleNormal, ref triangle.vA, out dot);
            switch (triangle.sidedness)
            {
                case TriangleSidedness.DoubleSided:
                    if (dot < F64.C0)
                        FixedV3.Negate(ref triangleNormal, out triangleNormal); //Normal must face outward.
                    break;
                case TriangleSidedness.Clockwise:
                    if (dot > F64.C0)
                        return false; //Wrong side, can't have a contact pointing in a reasonable direction.
                    break;
                case TriangleSidedness.Counterclockwise:
                    if (dot < F64.C0)
                        return false; //Wrong side, can't have a contact pointing in a reasonable direction.
                    break;

            }


            FixedV3 closestPoint;
            //Could optimize this process a bit.  The 'point' being compared is always zero.  Additionally, since the triangle normal is available,
            //there is a little extra possible optimization.
            lastRegion = Toolbox.GetClosestPointOnTriangleToPoint(ref triangle.vA, ref triangle.vB, ref triangle.vC, ref Toolbox.ZeroVector, out closestPoint);
            Fixed64 lengthSquared = closestPoint.LengthSquared();
            Fixed64 marginSum = triangle.collisionMargin + sphere.collisionMargin;

            if (lengthSquared <= marginSum * marginSum)
            {
                var contact = new ContactData();
                if (lengthSquared < Toolbox.Epsilon)
                {
                    //Super close to the triangle.  Normalizing would be dangerous.

                    FixedV3.Negate(ref triangleNormal, out contact.Normal);
                    contact.Normal.Normalize();
                    contact.PenetrationDepth = marginSum;
                    contactList.Add(ref contact);
                    return true;
                }

                lengthSquared = Fixed64.Sqrt(lengthSquared);
                FixedV3.Divide(ref closestPoint, lengthSquared, out contact.Normal);
                contact.PenetrationDepth = marginSum - lengthSquared;
                contact.Position = closestPoint;
                contactList.Add(ref contact);
                return true;

            }
            return false;




        }

        public override VoronoiRegion GetRegion(TriangleShape triangle, ref ContactData contact)
        {
            return lastRegion;
        }


        public override bool ShouldCorrectContactNormal
        {
            get
            {
                return false;
            }
        }

        ///<summary>
        /// Initializes the pair tester.
        ///</summary>
        ///<param name="convex">Convex shape to use.</param>
        public override void Initialize(ConvexShape convex)
        {
            this.sphere = (SphereShape)convex;
        }

        /// <summary>
        /// Cleans up the pair tester.
        /// </summary>
        public override void CleanUp()
        {
            sphere = null;
            Updated = false;
        }
    }

}
