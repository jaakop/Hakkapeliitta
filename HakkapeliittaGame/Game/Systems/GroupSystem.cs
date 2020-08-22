using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ReeGame.Components;
using MGPhysics;
using MGPhysics.Components;

namespace ReeGame.Systems
{
    public static class GroupSystem
    {
        /// <summary>
        /// Check if entity is a member or leader
        /// </summary>
        /// <param name="entity">Entity to check</param>
        /// <param name="group">Affected Group</param>
        /// <returns>True if group contains entity and false if not</returns>
        public static bool ContainsEntity(Entity entity, GroupComponent group)
        {
            if (group.LeaderEntity == entity)
                return true;

            return group.Members.Contains(entity);
        }

        /// <summary>
        /// Removes entity from groups members
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        /// <param name="group">Affected Group</param>
        public static void RemoveMember(Entity entity, GroupComponent group)
        {
            for (int i = 0; i < group.Members.Count; i++)
            {
                if (group.Members[i] == entity)
                    group.Members.RemoveAt(i);
            }
        }
        /// <summary>
        /// Calculates group positions
        /// </summary>
        /// <param name="leaderEntityPos">Position, which points the center of formation</param>
        /// <param name="rowLenght">number of entities in a row</param>
        /// <param name="spacing">Spacing between entities</param>'
        /// <param name="group">Calculated Group</param>
        /// <returns>Dictionary with the positions</returns>
        public static Dictionary<Entity, Vector> CalculateGroupMemberPositions(Vector leaderEntityPos, int rowLenght, float spacing, GroupComponent group)
        {
            Dictionary<Entity, Vector> positions = new Dictionary<Entity, Vector>();

            float rowXMiddle = (float)Math.Floor((decimal)(1 + rowLenght) / 2);
            float rowYMiddle = (float)Math.Floor(((float)Math.Ceiling((decimal)group.Members.Count / rowLenght) + 1) / 2);

            //Move the middle member to last, 'cause there the leader will be placed
            Entity middleMember = group.Members[Convert.ToInt32((rowXMiddle - 1) + (rowLenght * (rowYMiddle - 1)))];
            group.Members.Add(middleMember);

            for (int i = 0; i < Math.Ceiling((decimal)group.Members.Count / rowLenght); i++)
            {
                for (int j = 0; j < rowLenght; j++)
                {
                    int memberIndex = j + (rowLenght * i);
                    Vector pos = leaderEntityPos;

                    if (memberIndex >= group.Members.Count)
                        break;

                    if (i == rowYMiddle - 1 && j == rowXMiddle - 1)
                    {
                        j += 1;
                        memberIndex = j + (rowLenght * i);
                    }

                    pos.X += spacing * (j - (rowXMiddle - 1));
                    pos.Y += spacing * (i - (rowYMiddle - 1));

                    positions.Add(group.Members[memberIndex], pos);
                }
            }

            group.Members.RemoveAt(group.Members.Count - 1);

            return positions;
        }
    }
}
