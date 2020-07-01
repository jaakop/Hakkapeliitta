﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MGPhysics;
using MGPhysics.Components;

namespace ReeGame.Components
{
    public struct GroupComponent : IComponent
    {
        public Entity LeaderEntity { get; set; }
        public List<Entity> Members { get; set; }

        public GroupComponent(Entity leaderEntity)
        {
            LeaderEntity = leaderEntity;
            Members = new List<Entity>();
        }

        /// <summary>
        /// Check if entity is a member or leader
        /// </summary>
        /// <param name="entity">Entity to check</param>
        /// <returns></returns>
        public bool ContainsEntity(Entity entity)
        {
            if (LeaderEntity == entity)
                return true;

            return Members.Contains(entity);
        }

        /// <summary>
        /// Removes entity from groups members
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        public void RemoveMember(Entity entity)
        {
            for(int i = 0; i < Members.Count; i++)
            {
                if (Members[i] == entity)
                    Members.RemoveAt(i);
            }
        }

        /// <summary>
        /// Calculates group positions
        /// </summary>
        /// <param name="leaderEntityTrans">Transform, which points the center of formation</param>
        /// <param name="rowLenght">Number of entities in a row</param>
        /// <param name="spacing">Spacing between entities</param>
        /// <returns>Dictionary with the positions</returns>
        public Dictionary<Entity, Vector> CalculateGroupMemberPositions(Transform leaderEntityTrans, int rowLenght, float spacing)
        {
            Dictionary<Entity, Vector> positions = new Dictionary<Entity, Vector>();

            List<Entity> dummyMembers = Members;

            //Casting to decimal & float is just to make everything smooth with Math shit
            float rowXMiddle = (float)Math.Floor((decimal)(1 + rowLenght) / 2);
            float rowYMiddle = (float)Math.Floor(((float)Math.Ceiling((decimal)Members.Count / rowLenght) + 1) / 2);

            dummyMembers.Add(dummyMembers[Convert.ToInt32((rowXMiddle -1) + (rowLenght * (rowYMiddle-1)))]);

            for (int i = 0; i < Math.Ceiling((decimal)dummyMembers.Count / rowLenght); i++)
            {
                for (int j = 0; j < rowLenght; j++)
                {
                    int memberIndex = j + (rowLenght * i);
                    Vector pos = leaderEntityTrans.Position;

                    if (memberIndex >= dummyMembers.Count)
                        break;

                    if (i == rowYMiddle -1 && j == rowXMiddle - 1)
                    {
                        j+=1;
                        memberIndex = j + (rowLenght * i);
                    }

                    pos.X += spacing * (j - (rowXMiddle - 1));
                    pos.Y += spacing * (i - (rowYMiddle - 1));

                    positions.Add(dummyMembers[memberIndex], pos);
                }
            }

            /*
            Console.WriteLine("Member count: " + Members.Count);
            Console.WriteLine("Row middle ({0}, {1})", rowXMiddle, rowYMiddle);
            Console.WriteLine("Generating {0} rows for group leader {1}",Math.Ceiling((decimal)Members.Count / rowLenght), LeaderEntity.Key);
            */

            return positions;
        }
    }
}
