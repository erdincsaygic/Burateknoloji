using StilPay.BLL.Abstract;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StilPay.Desktop.Callback
{
    public partial class CallbackList : Form
    {
        private readonly ICallbackResponseLogManager _callbackResponseLogManager;
        public CallbackList(ICallbackResponseLogManager callbackResponseLogManager)
        {
            InitializeComponent();
            _callbackResponseLogManager = callbackResponseLogManager;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var callbackLogs = _callbackResponseLogManager.GetList(new List<FieldParameter>() {
                new FieldParameter("ServiceType", Enums.FieldType.NVarChar, null),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null) ,
                new FieldParameter("TransactionType", Enums.FieldType.NVarChar, null)
            });

            dataGridView1.DataSource = callbackLogs;
        }
    }
}
