using System.Collections.Specialized;
using System.Net.NetworkInformation;
using static System.Collections.Specialized.BitVector32;

namespace Console_Dunegon {
    internal class Room {
        private readonly string[] icons = new string[] { "         ", "/\\ /\\ /\\ ", "|| || || ", "-- -- -- ", "[ ][ ][ ]", "[]* * * *", "* * * *[]", " * * * * ",
            "  {___}  ", "|^|^|^|^|", };
        private readonly string[] characters = new string[] { "         ", "\\(`'w'`)o", "\\(`^w^`)o", "\\(`>_<`)o", " '( ^o^ )' ", " ( U_U ) ", " ( UwU ) ",
            " ( X_X ) ", " ( T_T ) ", " ( O_O ) ", " ( >.< ) ", "(* ^_- *)", "\\(/-T-\\)O", "i(.^_^.) ", "i(.>_<.) ", " (  ^_-)d", "b(-_^  ) ", "( '_')'_')",
            "<( AwA )>", "<(  AwA)>", "O(/-T-\\)/", "m('I_I')m", " ( VmV )r" };

        private string[,] Grid;
        private Dictionary<string, bool> Doors = new Dictionary<string, bool>();
        private string EventType = "";
        private bool ClearedEvent = false;
        private Character Monster;

        public delegate bool DoorCheckHandler(bool hasDoor);
        public event DoorCheckHandler DoorCheckEvent;

        public delegate void TriggerHandler(string EventType, ref Character monster, ref bool IsCleared);
        public event TriggerHandler TriggerEvent;
 

        public Room() {
            Grid = new string[9, 9];
            for (int i = 0; i < 9; i++) {
                for (int j = 0; j < 9; j++) {
                    Grid[i, j] = icons[0];
                }
            }
        }

        public void AddIcon(int y, int x, int index) {
            Grid[y, x] = icons[index];
        }

        public void RemoveIcon(int y, int x) {
            Grid[y, x] = icons[0];
        }

        public string GetIcon(int index) {
            return icons[index];
        }

        public void AddChar(int y, int x, int index) {
            Grid[y, x] = characters[index];
        }
        public void RemoveChar(int y, int x) {
            Grid[y, x] = characters[0];
        }
        public string GetChar(int index) {
            return characters[index];
        }

        public void Trigger() {
            if (EventType == "Loot") {
                if (!ClearedEvent) {
                    Console.WriteLine("Leo: \"Yo! We found some loot!\"");
                    Console.WriteLine("(Score increased by 2500!)");
                    Console.WriteLine("");
                }
            } else if (EventType == "Trap") {
                Console.WriteLine("Leo: \"Owch! let's try to avoid this room if we can...\"");
                Console.WriteLine("");
            } else if (EventType == "Monster") {
                if (ClearedEvent && Monster.HP > 0) {
                    Grid[2, 4] = characters[Monster.Icon];
                    Grid[4, 4] = characters[2];
                    Console.WriteLine($"{Monster.Name}: \"If you're lost, try going back to the dungeon entrance.\"");
                    Console.WriteLine($"{Monster.Name}: \"It's at the the bottom center of your minimap.\"");
                    Console.WriteLine("");
                } else if (Monster.HP <= 0) {
                    Grid[2, 4] = icons[0];
                    Grid[4, 4] = characters[1];
                    
                }
            }
            TriggerEvent(EventType, ref Monster, ref ClearedEvent);
        }

        public void SetEvent(string eventType) {
            EventType = eventType;
            Grid[4, 4] = characters[1];
            if (EventType == "Loot") {
                Grid[2, 4] = icons[8];
                Grid[4, 4] = characters[2];
            } else if (EventType == "Trap") {
                Grid[2, 3] = icons[9];
                Grid[2, 4] = icons[9];
                Grid[2, 5] = icons[9];
                Grid[3, 2] = icons[9];
                Grid[4, 2] = icons[9];
                Grid[5, 2] = icons[9];
                Grid[3, 6] = icons[9];
                Grid[4, 6] = icons[9];
                Grid[5, 6] = icons[9];
                Grid[6, 3] = icons[9];
                Grid[6, 4] = icons[9];
                Grid[6, 5] = icons[9];
                Grid[4, 4] = characters[3];
            } else if (EventType == "Monster") {
                NewMonster();
            } else if (EventType == "Boss") {
                Grid[2, 2] = icons[4];
                Grid[3, 2] = icons[5];
                Grid[4, 2] = icons[5];
                Grid[5, 2] = icons[5];
                Grid[6, 2] = icons[4];
                Grid[2, 3] = icons[4];
                Grid[3, 3] = icons[6];
                Grid[4, 3] = icons[6];
                Grid[5, 3] = icons[6];
                Grid[6, 3] = icons[4];
                Grid[4, 4] = icons[0];
                Grid[2, 6] = characters[1];
                Grid[4, 6] = characters[13];
                Grid[6, 6] = characters[12];
                Monster = new Character("Nether Golem", "Boss", 21);
            }
        }

