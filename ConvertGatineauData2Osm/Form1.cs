using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;
using System.Net;
using System.Web;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Drawing;
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
            m_Worker.ProgressChanged += m_Worker_ProgressChanged;
            lblWait.Left = (this.ClientSize.Width - lblWait.Width) / 2;
            lblWait.Top = (this.ClientSize.Height - lblWait.Height) / 2;
        }
        private enum OSMEntityType{node,way,relation}
        private enum WorkType { compare, writeout };


        UInt64 m_node_id = 0;
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
            m_Worker.WorkerReportsProgress = true;
            List<Object> args = new List<Object>();
            args.Add(WorkType.compare);
            args.Add(ckbReload.Checked);           
            m_Worker.RunWorkerAsync(args);
        }

        private void cmdWrite_Click(object sender, EventArgs e)
        {
            lblWait.Visible = true;
            m_Worker.WorkerReportsProgress = true;
            List<Object> args = new List<Object>();
            args.Add(WorkType.writeout);
            args.Add(dgvCompare.DataSource);
            m_Worker.RunWorkerAsync(args);
        }


        void m_Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblWait.Text = e.ProgressPercentage + "%..." + e.UserState.ToString();
        }

        /// <summary>
        /// On completed do the appropriate task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblWait.Visible = false;
            switch ((WorkType)((List<Object>)e.Result)[0])
            {
                case WorkType.compare:
                    dgvCompare.AutoGenerateColumns = false;
                    dgvCompare.Columns.Clear();
                    DataTable src = (DataTable)((List<Object>)e.Result)[1];
                    foreach (DataColumn dc in src.Columns)
                    {
                        if (dc.Caption != "Hidden")
                        {
                            DataGridViewColumn dgvc = new DataGridViewTextBoxColumn();
                            dgvc.Name=dc.ColumnName;
                            dgvc.HeaderText=dc.Caption;
                            dgvc.DataPropertyName=dc.ColumnName;
                            dgvc.ReadOnly = true;
                            dgvCompare.Columns.Add(dgvc);
                        }
                    }
                    dgvCompare.DataSource = src;
                    
                break;
                case WorkType.writeout:
                break;
            }                      
            
        }

        /// <summary>
        /// Time consuming operations go here
        /// i.e. Database operations,Reporting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Object> args = (List<Object>)e.Argument;
            List<Object> result = new List<Object>();
            result.Add((WorkType)args[0]);
            switch((WorkType)args[0]){
               case WorkType.compare:            
                   SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\compare.db;FailIfMissing=false;Version=3");
                   if ((bool)args[1])
                   {
                       check_db(ref conn);

                       m_Worker.ReportProgress(0, "Loading Old Changeset");
                       ReadGatFile(ref conn, txtBase.Text, "older_data");
                       m_Worker.ReportProgress(50, "Loading New Changeset");
                       ReadGatFile(ref conn, txtChanges.Text, "newer_data");
                       m_Worker.ReportProgress(90, "Comparing Data");
                   }
                   DataTable dt = CompareData(ref conn);
                   m_Worker.ReportProgress(100, "Done!");
                   result.Add(dt);
                break;
               case WorkType.writeout:
                    //Write Header
                    StreamWriter sw = new StreamWriter("gatineauchangeset.osm");
                    write_fileheader(ref sw);
                    //Process entry by entry
                    write_changeset(ref sw,(DataSet)((List<Object>)args)[1]);
                    //Write Footer
                    sw.WriteLine("</osm>");
                    sw.Flush();
                    sw.Close();
               break;
            }
           e.Result = result;
           
        }

#endregion


