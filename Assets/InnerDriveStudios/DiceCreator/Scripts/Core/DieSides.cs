﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace InnerDriveStudios.DiceCreator
{
    /**
     * DieSides stores a collection of DieSide instances and can tell which DieSide is currently
     * the closest match (based on side normals and transform rotation) and whether that match is exact.
     * Exact refers to the state where there can be no discussion about the result of the die.
     * 
     * The DieSide instances themselves are generated by the DieSidesEditor, based on information
     * gathered through the DieAreaFinder.
     * 
     * @author J.C. Wichman
     * @copyright Inner Drive Studios
     */
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class DieSides : MonoBehaviour
    {
        /**
         * A regular die only requires 1 data entry per side, eg for a D6: 1,2,3,4,5 and 6.
         * More complicated dice however such as the dice for Descent might require different
         * values for Range, Damage, Surges etc. An assumption is that 4 should be enough,
         * so the DieSideEditor does not let you go higher than that. In case you do need to 
         * be able to store more than 4 values simply change the value below.
         */
        public const int MAX_DATA_ENTRIES_PER_SIDE = 4;

        /**
         * The cut off value to determine whether a side is considered to be an exact match while comparing
         * that side's normal with the y axis vector (up or down based on the MatchType for this Die).
         * Only change this if you know what you are doing, have good reason and have reread this line twice :).
         */
        public static float EXACT_MATCH_VALUE = 0.995f;
        
        /**
         * Almost all dice work the same: if the normal for a specific side in world space is 
         * pointing exactly along the world up vector, that side is considered to be 'active'.
         * The only exception to this rule is the D4, since it's a pyramid shape which means
         * the side whose world normal vector matches the world down vector is consider to be 'active'.
         * Which matching type you want to use can be specified using these enums/fields:
         */
        public enum MatchType { MATCH_TOP_SIDE, MATCH_BOTTOM_SIDE };

        [Tooltip(
            "The MatchType is not used during generation, only during play while detecting which side" +
            " is 'up'. This should almost always be MATCH_TOP_SIDE, unless this Die is a D4, in which case " +
            " you should also base the side ID on the downward facing side."
        )]
        [SerializeField]
        private MatchType _matchType = MatchType.MATCH_TOP_SIDE;

        //contains the data for all the Die sides, can be filled with help of the DieSidesEditor 
        [SerializeField]
        private DieSide[] _dieSides = null;

        //how many values should we store for each die side? Also used by the DieSidesEditor
        [SerializeField]
        private int _valueCountPerSide = 1;

        /**
         * @return how many data values are stored by each DieSide
         */
        public int valueCountPerSide { get { return _valueCountPerSide; } }

        /**
         * @return the number of DieSide instances in this collection
         */
        public int dieSideCount { get { return _dieSides == null ? 0 : _dieSides.Length; } }

        /**
         * @param pIndex the index for which to return the DieSide
         * @return the DieSide at the given index
         */
        public DieSide GetDieSide(int pIndex)
        {
            Assert.IsTrue(pIndex >= 0 && pIndex < dieSideCount, "Invalid DieSide requested.");
            return _dieSides[pIndex];
        }

        /**
         * @return a DieSideMatchInfo object describing the closest matching DieSide and whether it
         * was an exact match or not
         */
        public DieSideMatchInfo GetDieSideMatchInfo()
        {
            //Some math background:
            //		- the dot product of Vector A & B, equals: 
            //			the length of A times the length of B times the cosine of the angle between them. 
            //Seeing normals are normalized to length one, this simplifies to:
            //		-	the dot of a & b is the cosine of the angle between them. 
            //If the vectors are pointing in the same direction the angle is 0 and cos (0) == 1, 
            //If the vectors are pointing in the opposite direction the angle is PI and cos (PI) == -1
            //And everything in between...
            //We use this to derive which side is aligned closest with the world UP or DOWN vector
            //based on our MatchType (D4 -> MATCH_BOTTOM_SIDE, rest -> MATCH_TOP_SIDE

            int sideCount = this.dieSideCount;

            //convert the world up/down to our local space, instead of each side normal to world space
            Vector3 worldVectorToMatch = (_matchType == MatchType.MATCH_TOP_SIDE ? Vector3.up : Vector3.down);
            Vector3 localVectorToMatch = transform.InverseTransformDirection(worldVectorToMatch);

            DieSide closestMatch = null;
            float closestDot = 0;
            bool exactMatch = false;

            for (int i = 0; i < sideCount; i++)
            {
                DieSide side = _dieSides[i];
                float dot = Vector3.Dot(side.normal, localVectorToMatch);

                if (closestMatch == null || dot > closestDot)
                {
                    closestMatch = side;
                    closestDot = dot;

                    //if the world up or down (based on matchtype) vector is exactly aligned
                    //with this DieSide, the dot between the side normal and matchVector will be
                    //1 or at least VERY close to one. To allow a little bit of leeway/room for error
                    //we compare with EXACT_MATCH_VALUE instead of 1. 
                    //If we have this match we immediately stop checking the other sides. 
                    //Note that this condition can also occur while the Die is still moving
                    if (closestDot > EXACT_MATCH_VALUE)
                    {
                        exactMatch = true;
                        break;
                    };
                }
            }

            return new DieSideMatchInfo(closestMatch, exactMatch);
        }

        /** 
         * Utility method for getting the world rotation to align your dice with the world up vector.
         * 
         * @param pSideIndex the side index you would like to know the world rotation for
         * @return the rotation you would have to set to make sure the requested side index 
         *         becomes an exact match with the world up vector.
         *         
         * Example Usage: transform.rotation = GetComponent<DieSides>.GetWorldRotationFor (x);
         */
        public Quaternion GetWorldRotationFor(int pSideIndex)
        {
            return GetWorldRotationFor(pSideIndex, Vector3.up);
        }

        /** 
         * Utility method for getting the world rotation to align your dice with an arbitrary up vector.
         * 
         * @param pSideIndex the side index you would like to know the world rotation for
         * @return the rotation you would have to set to make sure the requested side index 
         *         becomes an exact match with the given up vector.
         *         
         * Example Usage: transform.rotation = GetComponent<DieSides>.GetWorldRotationFor (x);
         */
        public Quaternion GetWorldRotationFor (int pSideIndex, Vector3 pUpVector)
        {
            Vector3 worldVectorToMatch = (_matchType == MatchType.MATCH_TOP_SIDE ? pUpVector : -pUpVector);
            Vector3 worldNormalToMatch = transform.TransformDirection(GetDieSide(pSideIndex).normal);
            return Quaternion.FromToRotation(worldNormalToMatch, worldVectorToMatch) * transform.rotation;
        }

		private void OnDrawGizmo()
		{
			Vector3[] vertices = GetComponent<MeshFilter>().sharedMesh.vertices;

			float minX = float.PositiveInfinity;
			float minY = float.PositiveInfinity;
			float minZ = float.PositiveInfinity;

			float maxX = float.NegativeInfinity;
			float maxY = float.NegativeInfinity;
			float maxZ = float.NegativeInfinity;

			for (int i = 0; i < vertices.Length;i++)
			{
				Vector3 point = vertices[i];
				point = transform.TransformPoint(point);

				minX = Mathf.Min(minX, point.x);
				minY = Mathf.Min(minY, point.y);
				minZ = Mathf.Min(minZ, point.z);

				maxX = Mathf.Max(maxX, point.x);
				maxY = Mathf.Max(maxY, point.y);
				maxZ = Mathf.Max(maxZ, point.z);
			}

			Vector3 center = new Vector3(0.5f*(maxX+minX), 0.5f*(maxY+minY), 0.5f*(maxZ+minZ));
			Vector3 size = new Vector3(maxX-minX, maxY-minY, maxZ-minZ);

			Gizmos.DrawCube(center, size);
		}


	}

}