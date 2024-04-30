using AutoLevelMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts._Game.PlayerManagement
{
    [CreateAssetMenu(fileName = nameof(PlayerTeamColors), menuName = Global.AssetMenuPathStart.gameData + "/" + nameof(PlayerTeamColors))]
    // TODO Switch it from materials to colors
    public class PlayerTeamColors : ScriptableObject 
    {
        public Color[] teamColors;
    }
}
