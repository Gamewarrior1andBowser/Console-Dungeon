using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Dunegon {
    internal class Character
    {
        public string Race { get; }
        public string Name { get; }
        public string Class { get; }
        public int HP { get; private set; }
        public int MaxHP { get; private set; }
        public int MP { get; set; }
        public int MaxMP { get; private set; }
        public int Atk { get; private set; }
        public int Def { get; private set; }
        public int Icon { get; set; }
        public string SpellName { get; private set; }
        public int SpellCost { get; private set; }

        public Character(string race, string type, int icon, string name)
        {
            Name = name;
            Race = race;
            Class = type;
            Icon = icon;
            SetClass();
        }

        public Character(string race, string type, int icon) {
            Name = race;
            Race = race;
            Class = type;
            Icon = icon;
            SetClass();
        }

        private void SetClass() {
            if (Class == "Hero") {
                HP = 75;
                MaxHP = 75;
                MP = 75;
                MaxMP = 75;
                Atk = 6;
                Def = 4;
                SpellName = "Victory's Fury";
                SpellCost = 25;
            } else if (Class == "Knight") {
                HP = 100;
                MaxHP = 100;
                MP = 50;
                MaxMP = 50;
                Atk = 5;
                Def = 5;
                SpellName = "Spin Slash";
                SpellCost = 10;
            }
            else if (Class == "Mage") {
                HP = 50;
                MaxHP = 50;
                MP = 100;
                MaxMP = 100;
                Atk = 4;
                Def = 6;
                if (Name == "Sayrin") {
                    SpellName = "Group Heal";
                    SpellCost = 10;
                } else {
                    SpellName = "Fireball";
                    SpellCost = 10;
                }
            }
            else if (Class == "Boss") {
                HP = 150;
                MaxHP = 150;
                MP = 150;
                MaxMP = 150;
                Atk = 7;
                Def = 7;
                SpellName = "Ultra Heal";
                SpellCost = 50;
            } else {
                HP = 25;
                MaxHP = 25;
                MP = 25;
                MaxMP = 25;
                Atk = 2;
                Def = 2;
            }
        }

        public void Heal(int amount) {
            HP += amount;
            if (HP > MaxHP) {
                HP = MaxHP;
            }
        }

        public void Hurt(int amount) {
            amount *= 3;
            amount -= Def;
            if (amount < 0) {
                amount = 0;
            }
            HP -= amount;
            if (HP < 0) {
                HP = 0;
            }
        }

        public void CastSpell(ref Character target) {
            if (SpellName == "Group Heal") {
                target.Heal(target.MaxHP/2);
            } else if (SpellName == "Ultra Heal") {
                target.Heal(target.MaxHP);
            } else if (SpellName == "Fireball" || SpellName == "Spin Slash") {
                target.Hurt(Atk * 2);
            } else if (SpellName == "Victory's Fury") {
                target.Hurt(Atk * 3);
            }
        }
    }
}
