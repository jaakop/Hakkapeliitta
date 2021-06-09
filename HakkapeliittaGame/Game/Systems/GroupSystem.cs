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
        private Entity groupEntity;

        private int rowLenght;
        private float spacing;

        public GroupSystem(Entity groupE, int rowL, float spacing)
        {
            groupEntity = groupE;

            rowLenght = rowL;
            this.spacing = spacing;
        }

        public void Call(ECSManager manager, int deltaTime)
        {
            GroupComponent group = manager.ComponentManager.GetComponent<GroupComponent>(groupEntity);

            //Calculate group dimensions
            float rowXMiddle = (float)Math.Floor((decimal)(1 + rowLenght) / 2);
            float rowYMiddle = (float)Math.Floor(((float)Math.Ceiling((decimal)group.Members.Count / rowLenght) + 1) / 2);

            int groupHeight = (int)Math.Ceiling((decimal)(group.Members.Count + 1) / rowLenght);

            //Move the middle member to last, 'cause there the leader will be placed
            Entity middleMember = group.Members[Convert.ToInt32((rowXMiddle - 1) + (rowLenght * (rowYMiddle - 1)))];
            group.Members.Add(middleMember);

            for (int i = 0; i < groupHeight; i++)
            {
                for (int j = 0; j < rowLenght; j++)
                {
                    int memberIndex = j + (rowLenght * i);
                    Vector pos = manager.ComponentManager.GetComponent<MovementComponent>(group.LeaderEntity).target;

                    if (memberIndex >= group.Members.Count)
                        break;

                    //Check if current member is the middle member. If so we skip it
                    if (i == rowYMiddle - 1 && j == rowXMiddle - 1)
                    {
                        j += 1;
                        memberIndex = j + (rowLenght * i);
                    }

                    //Set the position
                    pos.X += spacing * (j - (rowXMiddle - 1));
                    pos.Y += spacing * (i - (rowYMiddle - 1));

                    //Adjust the last row to be centered
                    if (i == groupHeight - 1 && group.Members.Count % rowLenght != 0)
                    {
                        int membersMissing = -(group.Members.Count - (i + 1) * rowLenght);
                        pos.X += spacing * (membersMissing - (rowLenght - membersMissing >= rowXMiddle ? 1 : 2));
                        if ((rowLenght - membersMissing) % 2 == 0) pos.X += spacing / 2;
                    }

                    //Apply the position
                    MovementComponent mvC = manager.ComponentManager.GetComponent<MovementComponent>(group.Members[memberIndex]);
                    mvC.target = pos;

                    manager.ComponentManager.UpdateComponent(group.Members[memberIndex], mvC);
                }
            }

            group.Members.RemoveAt(group.Members.Count - 1);
        }
    }
}
