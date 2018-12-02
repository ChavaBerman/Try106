using System;
using System.Windows.Forms;

namespace Client_WinForm.Worker
{
    public partial class ApplyToManager : Form
    {
        Models.Worker worker;
        public ApplyToManager(Models.Worker worker)
        {
            this.worker = worker;
            InitializeComponent();
        }
        //powering request to server which sends email to current worker's manager
        private void btn_sendApply_Click(object sender, EventArgs e)
        {
           if( Requests.WorkerRequests.sendMessageToManager(worker.WorkerId, txt_message.Text,txt_subject.Text!="" ? txt_subject.Text : "No Subject"))
                MessageBox.Show("Sent successfuly");
            else MessageBox.Show("Did not succeed to send your message, try again.");
        }
    }
}
