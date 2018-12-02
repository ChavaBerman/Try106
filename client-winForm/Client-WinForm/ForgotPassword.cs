using Client_WinForm.Requests;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Client_WinForm
{
    public partial class ForgotPassword : Form
    {
        /// <summary>
        /// Senf email with link to change the password case forget it.
        /// </summary>
        public ForgotPassword()
        {
            InitializeComponent();
            btn_send_email.Enabled = false;
            errorProvider1.SetError(txt_worker_name, "worker name is required");
            errorProvider1.SetError(txt_email_address, "email is required");
        }

        private void btn_send_email_Click(object sender, EventArgs e)
        { 
            MessageBox.Show(WorkerRequests.SendForgotPassword(txt_email_address.Text,txt_worker_name.Text));
         }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            if (txt_worker_name.Text.Length < 2 || txt_worker_name.Text.Length > 15)
            {
                errorProvider1.SetError(txt_worker_name, "worker name must contain between 2-15 charcters");
                errorProvider1.SetError(txt_worker_name, "");
            }
            EnableBtnClick();
        }

        private void txt_email_address_TextChanged(object sender, EventArgs e)
        {
            Regex reg = new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}$", RegexOptions.IgnoreCase); ///Object initialization for Regex 
            if (!reg.IsMatch(txt_email_address.Text))
                errorProvider1.SetError(txt_email_address, "email is not in valid pattern");
            else errorProvider1.SetError(txt_email_address, "");
            EnableBtnClick();
        }

        private void EnableBtnClick()
        {
            if (errorProvider1.GetError(txt_worker_name) == "" && errorProvider1.GetError(txt_email_address) == "")
                btn_send_email.Enabled = true;
        }
    }
    }

