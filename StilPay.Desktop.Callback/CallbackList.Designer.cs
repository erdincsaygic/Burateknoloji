using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Http;
using StilPay.BLL.Abstract;
using StilPay.Utility.Worker;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;

namespace StilPay.Desktop.Callback
{
    public partial class CallbackList
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView1 = new DataGridView();
            MDate = new DataGridViewTextBoxColumn();
            Company = new DataGridViewTextBoxColumn();
            CDate = new DataGridViewTextBoxColumn();
            TransactionID = new DataGridViewTextBoxColumn();
            ServiceType = new DataGridViewTextBoxColumn();
            TransactionType = new DataGridViewTextBoxColumn();
            Callback = new DataGridViewTextBoxColumn();
            CallbackStatus = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Company, CDate, TransactionID, ServiceType, TransactionType, Callback, CallbackStatus });
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new System.Drawing.Point(0, 0);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new System.Drawing.Size(836, 450);
            dataGridView1.TabIndex = 0;
            dataGridView1.AutoGenerateColumns = false;
            // 
            // MDate
            // 
            MDate.DataPropertyName = "MDate";
            MDate.HeaderText = "Son Atılan Callback Tarihi";
            MDate.Name = "MDate";
            MDate.ReadOnly = true;
            // 
            // Company
            // 
            Company.DataPropertyName = "Company";
            Company.HeaderText = "Üye İşyeri";
            Company.Name = "Company";
            Company.ReadOnly = true;
            // 
            // CDate
            // 
            CDate.DataPropertyName = "CDate";
            CDate.HeaderText = "İlk Atılan Callback Tarihi";
            CDate.Name = "CDate";
            CDate.ReadOnly = true;
            // 
            // TransactionID
            // 
            TransactionID.DataPropertyName = "TransactionID";
            TransactionID.HeaderText = "İşlem Numarası";
            TransactionID.Name = "TransactionID";
            TransactionID.ReadOnly = true;
            // 
            // ServiceType
            // 
            ServiceType.DataPropertyName = "ServiceType";
            ServiceType.HeaderText = "Servis Tipi";
            ServiceType.Name = "ServiceType";
            ServiceType.ReadOnly = true;
            // 
            // TransactionType
            // 
            TransactionType.DataPropertyName = "TransactionType";
            TransactionType.HeaderText = "İşlem Tipi";
            TransactionType.Name = "TransactionType";
            TransactionType.ReadOnly = true;
            // 
            // Callback
            // 
            Callback.DataPropertyName = "Callback";
            Callback.HeaderText = "Callback";
            Callback.Name = "Callback";
            Callback.ReadOnly = true;
            // 
            // CallbackStatus
            // 
            CallbackStatus.DataPropertyName = "CallbackStatus";
            CallbackStatus.HeaderText = "Callback Durumu";
            CallbackStatus.Name = "CallbackStatus";
            CallbackStatus.ReadOnly = true;
            // 
            // CallbackList
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(836, 450);
            Controls.Add(dataGridView1);
            Name = "CallbackList";
            Text = "Stilpay Üye İşyerine Atılan Callback ve Durumları";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }
        #endregion

        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn MDate;
        private DataGridViewTextBoxColumn Company;
        private DataGridViewTextBoxColumn CDate;
        private DataGridViewTextBoxColumn TransactionID;
        private DataGridViewTextBoxColumn ServiceType;
        private DataGridViewTextBoxColumn TransactionType;
        private DataGridViewTextBoxColumn Callback;
        private DataGridViewTextBoxColumn CallbackStatus;
    }
}