#region "Helper Functions"
        private String getOSMID(String Addr_No, String Street,ref OSMEntityType Type)
        {
            String searchquery = "http://overpass-api.de/api/interpreter?data="+ System.Web.HttpUtility.UrlEncode(String.Format(@"[out:xml][timeout:2500];area(3605356213)->.searchArea;
                                    (
                                        node[""addr:housenumber""=""{0}""][""addr:street""=""{1}""](area.searchArea);
                                        way[""addr:housenumber""=""{0}""][""addr:street""=""{1}""](area.searchArea);
                                    );
                                    out body;
                                    >;
                                    out skel qt;",Addr_No,Street));
            String responsexml;
            // used to build entire input
            StringBuilder sb = new StringBuilder();

            // used on each read operation
            byte[] buf = new byte[8192];

            // prepare the web page we will be asking for
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(searchquery);

            // execute the request
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // we will read data via the response stream
            Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;

            do
            {
                // fill the buffer with data
                count = resStream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0)
                {
                    // translate from bytes to UTF-8 text
                    tempString = Encoding.UTF8.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
            }
            while (count > 0); // any more data to read?

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(sb.ToString());
            sb.Clear();//output for node or way xml

            //Address is a Node ***************************************
            //Get nodes that have children : we need Tag elements
            XmlNodeList nodes = xml.SelectNodes("//osm/node");
            foreach (XmlNode node in nodes)
            {
                //Check if data matches query address
                if(node.InnerXml.ToString().Contains("<tag k=\"addr:source\" v=\"Gatineau.ca/donneesouvertes"))
                {   //add to return buffer
                    sb.AppendLine(node.OuterXml.ToString());
                    Type = OSMEntityType.node;
                }
            }       
            //End Address is a Node ***********************************

            
            //Address is a Way ****************************************
            nodes = xml.SelectNodes("//osm/way");
            List<String> refNodes = new List<String>();
            for(int i=0;i<nodes.Count;i++)
            {
                //Add nodes that might be associated with way
                if (nodes[i].InnerXml.ToString().Contains(String.Format("<tag k=\"addr:housenumber\" v=\"{0}\" />", Addr_No)) && nodes[i].InnerXml.ToString().Contains(String.Format("<tag k=\"addr:street\" v=\"{0}\" />", Street)))
                {
                    Type = OSMEntityType.way;
                    //Get Node references for the way
                    XmlNodeList ways = nodes[i].SelectNodes("nd");
                    foreach (XmlNode nd in ways)
                    {
                        if (nd.Attributes.Count >= 1)
                        {
                            refNodes.Add(nd.Attributes["ref"].Value);
                        }
                    }
                    ways = null;
                    //Download nodes associated with way (to get version code and other info)
                    XmlNodeList allNodeTags = xml.SelectNodes("//osm/node");
                    foreach (XmlNode nd in allNodeTags)
                    {
                        if (refNodes.Contains(nd.Attributes["id"].Value))
                        {
                            sb.AppendLine(getOSMVersionIDs(nd.Attributes["id"].Value,OSMEntityType.node));
                        }
                    }
                    sb.AppendLine(getOSMVersionIDs(nodes[i].Attributes["id"].Value, OSMEntityType.way));
                }
            }  
            //Get way tags
            //Check if data matches query address
            //Add to return buffer
            //fetch nodes associated with way.
            //End Address is a Way ************************************

            return sb.ToString();

        }
        private String getOSMVersionIDs(String ID, OSMEntityType Type)
        {

            String searchquery = "http://api.openstreetmap.org/" + String.Format(@"api/0.6/{0}/{1}", Type.ToString(), ID);
            // used to build entire input
            StringBuilder sb = new StringBuilder();

            // used on each read operation
            byte[] buf = new byte[8192];

            // prepare the web page we will be asking for
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(searchquery);

            // execute the request
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // we will read data via the response stream
            Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;

            do
            {
                // fill the buffer with data
                count = resStream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0)
                {
                    // translate from bytes to UTF-8 text
                    tempString = Encoding.UTF8.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
            }
            while (count > 0); // any more data to read?
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(sb.ToString());
            return xml.SelectNodes("//osm/" + Type.ToString())[0].OuterXml;
        }

        private void write_changeset(ref StreamWriter sw, DataSet dt)
        {

        }
        private DataTable CompareData(ref SQLiteConnection conn)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Entity_ID").Caption="New Entity ID";
            dt.Columns.Add("OEntity_ID").Caption="Old Entity ID";
            dt.Columns.Add("Addr_ID").Caption = "New Address";
            dt.Columns.Add("OAddr_ID").Caption = "Old Address";
            dt.Columns.Add("Geom").Caption = "New Geo point";
            dt.Columns.Add("OGeom").Caption = "Old Geo point";
            dt.Columns.Add("ChangeState").Caption = "Change State";
            dt.Columns.Add("Addr_No").Caption = "Hidden";
            dt.Columns.Add("OAddr_No").Caption = "Hidden";
            dt.Columns.Add("StreetName").Caption = "Hidden";
            dt.Columns.Add("OStreetName").Caption = "Hidden";
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(conn);
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmd.CommandText = @"select * from (
                                                    select a.ENTITEID,a.CODEID,a.MUNID,a.NUMERO_CIV,a.GENERIQUE,a.LIAISON,a.SPECIFIQUE,a.DIRECTION,a.ADDR_COMPLE,a.RUESID,a.GEOM,a.NUMERO_CIV, REPLACE (IFNULL(a.GENERIQUE,null)||case when a.GENERIQUE is not null then ' ' else null end|| IFNULL(a.LIAISON,null)||case when a.LIAISON is not null then ' ' else null end||IFNULL(a.SPECIFIQUE,null)||case when a.SPECIFIQUE is not null and a.DIRECTION is not null then ' ' else null end||IFNULL(a.DIRECTION,null),'  ',' ') Street,b.ENTITEID ENTITEID_1,b.CODEID CODEID_1,b.MUNID MUNID_1,b.NUMERO_CIV NUMERO_CIV_1,b.GENERIQUE GENERIQUE_1,b.LIAISON LIAISON_1,b.SPECIFIQUE SPECIFIQUE_1,b.DIRECTION DIRECTION_1,b.ADDR_COMPLE ADDR_COMPLE_1,b.RUESID RUESID_1,b.GEOM GEOM_1,b.NUMERO_CIV NUMERO_CIV_1, REPLACE(IFNULL(b.GENERIQUE,null)||case when b.GENERIQUE is not null then ' ' else null end||IFNULL(b.LIAISON,null)||case when b.LIAISON is not null then ' ' else null end||IFNULL(b.SPECIFIQUE,null)||case when b.SPECIFIQUE is not null and b.DIRECTION is not null then ' ' else null end||IFNULL(b.DIRECTION,null),'  ',' ') Street_1 from newer_data a 
                                                    left join older_data b 
                                                    on a.ENTITEID=b.ENTITEID
                                                    where (b.ENTITEID is null) or (a.ADDR_COMPLE<>b.ADDR_COMPLE)
                                                    UNION ALL
                                                    select b.ENTITEID,b.CODEID,b.MUNID,b.NUMERO_CIV,b.GENERIQUE,b.LIAISON,b.SPECIFIQUE,b.DIRECTION,b.ADDR_COMPLE,b.RUESID,b.GEOM,b.NUMERO_CIV, REPLACE(IFNULL(b.GENERIQUE,null)||case when b.GENERIQUE is not null then ' ' else null end||IFNULL(b.LIAISON,null)||case when b.LIAISON is not null then ' ' else null end||IFNULL(b.SPECIFIQUE,null)||case when b.SPECIFIQUE is not null and b.DIRECTION is not null then ' ' else null end||IFNULL(b.DIRECTION,null),'  ',' ') Street,a.ENTITEID ENTITEID_1,a.CODEID CODEID_1,a.MUNID MUNID_1,a.NUMERO_CIV NUMERO_CIV_1,a.GENERIQUE GENERIQUE_1,a.LIAISON LIAISON_1,a.SPECIFIQUE SPECIFIQUE_1,a.DIRECTION DIRECTION_1,a.ADDR_COMPLE ADDR_COMPLE_1,a.RUESID RUESID_1,a.GEOM GEOM_1,a.NUMERO_CIV NUMERO_CIV_1, REPLACE(IFNULL(a.GENERIQUE,null)||case when a.GENERIQUE is not null then ' ' else null end||IFNULL(a.LIAISON,null)||case when a.LIAISON is not null then ' ' else null end||IFNULL(a.SPECIFIQUE,null)||case when a.SPECIFIQUE is not null and a.DIRECTION is not null then ' ' else null end||IFNULL(a.DIRECTION,null),'  ', ' ') Street_1 from older_data a
                                                    left join newer_data b
                                                    on a.ENTITEID=b.ENTITEID
                                                    where (b.ENTITEID is null)                                            
                                                   ) as BQ
                                      order by ADDR_COMPLE_1";
                SQLiteDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DataRow dtr = dt.NewRow();
                    dtr["Entity_ID"] = dr["ENTITEID"];
                    dtr["OEntity_ID"] = dr["ENTITEID_1"];
                    dtr["Addr_ID"] = dr["ADDR_COMPLE"];
                    dtr["OAddr_ID"] = dr["ADDR_COMPLE_1"];
                    dtr["Geom"] = dr["GEOM"];
                    dtr["OGeom"] = dr["GEOM_1"];
                    dtr["ChangeState"] = GetChangeState(ref dr);
                    dtr["Addr_No"] = dr["NUMERO_CIV"];
                    dtr["OAddr_No"] = dr["NUMERO_CIV_1"];
                    dtr["StreetName"]= dr["Street"];
                    dtr["OStreetName"] = dr["Street_1"];
                    dt.Rows.Add(dtr);
                    
                }
               

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
        private String GetChangeState(ref SQLiteDataReader dr)
        {
            if (dr["ENTITEID"] == DBNull.Value)
            {
                return "Deletion";
            }
            else if (dr["ENTITEID_1"] == DBNull.Value)
            {
                return "New Node";
            }
            else if (dr["ADDR_COMPLE"] != dr["ADDR_COMPLE_1"])
            {
                return "Address Change";
            }
            return "";
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
            cmd.CommandText = "VACUUM";
            cmd.ExecuteNonQuery();

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
            m_node_id++;
            sw.WriteLine("<node id='-" + m_node_id.ToString() + "' lat='" + extract_lat_long(info[10], true) + "' lon='" + extract_lat_long(info[10], false) + "'>");
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
