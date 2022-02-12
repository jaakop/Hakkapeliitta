using MGPhysics;
using ReeGame;
using ReeGame.Components;
using ReeGame.Systems;
using System;
using System.Collections.Generic;

namespace ReeGame.Controllers
{
    public class ArmyController
    {
        private readonly ECSManager _manager;
        private readonly List<Entity> _groups;

        public ArmyController(ECSManager manager)
        {
            _manager = manager;
            _groups = new List<Entity>();
        }

        public Entity AddNewUnitGroup(int numberOfSoldiers, Vector sizeOfUnits, float movementSpeed = 5, int variance = 0, int rowLength = 1, float spacing = 0)
        {
            Entity groupEntity = _manager.EntityManager.CreateEntity();

            List<Entity> members = new List<Entity>();
            for (int i = 0; i < numberOfSoldiers - 1; i++)
            {
                members.Add(_manager.EntityManager.CreateEntity());
                Common.CreatePalikka(_manager, members[i], new Vector(0, 0), sizeOfUnits);
                Common.CreateMovement(_manager, members[i], new Vector(0, 0), movementSpeed + Common.RND.Next(0, variance));
            }

            GroupComponent groupComponent = new GroupComponent(_manager.EntityManager.CreateEntity())
            {
                Members = members,
                RowLenght = rowLength,
                Spacing = spacing,
            };

            Common.CreatePalikka(_manager, groupComponent.LeaderEntity, new Vector(0, 0), sizeOfUnits * 1.20f);
            Common.CreateMovement(_manager, groupComponent.LeaderEntity, new Vector(0, 0), movementSpeed + Common.RND.Next(0, variance));

            try
            {
                _manager.ComponentManager.GetComponentArray<GroupComponent>().Array.Add(groupEntity, groupComponent);
                _groups.Add(groupEntity);
                return groupEntity;
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error creating a unit group", ex);
            }
        }

        public Entity GetLeaderEntity(Entity group)
        {
            return _manager.ComponentManager.GetComponent<GroupComponent>(group).LeaderEntity;
        }

        public List<Entity> GetMembers(Entity group)
        {
            return _manager.ComponentManager.GetComponent<GroupComponent>(group).Members;
        }

        public List<Entity> GetGroups()
        {
            return _groups;
        }

        public void MoveGroup(Entity group, Vector position)
        {
            //Move leader
            Entity leader = _manager.ComponentManager.GetComponent<GroupComponent>(group).LeaderEntity;
            MovementComponent mvC = _manager.ComponentManager.GetComponent<MovementComponent>(leader);
            mvC.target = position;

            _manager.ComponentManager.UpdateComponent(leader, mvC);

            //Move members
            Common.CallOneTimeSystems(_manager, new GroupSystem(group));
        }
    }
}