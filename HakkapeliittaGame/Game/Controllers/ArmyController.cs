using MGPhysics;
using ReeGame.Components;
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

        public Entity AddNewUnitGroup(int numberOfSoldiers)
        {
            Entity groupEntity = _manager.EntityManager.CreateEntity();

            List<Entity> members = new List<Entity>();
            for (int i = 0; i < numberOfSoldiers - 1; i++)
            {
                members.Add(_manager.EntityManager.CreateEntity());
            }

            GroupComponent groupComponent = new GroupComponent()
            {
                LeaderEntity = _manager.EntityManager.CreateEntity(),
                Members = members
            };

            try
            {
                _manager.ComponentManager.GetComponentArray<GroupComponent>().Array.Add(groupEntity, groupComponent);
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
    }
}