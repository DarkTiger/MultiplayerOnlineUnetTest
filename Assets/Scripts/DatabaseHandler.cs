using UnityEngine;
using System;
//using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
//using System.Security.Cryptography;
//using MySql.Data;
//using MySql.Data.MySqlClient;

public class DatabaseHandler : MonoBehaviour 
{
 /*   private string host, database, user, password;
    public bool pooling = true;

    private string connectionString;
    public MySqlConnection conn = null;
    public MySqlCommand cmd = null;
    public MySqlDataReader dReader = null;
    private MD5 md5Hash;
    
	
	void Awake() 
    {
        DontDestroyOnLoad(this.gameObject);
        host = "sql4.freemysqlhosting.net";
        database = "sql499755";
        user = "sql499755";
        password = "XVAVpmpS7W";
                    
        connectionString = "Server=" + host + ";Database=" + database + ";User=" + user + ";Password=" + password + ";Pooling=";

        if (pooling)
        {
            connectionString += "true";
        }
        else
        {
            connectionString += "false";
        }
                        
        conn = new MySqlConnection(connectionString);
        cmd = new MySqlCommand();

        try
        {    
            conn.Open();
            cmd.Connection = conn;
            Debug.Log("MySql State: " + conn.State);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
	}

    void Start()
    {
        /*if (conn.State.ToString() != "Closed")
        {
            cmd.CommandText = "SELECT * FROM MYTEST";
            dReader = cmd.ExecuteReader();

            while (dReader.Read())
            { 
                Debug.Log("ID: " + dReader.GetString(dReader.GetOrdinal("ID")) + "  Name: " + dReader.GetString(dReader.GetOrdinal("Name")));           
            }

            dReader.Close();
        }*/
    /*}

    void InsertDB()
    { 
    
    }

    void OnApplicationQuit()
    {
        if (conn != null)
        {
            if (conn.State.ToString() != "Closed")
            {
                conn.Close();
                conn.Dispose();
                Debug.Log("MySql Connection Closed");
            }
        }
    }*/
}
