using System.Collections.Generic;

namespace Dreamers.InventorySystem.AbilitySystem
{
    public class SpellNode
    {
        public List<int> IndexOfConnectedNodes;
    }

    
[System.Serializable]
    public class Spell
    {
        public SpellNode[] SpellNodes = new SpellNode[13];

        public void CreateDisplay(Spell spellDisplay)
        {
            
        }
    }
}