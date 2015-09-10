using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            m_Worker = new BackgroundWorker();
            m_Worker.DoWork += new DoWorkEventHandler(m_Worker_DoWork);
            m_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_Worker_RunWorkerCompleted);
        }
        UInt64 node_id = 0;
        BackgroundWorker m_Worker;

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "*.csv|*.CSV";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {                
                if(sender.Equals(cmdBrowse))
                {
                    txtDataset.Text = openFileDialog1.FileName;
                }
                else if (sender.Equals(cmdBrowseCO))
                {
                    txtBase.Text = openFileDialog1.FileName;
                }
                else if (sender.Equals(cmdBrowseDC))
                {
                    txtChanges.Text = openFileDialog1.FileName;
                }
            }            
        }     



#region "Processing"

        private void cmdProcessSingle_Click(object sender, EventArgs e)
        {
            try
            {
                String curr = "";
                System.IO.StreamWriter outfile = new System.IO.StreamWriter("gatineau.osm");
                System.IO.StreamReader file = new System.IO.StreamReader(txtDataset.Text);

                write_fileheader(ref outfile);
                file.ReadLine(); // Skip header
                curr = file.ReadLine();
                while (file.EndOfStream == false)
                {
                    String[] info = curr.Split('|');
                    write_addressnode(ref outfile, ref info);

                    curr = file.ReadLine();
                }
                outfile.WriteLine("</osm>");
                outfile.Flush();
                outfile.Close();
                file.Close();
                MessageBox.Show("Output file written, have a nice day!");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmdCompare_Click(object sender, EventArgs e)
        {
            lblWait.Visible = true;
            m_Worker.RunWorkerAsync(ckbReload.Checked);               
        }

        /// <summary>
        /// On completed do the appropriate task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblWait.Visible = false;
        }

        /// <summary>
        /// Time consuming operations go here
        /// i.e. Database operations,Reporting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\compare.db;FailIfMissing=false;Version=3");
            if((bool)e.Argument){
            check_db(ref conn);

            ReadGatFile(ref conn, txtBase.Text, "older_data");
            ReadGatFile(ref conn, txtBase.Text, "newer_data");
            }
            DataTable dt = CompareData(ref conn);
        }

#endregion


