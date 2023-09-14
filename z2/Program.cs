using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static Random random = new Random();

    static void Main()
    {
        Console.WriteLine("Welcome to the Dungeon Crawler!");
        Console.WriteLine("You find yourself in a dark dungeon...");

        // Initialize the dungeon
        int numberOfCells = random.Next(5, 11); // Randomly determine the number of cells
        DungeonCell[] dungeon = InitializeDungeon(numberOfCells);

        // Initialize player
        Player player = new Player();

        // Start the game in the first cell
        int currentCellIndex = 0;

        while (player.Health > 0)
        {
            DungeonCell currentCell = dungeon[currentCellIndex];

            Console.WriteLine($"\nCurrent Health: {player.Health}");

            if (currentCell.HasMonster)
            {
                FightMonster(player, currentCell);
                if (player.Health <= 0)
                {
                    Console.WriteLine("The player is dead. The game is over!");
                    break;
                }
            }

            Console.WriteLine("Choose your action:");
            Console.WriteLine("1. Go east");
            Console.WriteLine("2. Go west");
            Console.WriteLine("3. Quit the game");

            string userInput = Console.ReadLine();

            switch (userInput)
            {
                case "1":
                    if (currentCellIndex < numberOfCells - 1)
                    {
                        currentCellIndex++;
                        Console.WriteLine("You move east.");
                    }
                    else
                    {
                        Console.WriteLine("You have beaten the dungeon!");
                        return;
                    }
                    break;
                case "2":
                    if (currentCellIndex > 0)
                    {
                        currentCellIndex--;
                        Console.WriteLine("You move west.");
                    }
                    else
                    {
                        Console.WriteLine("Sorry, but I can't go in that direction.");
                    }
                    break;
                case "3":
                    Console.WriteLine("You quit the game.");
                    return;
                default:
                    Console.WriteLine("I do not know what you mean.");
                    break;
            }

            // Check if the player found a weapon
            if (currentCell.HasWeapon && !player.HasWeapon)
            {
                player.PickUpWeapon(currentCell.Weapon);
                Console.WriteLine($"You found a {currentCell.Weapon.Name}! Your damage increases.");
            }
        }
    }

    // Initialize the dungeon with random monsters, weapons, and exits
    static DungeonCell[] InitializeDungeon(int numberOfCells)
    {
        DungeonCell[] dungeon = new DungeonCell[numberOfCells];

        for (int i = 0; i < numberOfCells; i++)
        {
            bool hasMonster = random.Next(2) == 0; // 50% chance of having a monster
            bool hasWeapon = random.Next(2) == 0;  // 50% chance of having a weapon

            Weapon weapon = null;
            if (hasWeapon)
            {
                string weaponName = random.Next(2) == 0 ? "Sword" : "Stick"; // 50% chance of each weapon
                int weaponDamage = weaponName == "Sword" ? 3 : 1;

                // Create instances of the concrete weapon classes based on weaponName
                weapon = weaponName == "Sword" ? new Sword() : new Stick();
            }

            dungeon[i] = new DungeonCell(hasMonster, weapon);
        }

        return dungeon;
    }

    // Handle a fight with a monster
    static void FightMonster(Player player, DungeonCell cell)
    {
        Console.WriteLine("There is a monster here!");

        while (cell.Monster.Health > 0 && player.Health > 0)
        {
            int playerAttackChance = random.Next(10); // 10% chance of missing
            int monsterAttackChance = random.Next(5); // 20% chance of missing

            if (playerAttackChance != 0)
            {
                player.AttackMonster(cell.Monster);
                Console.WriteLine("You hit the monster!");
            }
            else
            {
                Console.WriteLine("You miss the monster.");
            }

            if (cell.Monster.Health > 0)
            {
                if (monsterAttackChance != 0)
                {
                    player.TakeDamage(4);
                    Console.WriteLine("The monster hits you!");
                }
                else
                {
                    Console.WriteLine("The monster missed you.");
                }

                Console.WriteLine($"Your Health: {player.Health} | Monster's Health: {cell.Monster.Health}");
            }
            else
            {
                Console.WriteLine("You have defeated the monster!");
                break;
            }
        }
    }
}

abstract class Weapon
{
    public string Name { get; }
    public int Damage { get; }

    public Weapon(string name, int damage)
    {
        Name = name;
        Damage = damage;
    }
}

class Sword : Weapon
{
    public Sword() : base("Sword", 3)
    {
    }
}

class Stick : Weapon
{
    public Stick() : base("Stick", 1)
    {
    }
}

abstract class Participant
{
    public int Health { get; protected set; }

    public Participant()
    {
        Health = 100; // Default health
    }

    public abstract void AttackMonster(Monster monster);
    public abstract void TakeDamage(int damage);
}

class Player : Participant
{
    public bool HasWeapon { get; private set; }
    private Weapon equippedWeapon;

    public Player()
    {
        HasWeapon = false;
    }

    public void PickUpWeapon(Weapon weapon)
    {
        HasWeapon = true;
        equippedWeapon = weapon;
    }

    public override void AttackMonster(Monster monster)
    {
        if (HasWeapon)
        {
            monster.TakeDamage(equippedWeapon.Damage);
        }
        else
        {
            monster.TakeDamage(5); // Default damage without a weapon
        }
    }

    public override void TakeDamage(int damage)
    {
        Health -= damage;
    }
}

class Monster : Participant
{
    public Monster() : base()
    {
        // Set the monster's health to 20
        Health = 20;
    }

    public override void AttackMonster(Monster monster)
    {
        // Monsters don't attack each other in this implementation
    }

    public override void TakeDamage(int damage)
    {
        Health -= damage;
    }
}




class DungeonCell
{
    public bool HasMonster { get; }
    public bool HasWeapon { get; }
    public Weapon Weapon { get; }
    public Monster Monster { get; }

    public DungeonCell(bool hasMonster, Weapon weapon)
    {
        HasMonster = hasMonster;
        HasWeapon = weapon != null;
        Weapon = weapon;
        Monster = hasMonster ? new Monster() : null;
    }
}
    
