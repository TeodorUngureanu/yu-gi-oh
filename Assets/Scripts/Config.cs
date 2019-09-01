using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Data;
using Mono.Data.SqliteClient;
using System.Data.Common;

public class Config
{
    private static Config instance = new Config();

    private int mainDeck;
    private int wins;
    private int draws;
    private int losses;
    private string dbName = "URI=file:Yugioh.db";
    private string path = "C:/Users/teodo/Desktop/Card Images/New folder/";

    public Dictionary<string, int> _User_Config = new Dictionary<string, int>();
    public Dictionary<int, Dictionary<string, string>> _Card_Attribute = new Dictionary<int, Dictionary<string, string>>();
    public Dictionary<int, Dictionary<string, string>> _Monster_Type = new Dictionary<int, Dictionary<string, string>>();
    public Dictionary<int, Dictionary<string, string>> _Magic_Card_Type = new Dictionary<int, Dictionary<string, string>>();
    public Dictionary<int, string> _Effect_Key = new Dictionary<int, string>();
    public Dictionary<int, Dictionary<string, int>> _Deck = new Dictionary<int, Dictionary<string, int>>();
    public Dictionary<int, Monster> _Monster_Cards = new Dictionary<int, Monster>();
    public Dictionary<int, NonMonster> _Magic_Cards = new Dictionary<int, NonMonster>();
    public List<int> _User_Deck = new List<int>();
    public Dictionary<int, List<Dictionary<int, Constants.CardInfo>>> _Deck_Cards = new Dictionary<int, List<Dictionary<int, Constants.CardInfo>>>();

    public static Config Get()
    {
        return instance;
    }

    public void Load()
    {
        // Open the db connection.
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            bool noRow = false;

            // Create an sql command that creates a new table.
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS User_Config (ID_User INTEGER PRIMARY KEY AUTOINCREMENT, ID_Main_Deck INTEGER NOT NULL, " +
                    " Wins INT NOT NULL, Draws INT NOT NULL, Losses INT NOT NULL); ";
                command.ExecuteNonQuery();

                command.CommandText = "SELECT COUNT(*) AS InstanceCount FROM User_Config";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetInt32(0).Equals(0))
                        {
                            noRow = true;
                            break;
                        }
                    }

