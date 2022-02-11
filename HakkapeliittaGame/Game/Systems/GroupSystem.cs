using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ReeGame.Components;
using MGPhysics;

namespace ReeGame.Systems
{
    public class GroupSystem : ISystem
    {
        private readonly Entity groupEntity;

        public GroupSystem(Entity groupE)
        {
            groupEntity = groupE;
        }

        public void Call(ECSManager manager, int deltaTime)
        {
            GroupComponent group = manager.ComponentManager.GetComponent<GroupComponent>(groupEntity);

            //Check if no members
            if (group.Members.Count < 1)
                return;

            //Check if rowlenght is greater than the amount of members, if so then make the rowlenght match the amount of members
            if(group.RowLenght > group.Members.Count + 1) group.RowLenght = group.Members.Count + 1;

            //Calculate group dimensions
            float rowXMiddle = (float)Math.Floor((decimal)(1 + group.RowLenght) / 2);
            float rowYMiddle = (float)Math.Floor(((float)Math.Ceiling((decimal)group.Members.Count / group.RowLenght) + 1) / 2);

            int groupHeight = (int)Math.Ceiling((decimal)(group.Members.Count + 1) / group.RowLenght);

            //Move the middle member to last, 'cause there the leader will be placed
            Entity middleMember = group.Members[Convert.ToInt32((rowXMiddle - 1) + (group.RowLenght * (rowYMiddle - 1)))];
            group.Members.Add(middleMember);

            for (int i = 0; i < groupHeight; i++)
            {
                for (int j = 0; j < group.RowLenght; j++)
                {
                    int memberIndex = j + (group.RowLenght * i);
                    Vector pos = manager.ComponentManager.GetComponent<MovementComponent>(group.LeaderEntity).target;

                    if (memberIndex >= group.Members.Count)
                        break;

                    //Check if current member is the middle member. If so we skip it
                    if (i == rowYMiddle - 1 && j == rowXMiddle - 1)
                    {
                        j += 1;
                        memberIndex = j + (group.RowLenght * i);
                    }

                    //Set the position
                    Vector adjustmentVector = new Vector();
                    adjustmentVector.X += group.Spacing * (j - (rowXMiddle - 1)) + (group.RowVariance * (float)(Common.RND.NextDouble() * 2 - 1));
                    adjustmentVector.Y += group.Spacing * (i - (rowYMiddle - 1)) + (group.RowVariance * (float)(Common.RND.NextDouble() * 2 - 1));

                    //Adjust the last row to be centered
                    if (i == groupHeight - 1 && group.Members.Count % group.RowLenght != 0)
                    {
                        int membersMissing = -(group.Members.Count - (i + 1) * group.RowLenght);
                        adjustmentVector.X += group.Spacing * (membersMissing / 2);

                        //Adjust members if they need to be "between" the member positions
                        if ((group.RowLenght - (group.RowLenght - membersMissing)) % 2 != 0)
                            adjustmentVector.X += group.Spacing / 2;
                    }

                    //Apply angle
                    adjustmentVector = Vector.RotateVector(adjustmentVector, group.Direction);

                    //Apply the position
                    pos += adjustmentVector;
                    MovementComponent mvC = manager.ComponentManager.GetComponent<MovementComponent>(group.Members[memberIndex]);
                    mvC.target = pos;

                    manager.ComponentManager.UpdateComponent(group.Members[memberIndex], mvC);
                }
            }

            group.Members.RemoveAt(group.Members.Count - 1);
        }
    }
}
