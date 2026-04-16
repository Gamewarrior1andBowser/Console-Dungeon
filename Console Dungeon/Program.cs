using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Console_Dunegon {
    internal class Program {
        public static Character Leo = new Character("Eternian Cat", "Hero", 1, "Leo");
        public static Character TubaKnight = new Character("Musical", "Knight", 12, "Tuba Knight");
        public static Character Sayrin = new Character("Eternian Angel", "Mage", 13, "Sayrin");
        public static Room[,] Map;
        public static int Score = 100000;
        static bool IsPaused = false;
        static bool SameRoom = false;
        static int playerX = 4;
        static int playerY = 8;

        static void Main(string[] args) {
            BootUp();
        }

        static void BootUp() {
            Room icons = new Room();
            string userInput = "0";
            while (userInput.ToLower() != "z" && userInput.ToLower() != "x") { //&& userInput.ToLower() != "c"
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine($"The Hero of Avalon! [Beta]    {icons.GetChar(1)}");
                Console.WriteLine("");
                Console.WriteLine("(Z): New Game,");
                //Console.WriteLine("(C): Run Map Test,");
                Console.WriteLine("(X): Exit to Desktop,");
                userInput = Console.ReadKey().KeyChar.ToString();
                Console.WriteLine("");
                if (userInput.ToLower() == "z") {
                    NewGame();
                //} else if (userInput.ToLower() == "c") { 
                    //TestGame();
                } else if (userInput.ToLower() == "x") {
                    Console.WriteLine("Shutting Down... Have a good rest of your day!");
                }
            }
        }

        static void NewGame() {
            Map = BuildRandomDungeon();
            string userInput = "";
            Console.Clear();
            Console.WriteLine("Avalon, the Land between worlds. Was once a peaceful and sacred place, " +
                "but recently... a dark force known as the Lord of Corruption has started infecting it.");
            Console.WriteLine("In the face of this threat, a young Cat-Human named Leo Northstar. " +
                "Volunteered to leave Avalon and his home to find information on the threat.");
            Console.WriteLine("However... on his journey, Leo entered the hidden tunnels under a Graveyard. " +
                "With no clue where he's going, him and his friends now needs to find the Relic hidden within!");
            ContinueDialouge();
            Map = BuildRandomDungeon();
            Explore();
        }

        static bool CheckPartyDefeat() {
            if (Leo.HP == 0 && TubaKnight.HP == 0 && Sayrin.HP == 0) {
                return true;
            } else {
                return false;
            }
        }

        static void PlayEnding() {
            Console.WriteLine("Thanks to the help of his friends, Leo finally reached the Nether portal at the end of the Graveyard.");
            Console.WriteLine("Leo said goodbye to Sayrin and Tuba Knight, before Leo parted ways from his new friends and entered the portal.");
            Console.WriteLine("However... Leo's adventure doesn't end here. The Nether Usurper must be stopped and Nydam must be freed!");
            Console.WriteLine("");
            Console.WriteLine("                Leo will return... In the Hero of Avalon II: Trial by Fire!");
            Console.WriteLine("");
            Console.WriteLine("                               THE END! (Of the Beta)");
            Console.WriteLine($"                                  Score: {Score}");
            Console.WriteLine("");
        }

        static void ContinueDialouge() {
            Console.Write("(Press any key to continue...) ");
            Console.ReadKey();
            Console.Clear();
        }
        
        static void TestGame() {
            Map = BuildRandomDungeon();
            for (int y = 0; y < 9; y++) {
                Map[y, 0].PrintMap();
            }
            Console.WriteLine("");
            string userInput = "";
            while (userInput != "Escape") {
                Console.WriteLine("Input test (Press Esc to exit):");
                userInput = Console.ReadKey().Key.ToString();
                Console.WriteLine(userInput);
            }
        }

        static Room NewPresetRoom(string roomType, int CharacterIcon) {
            Room targetRoom = new Room();
            targetRoom.AddChar(4, 4, CharacterIcon);
            if (roomType == "Fork") {
                targetRoom.BuildCave(false, true, true, true, "");
            } else if (roomType == "Straight") {
                targetRoom.BuildCave(true, false, true, false, "");
            } else if (roomType == "Exit") {
                targetRoom.BuildForest(false, false, true, false, "");
            } else if (roomType == "Entrance") {
                targetRoom.BuildForest(true, false, false, false, "");
            } else if (roomType == "Secret2") {
                targetRoom.BuildCave(true, true, false, false, "");
            } else if (roomType == "Secret1") {
                targetRoom.BuildCave(true, false, false, true, "");
            } else if (roomType == "Four way") {
                targetRoom.BuildCave(true, true, true, true, "");
            }
            return targetRoom;
        }

        static void ShowMiniMap() {
            for (int i = 0; i < 9; i++) {
                for (int j = 0; j < 9; j++) {
                    if (j == playerX && i == playerY) {
                        Console.ForegroundColor = ConsoleColor.Green;
                    } else {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    }
                    if (j == 8) {
                        Console.WriteLine("[]");
                    } else {
                        Console.Write("[]");
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        static void Explore() {
            string userInput = "";
            bool victory = false;
            while (!CheckPartyDefeat() && !victory) {
                Console.Clear();
                Room location = Map[playerX, playerY];
                location.TriggerEvent += EventTrigger;
                if (!SameRoom) {
                    SameRoom = true;
                    location.Trigger();
                }
                if (!(playerY == 0 && playerX == 0)) {
                    location.PrintMap();
                    Console.WriteLine();
                    Console.WriteLine("(Arrow Keys): Move arround,");
                    Console.WriteLine("(Z): Pause and open Party Menu,");
                    Console.WriteLine("(X): Check the Minimap");
                    userInput = Console.ReadKey().Key.ToString();
                    if (userInput.ToLower() == "z") {
                        PauseGame();
                    } else if (userInput.ToLower() == "x") {
                        OpenMiniMap();
                    } else if (userInput == "UpArrow" && location.CheckPath("North")) {
                        playerY -= 1;
                        SameRoom = false;
                    } else if (userInput == "RightArrow" && location.CheckPath("East")) {
                        playerX += 1;
                        SameRoom = false;
                    } else if (userInput == "DownArrow" && location.CheckPath("South")) {
                        playerY += 1;
                        SameRoom = false;
                    } else if (userInput == "LeftArrow" && location.CheckPath("West")) {
                        playerX -= 1;
                        SameRoom = false;
                    }
                } else {
                    victory = true;
                    Console.WriteLine("Leo: The Nether Portal! We made it!");
                    Console.WriteLine("");
                    location.PrintMap();
                    ContinueDialouge();
                }
            }
            if (CheckPartyDefeat()) {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine("           Game Over...         ");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(" /(`>_<`)o /(/-T-\\)O i(.>_<.) ");
                Console.WriteLine("");
                Console.WriteLine($"      Score: {Score}");
                Console.WriteLine("");
            } else {
                PlayEnding();
            }
        }
        
        static void OpenMiniMap() {
            IsPaused = true;
            string userInput = "";
            while (IsPaused) {
                Console.Clear();
                ShowMiniMap();
                Console.WriteLine("(X): Return to the game,");
                userInput = Console.ReadKey().Key.ToString();
                if (userInput.ToLower() == "x") {
                    IsPaused = false;
                }
            }
        }

        static void PauseGame() {
            IsPaused = true;
            string userInput = "";
            while (IsPaused) {
                Console.Clear();
                ShowPartyStats();
                Console.WriteLine("(Z): Exit and Unpause the Game, ");
                Console.WriteLine("(X): Heal Party using 10 MP from Sayrin,");
                userInput = Console.ReadKey().Key.ToString();
                if (userInput.ToLower() == "z") {
                    IsPaused = false;
                } else if (userInput.ToLower() == "x") {
                    if (Sayrin.MP >= 10 && (Leo.HP < Leo.MaxHP || TubaKnight.HP < TubaKnight.MaxHP || Sayrin.HP < Sayrin.MaxHP)) {
                        Sayrin.MP -= 10;
                        Leo.Heal(Leo.MaxHP/2);
                        TubaKnight.Heal(TubaKnight.MaxHP/2);
                        Sayrin.Heal(Sayrin.MaxHP/2);
                    }
                }
            }
        }

        static void ShowPartyStats() {
            Room icons = new Room();
            //Leo
            Console.WriteLine($"|-----------------------------------------|");
            Console.WriteLine($"|                {icons.GetChar(Leo.Icon)}                |");
            Console.WriteLine($"|                                         |");
            Console.WriteLine($"|     {Leo.Name},  Level 3 {Leo.Race} {Leo.Class}     |");
            Console.WriteLine($"|  HP: {Leo.HP}/{Leo.MaxHP},  MP: {Leo.MP}/{Leo.MaxMP}, ATK: {Leo.Atk}, Def: {Leo.Def}  |");
            Console.WriteLine($"|-----------------------------------------|");
            //Tuba Knight
            Console.WriteLine($"|-----------------------------------------|");
            Console.WriteLine($"|                {icons.GetChar(TubaKnight.Icon)}                |");
            Console.WriteLine($"|                                         |");
            Console.WriteLine($"|  {TubaKnight.Name},  Level 3 {TubaKnight.Race} {TubaKnight.Class}   |");
            Console.WriteLine($"| HP: {TubaKnight.HP}/{TubaKnight.MaxHP},  MP: {TubaKnight.MP}/{TubaKnight.MaxMP}, ATK: {TubaKnight.Atk}, Def: {TubaKnight.Def} |");
            Console.WriteLine($"|-----------------------------------------|");
            //Sayrin
            Console.WriteLine($"|-----------------------------------------|");
            Console.WriteLine($"|                {icons.GetChar(Sayrin.Icon)}                |");
            Console.WriteLine($"|                                         |");
            Console.WriteLine($"|   {Sayrin.Name},  Level 3 {Sayrin.Race} {Sayrin.Class}  |");
            Console.WriteLine($"|  HP: {Sayrin.HP}/{Sayrin.MaxHP}, MP: {Sayrin.MP}/{Sayrin.MaxMP}, ATK: {Sayrin.Atk}, Def: {Sayrin.Def} |");
            Console.WriteLine($"|-----------------------------------------|");
            //Extra
            Console.WriteLine("");
            Console.WriteLine($"             Score: {Score}");
            Console.WriteLine("");
        }

        static Room[,] BuildRandomDungeon() {
            Room[,] map = new Room[9, 9];
            
            for (int y = 0; y < 9; y++) {
                for (int x = 0; x < 9; x++) {
                    Room room = new Room();
                    room.DoorCheckEvent += DoorCheck;
                    bool north = true;
                    bool east = true;
                    bool south = true;
                    bool west = true;
                    string[] events = ["", "Loot", "", "Trap", "", "Monster"];
                    string type = events[new Random().Next(0, 6)];
                    if (y == 0) {
                        west = false;
                    } else if (map[y-1, x].CheckDirection("West") == false) {
                        west = false;
                    }
                    if (x == 0) {
                        north = false;
                    } else if (map[y, x-1].CheckDirection("North") == false) {
                        north = false;
                    }
                    if (y == 8 || new Random().Next(0,3) == 0) {
                        east = false;
                    }
                    if (x == 8 || new Random().Next(0, 3) == 0) {
                        south = false;
                    }
                    if (x == 0 && y == 0) {
                        type = "Boss";
                        east = true;
                        south = false;
                    } else if (x == 7 && y == 4) {
                        south = true;
                    } else if (x == 8 && y == 3) {
                        east = true;
                    } else if (x == 8 && y == 4) {
                        north = true;
                        east = true;
                        south = false;
                        west = true;
                        type = "";
                    } else if (x == 8 && y == 5) {
                        west = true;
                    }
                    room.BuildCave(north, east, south, west, type);
                    if (x == 0 && y == 0) {
                        for (int i = 0; i < 8; i++) {
                            room.AddIcon(0, i, 1);
                        }
                        for (int i = 1; i < 8; i++) {
                            room.AddIcon(i, 0, 1);
                        }
                    } else if (x == 8 && y == 4) {
                        room.AddIcon(7, 1, 1);
                        room.AddIcon(7, 2, 1);
                        room.AddIcon(7, 6, 1);
                        room.AddIcon(7, 7, 1);
                        for (int i = 1; i < 8; i++) {
                            room.AddIcon(8, i, 1);
                        }
                    }
                    map[y, x] = room;
                }
            }
            return map;
        }

        private static bool DoorCheck(bool hasDoor) {
            return hasDoor;
        }

        private static void EventTrigger(string type, ref Character monster, ref bool IsCleared) {
            if (type == "Loot") {
                if (!IsCleared) {
                    Score += 2500;
                    IsCleared = true;
                }
            } else if (type == "Trap") {
                DamageParty(5);
            } else if (type == "Monster") {
                if (!IsCleared) {
                    IsCleared = true;
                    StartBattle(ref monster);
                }
            } else if (type == "Boss") {
                if (!IsCleared) {
                    IsCleared = true;
                    StartBattle(ref monster);
                }
            }
        }

        private static void DamageParty(int damage) {
            Leo.Hurt(damage);
            TubaKnight.Hurt(damage);
            Sayrin.Hurt(damage);
        }

        private static void StartBattle(ref Character monster) {
            string userInput = "";
            bool isSpared = false;
            bool canBeSpared = false;
            int option = 0;
            Room battle = new Room();
            Character[] turnOrder = [Leo, TubaKnight, Sayrin, monster];
            int turn = 0;
            battle.AddChar(2, 6, Leo.Icon);
            battle.AddChar(4, 6, TubaKnight.Icon);
            battle.AddChar(6, 6, Sayrin.Icon);
            battle.AddChar(4, 2, monster.Icon);
            while (!CheckPartyDefeat() && !isSpared && !(monster.HP <= 0)) {
                Console.Clear();
                Character initiator = turnOrder[turn];
                battle.PrintMap();
                if (turn != 3 && initiator.HP > 0) {
                    if (monster.HP <= monster.MaxHP / 4 && canBeSpared) {
                        Console.WriteLine($"The *{monster.Name} is scared.");
                        Console.WriteLine($"Use *Spare to end the battle!");
                    } else if (canBeSpared) {
                        Console.WriteLine($"The *{monster.Name} is calm now.");
                        Console.WriteLine($"Use *Spare to end the battle!");
                    } else if (monster.HP <= monster.MaxHP / 4 && monster.Name == "Nether Golem") {
                        Console.WriteLine($"The {monster.Name} is crumbling.");
                    } else {
                        Console.WriteLine($"The {monster.Name} is agitated.");
                    }
                    Console.Write($"{monster.Name} HP: ");
                    for (int i = 1; i < monster.MaxHP/5; i++) {
                        if (i * 5 <= monster.HP) {
                            Console.Write("|");
                        } else {
                            Console.Write("-");
                        }
                    }
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine($"{battle.GetChar(Leo.Icon)}   Leo's HP: {Leo.HP} / {Leo.MaxHP},    MP: {Leo.MP} / {Leo.MaxMP}");
                    Console.WriteLine($"{battle.GetChar(TubaKnight.Icon)}   Tuba Knight's HP: {TubaKnight.HP} / {TubaKnight.MaxHP},    MP: {TubaKnight.MP} / {TubaKnight.MaxMP}");
                    Console.WriteLine($"{battle.GetChar(Sayrin.Icon)}   Sayrin's HP: {Sayrin.HP} / {Sayrin.MaxHP},    MP: {Sayrin.MP} / {Sayrin.MaxMP}");
                    Console.WriteLine("");
                    Console.WriteLine($"(Arrow Keys): Choose an option for {initiator.Name} below, (Z): Select option,");
                    Console.WriteLine("");
                    if (option == 0) {
                        Console.WriteLine("---> Fight");
                    } else {
                        Console.WriteLine("- Fight");
                    }
                    if (option == 1) {
                        Console.WriteLine("---> Act");
                    } else {
                        Console.WriteLine("- Act");
                    }
                    if (option == 2) {
                        Console.WriteLine($"---> {initiator.SpellName} (Costs {initiator.SpellCost} MP)");
                    } else {
                        Console.WriteLine($"- {initiator.SpellName} (Costs {initiator.SpellCost} MP)");
                    }
                    if (canBeSpared) {
                        if (option == 3) {
                            Console.WriteLine("---> *Spare");
                        } else {
                            Console.WriteLine("- *Spare");
                        }
                    } else {
                        if (option == 3) {
                            Console.WriteLine("---> Spare");
                        } else {
                            Console.WriteLine("- Spare");
                        }
                    }
                    Console.WriteLine("");
                    userInput = Console.ReadKey().Key.ToString();
                    Console.WriteLine("");
                    if (userInput.ToLower() == "z") {
                        if (option == 0) {
                            monster.Hurt(initiator.Atk);
                            Console.WriteLine($"{initiator.Name} attacks {monster.Name}!");
                        } else if (option == 1) {
                            if (monster.Name != "Nether Golem") {
                                if (initiator.Name == "Leo") {
                                    Console.WriteLine($"{initiator.Name} starts playing with *{monster.Name}!");
                                } else if (initiator.Name == "Tuba Knight") {
                                    Console.WriteLine($"{initiator.Name} doesn't attack *{monster.Name}!");
                                } else if (initiator.Name == "Sayrin") {
                                    Console.WriteLine($"{initiator.Name} sings a song to calm *{monster.Name}!");
                                }
                                canBeSpared = true;
                            } else {
                                Console.WriteLine($"The {monster.Name} cannot be spared!");
                            }
                        } else if (option == 2) {
                            if (initiator.MP >= initiator.SpellCost) {
                                initiator.MP -= initiator.SpellCost;
                                if (initiator.SpellName == "Group Heal") {
                                    Console.WriteLine($"{initiator.Name} casts {initiator.SpellName} on Leo's party!");
                                    initiator.CastSpell(ref Leo);
                                    initiator.CastSpell(ref TubaKnight);
                                    initiator.CastSpell(ref Sayrin);
                                } else {
                                    Console.WriteLine($"{initiator.Name} casts {initiator.SpellName} on the {monster.Name}!");
                                    initiator.CastSpell(ref monster);
                                }
                                
                            } else {
                                Console.WriteLine($"{initiator.Name} doesn't have enough MP...");
                            }
                        } else if (option == 3) {
                            if (canBeSpared && monster.Name != "Nether Golem") {
                                isSpared = true;
                                Console.WriteLine($"{initiator.Name} spared the {monster.Name}");
                                Console.WriteLine($"({monster.Name} increases the Score by 3000)");
                            } else if (monster.Name == "Nether Golem") {
                                Console.WriteLine($"The {monster.Name} cannot be spared!");
                            } else {
                                Console.WriteLine($"The {monster.Name} doesn't have a * next to their name!");
                            }
                        }
                        if (monster.HP <= monster.MaxHP / 4 && monster.Name != "Nether Golem") {
                            canBeSpared = true;
                        }
                        if (turn < 3) {
                            turn++;
                        } else {
                            turn = 0;
                        }
                        ContinueDialouge();
                    } else if (userInput == "UpArrow" && option > 0) {
                        option--;
                    } else if (userInput == "DownArrow" && option < 3) {
                        option++;
                    }
                } else if (turn == 3) {
                    if (canBeSpared == true) {
                        Console.WriteLine($"The *{monster.Name} doesn't want to fight anymore!");
                    } else if (monster.Name == "Nether Golem") {
                        int target = new Random().Next(0, 3);
                        if (monster.HP <= monster.MaxHP/4) {
                            if (monster.MP >= monster.SpellCost) {
                                monster.MP -= monster.SpellCost;
                                Console.WriteLine($"{initiator.Name} casts {initiator.SpellName} on themself!");
                                monster.CastSpell(ref monster);
                            } else {
                                Console.WriteLine($"{monster.Name} doesn't have enough MP...");
                            }
                            
                        } else {
                            Console.WriteLine($"{monster.Name} attacks {turnOrder[target].Name}!");
                            turnOrder[target].Hurt(monster.Atk);
                        }
                    } else {
                        int target = new Random().Next(0, 3);
                        if (new Random().Next(0, 3) == 0) {
                            if (monster.MP >= monster.SpellCost) {
                                monster.MP -= monster.SpellCost;
                                Console.WriteLine($"{initiator.Name} casts {initiator.SpellName} on {turnOrder[target]}!");
                                monster.CastSpell(ref turnOrder[target]);
                            } else {
                                Console.WriteLine($"{monster.Name} doesn't have enough MP...");
                            }

                        } else {
                            Console.WriteLine($"{monster.Name} attacks {turnOrder[target].Name}!");
                            turnOrder[target].Hurt(monster.Atk);
                        }
                    }
                    if (turn < 3) {
                        turn++;
                    } else {
                        turn = 0;
                    }
                    ContinueDialouge();
                }
            }
        }
    }
}