                    reader.Close();
                }
            }

            if (noRow)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO User_Config (ID_Main_Deck, Wins, Draws, Losses) VALUES (1, 0, 0, 0);";
                    command.ExecuteNonQuery();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE Card_Attribute (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name VARCHAR(50) NOT NULL, Description VARCHAR(250) NULL);";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE Monster_Type (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name VARCHAR(50) NOT NULL, Description VARCHAR(250) NULL);";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE Magic_Card_Type (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name VARCHAR(50) NOT NULL, Description VARCHAR(250) NULL);";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE Effect_Key (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name VARCHAR(250) NOT NULL);";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE Deck (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name VARCHAR(50) NOT NULL, Deck_Order INTEGER NOT NULL);";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE Monster_Cards (" +
                        "ID INTEGER PRIMARY KEY AUTOINCREMENT, CardNumber VARCHAR(50) NOT NULL, Image BLOB NULL, Name VARCHAR(50) NOT NULL, Description VARCHAR(250) NULL, Effect_Key VARCHAR(50) NULL, " +
                        "Attribute INTEGER NOT NULL, Type INTEGER NOT NULL, ATK INTEGER NOT NULL, DEF INTEGER NOT NULL, Rarity INTEGER NOT NULL, IsFusion INTEGER NULL " +
                        ");";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE Magic_Cards (" +
                        "ID INTEGER PRIMARY KEY AUTOINCREMENT, CardNumber VARCHAR(50) NOT NULL, Image BLOB NULL, Name VARCHAR(50) NOT NULL, Description VARCHAR(250) NULL, " +
                        "Effect_Key VARCHAR(50) NULL, Magic_Card_Type INTEGER NOT NULL" +
                        ");";
                    command.ExecuteNonQuery();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Card_Attribute (Name) VALUES ('Dark'), ('Divine'), ('Earth'), ('Fire'), ('Light'), ('Water'), ('Wind');";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Monster_Type (Name, Description) VALUES ('Aqua', 'Aqua'), ('Beast', 'Beast'), ('BW', 'Beast-Warrior'), ('Cyberse', 'Cyberse'), " +
                        " ('Dinosaur', 'Dinosaur'), ('DB', 'Divine-Beast'), ('Dragon', 'Dragon'), ('Fairy', 'Fairy'), ('Fiend', 'Fiend'), ('Fish', 'Fish'), ('Insect', 'Insect'), " +
                        " ('Machine', 'Machine'), ('Plant', 'Plant'), ('Psychic', 'Psychic'), ('Pyro', 'Pyro'), ('Reptile', 'Reptile'), ('Rock', 'Rock'), ('SS', 'Sea Serpent'), " +
                        " ('Spellcaster', 'Spellcaster'), ('Thunder', 'Thunder'), ('Warrior', 'Warrior'), ('WB', 'Winged Beast'), ('Wyrm', 'Wyrm'), ('Zombie', 'Zombie');";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Magic_Card_Type (Name) VALUES ('Spell'), ('Trap');";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Effect_Key (Name) VALUES " +
                        " ('FLIP: Return 1 monster on the field to its owner`s hand.'), " +
                        " ('All Dragon-Type monsters cannot be targeted by Magic Cards, Trap Cards, or other effects that specifically designate a target while this card is face-up on the field.'), " +
                        " ('FLIP: Destroys 1 monster on the field (regardless of position).'), " +
                        " ('As long as this card remains face-up on the field, the Life Points of this card`s controller increase by 500 points for each additional monster summoned (excluding Special Summon, but including your opponent`s monsters).'), " +
                        " ('FLIP: All face-down cards on the field are turned face-up, and then returned to their original positions. No card effects are activated when cards are turned face-up.'), " +
                        " ('This card is returned to your hand at the end of your turn.'), " +
                        " ('FLIP: Destroys 1 Trap Card on the field. If this card`s target is face-down, flip it face-up. If the card is a Trap Card, it is destroyed. If not, it is returned to its face-down position. The flipped card is not activated.'), " +
                        " ('The monster attacking this creature is returned to its owner`s hand. Any damage resulting from the attack is calculated normally.'), " +
                        " ('See the top 5 cards of your opponent`s Deck. Return the cards to the Deck in the same order.'), " +
                        " ('A Spellcaster-Type monster equipped with this card increases its ATK and DEF by 300 points.'), " +
                        " ('Both players must discard their entire hands and draw the same number of cards that they discarded from their respective Decks.'), " +
                        " ('Increase a selected monster`s DEF by 500 points during the turn this card is activated.'), " +
                        " ('Select and control 1 opposing monster (regardless of position) on the field until the end of your turn.'), " +
                        " ('A Fiend-Type monster equipped with this card increases its ATK and DEF by 300 points.'), " +
                        " ('Destroys all monsters on the field.'), " +
                        " ('Destroys 1 Magic Card on the field. If this card`s target is face-down flip it face-up. If the card is a Magic Card, it is destroyed. If not, it is returned to its face-down position The flipped card is not activated.'), " +
                        " ('Increases your Life Points by 1000 points.'), " +
                        " ('All dragon-Type monsters on the field are switched to Defense Position and remain in this position as long as this card is active.'), " +
                        " ('Destroys 1 opponent`s face-up monster with the lowest ATK.'), " +
                        " ('An EARTH monster equipped with this card increases its ATK by 400 points and decreases its DEF by 200 points.'), " +
                        " ('Inflict 500 points of Direct Damage to your opponent`s Life Points for each monster your opponent has on the field.'), " +
                        " ('If a monster of yours is sent from the field to Graveyard during the turn that you`ve played this card, you can select a monster with an ATK of 1500 points or less from your Deck and play it as a Special Summon. Shuffle the Deck after playing the card. This card is active for 1 turn only.'), " +
                        " ('Select 1 Monster Card from either your opponent`s or your own Graveyard and place it on the field under your control in Attack or Defense Position (face-up). This is considered a Special Summon.'), " +
                        " ('Inflict 800 damage to your opponent.'), " +
                        " ('Increase 1 selected monster`s ATK by 500 points during the turn this card is activated.'), " +
                        " ('Destroys 1 face-up Trap Card on the field.'), " +
                        " ('All increases and decreases to ATK and DEF are reversed for the turn in which this card is activated.'), " +
                        " ('Increases the ATK and DEF of all Beast-Warrior and Warrior-Type monsters by 200 points.'), " +
                        " ('Select 1 monster on your opponent`s side of the field. This turn, if you would Tribute a monster on your side of the field, Tribute the selected monster instead. You cannot conduct your Battle Phase duing the turn that you activate this card.'), " +
                        " ('A DARK monster equipped with this card increases its ATK by 400 points and decreases its DEF by 200 points.'), " +
                        " ('Playing this card when you have a Lord of D. card face-up on the field allows you to play up to 2 Dragon-Type cards from your hand as a Special Summon.'), " +
                        " ('Select and see 1 card in your opponent`s hand.'), " +
                        " ('If the ATK of a monster summoned by your opponent (excluding Special Summon) is 1000 points or more, the monster is destroyed.'), " +
                        " ('Select and destroy 2 of your monsters and 1 of your opponent`s monters.'), " +
                        " ('At the cost of 500 Life Points per monster, a player is allowed an extra Normal Summon or Set.'), " +
                        " ('Any damage inflicted by an opponent`s monster is decreased to 0 during the turn this card is activated.'), " +
                        " ('Increases the ATK and DEF of all Fiend and Spellcaster-Type monsters by 200 points. Also decreases the ATK and DEF of all Fairly-Type monsters by 200 points.'); ";
                    command.ExecuteNonQuery();

                    command.CommandText = " INSERT INTO Deck (Name, Deck_Order) VALUES ('YUGI', 1), ('KAIBA', 2); ";
                    command.ExecuteNonQuery();
                }

                /**
                 * BEGIN Adding Monster Cards
                 */
                using (var command = connection.CreateCommand())
                {
                    List<byte[]> monsterBlob = new List<byte[]>();
                    byte[] photo;

                    photo = File.ReadAllBytes(path + "Monster/AncientElf.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/Ansatsu.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/BaronOfTheFiendSword.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/BattleOx.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/BeaverWarrior.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/BlueEyesWhiteDragon.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/CelticGuardian.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/ClawReacher.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/CurseOfDragon.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/DarkAssailant.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/DarkMagician.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/DarkTitanOfTerror.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/DestroyerGolem.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/DHuman.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/DomaTheAngelOfSilence.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/DragonZombie.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/FeralImp.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/GaiaTheFierceKnight.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/GiantSoldierOfStone.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/GreatWhite.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/GyakutennoMegami.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/HaneHane.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/HitotsuMeGiant.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/JudgeMan.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/Kojikocy.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/KoumoriDragon.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/LaJinnTheMysticalGenieOfTheLamp.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/LordOfD.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/MagicalGhost.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/MammothGraveyard.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/ManEaterBug.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/ManEatingTreasureChest.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/MasterAndExpert.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/MysteriousPuppeteer.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/MysticalElf.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/MysticClown.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/MysticHorseman.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/NeoTheMagicSwordsman.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/OgreOfTheBlackShadow.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/PaleBeast.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/RogueDoll.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/RudeKaiser.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/RyuKishin.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/RyuKishinPowered.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/SilverFang.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/SkullRedBird.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/SorcererOfTheDoomed.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/SummonedSkull.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/Swordstalker.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/TerraTheTerrible.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/TheSternMystic.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/TheWickedWormBeast.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/TrapMaster.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/UnknownWarriorOfFiend.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/Uraby.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/WallOfIllusion.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/WingedDragonGuardianOfTheFortress.png");
                    monsterBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Monster/WittyPhantom.png");
                    monsterBlob.Add(photo);

                    command.CommandText = " INSERT INTO Monster_Cards (CardNumber, Image, Name, Description, Effect_Key, Attribute, Type, ATK, DEF, Rarity, IsFusion) VALUES " +
                        " ('93221206', @image_1, 'Ancient Elf', 'This elf is rumored to have lived for thousands of years. He leads an army of spirits against his enemies.', NULL, 5, 19, 1450, 1200, 4, NULL), " +
                        " ('48365709', @image_2, 'Ansatsu', 'A silent and deadly warrior specializing in assassinations.', NULL, 3, 21, 1700, 1200, 5, NULL), " +
                        " ('86325596', @image_3, 'Baron of the Fiend Sword', 'An aristocrat who wields a sword possessed by a malicious spirit that preys on the weak.', NULL, 1, 9, 1550, 800, 4, NULL), " +
                        " ('05053103', @image_4, 'Battle Ox', 'A monster with tremendous power, it destroys enemies with a swing of its axe.', NULL, 3, 3, 1700, 1000, 4, NULL), " +
                        " ('32452818', @image_5, 'Beaver Warrior', 'What this creature lacks in size it makes up for in defense when battling in the prairie.', NULL, 3, 3, 1200, 1500, 4, NULL), " +
                        " ('89631139', @image_6, 'Blue-Eyes White Dragon', 'This legendary dragon is a powerful engine of destruction. Virtually invincible, very few have faced this awesome creature and lived to tell the tale.', NULL, 5, 7, 3000, 2500, 8, NULL), " +
                        " ('91152256', @image_7, 'Celtic Guardian', 'An elf who learned to wield a sword, he baffles enemies with lightning-swift attacks.', NULL, 3, 21, 1400, 1200, 4, NULL), " +
                        " ('41218256', @image_8, 'Claw Reacher', 'Stretching arms and razor-sharp claws make this monster a formidable opponent.', NULL, 1, 9, 1000, 800, 3, NULL), " +
                        " ('28279543', @image_9, 'Curse of Dragon', 'A wicked dragon that taps into dark forces to execute a powerful attack.', NULL, 1, 7, 2000, 1500, 5, NULL), " +
                        " ('41949033', @image_10, 'Dark Assailant', 'Armed with the Psycho Sword, this sinister assassin rules the bad land.', NULL, 1, 24, 1200, 1200, 4, NULL), " +
                        " ('46986414', @image_11, 'Dark Magician', 'The ultimate wizard in terms of attack and defense.', NULL, 1, 19, 2500, 2100, 7, NULL), " +
                        " ('89494469', @image_12, 'Dark Titan of Terror', 'A fiend said to dwell in the world of dreams, it attacks enemies in their sleep.', NULL, 1, 9, 1300, 1100, 4, NULL), " +
                        " ('73481154', @image_13, 'Destroyer Golem', 'A golem with a massive right hand for crushing its victims.', NULL, 3, 17, 1500, 1000, 4, NULL), " +
                        " ('81057959', @image_14, 'D. Human', 'Gifted with the power of dragons, this warrior wields a sword created from a dragon`s fang.', NULL, 3, 21, 1300, 1100, 4, NULL), " +
                        " ('16972957', @image_15, 'Doma The Angel of Silence', 'This fairy rules over the end of existence.', NULL, 1, 8, 1600, 1400, 5, NULL), " +
                        " ('66672569', @image_16, 'Dragon Zombie', 'A dragon revived by sorcery. Its breath is highlycorrosive.', NULL, 1, 24, 1600, 0, 3, NULL), " +
                        " ('41392891', @image_17, 'Feral Imp', 'A playful little fiend that lurks in the dark, waiting to attack an unwary enemy.', NULL, 1, 9, 1300, 1400, 4, NULL), " +
                        " ('06368038', @image_18, 'Gaia The Fierce Knight', 'A knight whose horse travels faster than the wind. His battle-charge is a force to be reckoned with.', NULL, 3, 21, 2300, 2100, 7, NULL), " +
                        " ('13039848', @image_19, 'Giant Soldier of Stone', 'A giant warrior made of stone. A punch from this creature has earth-shaking results.', NULL, 3, 17, 1300, 2000, 3, NULL), " +
                        " ('13429800', @image_20, 'Great White', 'A giant white shark with razor-sharp teeth.', NULL, 6, 10, 1600, 800, 4, NULL), " +
                        " ('31122090', @image_21, 'Gyakutenno Megami', 'This fairy uses her mystical power to protect the weak and provide spiritual support.', NULL, 5, 8, 1800, 2000, 6, NULL), " +
                        " ('07089711', @image_22, 'Hane-Hane', 'FLIP: Return 1 monster on the field to its owner`s hand.', 1, 3, 2, 450, 500, 2, NULL), " +
                        " ('76184692', @image_23, 'Hitotsu-Me Giant', 'A one-eyed behemoth with thick, powerful arms made for delivering punishing blows.', NULL, 3, 3, 1200, 1000, 4, NULL), " +
                        " ('30113682', @image_24, 'Judge Man', 'This club-wielding warrior battles to the end and will never surrender.', NULL, 3, 21, 2200, 1500, 6, NULL), " +
                        " ('01184620', @image_25, 'Kojikocy', 'A man-hunter with powerful arms that can crush boulders.', NULL, 3, 21, 1500, 1200, 4, NULL), " +
                        " ('67724379', @image_26, 'Koumori Dragon', 'A vicious, fire-breathing dragon whose wicked flame corrupts the souls of its victims.', NULL, 1, 7, 1500, 1200, 4, NULL), " +
                        " ('97590747', @image_27, 'La Jinn the Mystical Genie of the Lamp', 'A genie of the lamp that`s at the beck and call of its master.', NULL, 1, 9, 1800, 1000, 4, NULL), " +
                        " ('17985575', @image_28, 'Lord of D.', 'All Dragon-Type monsters cannot be targeted by Magic Cards, Trap Cards, or other effects that specifically designate a target while this card is face-up on the field.', 2, 1, 19, 1200, 1100, 4, NULL), " +
                        " ('46474915', @image_29, 'Magical Ghost', 'This creature casts a spell of terror and confusion just before attacking its enemies.', NULL, 1, 24, 1300, 1400, 4, NULL), " +
                        " ('40374923', @image_30, 'Mammoth Graveyard', 'A mammoth that protects the graves of its pack and is absolutely merciless when facing grave-robbers.', NULL, 3, 5, 1200, 800, 3, NULL), " +
                        " ('54652250', @image_31, 'Man-Eater Bug', 'FLIP: Destroys 1 monster on the field (regardless of position).', 3, 3, 11, 450, 600, 2, NULL), " +
                        " ('13723605', @image_32, 'Man-Eating Treasure Chest', 'A monster disguised as a treasure chest that is known to attack the unwary adventurer.', NULL, 1, 9, 1600, 1000, 4, NULL), " +
                        " ('75499502', @image_33, 'Master & Expert', 'A deadly duo consisting of a beast master and its loyal servant.', NULL, 3, 2, 1200, 1000, 4, NULL), " +
                        " ('54098121', @image_34, 'Mysterious Puppeteer', 'As long as this card remains face-up on the field, the Life Points of this card`s controller increase by 500 points for each additional monster summoned (excluding Special Summon, but including your opponent`s monsters).', 4, 3, 21, 1000, 1500, 4, NULL), " +
                        " ('15025844', @image_35, 'Mystical Elf', 'A delicate elf that lacks offense, but has a terrific defense backed by mystical power.', NULL, 5, 19, 800, 2000, 4, NULL), " +
                        " ('47060154', @image_36, 'Mystic Clown', 'Nothing can stop the mad attack of this powerful creature.', NULL, 1, 9, 1500, 1000, 4, NULL), " +
                        " ('68516705', @image_37, 'Mystic Horseman', 'Half man and half horse, this monster is knows for its extreme speed.', NULL, 3, 2, 1300, 1550, 4, NULL), " +
                        " ('50930991', @image_38, 'Neo the Magic Swordsman', 'A dimensional drifter who not only practices sorcery, but is also a sword and martial arts master.', NULL, 5, 19, 1700, 1000, 4, NULL), " +
                        " ('45121025', @image_39, 'Ogre of the Black Shadow', 'An ogre possessed by the powers of the dark. Few can withstand its rapid chage.', NULL, 3, 3, 1200, 1400, 4, NULL), " +
                        " ('21263083', @image_40, 'Pale Beast', 'With skin tinged a bluish-white, this strange creature is a fearsome sight to behold.', NULL, 3, 2, 1500, 1200, 4, NULL), " +
                        " ('91939608', @image_41, 'Rogue Doll', 'A deadly doll gited with mystical power, it is particularly powerful when attacking against dark forces.', NULL, 5, 19, 1600, 1000, 4, NULL), " +
                        " ('26378150', @image_42, 'Rude Kaiser', 'With an axe in each hand, this monster delivers heavy damage.', NULL, 3, 3, 1800, 1600, 5, NULL), " +
                        " ('15303296', @image_43, 'Ryu-Kishin', 'A very elusive creature that looks like a harmless statue until it attacks.', NULL, 1, 9, 1000, 500, 3, NULL), " +
                        " ('24611934', @image_44, 'Ryu-Kishin Powered', 'A gargoyle enhanced by the powers of darkness. Very sharp talons make it a worthy opponent.', NULL, 1, 9, 1600, 1200, 4, NULL), " +
                        " ('90357090', @image_45, 'Silver Fang', 'A snow wolf that`s beautiful to the eye, but absolutely vicious in battle.', NULL, 3, 2, 1200, 800, 4, NULL), " +
                        " ('10202894', @image_46, 'Skull Red Bird', 'This monster swoops down and attacks with a rain of knives stores in its wings.', NULL, 7, 22, 1550, 1200, 4, NULL), " +
                        " ('49218300', @image_47, 'Sorcerer of the Doomed', 'A slave of the dark arts, this sorcerer is a monster of life-extinguishing spells.', NULL, 1, 19, 1450, 1200, 4, NULL), " +
                        " ('70781052', @image_48, 'Summoned Skull', 'A fiend with dark powers for confusing the enemy. Among the Field-Type monsters, this monster boasts considerable force.', NULL, 1, 9, 2500, 1200, 6, NULL), " +
                        " ('50005633', @image_49, 'Swordstalker', 'A monster formed by the vengeful souls of those who passed away in battle.', NULL, 1, 21, 2000, 1600, 6, NULL), " +
                        " ('63308047', @image_50, 'Terra the Terrible', 'Known as a swamp dweller, this creature is a minion of dark forces.', NULL, 1, 9, 1200, 1300, 4, NULL), " +
                        " ('87557188', @image_51, 'The Stern Mystic', 'FLIP: All face-down cards on the field are turned face-up, and then returned to their original positions. No card effects are activated when cards are turned face-up.', 5, 5, 19, 1500, 1200, 4, NULL), " +
                        " ('06285791', @image_52, 'The Wicked Worm Beast', 'This card is returned to your hand at the end of your turn.', 6, 3, 2, 1400, 700, 3, NULL), " +
                        " ('46461247', @image_53, 'Trap Master', 'FLIP: Destroys 1 Trap Card on the field. If this card`s target is face-down, flip it face-up. If the card is a Trap Card, it is destroyed. If not, it is returned to its face-down position. The flipped card is not activated.', 7, 3, 21, 500, 1100, 3, NULL), " +
                        " ('97360116', @image_54, 'Unknown Warrior of Fiend', 'The speed of this warrior creates an intense vacuum that can slice through a monster`s hide.', NULL, 1, 21, 1000, 500, 3, NULL), " +
                        " ('01784619', @image_55, 'Uraby', 'Fast on its feet, this dinosaur rips enemies to shreds with its sharp claws.', NULL, 3, 5, 1500, 800, 4, NULL), " +
                        " ('13945283', @image_56, 'Wall of Illusion', 'The monster attacking this creature is returned to its owner`s hand. Any damage resulting from the attack is calculated normally.', 8, 1, 9, 1000, 1850, 4, NULL), " +
                        " ('87796900', @image_57, 'Winger Dragon, Guardian of the Fortress #1', 'A dragon commonly found guarding mountain fortresses. Its signature attack is a sweeping dive from out of the blue.', NULL, 7, 7, 1400, 1200, 4, NULL), " +
                        " ('36304921', @image_58, 'Witty Phantom', 'Dressed in a night-black tuxedo, this creature presides over the darkness.', NULL, 1, 9, 1400, 1300, 4, NULL); ";

                    DbParameter param;

                    for (var index = 0; index < monsterBlob.Count; index++)
                    {
                        param = command.CreateParameter();
                        param.ParameterName = "@image_" + (index + 1);
                        param.DbType = DbType.Binary;
                        param.Value = monsterBlob[index];

                        command.Parameters.Add(param);
                    }

                    command.ExecuteNonQuery();
                }
                /**
                 * END Adding Monster Cards
                 */

                /**
                 * BEGIN Adding Magic Cards
                 */
                using (var command = connection.CreateCommand())
                {
                    List<byte[]> magicBlob = new List<byte[]>();
                    byte[] photo;

                    photo = File.ReadAllBytes(path + "Magic/AncientTelescope.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/BookOfSecretArts.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/CardDestruction.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/CastleWalls.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/ChangeOfHeart.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/DarkEnergy.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/DarkHole.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/DeSpell.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/DianKetoTheCureMaster.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/DragonCaptureJar.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/Fissure.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/Invigoration.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/JustDesserts.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/LastWill.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/MonsterReborn.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/Ookazi.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/Reinforcements.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/RemoveTrap.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/ReverseTrap.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/Sogen.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/SoulExchange.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/SwordOfDarkDestruction.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/TheFluteOfSummoningDragon.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/TheInexperiencedSpy.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/TrapHole.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/TwoProngedAttack.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/UltimateOffering.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/Waboku.png");
                    magicBlob.Add(photo);
                    photo = File.ReadAllBytes(path + "Magic/Yami.png");
                    magicBlob.Add(photo);

                    command.CommandText = "INSERT INTO Magic_Cards (CardNumber, Image, Name, Description, Effect_Key, Magic_Card_Type) VALUES " +
                        " ('17092736', @image_1, 'Ancient Telescope', 'See the top 5 cards of your opponent`s Deck. Return the cards to the Deck in the same order.', 9, 1), " +
                        " ('91595718', @image_2, 'Book of Secret Arts', 'A Spellcaster-Type monster equipped with this card increases its ATK and DEF by 300 points.', 10, 1), " +
                        " ('72892473', @image_3, 'Card Destruction', 'Both players must discard their entire hands and draw the same number of cards that they discarded from their respective Decks.', 11, 1), " +
                        " ('44209392', @image_4, 'Castle Walls', 'Increase a selected monster`s DEF by 500 points during the turn this card is activated.', 12, 2), " +
                        " ('04031928', @image_5, 'Change of Heart', 'Select and control 1 opposing monster (regardless of position) on the field until the end of your turn.', 13, 1), " +
                        " ('04614116', @image_6, 'Dark Energy', 'A Fiend-Type monster equipped with this card increases its ATK and DEF by 300 points.', 14, 1), " +
                        " ('53129443', @image_7, 'Dark Hole', 'Destroys all monsters on the field.', 15, 1), " +
                        " ('19159413', @image_8, 'De-Spell', 'Destroys 1 Magic Card on the field. If this card`s target is face-down flip it face-up. If the card is a Magic Card, it is destroyed. If not, it is returned to its face-down position The flipped card is not activated.', 16, 1), " +
                        " ('84257639', @image_9, 'Dian Keto the Cure Master', 'Increases your Life Points by 1000 points.', 17, 1), " +
                        " ('50045299', @image_10, 'Dragon Capture Jar', 'All dragon-Type monsters on the field are switched to Defense Position and remain in this position as long as this card is active.', 18, 2), " +
                        " ('66788016', @image_11, 'Fissure', 'Destroys 1 opponent`s face-up monster with the lowest ATK.', 19, 1), " +
                        " ('98374133', @image_12, 'Invigoration', 'An EARTH monster equipped with this card increases its ATK by 400 points and decreases its DEF by 200 points.', 20, 1), " +
                        " ('24068492', @image_13, 'Just Desserts', 'Inflict 500 points of Direct Damage to your opponent`s Life Points for each monster your opponent has on the field.', 21, 2), " +
                        " ('85602018', @image_14, 'Last Will', 'If a monster of yours is sent from the field to Graveyard during the turn that you`ve played this card, you can select a monster with an ATK of 1500 points or less from your Deck and play it as a Special Summon. Shuffle the Deck after playing the card. This card is active for 1 turn only.', 22, 1), " +
                        " ('83764718', @image_15, 'Monster Reborn', 'Select 1 Monster Card from either your opponent`s or your own Graveyard and place it on the field under your control in Attack or Defense Position (face-up). This is considered a Special Summon.', 23, 1), " +
                        " ('19523799', @image_16, 'Ookazi', 'Inflict 800 damage to your opponent.', 24, 1), " +
                        " ('17814387', @image_17, 'Reinforcements', 'Increase 1 selected monster`s ATK by 500 points during the turn this card is activated.', 25, 2), " +
                        " ('51482758', @image_18, 'Remove Trap', 'Destroys 1 face-up Trap Card on the field.', 26, 1), " +
                        " ('77622396', @image_19, 'Reverse Trap', 'All increases and decreases to ATK and DEF are reversed for the turn in which this card is activated.', 27, 2), " +
                        " ('86318356', @image_20, 'Sogen', 'Increases the ATK and DEF of all Beast-Warrior and Warrior-Type monsters by 200 points.', 28, 1), " +
                        " ('68005187', @image_21, 'Soul Exchange', 'Select 1 monster on your opponent`s side of the field. This turn, if you would Tribute a monster on your side of the field, Tribute the selected monster instead. You cannot conduct your Battle Phase duing the turn that you activate this card.', 29, 1), " +
                        " ('37120512', @image_22, 'Sword of Dark Destruction', 'A DARK monster equipped with this card increases its ATK by 400 points and decreases its DEF by 200 points.', 30, 1), " +
                        " ('43973174', @image_23, 'The Flute of Summoning Dragon', 'Playing this card when you have a Lord of D. card face-up on the field allows you to play up to 2 Dragon-Type cards from your hand as a Special Summon.', 31, 1), " +
                        " ('81820689', @image_24, 'The Inexperienced Spy', 'Select and see 1 card in your opponent`s hand.', 32, 1), " +
                        " ('04206964', @image_25, 'Trap Hole', 'If the ATK of a monster summoned by your opponent (excluding Special Summon) is 1000 points or more, the monster is destroyed.', 33, 2), " +
                        " ('83887306', @image_26, 'Two-Pronged Attack', 'Select and destroy 2 of your monsters and 1 of your opponent`s monters.', 34, 2), " +
                        " ('80604091', @image_27, 'Ultimate Offering', 'At the cost of 500 Life Points per monster, a player is allowed an extra Normal Summon or Set.', 35, 2), " +
                        " ('12607053', @image_28, 'Waboku', 'Any damage inflicted by an opponent`s monster is decreased to 0 during the turn this card is activated.', 36, 2), " +
                        " ('59197169', @image_29, 'Yami', 'Increases the ATK and DEF of all Fiend and Spellcaster-Type monsters by 200 points. Also decreases the ATK and DEF of all Fairly-Type monsters by 200 points.', 37, 1); ";

                    DbParameter param;

                    for (var index = 0; index < magicBlob.Count; index++)
                    {
                        param = command.CreateParameter();
                        param.ParameterName = "@image_" + (index + 1);
                        param.DbType = DbType.Binary;
                        param.Value = magicBlob[index];

                        command.Parameters.Add(param);
                    }

                    command.ExecuteNonQuery();
                }
                /**
                 * END Adding Magic Cards
                 */

                /**
                 * BEGIN Creating Associative Tables
                 */
                using (var command = connection.CreateCommand()) {
                    command.CommandText = "CREATE TABLE User_Deck (ID INTEGER PRIMARY KEY AUTOINCREMENT, ID_User INTEGER NOT NULL, ID_Deck INTEGER NOT NULL);";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE Deck_Cards (ID INTEGER PRIMARY KEY AUTOINCREMENT, ID_Deck INTEGER NOT NULL, ID_Card INTEGER NOT NULL, Card_Type INTEGER NOT NULL, Card_Order INTEGER NOT NULL);";
                    command.ExecuteNonQuery();
                }
                /**
                 * END Creating Associative Tables
                 */

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO User_Deck (ID_User, ID_Deck) VALUES (1, 1), (1, 2);";
                    command.ExecuteNonQuery();

                    // Card_Type - 1 - Monster, 2 - Magic
                    command.CommandText = "INSERT INTO Deck_Cards (ID_Deck, ID_Card, Card_Type, Card_Order) VALUES " +
                        // Yugi Default Deck
                        " (1, 1, 1, 1), " +
                        " (1, 2, 1, 2), " +
                        " (1, 3, 1, 3), " +
                        " (1, 5, 1, 4), " +
                        " (1, 7, 1, 5), " +
                        " (1, 8, 1, 6), " +
                        " (1, 9, 1, 7), " +
                        " (1, 11, 1, 8), " +
                        " (1, 15, 1, 9), " +
                        " (1, 16, 1, 10), " +
                        " (1, 17, 1, 11), " +
                        " (1, 18, 1, 12), " +
                        " (1, 19, 1, 13), " +
                        " (1, 20, 1, 14), " +
                        " (1, 29, 1, 15), " +
                        " (1, 30, 1, 16), " +
                        " (1, 31, 1, 17), " +
                        " (1, 32, 1, 18), " +
                        " (1, 35, 1, 19), " +
                        " (1, 36, 1, 20), " +
                        " (1, 38, 1, 21), " +
                        " (1, 45, 1, 22), " +
                        " (1, 47, 1, 23), " +
                        " (1, 48, 1, 24), " +
                        " (1, 51, 1, 25), " +
                        " (1, 53, 1, 26), " +
                        " (1, 56, 1, 27), " +
                        " (1, 57, 1, 28), " +
                        " (1, 58, 1, 29), " +
                        " (1, 2, 2, 30), " +
                        " (1, 3, 2, 31), " +
                        " (1, 4, 2, 32), " +
                        " (1, 5, 2, 33), " +
                        " (1, 7, 2, 34), " +
                        " (1, 8, 2, 35), " +
                        " (1, 9, 2, 36), " +
                        " (1, 10, 2, 37), " +
                        " (1, 11, 2, 38), " +
                        " (1, 14, 2, 39), " +
                        " (1, 15, 2, 40), " +
                        " (1, 17, 2, 41), " +
                        " (1, 18, 2, 42), " +
                        " (1, 19, 2, 43), " +
                        " (1, 21, 2, 44), " +
                        " (1, 22, 2, 45), " +
                        " (1, 25, 2, 46), " +
                        " (1, 26, 2, 47), " +
                        " (1, 27, 2, 48), " +
                        " (1, 28, 2, 49), " +
                        " (1, 29, 2, 50), " +
                        // Kaiba Default Deck
                        " (2, 4, 1, 1), " +
                        " (2, 6, 1, 2), " +
                        " (2, 10, 1, 3), " +
                        " (2, 12, 1, 4), " +
                        " (2, 13, 1, 5), " +
                        " (2, 14, 1, 6), " +
                        " (2, 21, 1, 7), " +
                        " (2, 22, 1, 8), " +
                        " (2, 23, 1, 9), " +
                        " (2, 24, 1, 10), " +
                        " (2, 25, 1, 11), " +
                        " (2, 26, 1, 12), " +
                        " (2, 27, 1, 13), " +
                        " (2, 28, 1, 14), " +
                        " (2, 33, 1, 15), " +
                        " (2, 34, 1, 16), " +
                        " (2, 36, 1, 17), " +
                        " (2, 37, 1, 18), " +
                        " (2, 39, 1, 19), " +
                        " (2, 40, 1, 20), " +
                        " (2, 41, 1, 21), " +
                        " (2, 42, 1, 22), " +
                        " (2, 43, 1, 23), " +
                        " (2, 44, 1, 24), " +
                        " (2, 46, 1, 25), " +
                        " (2, 49, 1, 26), " +
                        " (2, 50, 1, 27), " +
                        " (2, 52, 1, 28), " +
                        " (2, 53, 1, 29), " +
                        " (2, 54, 1, 30), " +
                        " (2, 55, 1, 31), " +
                        " (2, 1, 2, 32), " +
                        " (2, 4, 2, 33), " +
                        " (2, 6, 2, 34), " +
                        " (2, 7, 2, 35), " +
                        " (2, 8, 2, 36), " +
                        " (2, 11, 2, 37), " +
                        " (2, 12, 2, 38), " +
                        " (2, 13, 2, 39), " +
                        " (2, 15, 2, 40), " +
                        " (2, 16, 2, 41), " +
                        " (2, 17, 2, 42), " +
                        " (2, 18, 2, 43), " +
                        " (2, 19, 2, 44), " +
                        " (2, 20, 2, 45), " +
                        " (2, 23, 2, 46), " +
                        " (2, 24, 2, 47), " +
                        " (2, 25, 2, 48), " +
                        " (2, 26, 2, 49), " +
                        " (2, 27, 2, 50); ";
                    command.ExecuteNonQuery();
                }
            }





            /////////////////////////
            // BEGIN Get Info From DB

            using (var command = connection.CreateCommand())
            {
                /**
                 * BEGIN Get User Config
                 */
                command.CommandText =
                    " SELECT ID_Main_Deck, Wins, Draws, Losses " +
                    " FROM User_Config " +
                    " WHERE ID_User = 1; ";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _User_Config.Add("mainDeck", reader.GetInt32(0));
                        _User_Config.Add("wins", reader.GetInt32(1));
                        _User_Config.Add("draws", reader.GetInt32(2));
                        _User_Config.Add("losses", reader.GetInt32(3));
                    }

                    reader.Close();
                }

                /**
                 * BEGIN Get Card Attributes
                 */
                command.CommandText =
                    " SELECT ID, Name, Description " +
                    " FROM Card_Attribute; ";

                using (IDataReader reader = command.ExecuteReader())
                {
                    Dictionary<string, string> _aux;

                    while (reader.Read())
                    {
                        _aux = new Dictionary<string, string>();
                        string _description = string.Empty;

                        if ( ! reader.IsDBNull(reader.GetOrdinal("Description"))) {
                            _description = reader.GetString(2);
                        }

                        _aux.Add(reader.GetString(1), _description);

                        _Card_Attribute.Add(reader.GetInt32(0), _aux);
                    }

                    reader.Close();
                }

                /**
                 * BEGIN Get Monster Type
                 */
                command.CommandText =
                    " SELECT ID, Name, Description " +
                    " FROM Monster_Type; ";

                using (IDataReader reader = command.ExecuteReader())
                {
                    Dictionary<string, string> _aux;

                    while (reader.Read())
                    {
                        _aux = new Dictionary<string, string>();
                        string _description = string.Empty;

                        if ( ! reader.IsDBNull(reader.GetOrdinal("Description"))) {
                            _description = reader.GetString(2);
                        }

                        _aux.Add(reader.GetString(1), _description);

                        _Monster_Type.Add(reader.GetInt32(0), _aux);
                    }

                    reader.Close();
                }

                /**
                 * BEGIN Get Magic Card Type
                 */
                command.CommandText =
                    " SELECT ID, Name, Description " +
                    " FROM Magic_Card_Type; ";

                using (IDataReader reader = command.ExecuteReader())
                {
                    Dictionary<string, string> _aux;

                    while (reader.Read())
                    {
                        _aux = new Dictionary<string, string>();
                        string _description = string.Empty;

                        if ( ! reader.IsDBNull(reader.GetOrdinal("Description"))) {
                            _description = reader.GetString(2);
                        }

                        _aux.Add(reader.GetString(1), _description);

                        _Magic_Card_Type.Add(reader.GetInt32(0), _aux);
                    }

                    reader.Close();
                }

                /**
                 * BEGIN Get Effect Key
                 */
                command.CommandText =
                    " SELECT ID, Name " +
                    " FROM Effect_Key; ";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _Effect_Key.Add(reader.GetInt32(0), reader.GetString(1));
                    }

                    reader.Close();
                }

                /**
                 * BEGIN Get Deck
                 */
                command.CommandText =
                    " SELECT ID, Name, Deck_Order " +
                    " FROM Deck; ";

                using (IDataReader reader = command.ExecuteReader())
                {
                    Dictionary<string, int> _aux;

                    while (reader.Read())
                    {
                        _aux = new Dictionary<string, int>();
                        _aux.Add(reader.GetString(1), reader.GetInt32(2));

                        _Deck.Add(reader.GetInt32(0), _aux);
                    }

                    reader.Close();
                }

                /**
                 * BEGIN Get Monster Cards
                 */
                command.CommandText =
                    " SELECT ID, CardNumber, Image, Name, Description, Effect_Key, Attribute, Type, ATK, DEF, Rarity, IsFusion " +
                    " FROM Monster_Cards; ";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string _description = string.Empty;
                        bool _isFusion = false;

                        if ( ! reader.IsDBNull(reader.GetOrdinal("Description")))
                        {
                            _description = reader.GetString(4);
                        }

                        if ( ! reader.IsDBNull(reader.GetOrdinal("IsFusion")))
                        {
                            _isFusion = (bool)reader["IsFusion"];
                        }

                        Monster _currentMonster = new Monster(
                            reader.GetString(1),        // CardNumber
                            (byte[])reader["Image"],    // Image 
                            reader.GetString(3),        // CardName 
                            _description,               // Description
                            reader.GetInt32(5),         // EffectKey
                            reader.GetInt32(6),         // Attribute
                            reader.GetInt32(7),         // Type
                            reader.GetInt32(8),         // ATK
                            reader.GetInt32(9),         // DEF
                            reader.GetInt32(10),        // Rarity
                            _isFusion                   // IsFusion
                        );

                        _Monster_Cards.Add(reader.GetInt32(0), _currentMonster);
                    }

                    reader.Close();
                }

                /**
                 * BEGIN Get Magic Cards
                 */
                command.CommandText =
                    " SELECT ID, CardNumber, Image, Name, Description, Effect_Key, Magic_Card_Type " +
                    " FROM Magic_Cards; ";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string _description = string.Empty;

                        if (!reader.IsDBNull(reader.GetOrdinal("Description")))
                        {
                            _description = reader.GetString(4);
                        }

                        NonMonster _currentMagicCard = new NonMonster(
                            reader.GetString(1),        // CardNumber
                            (byte[])reader["Image"],    // Image 
                            reader.GetString(3),        // CardName 
                            _description,               // Description
                            reader.GetInt32(5),         // EffectKey
                            reader.GetInt32(6)          // MagicCardType
                        );

                        _Magic_Cards.Add(reader.GetInt32(0), _currentMagicCard);
                    }

                    reader.Close();
                }

                /**
                 * BEGIN User Deck
                 */
                command.CommandText =
                    " SELECT ID_Deck " +
                    " FROM User_Deck " +
                    " WHERE ID_User = 1; ";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _User_Deck.Add(reader.GetInt32(0));
                    }

                    reader.Close();
                }

                /**
                 * BEGIN Deck Cards
                 */
                command.CommandText =
                    " SELECT ID_Deck, ID_Card, Card_Type, Card_Order " +
                    " FROM Deck_Cards; ";

                using (IDataReader reader = command.ExecuteReader())
                {
                    Dictionary<int, Constants.CardInfo> _aux;
                    List<Dictionary<int, Constants.CardInfo>> _auxList;

                    while (reader.Read())
                    {
                        _aux = new Dictionary<int, Constants.CardInfo>();
                        _auxList = new List<Dictionary<int, Constants.CardInfo>>();

                        Constants.CardInfo _cardInfo;
                        _cardInfo.Card_Type = reader.GetInt32(2);
                        _cardInfo.Card_Order = reader.GetInt32(3);

                        _aux.Add(reader.GetInt32(1), _cardInfo);

                        if (_Deck_Cards.ContainsKey(reader.GetInt32(0)))
                        {
                            _auxList = _Deck_Cards[reader.GetInt32(0)];
                            _auxList.Add(_aux);
                            _Deck_Cards[reader.GetInt32(0)] = _auxList;
                        }
                        else {
                            _auxList.Add(_aux);
                            _Deck_Cards.Add(reader.GetInt32(0), _auxList);
                        }

                    }

                    reader.Close();
                }
            }

            // END Get Info From DB
            ///////////////////////





            connection.Close();
        }
    }

    public void DisplayConfig()
    {
        Debug.Log("PROCESSING");

        Debug.Log("_User_Config");
        foreach (KeyValuePair<string, int> kvp in _User_Config)
        {
            Debug.Log(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
        }

        Debug.Log("_Card_Attribute");
        foreach (KeyValuePair<int, Dictionary<string, string>> kvp in _Card_Attribute)
        {
            foreach (KeyValuePair<string, string> kvp2 in kvp.Value)
            {
                Debug.Log(string.Format("Key = {0}, Key2 = {1}, Value2 = {2}", kvp.Key, kvp2.Key, kvp2.Value));
            }
        }

        Debug.Log("_Monster_Type");
        foreach (KeyValuePair<int, Dictionary<string, string>> kvp in _Monster_Type)
        {
            foreach (KeyValuePair<string, string> kvp2 in kvp.Value)
            {
                Debug.Log(string.Format("Key = {0}, Key2 = {1}, Value2 = {2}", kvp.Key, kvp2.Key, kvp2.Value));
            }
        }

        Debug.Log("_Magic_Card_Type");
        foreach (KeyValuePair<int, Dictionary<string, string>> kvp in _Magic_Card_Type)
        {
            foreach (KeyValuePair<string, string> kvp2 in kvp.Value)
            {
                Debug.Log(string.Format("Key = {0}, Key2 = {1}, Value2 = {2}", kvp.Key, kvp2.Key, kvp2.Value));
            }
        }

        Debug.Log("_Effect_Key");
        foreach (KeyValuePair<int, string> kvp in _Effect_Key)
        {
            Debug.Log(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
        }

        Debug.Log("_Deck");
        foreach (KeyValuePair<int, Dictionary<string, int>> kvp in _Deck)
        {
            foreach (KeyValuePair<string, int> kvp2 in kvp.Value)
            {
                Debug.Log(string.Format("Key = {0}, Key2 = {1}, Value2 = {2}", kvp.Key, kvp2.Key, kvp2.Value));
            }
        }

        Debug.Log("_Monster_Cards");
        foreach (KeyValuePair<int, Monster> kvp in _Monster_Cards)
        {
            Debug.Log(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value.getCardNumber()));
        }

        Debug.Log("_Magic_Cards");
        foreach (KeyValuePair<int, NonMonster> kvp in _Magic_Cards)
        {
            Debug.Log(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value.getCardNumber()));
        }

        Debug.Log("_User_Deck");
        foreach (int _deck in _User_Deck)
        {
            Debug.Log(_deck);
        }

        Debug.Log("_Deck_Cards");
        foreach (KeyValuePair<int, List<Dictionary<int, Constants.CardInfo>>> kvp in _Deck_Cards)
        {
            foreach (Dictionary<int, Constants.CardInfo> _dictionary in kvp.Value)
            {
                foreach (KeyValuePair<int, Constants.CardInfo> kvp2 in _dictionary)
                {
                    Debug.Log(string.Format("ID_Deck = {0}, ID_Card = {1}, Card_Type = {2}, Card_Order = {3}", kvp.Key, kvp2.Key, kvp2.Value.Card_Type, kvp2.Value.Card_Order));
                }
            }
        }

        Debug.Log("DONE");
    }

    public void SaveData(int mainDeck, int currentWins, int currentDraws, int currentLosses)
    {
        // Open the db connection.
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            // Create an sql command that creates a new table.
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE User_Config SET ID_Main_Deck = " + mainDeck + ", Wins = " + currentWins + ", Draws = " + currentDraws + ", Losses = " + currentLosses + " WHERE ID = 1; ";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

}