#region "Helper Functions"
        private DataTable CompareData(ref SQLiteConnection conn)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Entity_ID").Caption="New Entity ID";
            dt.Columns.Add("OEntity_ID").Caption="Old Entity ID";
            dt.Columns.Add("Addr_ID").Caption = "New Address";
            dt.Columns.Add("OAddr_ID").Caption = "Old Address";
            dt.Columns.Add("Geom").Caption = "New Geo point";
            dt.Columns.Add("OGeom").Caption = "Old Geo point";
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(conn);
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmd.CommandText = @"select * from (
                                                    select a.ENTITID,a.CODEID,a.MUNID,a.NUMERO_CIV,a.GENERIQUE,a.LIAISON,a.SPECIFIQUE,a.DIRECTION,a.ADDR_COMPLE,a.RUESID,a.GEOM,b.ENTITID ENTITID_1,b.CODEID CODEID_1,b.MUNID MUNID_1,b.NUMERO_CIV NUMERO_CIV_1,b.GENERIQUE GENERIQUE_1,b.LIAISON LIAISON_1,b.SPECIFIQUE SPECIFIQUE_1,b.DIRECTION DIRECTION_1,b.ADDR_COMPLE ADDR_COMPLE_1,b.RUESID RUESID_1,b.GEOM GEOM_1 from newer_data a 
                                                    left join older_data b 
                                                    on a.ENTITE_ID=b.ENTITE_ID
                                                    where (b.ENTITEID is null) or (a.ADDR_COMPLE<>b.ADDR_COMPLE)
                                                    UNION ALL
                                                    select b.ENTITID,b.CODEID,b.MUNID,b.NUMERO_CIV,b.GENERIQUE,b.LIAISON,b.SPECIFIQUE,b.DIRECTION,b.ADDR_COMPLE,b.RUESID,b.GEOM,a.ENTITID ENTITID_1,a.CODEID CODEID_1,a.MUNID MUNID_1,a.NUMERO_CIV NUMERO_CIV_1,a.GENERIQUE GENERIQUE_1,a.LIAISON LIAISON_1,a.SPECIFIQUE SPECIFIQUE_1,a.DIRECTION DIRECTION_1,a.ADDR_COMPLE ADDR_COMPLE_1,a.RUESID RUESID_1,a.GEOM GEOM_1 from older_data a
                                                    left join newer_data b
                                                    on a.ENTITE_ID=b.ENTITE_ID
                                                    where (b.ENTITEID is null)                                            
                                                   ) as BQ
                                      order by ADDR_COMPLE_1";
                SQLiteDataReader dr = cmd.ExecuteReader();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }


            return dt;
        }
        private void ReadGatFile(ref SQLiteConnection conn,String filepath,String SQLTableName)
        {
            SQLiteBulkInsert target = new SQLiteBulkInsert(conn, SQLTableName);
            AddParameters(target);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            System.IO.StreamReader sr = new System.IO.StreamReader(filepath);
            try
            {
                sr.ReadLine(); //Skip header
                String line;
                do
                {
                    line = sr.ReadLine();
                    target.Insert(line.Split('|'));
                }
                while (sr.EndOfStream == false);
                target.Flush();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                target.Rollback();
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

        }
        private void AddParameters(SQLiteBulkInsert target)
        {
            target.AddParameter("ENTITEID", DbType.String);
            target.AddParameter("CODEID", DbType.String);
            target.AddParameter("MUNID", DbType.String);
            target.AddParameter("NUMERO_CIV", DbType.String);
            target.AddParameter("GENERIQUE", DbType.String);
            target.AddParameter("LIAISON", DbType.String);
            target.AddParameter("SPECIFIQUE", DbType.String);
            target.AddParameter("DIRECTION", DbType.String);
            target.AddParameter("ADDR_COMPLE", DbType.String);
            target.AddParameter("RUESID", DbType.String);
            target.AddParameter("GEOM", DbType.String);
            
        }

        private void check_db(ref SQLiteConnection conn)
        {
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(conn);
            String createtbl = "create table {0}(ENTITEID TEXT PRIMARY KEY,CODEID TEXT,MUNID TEXT,NUMERO_CIV TEXT,GENERIQUE TEXT,LIAISON TEXT,SPECIFIQUE TEXT,DIRECTION TEXT,ADDR_COMPLE TEXT,RUESID TEXT,GEOM TEXT)";
            String emptytbl = "delete from {0}";
            String findtbl = "select name from sqlite_master where type='table' and name='{0}'";
            
            cmd.CommandText=String.Format(findtbl,"older_data");
            Object name = cmd.ExecuteScalar();
            if (name == null)
            {
                cmd.CommandText = String.Format(createtbl,"older_data");
                cmd.ExecuteNonQuery();
            }
            else
            {
                cmd.CommandText = String.Format(emptytbl, "older_data");
                cmd.ExecuteNonQuery();
            }

            cmd.CommandText = String.Format(findtbl, "newer_data");
            name = cmd.ExecuteScalar();
            if (name == null)
            {
                cmd.CommandText = String.Format(createtbl, "newer_data");
                cmd.ExecuteNonQuery();
            }
            else
            {
                cmd.CommandText = String.Format(emptytbl, "newer_data");
                cmd.ExecuteNonQuery();
            }

            conn.Close();

        }
        private void write_fileheader(ref System.IO.StreamWriter sw)
        {
            sw.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
            sw.WriteLine("<osm version='0.6' upload='true' generator='JOSM'>");

        }

        private String extract_lat_long(String Coordinates, Boolean Latitude)
        {
            Coordinates = Coordinates.Replace("POINT (", "").Replace(")", "");
            if (Latitude)
            {
                return Coordinates.Split(' ')[1];
            }
            else
            {
                return Coordinates.Split(' ')[0];
            }
        }

        private void write_addressnode(ref System.IO.StreamWriter sw, ref String[] info)
        {
            node_id++;
            sw.WriteLine("<node id='-" + node_id.ToString() + "' lat='" + extract_lat_long(info[10], true) + "' lon='" + extract_lat_long(info[10], false) + "'>");
            sw.WriteLine("\t<tag k='addr:city' v='Gatineau' />");
            sw.WriteLine("\t<tag k='addr:housenumber' v='" + info[3].ToString() + "' />");
            sw.WriteLine("\t<tag k='addr:street' v='" + iif(info[4].Replace("'", "&apos;").Trim() != String.Empty, info[4].Replace("'", "&apos;").Trim() + " ", "") + iif(info[5].Replace("'", "&apos;").Trim() != String.Empty, info[5].Replace("'", "&apos;").Trim() + " ", "") + iif(info[6].Replace("'", "&apos;").Trim() != String.Empty, info[6].Replace("'", "&apos;").Trim(), "") + "' />");
            sw.WriteLine("\t<tag k='addr:source' v='Gatineau.ca/donneesouvertes 05Sept2015' />");
            sw.WriteLine("</node>");
        } 

        private String iif(Boolean b, String Left, String Right)
        {
            if (b)
                return Left;
            else
                return Right;
        }

#endregion
    }
}
