using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Configuration;
using System.Web;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace TestRazor 
{
    public class DBRoutines
    {

        private string mConnString = null;
        private MySqlConnection mConn = null;
        private string mError = null;
        private string mDateFormat = null;
        private string mDBDateFormat = null;

        private const string EMAILS_TABLE = "emails";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connstring"></param>
        public DBRoutines(string connstring)
        {
            mConnString = connstring;
            string dateFormat = ConfigurationManager.AppSetting["dateFormat"];
            mDateFormat = dateFormat;
            string dbDateFormat = ConfigurationManager.AppSetting["dbdateFormat"];
            mDBDateFormat = dbDateFormat;
        }

        public string getError() { return mError; }

        public long getCount(string email)
        {
            long retVal = 0;
            mError = null;

            try
            {

                mConn = new MySqlConnection(mConnString);
                mConn.Open();

                string sql = "SELECT count(*) as total from " + EMAILS_TABLE + " where ID > 0";
                if (email != null && email != "")
                    sql += " and email like :emailid";
                MySqlCommand cmd = new MySqlCommand(sql, mConn);
               retVal = Int32.Parse(cmd.ExecuteScalar().ToString());

            }
            catch (Exception ex)
            {
                mError = ex.Message;
            }

            return retVal;

        }

        public DataTable getList(string email, int startRow, int pageSize)
        {
            DataTable dt = new DataTable();
            mError = null;

            try
            {

                mConn = new MySqlConnection(mConnString);
                mConn.Open();

              	string sql = "SELECT * from " + EMAILS_TABLE +  " where ID > 0";
                if (email != null && email != "")
			        sql += " and email like @emailid";
        		sql += " order by email ";
		        sql += " limit " + startRow +  "," + pageSize;
                MySqlCommand cmd = new MySqlCommand(sql, mConn);
                if (email != null && email != "")
                {
                    cmd.Parameters.AddWithValue("@emailid", email);
                    cmd.Prepare();
                }
                MySqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
                
            } catch (Exception ex)
            {
                mError = ex.Message;
            }

            return dt;
        }


        public DataTable getRowByEmailId(string email)
        {
            DataTable dt = new DataTable();
            mError = null;

            try
            {

                mConn = new MySqlConnection(mConnString);
                mConn.Open();

                string sql = "SELECT * from " + EMAILS_TABLE + " where email=@emailid";
                MySqlCommand cmd = new MySqlCommand(sql, mConn);
                if (email != null && email != "")
                {
                    cmd.Parameters.AddWithValue("@emailid", email);
                    cmd.Prepare();
                }
                MySqlDataReader rdr = cmd.ExecuteReader();

                dt.Load(rdr);
            }
            catch (Exception ex)
            {
                mError = ex.Message;
            }
            return dt;
        }

        public DataTable getRowByVerifyCode(string vcode)
        {
            DataTable dt = new DataTable();
            mError = null;

            try
            {

                mConn = new MySqlConnection(mConnString);
                mConn.Open();

                string sql = "SELECT * from " + EMAILS_TABLE + " where vkey=@vkey";
                MySqlCommand cmd = new MySqlCommand(sql, mConn);
               
                cmd.Parameters.AddWithValue("@vkey", vcode);
                cmd.Prepare();
                MySqlDataReader rdr = cmd.ExecuteReader();

                dt.Load(rdr);
            }
            catch (Exception ex)
            {
                mError = ex.Message;
            }
            return dt;
        }

        public bool addRow(EmailData ed)
        {
            mError = null;
            bool retVal = true;
            try
            {

                mConn = new MySqlConnection(mConnString);
                mConn.Open();

                string sql = "INSERT INTO " + EMAILS_TABLE + " (email, name, signup, vkey, verified) values(@email, @name, @signup, @vkey, @verified)";
                MySqlCommand cmd = new MySqlCommand(sql, mConn);

                cmd.Parameters.AddWithValue("@email", ed.email);
                cmd.Parameters.AddWithValue("@name", ed.name);
                cmd.Parameters.AddWithValue("@signup", ed.signup);
                cmd.Parameters.AddWithValue("@vkey", ed.vkey);
                cmd.Parameters.AddWithValue("@verified", ed.verified);
                cmd.Prepare();
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                    retVal = true;
                else
                    retVal = false;
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                retVal = false;
            }
            return retVal;
        }

        public bool verifyAccount(int id)
        {
            mError = null;
            bool retVal = true;
            try
            {

                mConn = new MySqlConnection(mConnString);
                mConn.Open();

                string sql = "UPDATE " + EMAILS_TABLE + " set verified = 1 where ID=@id";
                MySqlCommand cmd = new MySqlCommand(sql, mConn);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Prepare();
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                    retVal = true;
                else
                    retVal = false;
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                retVal = false;
            }
            return retVal;
        }
    }
}
