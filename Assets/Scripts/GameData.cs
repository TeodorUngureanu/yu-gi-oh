using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using System.Data;
using System.Data.SqlClient;
using Mono.Data.SqliteClient;

// For XML serialization
// using System.Xml.Serialization;

// [XmlRoot("GameData")]
public class GameData
{
    public static GameData Instance = new GameData();

    public int health;
    public int charisma;
    public int coins;
    public int sticks;
    public int chests;

    public static void Load()
    {
        // The name of the db.
        string dbName = "URI=file:Yugioh.db";

        // Open the db connection.
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            // Create an sql command that creates a new table.
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS User_Config (ID_User INTEGER PRIMARY KEY AUTOINCREMENT, Health INT NOT NULL, Charisma INT NOT NULL, Coins INT NOT NULL);";
                command.ExecuteNonQuery();

                // RESOURCES
                command.CommandText = "CREATE TABLE IF NOT EXISTS Resources (ID_Resource INTEGER PRIMARY KEY AUTOINCREMENT, Name VARCHAR(50) NOT NULL, Description VARCHAR(50));";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Resources (Name) VALUES ('Stick'), ('Chest');";
                command.ExecuteNonQuery();
                // RESOURCES

                command.CommandText = "CREATE TABLE IF NOT EXISTS User_Resources_Association (ID INTEGER PRIMARY KEY AUTOINCREMENT, ID_User INTEGER NOT NULL, ID_Resource INTEGER NOT NULL, Number_Resources INT NOT NULL);";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO User_Resources_Association (ID_User, ID_Resource, Number_Resources) VALUES (1, 1, 0), (1, 2, 0);";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS Save_Scenes (ID_Save INTEGER PRIMARY KEY AUTOINCREMENT, Name VARCHAR(50) NOT NULL, Description VARCHAR(50), Save_Date VARCHAR(50) NOT NULL);";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS User_Save_Scenes_Association (ID INTEGER PRIMARY KEY AUTOINCREMENT, ID_User INTEGER NOT NULL, ID_Save INTEGER NOT NULL);";
                command.ExecuteNonQuery();

                command.CommandText = "SELECT COUNT(*) AS InstanceCount FROM User_Config";
                bool noRow = false;
                using (IDataReader reader = command.ExecuteReader())
                {
                    int x = 0;
                    int y = 0;

                    while (reader.Read())
                    {
                        if (reader.GetInt32(0).CompareTo(0) == 0)
                        {
                            command.CommandText = "INSERT INTO User_Config (Health, Charisma, Coins) VALUES (100, 100, 0);";
                            command.ExecuteNonQuery();

                            noRow = true;
                        }
                    }
                    reader.Close();
                }

                if (noRow)
                {
                    Instance.health = 100;
                    Instance.charisma = 100;
                    Instance.coins = 0;
                    Instance.sticks = 0;
                    Instance.chests = 0;
                }
                else
                {
                    command.CommandText = 
                        "SELECT User_Config.Health, User_Config.Charisma, User_Config.Coins, User_Resources_Association.Number_Resources " +
                        "FROM User_Config, User_Resources_Association " +
                        "WHERE User_Config.ID_User = 1 AND User_Config.ID_User = User_Resources_Association.ID_User AND User_Resources_Association.ID_Resource = 1;";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        int x = 0;
                        int y = 0;

                        while (reader.Read())
                        {
                            Instance.health = reader.GetInt32(0);
                            Instance.charisma = reader.GetInt32(1);
                            Instance.coins = reader.GetInt32(2);
                            Instance.sticks = reader.GetInt32(3);
                            Debug.Log(reader.GetInt32(3));
                            Instance.chests = 0;
                        }
                        reader.Close();
                    }
                }

            }
            connection.Close();
        }
    }

    public static void SaveData(int currentHealth, int currentCharisma, int sticks, int coins)
    {
        // The name of the db.
        string dbName = "URI=file:Yugioh.db";

        // Open the db connection.
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            // Create an sql command that creates a new table.
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE User_Config SET Health = " + currentHealth + ", Charisma = " + currentCharisma + ", Coins = " + 5;
                command.ExecuteNonQuery();

                command.CommandText = "UPDATE User_Resources_Association SET Number_Resources = " + sticks + " WHERE ID_User = 1 AND ID_Resource = 1;";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
}