        public void BuildCave(bool northDoor, bool eastDoor, bool southDoor, bool westDoor, string EventType) {
            SetEvent(EventType);
            Doors["North"] = northDoor;
            Doors["East"] = eastDoor;
            Doors["South"] = southDoor;
            Doors["West"] = westDoor;
            if (northDoor) {
                Grid[0, 1] = icons[3];
                Grid[0, 2] = icons[3];
                Grid[0, 6] = icons[3];
                Grid[0, 7] = icons[3];
            } else {
                for (int i = 1; i < 8; i++) {
                    Grid[0, i] = icons[3];
                }
            }
            if (eastDoor) {
                Grid[0, 8] = icons[2];
                Grid[1, 8] = icons[2];
                Grid[2, 8] = icons[3];
                Grid[6, 8] = icons[3];
                Grid[7, 8] = icons[2];
                Grid[8, 8] = icons[2];
            } else {
                for (int i = 1; i < 8; i++) {
                    Grid[i, 8] = icons[2];
                }
            }
            if (southDoor) {
                Grid[8, 1] = icons[3];
                Grid[8, 2] = icons[3];
                Grid[8, 6] = icons[3];
                Grid[8, 7] = icons[3];
            } else {
                for (int i = 1; i < 8; i++) {
                    Grid[8, i] = icons[3];
                }
            }
            if (westDoor) {
                Grid[0, 0] = icons[2];
                Grid[1, 0] = icons[2];
                Grid[2, 0] = icons[3];
                Grid[6, 0] = icons[3];
                Grid[7, 0] = icons[2];
                Grid[8, 0] = icons[2];
            } else {
                for (int i = 1; i < 8; i++) {
                    Grid[i, 0] = icons[2];
                }
            }
            Grid[0, 0] = icons[2];
            Grid[0, 8] = icons[2];
            Grid[8, 0] = icons[2];
            Grid[8, 8] = icons[2];
        }

        public void BuildForest(bool northDoor, bool eastDoor, bool southDoor, bool westDoor, string EventType) {
            SetEvent(EventType);
            Doors["North"] = northDoor;
            Doors["East"] = eastDoor;
            Doors["South"] = southDoor;
            Doors["West"] = westDoor;
            if (northDoor) {
                Grid[0, 1] = icons[1];
                Grid[0, 2] = icons[1];
                Grid[0, 6] = icons[1];
                Grid[0, 7] = icons[1];
            } else {
                for (int i = 1; i < 8; i++) {
                    Grid[0, i] = icons[1];
                }
            }
            if (eastDoor) {
                Grid[0, 8] = icons[1];
                Grid[1, 8] = icons[1];
                Grid[2, 8] = icons[1];
                Grid[6, 8] = icons[1];
                Grid[7, 8] = icons[1];
                Grid[8, 8] = icons[1];
            } else {
                for (int i = 1; i < 8; i++) {
                    Grid[i, 8] = icons[1];
                }
            }
            if (southDoor) {
                Grid[8, 1] = icons[1];
                Grid[8, 2] = icons[1];
                Grid[8, 6] = icons[1];
                Grid[8, 7] = icons[1];
            } else {
                for (int i = 1; i < 8; i++) {
                    Grid[8, i] = icons[1];
                }
            }
            if (westDoor) {
                Grid[0, 0] = icons[1];
                Grid[1, 0] = icons[1];
                Grid[2, 0] = icons[1];
                Grid[6, 0] = icons[1];
                Grid[7, 0] = icons[1];
                Grid[8, 0] = icons[1];
            } else {
                for (int i = 1; i < 8; i++) {
                    Grid[i, 0] = icons[1];
                }
            }
            Grid[0, 0] = icons[1];
            Grid[0, 8] = icons[1];
            Grid[8, 0] = icons[1];
            Grid[8, 8] = icons[1];
        }

        public bool CheckPath(string direction) {
            return Doors[direction];
        }

        public bool CheckDirection(string direction) {
            if (direction == "North" && Doors["South"] == true) {
                return DoorCheckEvent(true);
            } else if (direction == "East" && Doors["West"] == true) {
                return DoorCheckEvent(true);
            } else if (direction == "South" && Doors["North"] == true) {
                return DoorCheckEvent(true);
            } else if (direction == "West" && Doors["East"] == true) {
                return DoorCheckEvent(true);
            }
            return DoorCheckEvent(false);
        }

        public void NewMonster() {
            string[] races = ["Gohst", "Skull Warrior", "Blocking Bros"];
            string[] types = ["Mage", "Knight", "Hero"];
            int[] icons = [18, 22, 17];
            int id = new Random().Next(0, 3);
            Monster = new Character(races[id], types[id], icons[id]);
        }

        public void PrintMap() {
            for (int i = 0; i < 9; i++) {
                for (int j = 0; j < 9; j++) {
                    if (Grid[i, j] == icons[1]) {
                        Console.ForegroundColor = ConsoleColor.Green;
                    } else if(Grid[i, j] == icons[2] || Grid[i, j] == icons[3]) {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    } 
                    if (j == 8) {
                        Console.WriteLine(Grid[i, j]);
                    } else {
                        Console.Write(Grid[i, j]);
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}
