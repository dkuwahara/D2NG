using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG.D2GS
{
    public class Self : Player
    {

        private Skill[] _skill = new Skill[2];

        internal Self(AssignPlayerPacket assignPlayer) : base(assignPlayer)
        {
        }

        internal void SetSkill(Hand hand, Skill skill) => _skill[(int)hand] = skill;

        public Skill GetSkill(Hand hand) => _skill[(int)hand];
    }
}
