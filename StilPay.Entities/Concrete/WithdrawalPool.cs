using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Entities.Concrete
{
    public class WithdrawalPool : BaseEntity
    {
        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime CDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "MDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime? MDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionDate", FieldType = Enums.FieldType.DateTime, Description = "", Nullable = false)]
        public DateTime TransactionDate { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "IDBank", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string IDBank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = true, Name = "Bank", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Bank { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ReceiverName", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string ReceiverName { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ReceiverIban", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string ReceiverIban { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "TransactionKey", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = false)]
        public string TransactionKey { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Amount", FieldType = Enums.FieldType.Decimal, Description = "", Nullable = false)]
        public decimal Amount { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Description", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string Description { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ResponseDescription", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string ResponseDescription { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "ResponseTransactionNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string ResponseTransactionNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "RequestNr", FieldType = Enums.FieldType.NVarChar, Description = "", Nullable = true)]
        public string RequestNr { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Status", FieldType = Enums.FieldType.Tinyint, Description = "", Nullable = false)]
        public byte Status { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "Company", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string Company { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "CompanyBankAccountID", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string CompanyBankAccountID { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IDCompany", FieldType = Enums.FieldType.None, Description = "", Nullable = false)]
        public string IDCompany { get; set; }

        [FieldAttribute(AutoIncrement = false, PK = false, FK = false, Name = "IsRebate", FieldType = Enums.FieldType.Bit, Description = "", Nullable = false)]
        public bool IsRebate { get; set; }
    }
}
