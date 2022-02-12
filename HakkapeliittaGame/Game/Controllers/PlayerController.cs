using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MGPhysics;

using ReeGame.Components;
using ReeGame.Systems;

namespace ReeGame.Controllers
{
    public class PlayerController
    {
        private ECSManager _manager;
        private Entity _selectedGroup;
        public ArmyController ArmyController { get; set; }

        public Entity SelectedGroup { get { return _selectedGroup; } }

        public GroupComponent SelectedGroupComponent { 
            get
            {
                return _manager.ComponentManager.GetComponent<GroupComponent>(SelectedGroup);
            }
            set
            {
                _manager.ComponentManager.UpdateComponent(SelectedGroup, value);
            }
        }

        public PlayerController(ECSManager manager, ArmyController armyController)
        {
            _manager = manager;
            ArmyController = armyController;
        }

        public Entity SelectGroup(int groupIndex)
        {
            var groups = ArmyController.GetGroups();
            if (groupIndex < 0 || groups.Count <= groupIndex) throw new IndexOutOfRangeException();

            var group = groups[groupIndex];

            _selectedGroup = group;
            return group;
        }
    }
}
