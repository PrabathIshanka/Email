using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Mail;


namespace NewEmail
{
    public partial class Mail : Form
    {
        SAPbobsCOM.Company oCompany;

        public Mail()
        {
            // I use SAPHANA database
            oCompany = new SAPbobsCOM.Company();

            String sqlServer = System.Configuration.ConfigurationManager.AppSettings["SS"];
            if (sqlServer == "SQL2012") oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012;
            else if (sqlServer == "SQL2014") oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2014;
            else if (sqlServer == "SQL2016") oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2016;
            else if (sqlServer == "HANA") oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
            else oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL;

            oCompany.Server = ConfigurationManager.AppSettings["DS"];
            oCompany.language = SAPbobsCOM.BoSuppLangs.ln_English;
            oCompany.UseTrusted = Convert.ToBoolean(ConfigurationManager.AppSettings["AU"]);
            oCompany.DbUserName = ConfigurationManager.AppSettings["UN"];
            oCompany.DbPassword = ConfigurationManager.AppSettings["PW"];
            oCompany.CompanyDB = ConfigurationManager.AppSettings["DB"];
            oCompany.UserName = ConfigurationManager.AppSettings["SAPUN"];
            oCompany.Password = ConfigurationManager.AppSettings["SAPPW"];

            int con = oCompany.Connect();
            string error = oCompany.GetLastErrorDescription();
            //end
            InitializeComponent();
        }

        private void Email_Load(object sender, EventArgs e)
        {

        }
        //file attach link
        private void lblAttachments_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            openFileDialog1.ShowDialog();
        }
        //file name get to texbox
        private void openFileDialog1_FileOk_1(object sender, CancelEventArgs e)
        {
            foreach (string filePath in openFileDialog1.FileNames)
            {
                if (File.Exists(filePath))
                {
                    string fileName = Path.GetFileName(filePath);
                    txtAttach.Text += fileName + Environment.NewLine;

                }
            }

        }

        private void btnSend_Click(object sender, EventArgs e)
        {

            SAPbobsCOM.Recordset oRecordset = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            oRecordset.DoQuery("Select \"E_Mail\" from \"OCRD\" where \"E_Mail\" IS NOT NULL ");

            string[] array = new string[oRecordset.RecordCount];

            for (int x = 0; x < array.Length; x++)
            {
                array[x] = oRecordset.Fields.Item("E_Mail").Value;
                string sam = array[x];

                //Email function
                MailMessage message = new MailMessage();
                message.From = new MailAddress("perfectbss819@gmail.com");
                message.To.Add(sam); 
                message.Subject = txtSub.Text ;
                message.Body = TxtBody.Text;

                //attach file
                foreach (string filePath in openFileDialog1.FileNames)
                {
                    if (File.Exists(filePath))
                    {
                        string fileName = Path.GetFileName(filePath);
                        message.Attachments.Add(new Attachment(filePath));
                    }
                }

                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);    
                System.Net.NetworkCredential basicCredential1 = new
                System.Net.NetworkCredential("perfectbss819@gmail.com", "Admin@1234");
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = basicCredential1;

                try
                {
                    client.Send(message);                
                }

                catch (Exception ex)
                {
                    throw ex;
                }
                //end Email function

                oRecordset.MoveNext();
            }
            MessageBox.Show("OK");

            }


     
        private void label2_Click(object sender, EventArgs e)
        {

        }

     
    }

    }

