using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2010.Excel;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using StilPay.Utility.AutoNotificationCheckReferenceNr;
using StilPay.Utility.Helper;
using StilPay.Utility.JobBank;
using StilPay.Utility.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.Xml;
using System.ServiceModel.Channels;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace StilPay.Utility.Worker
{
    public class tSQLBankManager
    {
        private static string _defaultConecction;
        static tSQLBankManager()
        {
            _defaultConecction = tAES3.Instance.Decrypt();
        }

        public static CompanyIntegrationModel GetCompanyIntegration(string serviceId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("Select * From CompanyIntegrations Where ServiceID='{0}'", serviceId);

                    DataTable dt = new DataTable();
                    SqlDataAdapter adptr = new SqlDataAdapter(cmd);
                    adptr.Fill(dt);

                    connection.Close();

                    return new CompanyIntegrationModel()
                    {
                        ID = dt.Rows[0]["ID"].ToString(),
                        ServiceID = serviceId,
                        SecretKey = dt.Rows[0]["SecretKey"].ToString(),
                        SiteUrl = dt.Rows[0]["SiteUrl"] == DBNull.Value ? null : dt.Rows[0]["SiteUrl"].ToString(),
                        CallbackUrl = dt.Rows[0]["CallbackUrl"] == DBNull.Value ? null : dt.Rows[0]["CallbackUrl"].ToString(),
                        WithdrawalRequestCallback = dt.Rows[0]["WithdrawalRequestCallback"] == DBNull.Value ? null : dt.Rows[0]["WithdrawalRequestCallback"].ToString(),
                    };
                }
            }
            catch { }

            return null;
        }

        public static CompanyIntegrationModel GetCompanyIntegrationByID(string id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("Select * From CompanyIntegrations Where ID='{0}'", id);

                    DataTable dt = new DataTable();
                    SqlDataAdapter adptr = new SqlDataAdapter(cmd);
                    adptr.Fill(dt);

                    connection.Close();

                    return new CompanyIntegrationModel()
                    {
                        ID = dt.Rows[0]["ID"].ToString(),
                        ServiceID = dt.Rows[0]["ServiceID"].ToString(),
                        SecretKey = dt.Rows[0]["SecretKey"].ToString(),
                        SiteUrl = dt.Rows[0]["SiteUrl"] == DBNull.Value ? null : dt.Rows[0]["SiteUrl"].ToString(),
                        CallbackUrl = dt.Rows[0]["CallbackUrl"] == DBNull.Value ? null : dt.Rows[0]["CallbackUrl"].ToString(),
                        WithdrawalRequestCallback = dt.Rows[0]["WithdrawalRequestCallback"] == DBNull.Value ? null : dt.Rows[0]["WithdrawalRequestCallback"].ToString(),
                    };
                }
            }
            catch { }

            return null;
        }

        public static bool HasNotificationTransaction(string transactionKey)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("Select Count(*) From NotificationTransactions Where TransactionKey='{0}'", transactionKey);
                    var count = (int)cmd.ExecuteScalar();

                    connection.Close();

                    if (count == 0) return false;
                }
            }
            catch { }

            return true;
        }

        public static bool HasPaymentTransaction(string transactionKey)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("Select Count(*) From PaymentNotifications Where TransactionKey='{0}'", transactionKey);
                    var count = (int)cmd.ExecuteScalar();

                    connection.Close();

                    if (count == 0) return false;
                }
            }
            catch { }

            return true;
        }

        public static bool HasPaymentTransferPool(string transactionKey)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("Select Count(*) From PaymentTransferPools Where TransactionKey='{0}'", transactionKey);
                    var count = (int)cmd.ExecuteScalar();

                    connection.Close();

                    if (count == 0) return false;
                }
            }
            catch { }

            return true;
        }

        public static bool CheckCardBinNumber(string cardBinNumber)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("Select Count(*) From CreditCardBinList Where Bin='{0}'", cardBinNumber);
                    var count = (int)cmd.ExecuteScalar();

                    connection.Close();

                    if (count == 0) return false;
                }
            }
            catch { }

            return true;
        }

        #region Çekim Talebi ve İade İşlemlerinin Havuza Kayıt ve Havuzdan Onay Kodları
        public static bool HasWithdrawalPool(string transactionKey)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("Select Count(*) From WithdrawalPools Where TransactionKey='{0}'", transactionKey);
                    var count = (int)cmd.ExecuteScalar();

                    connection.Close();

                    if (count == 0) return false;
                }
            }
            catch { }

            return true;
        }

        public static bool HasWithdrawalPoolReverse(string transactionKey)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("SELECT COUNT(*) FROM WithdrawalPools WHERE Description LIKE '%TERS: {0}%' and TransactionDate >= DATEADD(day, -1, TransactionDate)", transactionKey);
                    var count = (int)cmd.ExecuteScalar();

                    connection.Close();

                    if (count == 0) return false;
                }
            }
            catch { }

            return true;
        }

        public static string GetWithdrawalPoolReverseID(string transactionKey)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("SELECT ID FROM WithdrawalPools WHERE Description LIKE '%TERS: {0}%' and TransactionDate >= DATEADD(day, -1, TransactionDate)", transactionKey);
                    var id = cmd.ExecuteScalar().ToString();

                    connection.Close();

                    return id;
                }
            }
            catch { }

            return null;
        }

        public static List<WithdrawalPoolModel> GetWithdrawalPool()
        {
            try
            {             
                using SqlConnection connection = new SqlConnection(_defaultConecction);
                connection.Open();

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                //cmd.CommandText = string.Format("Select * From WithdrawalPools Where TransactionKey='{0}', Amount='{1}', ReceiverName='{2}'", transactionKey, amount, receiverName);

                cmd.CommandText = string.Format("Select * From WithdrawalPools " +
                "Where Status=1 " +
                "AND Description not like '%Gönderen Hesap No: 98681302,Ek No:1,Alıcı Hesap No: 98681302,Ek No:2 Para Transferi%' " +
                "AND Description not like '%TERS:%' " +
                "Order By TransactionDate");

                DataTable dt = new DataTable();
                SqlDataAdapter adptr = new SqlDataAdapter(cmd);
                adptr.Fill(dt);

                connection.Close();

                var list = new List<WithdrawalPoolModel>();

                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new WithdrawalPoolModel
                    {
                        ID = dr["ID"] == DBNull.Value ? null : dr["ID"].ToString(),
                        CDate = Convert.ToDateTime(dr["CDate"].ToString()),
                        MDate = dr["MDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(dr["MDate"].ToString()),
                        TransactionDate = Convert.ToDateTime(dr["TransactionDate"].ToString()),
                        IDBank = dr["IDBank"] == DBNull.Value ? null : dr["IDBank"].ToString(),
                        ReceiverName = dr["ReceiverName"] == DBNull.Value ? null : dr["ReceiverName"].ToString(),
                        ReceiverIban = dr["ReceiverIban"] == DBNull.Value ? null : dr["ReceiverIban"].ToString(),
                        TransactionKey = dr["TransactionKey"] == DBNull.Value ? null : dr["TransactionKey"].ToString(),
                        Amount = dr["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Amount"].ToString()),
                        Description = dr["Description"] == DBNull.Value ? null : dr["Description"].ToString(),
                        Status = dr["Status"] == DBNull.Value ? (byte)0 : Convert.ToByte(dr["Status"].ToString()),
                        CompanyBankAccountID = dr["CompanyBankAccountID"] == DBNull.Value ? null : dr["CompanyBankAccountID"].ToString()
                    });
                }

                return list;
            }
            catch { }


            return new List<WithdrawalPoolModel>();
        }

        public static List<WithdrawalAndRebateRequestModel> GetInProcessWithdrawalAndRebateRequests()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    using SqlCommand cmd = new SqlCommand("GetInProcessWithdrawalAndRebateRequests", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    DataTable dt = new DataTable();
                    SqlDataAdapter adptr = new SqlDataAdapter(cmd);
                    adptr.Fill(dt);

                    connection.Close();

                    var list = new List<WithdrawalAndRebateRequestModel>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new WithdrawalAndRebateRequestModel
                        {
                            ID = dr["ID"].ToString(),
                            MDate = Convert.ToDateTime(dr["MDate"]),
                            IDCompany = dr["IDCompany"].ToString(),
                            TransactionID = dr["TransactionID"].ToString(),
                            Title = dr["Title"].ToString(),
                            Amount = Convert.ToDecimal(dr["Amount"]),
                            Iban = dr["Iban"].ToString(),
                            Description = dr["Description"].ToString(),
                            RequestNr = dr["RequestNr"].ToString(),
                            IsRebate = Convert.ToBoolean(dr["IsRebate"]),
                            CompanyBankAccountID = dr["CompanyBankAccountID"].ToString(),
                            SIDBank = dr["SIDBank"].ToString(),
                            TransactionNr = dr["TransactionNr"].ToString()
                        });
                    }

                    return list;
                }
            }
            catch { }

            return new List<WithdrawalAndRebateRequestModel>();
        }

        public static string AddWithdrawalPool(DateTime cDate, DateTime transactionDate, string bankId, string receiverName, decimal amount, string description, string receiverIBAN, string transactionKey, string companyBankAccountID, string requestNr)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"
                                DECLARE @NewLine AS NCHAR(2) = NCHAR(13) + NCHAR(10);
                                DECLARE @CUSTOMERRORMESSAGE NVARCHAR(50) = N'MY' + @NewLine;

                                DECLARE @IDOut NVARCHAR(50);
                                SET @IDOut = NEWID();

                                INSERT INTO WithdrawalPools
                                    (ID, CDate, TransactionDate, IDBank, ReceiverName, ReceiverIban,
                                    Amount, TransactionKey, Description, Status, CompanyBankAccountID, RequestNr)
                                VALUES
                                    (@IDOut, @CDate, @TransactionDate, @IDBank, @ReceiverName, @ReceiverIBAN, 
                                    @Amount, @TransactionKey, @Description, 1, @CompanyBankAccountID, @RequestNr);

                                SELECT @IDOut;
                                ";

                    cmd.Parameters.Add("@CDate", SqlDbType.DateTime).Value = cDate;
                    cmd.Parameters.Add("@TransactionDate", SqlDbType.DateTime).Value = transactionDate;
                    cmd.Parameters.Add("@IDBank", SqlDbType.NVarChar, 2).Value = bankId;
                    cmd.Parameters.Add("@ReceiverName", SqlDbType.NVarChar, 100).Value = receiverName;
                    cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = amount;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = string.IsNullOrEmpty(description) ? (object)DBNull.Value : description;
                    cmd.Parameters.Add("@ReceiverIBAN", SqlDbType.NVarChar, 50).Value = receiverIBAN;
                    cmd.Parameters.Add("@TransactionKey", SqlDbType.NVarChar, 100).Value = transactionKey;
                    cmd.Parameters.Add("@CompanyBankAccountID", SqlDbType.NVarChar, 50).Value = companyBankAccountID;
                    cmd.Parameters.Add("@RequestNr", SqlDbType.NVarChar, 50).Value = requestNr;

                    var IDOut = Convert.ToString(cmd.ExecuteScalar());

                    connection.Close();

                    return IDOut;
                }
            }
            catch { }

            return null;
        }

        public static string WithdrawalPoolSetStatus(string id, string requestNr, string idCompany, bool isRebate, byte status, string responseDescription = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    using SqlCommand cmd = new SqlCommand("WithdrawalPools_SetStatus", connection);
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = id;
                    cmd.Parameters.Add("@MDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@RequestNr", SqlDbType.NVarChar, 50).Value = requestNr;
                    cmd.Parameters.Add("@ResponseTransactionNr", SqlDbType.NVarChar, 50).Value = requestNr;
                    cmd.Parameters.Add("@Status", SqlDbType.TinyInt).Value = status;
                    cmd.Parameters.Add("@IDCompany", SqlDbType.NVarChar, 50).Value = idCompany;
                    cmd.Parameters.Add("@IsRebate", SqlDbType.Bit).Value = isRebate;
                    cmd.Parameters.Add("@ResponseDescription", SqlDbType.NVarChar, 500).Value = responseDescription;
                    cmd.Parameters.Add("@IDOut", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();

                    var ID = cmd.Parameters["@IDOut"].Value.ToString();

                    connection.Close();

                    return ID;
                }
            }
            catch { }

            return null;
        }

        public static string RebateRequestSetStatus(string id, string description, string idBank, string companyBankAccountID, byte status)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    using SqlCommand cmd = new SqlCommand("CompanyRebateRequests_SetStatus", connection);
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = id;
                    cmd.Parameters.Add("@MUser", SqlDbType.NVarChar, 50).Value = "00000000-0000-0000-0000-000000000000";
                    cmd.Parameters.Add("@MDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50).Value = description;
                    cmd.Parameters.Add("@Status", SqlDbType.TinyInt).Value = status;
                    cmd.Parameters.Add("@SIDBank", SqlDbType.NVarChar, 2).Value = idBank;
                    cmd.Parameters.Add("@CompanyBankAccountID", SqlDbType.NVarChar, 50).Value = companyBankAccountID;
                    cmd.Parameters.Add("@IDOut", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();

                    var ID = cmd.Parameters["@IDOut"].Value.ToString();

                    connection.Close();

                    return ID;
                }
            }
            catch { }

            return null;
        }
        public static string WithdrawalRequestSetStatus(string id, string description, string idBank, string companyBankAccountID, byte status)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    using SqlCommand cmd = new SqlCommand("CompanyWithdrawalRequests_SetStatus", connection);
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = id;
                    cmd.Parameters.Add("@MUser", SqlDbType.NVarChar, 50).Value = "00000000-0000-0000-0000-000000000000";
                    cmd.Parameters.Add("@MDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50).Value = description;
                    cmd.Parameters.Add("@Status", SqlDbType.TinyInt).Value = status;
                    cmd.Parameters.Add("@SIDBank", SqlDbType.NVarChar, 2).Value = idBank;
                    cmd.Parameters.Add("@CompanyBankAccountID", SqlDbType.NVarChar, 50).Value = companyBankAccountID;
                    cmd.Parameters.Add("@IsProcess", SqlDbType.Bit).Value = true;
                    cmd.Parameters.Add("@IDOut", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();

                    var ID = cmd.Parameters["@IDOut"].Value.ToString();

                    connection.Close();

                    return ID;
                }
            }
            catch { }

            return null;
        }

        #endregion

        public static void SetPaymentTransferPool(string Id, int Status, string TransactionNr, string TransactionId, string ResponseDescription, string fraudDescription = "Fraud kontrolleri başarıyla tamamlandı.")
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Update PaymentTransferPools Set Status = @Status ,  ResponseTransactionNr = @TransactionNr , ResponseTransactionId = @TransactionId , ResponseDescription = @ResponseDescription, FraudControlDescription = @FraudControlDescription Where Id=@Id";
                    cmd.Parameters.Add("@TransactionNr", SqlDbType.NVarChar, 50).Value = TransactionNr;
                    cmd.Parameters.Add("@TransactionId", SqlDbType.NVarChar, 50).Value = TransactionId;
                    cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = Status;
                    cmd.Parameters.Add("@Id", SqlDbType.NVarChar, 50).Value = Id;
                    cmd.Parameters.Add("@ResponseDescription", SqlDbType.NVarChar, 50).Value = ResponseDescription;
                    cmd.Parameters.Add("@FraudControlDescription", SqlDbType.NVarChar, 500).Value = fraudDescription;
                    var count = cmd.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch { }
        }

        public static void SetPaymentTransferPoolFraudControl(string Id, string fraudDescription = "Fraud kontrolleri başarıyla tamamlandı.")
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = String.Format("Update PaymentTransferPools Set IsCaughtInFraudControl = 1, FraudControlDescription= '{0}' Where Id=@Id", fraudDescription);
                    cmd.Parameters.Add("@Id", SqlDbType.NVarChar, 50).Value = Id;

                    var count = cmd.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch { }
        }

        public static void SetPaymentTransferPoolIsHaveReferenceNr(string Id, bool isHaveReferenceNr)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Update PaymentTransferPools Set IsHaveReferenceNr = @IsHaveReferenceNr Where Id=@Id";

                    cmd.Parameters.Add("@IsHaveReferenceNr", SqlDbType.Bit, 1).Value = isHaveReferenceNr;
                    cmd.Parameters.Add("@Id", SqlDbType.NVarChar, 50).Value = Id;

                    var count = cmd.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch { }
        }

        public static string AddNotificationTransaction(DateTime notificationDate, DateTime transactionDate, DateTime? transferDate, string bankId, string serviceId, string transactionId, string transactionKey, decimal amount, string description, string idMember, string senderName, string senderIdentityNr, bool IsAutomatic, bool isOK)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "declare @IDOut nvarchar(50) = NEWID();" + Environment.NewLine +
                        "declare @TransactionNr nvarchar(50) = @ServiceID+ CONVERT(varchar, GETDATE(), 12) + replace(CONVERT(varchar, GETDATE(), 14),':','') + SUBSTRING(@IDOut,28,5)" + Environment.NewLine +
                        "Insert Into NotificationTransactions(ID, NotificationDate, TransactionDate, TransferDate, TransactionNr, IDBank, ServiceID, TransactionID, TransactionKey, Amount, Description, IDMember, SenderName, SenderIdentityNr, IsAutomatic, IsOK, IsAccepted) " + Environment.NewLine +
                        "Values(@IDOut, @NotificationDate, @TransactionDate, @TransferDate, @TransactionNr, @IDBank, @ServiceID, @TransactionID, @TransactionKey, @Amount, @Description, @IDMember, @SenderName, @SenderIdentityNr, @IsAutomatic, @IsOK, 0); " + Environment.NewLine +
                        "SELECT @TransactionNr;";
                    cmd.Parameters.Add("@NotificationDate", SqlDbType.DateTime).Value = notificationDate;
                    cmd.Parameters.Add("@TransactionDate", SqlDbType.DateTime).Value = transactionDate;
                    cmd.Parameters.Add("@TransferDate", SqlDbType.DateTime).Value = transferDate == null ? (object)DBNull.Value : transferDate;
                    cmd.Parameters.Add("@IDBank", SqlDbType.NVarChar, 2).Value = bankId;
                    cmd.Parameters.Add("@ServiceID", SqlDbType.NVarChar, 4).Value = serviceId;
                    cmd.Parameters.Add("@TransactionID", SqlDbType.NVarChar, 50).Value = transactionId;
                    cmd.Parameters.Add("@TransactionKey", SqlDbType.NVarChar, 100).Value = transactionKey;
                    cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = amount;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = string.IsNullOrEmpty(description) ? (object)DBNull.Value : description;
                    cmd.Parameters.Add("@IDMember", SqlDbType.NVarChar, 50).Value = string.IsNullOrEmpty(idMember) ? (object)DBNull.Value : idMember;
                    cmd.Parameters.Add("@SenderName", SqlDbType.NVarChar, 50).Value = string.IsNullOrEmpty(senderName) ? (object)DBNull.Value : senderName;
                    cmd.Parameters.Add("@SenderIdentityNr", SqlDbType.NVarChar, 11).Value = string.IsNullOrEmpty(senderIdentityNr) ? (object)DBNull.Value : senderIdentityNr;
                    cmd.Parameters.Add("@IsAutomatic", SqlDbType.Bit).Value = IsAutomatic;
                    cmd.Parameters.Add("@IsOK", SqlDbType.TinyInt).Value = isOK;

                    var transactionNr = Convert.ToString(cmd.ExecuteScalar());

                    connection.Close();

                    return transactionNr;
                }
            }
            catch { }

            return null;
        }

        public static string AddPaymentTransferPool(DateTime? transactionDate, string bankId, string senderName, string senderIban, decimal amount, string transactionKey, string description, string companyBankAccountID, int status = 1, bool isCaughtInFraudControl = false, string fraudDescription = "Fraud kontrolleri başarıyla tamamlandı.")
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "declare @IDOut nvarchar(50) = NEWID();" + Environment.NewLine +
                        "Insert Into PaymentTransferPools(ID, CDate, MDate, TransactionDate, IDBank, SenderName, SenderIban, Amount, TransactionKey, Description, Status, IsHaveReferenceNr, CompanyBankAccountID, IsCaughtInFraudControl, FraudControlDescription) " + Environment.NewLine +
                        "Values(@IDOut, GETDATE(), null, @TransactionDate, @IDBank, @SenderName, @SenderIban, @Amount, @TransactionKey, @Description, @Status, 0, @CompanyBankAccountID, @IsCaughtInFraudControl, @FraudControlDescription); " + Environment.NewLine +
                        "SELECT @IDOut;";
                    cmd.Parameters.Add("@TransactionDate", SqlDbType.DateTime).Value = transactionDate;
                    cmd.Parameters.Add("@IDBank", SqlDbType.NVarChar, 2).Value = bankId;
                    cmd.Parameters.Add("@SenderName", SqlDbType.NVarChar, 100).Value = senderName;
                    cmd.Parameters.Add("@SenderIban", SqlDbType.NVarChar, 50).Value = senderIban;
                    cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = amount;
                    cmd.Parameters.Add("@TransactionKey", SqlDbType.NVarChar, 100).Value = transactionKey;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 250).Value = description;
                    cmd.Parameters.Add("@Status", SqlDbType.TinyInt, 1).Value = status; 
                    cmd.Parameters.Add("@IsCaughtInFraudControl", SqlDbType.Bit, 1).Value = isCaughtInFraudControl;
                    cmd.Parameters.Add("@FraudControlDescription", SqlDbType.NVarChar, 500).Value = fraudDescription;
                    cmd.Parameters.Add("@CompanyBankAccountID", SqlDbType.NVarChar, 250).Value = string.IsNullOrEmpty(companyBankAccountID) ? (object)DBNull.Value : companyBankAccountID;

                    var IDOut = Convert.ToString(cmd.ExecuteScalar());

                    connection.Close();

                    return IDOut;
                }
            }
            catch { }

            return null;
        }

        public static string AddPaymentTransferPoolWithReference(DateTime? transactionDate, string bankId, string senderName, string senderIban, decimal amount, string transactionKey, string description, bool isHaveReferenceNr, string companyBankAccountID, string pyTransactionNr, string transactionId, bool isCaughtInFraudControl = false, string fraudDescription = "Fraud kontrolleri başarıyla tamamlandı.")
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "declare @IDOut nvarchar(50) = NEWID();" + Environment.NewLine +
                        "Insert Into PaymentTransferPools(ID, CDate, MDate, TransactionDate, IDBank, SenderName, SenderIban, Amount, TransactionKey, Description, Status, IsHaveReferenceNr, CompanyBankAccountID, ResponseTransactionNr, ResponseTransactionId ,  IsCaughtInFraudControl, FraudControlDescription ) " + Environment.NewLine +
                        "Values(@IDOut, GETDATE(), null, @TransactionDate, @IDBank, @SenderName, @SenderIban, @Amount, @TransactionKey, @Description, 2, @IsHaveReferenceNr, @CompanyBankAccountID, @ResponseTransactionNr, @ResponseTransactionId, @IsCaughtInFraudControl, @FraudControlDescription); " + Environment.NewLine +
                        "SELECT @IDOut;";
                    cmd.Parameters.Add("@TransactionDate", SqlDbType.DateTime).Value = transactionDate;
                    cmd.Parameters.Add("@IDBank", SqlDbType.NVarChar, 2).Value = bankId;
                    cmd.Parameters.Add("@SenderName", SqlDbType.NVarChar, 100).Value = senderName;
                    cmd.Parameters.Add("@SenderIban", SqlDbType.NVarChar, 50).Value = string.IsNullOrEmpty(senderIban) ? (object)DBNull.Value : senderIban;
                    cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = amount;
                    cmd.Parameters.Add("@TransactionKey", SqlDbType.NVarChar, 100).Value = transactionKey;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 250).Value = description;
                    cmd.Parameters.Add("@ResponseTransactionNr", SqlDbType.NVarChar, 250).Value = pyTransactionNr;
                    cmd.Parameters.Add("@ResponseTransactionId", SqlDbType.NVarChar, 250).Value = transactionId;
                    cmd.Parameters.Add("@IsHaveReferenceNr", SqlDbType.Bit, 1).Value = isHaveReferenceNr;
                    cmd.Parameters.Add("@IsCaughtInFraudControl", SqlDbType.Bit, 1).Value = isCaughtInFraudControl;
                    cmd.Parameters.Add("@FraudControlDescription", SqlDbType.NVarChar, 500).Value = fraudDescription;
                    cmd.Parameters.Add("@CompanyBankAccountID", SqlDbType.NVarChar, 250).Value = string.IsNullOrEmpty(companyBankAccountID) ? (object)DBNull.Value : companyBankAccountID;

                    var IDOut = Convert.ToString(cmd.ExecuteScalar());

                    connection.Close();

                    return IDOut;
                }
            }
            catch { }

            return null;
        }

        public static void AcceptNotificationTransaction(string trabsactionNr)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Update NotificationTransactions Set IsAccepted=1 Where TransactionNr=@TransactionNr";
                    cmd.Parameters.Add("@TransactionNr", SqlDbType.NVarChar, 18).Value = trabsactionNr;
                    var count = cmd.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch { }
        }

        public static List<PaymentNotificationModel> GetPaymentNotifications()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("Select py.ID, py.CDate, py.ServiceID, py.TransactionID, py.TransactionNr, py.IDMember, py.ActionDate, py.ActionTime, py.Amount, py.SenderName, py.SenderIdentityNr, py.Iban, m.Name AS Member, py.MemberIPAddress, py.MemberPort, cif.CustomerName as CustomerName , cif.CustomerEmail as CustomerEmail, cif.CustomerPhone From PaymentNotifications py  INNER JOIN Members m ON m.ID=py.IDMember LEFT JOIN CustomerInfos cif on cif.TransactionID = py.TransactionID Where Status=1 And IsAutoNotification = 1 and py.Amount <= 50000 Order By CDate");

                    DataTable dt = new DataTable();
                    SqlDataAdapter adptr = new SqlDataAdapter(cmd);
                    adptr.Fill(dt);

                    connection.Close();

                    var list = new List<PaymentNotificationModel>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new PaymentNotificationModel
                        {
                            ID = dr["ID"].ToString(),
                            CDate = Convert.ToDateTime(dr["CDate"]),
                            ServiceID = dr["ServiceID"].ToString(),
                            IDMember = dr["IDMember"].ToString(),
                            TransactionID = dr["TransactionID"].ToString(),
                            TransactionNr = dr["TransactionNr"].ToString(),
                            ActionDateTime = Convert.ToDateTime(string.Concat(Convert.ToDateTime(dr["ActionDate"]).ToString("dd.MM.yyyy"), " ", dr["ActionTime"].ToString())),
                            Amount = Convert.ToDecimal(dr["Amount"]),
                            SenderName = dr["SenderName"].ToString(),
                            SenderIdentityNr = dr["SenderIdentityNr"].ToString(),
                            Iban = dr["Iban"].ToString(),
                            ActionDate = Convert.ToDateTime(dr["ActionDate"]),
                            ActionTime = dr["ActionTime"].ToString(),
                            Member = dr["Member"].ToString(),
                            MemberIPAddress = dr["MemberIPAddress"].ToString(),
                            MemberPort = dr["MemberPort"].ToString(),
                            CustomerName = dr["CustomerName"].ToString(),
                            CustomerEmail = dr["CustomerEmail"].ToString(),
                            CustomerPhone = dr["CustomerPhone"].ToString(),
                        });
                    }

                    return list;
                }
            }
            catch { }

            return new List<PaymentNotificationModel>();
        }

        public static List<PaymentTransferPoolModel> GetPaymentTransferPool()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("Select py.ID, py.CDate, py.TransactionDate, py.IDBank, py.SenderName, py.SenderIban, py.Amount, py.TransactionKey, py.Description, py.IsCaughtInFraudControl, py.FraudControlDescription From PaymentTransferPools py " +
                        "Where py.Status=1 " +
                        "AND py.IsHaveReferenceNr=0 " +
                        "AND NOT EXISTS ( select  * from WhereConditions wc WHERE (wc.ColumnName = 'SenderName' and py.SenderName LIKE '%' + wc.ConditionName + '%') OR (wc.ColumnName = 'Description' and  py.Description LIKE '%' + wc.ConditionName + '%')) " +
                        "AND py.Amount > 0 " +
                        "Order By py.TransactionDate");

                    DataTable dt = new DataTable();
                    SqlDataAdapter adptr = new SqlDataAdapter(cmd);
                    adptr.Fill(dt);

                    connection.Close();

                    var list = new List<PaymentTransferPoolModel>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new PaymentTransferPoolModel
                        {
                            ID = dr["ID"].ToString(),
                            CDate = Convert.ToDateTime(dr["CDate"]),
                            TransactionDate = Convert.ToDateTime(dr["TransactionDate"]),
                            IDBank = dr["IDBank"].ToString(),
                            SenderName = dr["SenderName"].ToString(),
                            SenderIban = dr["SenderIban"].ToString(),
                            Amount = Convert.ToDecimal(dr["Amount"]),
                            TransactionKey = dr["TransactionKey"].ToString(),
                            Description = dr["Description"].ToString(),
                            IsCaughtInFraudControl = Convert.ToBoolean(dr["IsCaughtInFraudControl"].ToString()),
                            FraudControlDescription = dr["FraudControlDescription"].ToString()
                        });
                    }

                    return list;
                }
            }
            catch { }

            return new List<PaymentTransferPoolModel>();
        }

        public static void SetPaymentNotificationIbanAndBank(string id, string iban, string bankId, string transactionKey)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = String.Format("Update PaymentNotifications Set Iban= '{0}', IDBank= '{1}', TransactionKey='{2}' Where ID=@ID", iban, bankId, transactionKey);
                    cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = id;
                    var count = cmd.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch { }
        }

        public static void SetManuelNotification(string id, string fraudControlDescription)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_defaultConecction);
                connection.Open();

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = String.Format("Update PaymentNotifications Set IsAutoNotification= 0, IsCaughtInFraudControl= 1, FraudControlDescription= '{0}' Where ID=@ID", fraudControlDescription);
                cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = id;
                var count = cmd.ExecuteNonQuery();

                connection.Close();
            }
            catch { }
        }

        public static bool SetPaymentNotificationIban(string pyID, string iban, string bankId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = String.Format("Update PaymentNotifications Set Iban= '{0}', IDBank= '{1}' Where ID=@ID", iban, bankId);
                    cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = pyID;
                    var rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0) return false;

                    connection.Close();
                }
            }
            catch { return false; }

            return true;
        }

        public static void SetPaymentTransactionAlive(string id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Update PaymentNotifications Set IsAlive=0 Where ID=@ID";
                    cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = id;
                    var count = cmd.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch { }
        }

        public static void SetPaymentTransactionStatus(string id, int status, string description, string fraudControlDescription = "Fraud kontrolleri başarıyla tamamlandı")
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    var MUser = "00000000-0000-0000-0000-000000000000";
                    var MDate = DateTime.Now;
                    connection.Open();
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = String.Format("Update PaymentNotifications Set Status= '{0}', Description= '{1}', MDate= GETDATE(), MUser= '{3}', FraudControlDescription='{4}' Where ID=@ID", status, description, MDate, MUser, fraudControlDescription);
                    cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = id;
                    var rowsAffected = cmd.ExecuteNonQuery();

                    connection.Close();

                }
            }
            catch { }
        }

        public static BankModel GetBank(string bankId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("Select * From Banks Where ID='{0}'", bankId);

                    DataTable dt = new DataTable();
                    SqlDataAdapter adptr = new SqlDataAdapter(cmd);
                    adptr.Fill(dt);

                    connection.Close();

                    return new BankModel()
                    {
                        ID = dt.Rows[0]["ID"].ToString(),
                        Name = dt.Rows[0]["Name"].ToString(),
                        Title = dt.Rows[0]["Title"].ToString(),
                        Branch = dt.Rows[0]["Branch"].ToString(),
                        AccountNr = dt.Rows[0]["AccountNr"].ToString(),
                        IBAN = dt.Rows[0]["IBAN"].ToString(),
                        Img = string.Concat("https://burateknoloji.com/img/banks/", dt.Rows[0]["Img"])
                    };
                }
            }
            catch { }

            return null;
        }

        public static List<BankModel> GetBankList()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("SELECT * FROM Banks WHERE ID<>'00'");

                    DataTable dt = new DataTable();
                    SqlDataAdapter adptr = new SqlDataAdapter(cmd);
                    adptr.Fill(dt);

                    connection.Close();

                    var list = new List<BankModel>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new BankModel
                        {
                            ID = dr["ID"].ToString(),
                            Name = dr["Name"].ToString(),
                            Title = dr["Title"].ToString(),
                            Branch = dr["Branch"].ToString(),
                            AccountNr = dr["AccountNr"].ToString(),
                            IBAN = dr["IBAN"].ToString(),
                            Img = null
                        });
                    }

                    return list;
                }
            }
            catch { }

            return new List<BankModel>();

        }

        public static string AddCallbackResponseLog(string transactionID, string serviceType, string callback, string idCompany, string transactionType, int responseStatus)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "declare @IDOut nvarchar(50) = NEWID();" + Environment.NewLine +
                        "Insert Into CallbackResponseLogs(ID, CDate, TransactionID, ServiceType, Callback, IDCompany, TransactionType, ResponseStatus) " + Environment.NewLine +
                        "Values(@IDOut,GETDATE(), @TransactionID, @ServiceType, @Callback, @IDCompany, @TransactionType, @ResponseStatus); " + Environment.NewLine +
                        "SELECT @IDOut;";
                    cmd.Parameters.Add("@TransactionID", SqlDbType.NVarChar).Value = transactionID;
                    cmd.Parameters.Add("@serviceType", SqlDbType.NVarChar).Value = serviceType;
                    cmd.Parameters.Add("@Callback", SqlDbType.NVarChar, -1).Value = callback;
                    cmd.Parameters.Add("@IDCompany", SqlDbType.NVarChar).Value = idCompany;
                    cmd.Parameters.Add("@TransactionType", SqlDbType.NVarChar).Value = transactionType;
                    cmd.Parameters.Add("@ResponseStatus", SqlDbType.Int).Value = responseStatus;




                    var IDOut = Convert.ToString(cmd.ExecuteScalar());

                    connection.Close();

                    return IDOut;
                }
            }
            catch { }
            return null;
        }

        public static List<SettingModel> GetSystemSettingValues(string paramType)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("SELECT * FROM Settings where @ParamType IS NULL OR ParamType = @ParamType or ParamType = 'GENEL'");
                    cmd.Parameters.Add("@ParamType", SqlDbType.NVarChar).Value = paramType;

                    DataTable dt = new DataTable();
                    SqlDataAdapter adptr = new SqlDataAdapter(cmd);
                    adptr.Fill(dt);

                    connection.Close();

                    var list = new List<SettingModel>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, string> item = new Dictionary<string, string>
                        {
                            { "ParamType", dr["ParamType"].ToString() },
                            { "ParamDef", dr["ParamDef"].ToString() },
                            { "ParamVal", dr["ParamVal"].ToString() },
                            { "ActivatedForGeneralUse", dr["ActivatedForGeneralUse"].ToString() }
                        };

                        list.Add(new SettingModel
                        {
                            ParamType = dr["ParamType"].ToString(),
                            ParamDef = dr["ParamDef"].ToString(),
                            ParamVal = dr["ParamVal"].ToString(),
                            ActivatedForGeneralUse = Convert.ToBoolean(dr["ActivatedForGeneralUse"])
                        });
                    }

                    return list;
                }
            }
            catch { }

            return new List<SettingModel>();
        }

        public static string AddSystemSQLErrors(string entityID, string transactionID, string exMessage, string description)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "declare @IDOut nvarchar(50) = NEWID();" + Environment.NewLine +
                        "Insert Into SystemSQLErrors(ID, CDate, EntityID, TransactionID, Sp, ExMessage, Description) " + Environment.NewLine +
                        "Values(@IDOut,GETDATE(), @EntityID, @TransactionID, SystemSQLErrors_Insert, @ExMessage, @Description); " + Environment.NewLine +
                        "SELECT @IDOut;";
                    cmd.Parameters.Add("@EntityID", SqlDbType.NVarChar).Value = entityID;
                    cmd.Parameters.Add("@TransactionID", SqlDbType.NVarChar).Value = transactionID;
                    cmd.Parameters.Add("@ExMessage", SqlDbType.NVarChar).Value = exMessage;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value = description;
                    var IDOut = Convert.ToString(cmd.ExecuteScalar());

                    connection.Close();

                    return IDOut;
                }
            }
            catch { }
            return null;
        }

        #region CheckReferenceNr
        public static CompanyAutoNotificationSettingModel GetCompanyAutoNotificationSetting(string referenceId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_defaultConecction);
                connection.Open();

                using SqlCommand cmd = new SqlCommand("CompanyAutoNotificationSettings_GetSingle", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ReferenceId", SqlDbType.NVarChar, 4).Value = referenceId;

                DataTable dt = new DataTable();
                SqlDataAdapter adptr = new SqlDataAdapter(cmd);
                adptr.Fill(dt);

                connection.Close();

                return new CompanyAutoNotificationSettingModel()
                {
                    IDCompany = dt.Rows[0]["IDCompany"] == DBNull.Value ? null : dt.Rows[0]["IDCompany"].ToString(),
                    ReferenceId = dt.Rows[0]["ReferenceId"] == DBNull.Value ? null : dt.Rows[0]["ReferenceId"].ToString(),
                    IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"].ToString()),
                    RequestUrl = dt.Rows[0]["RequestUrl"] == DBNull.Value ? null : dt.Rows[0]["RequestUrl"].ToString(),
                    ServiceId = dt.Rows[0]["ServiceID"] == DBNull.Value ? null : dt.Rows[0]["ServiceID"].ToString(),
                    CallbackUrl = dt.Rows[0]["CallbackUrl"] == DBNull.Value ? null : dt.Rows[0]["CallbackUrl"].ToString(),
                    AutoTransferLimit = dt.Rows[0]["AutoTransferLimit"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[0]["AutoTransferLimit"].ToString())
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static CompanyAutoNotificationSettingModel GetCompanyAutoNotificationSettingByIDCompany(string idCompany)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_defaultConecction);
                connection.Open();

                using SqlCommand cmd = new SqlCommand("CompanyAutoNotificationSettings_GetSingleByIDCompany", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@IDCompany", SqlDbType.NVarChar, 50).Value = idCompany;

                DataTable dt = new DataTable();
                SqlDataAdapter adptr = new SqlDataAdapter(cmd);
                adptr.Fill(dt);

                connection.Close();

                return new CompanyAutoNotificationSettingModel()
                {
                    IDCompany = dt.Rows[0]["IDCompany"] == DBNull.Value ? null : dt.Rows[0]["IDCompany"].ToString(),
                    ReferenceId = dt.Rows[0]["ReferenceId"] == DBNull.Value ? null : dt.Rows[0]["ReferenceId"].ToString(),
                    IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"].ToString()),
                    RequestUrl = dt.Rows[0]["RequestUrl"] == DBNull.Value ? null : dt.Rows[0]["RequestUrl"].ToString(),
                    ServiceId = dt.Rows[0]["ServiceID"] == DBNull.Value ? null : dt.Rows[0]["ServiceID"].ToString(),
                    CallbackUrl = dt.Rows[0]["CallbackUrl"] == DBNull.Value ? null : dt.Rows[0]["CallbackUrl"].ToString(),
                    AutoTransferLimit = dt.Rows[0]["AutoTransferLimit"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[0]["AutoTransferLimit"].ToString())
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static (string Result, string ReferenceNr, string ServiceId, string CallbackUrl, decimal AutoTransferLimit) CheckReferenceNr(string description)
        {
            try
            {
                var match = Regex.Match(description, @"\b\d{12}\b");
                var referenceNr = match.Value;

                if (referenceNr != null && referenceNr != "")
                {
                    var referenceId = referenceNr[..4];

                    var res = GetCompanyAutoNotificationSetting(referenceId);

                    if (res != null)
                    {
                        var result = AutoNotificationCheckReferenceNrRequest.CheckReferenceNrRequest(referenceNr, res.RequestUrl);

                        var opt = new JsonSerializerOptions() { WriteIndented = true };
                        tSQLBankManager.AddCallbackResponseLog(referenceNr, "STILPAY", System.Text.Json.JsonSerializer.Serialize(result, opt), res.IDCompany, "ReferenceNr Request Response", 1);

                        return (result.status, referenceNr, res.ServiceId, res.CallbackUrl, res.AutoTransferLimit);
                    }
                }
            }
            catch { }

            return ("", "", "", "", 0);

        }


        public static string AddAutoPaymentNotification(DateTime transactionDate, string bankId, string senderName, string serviceId, string transactionId, string transactionKey, decimal amount, string description, string iban, string companyBankAccountID, bool isCaughtInFraudControl = false, string fraudDescription = "Fraud kontrolleri başarıyla tamamlandı.")
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_defaultConecction);
                connection.Open();

                using SqlCommand cmd = new SqlCommand("PaymentNotifications_Insert", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ServiceID", SqlDbType.NVarChar, 4).Value = serviceId;
                cmd.Parameters.Add("@TransactionID", SqlDbType.NVarChar, 50).Value = transactionId;
                cmd.Parameters.Add("@IDMember", SqlDbType.NVarChar, 50).Value = "00000000-0000-0000-0000-000000000000";
                cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = "05555555554";
                cmd.Parameters.Add("@IDBank", SqlDbType.NVarChar, 2).Value = bankId;
                cmd.Parameters.Add("@ActionDate", SqlDbType.Date).Value = transactionDate;
                cmd.Parameters.Add("@ActionTime", SqlDbType.NVarChar, 5).Value = transactionDate.ToString("HH:mm");
                cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = amount;
                cmd.Parameters.Add("@SenderName", SqlDbType.NVarChar, 50).Value = string.IsNullOrEmpty(senderName) ? "STILPAY AUTO" : senderName;
                cmd.Parameters.Add("@SenderIdentityNr", SqlDbType.NVarChar, 11).Value = "11111111111";
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = string.IsNullOrEmpty(description) ? (object)DBNull.Value : description;
                cmd.Parameters.Add("@Status", SqlDbType.TinyInt).Value = 1;
                cmd.Parameters.Add("@Iban", SqlDbType.NVarChar, 50).Value = string.IsNullOrEmpty(iban) ? (object)DBNull.Value : iban;
                cmd.Parameters.Add("@TransactionKey", SqlDbType.NVarChar, 50).Value = transactionKey;
                cmd.Parameters.Add("@CompanyBankAccountID", SqlDbType.NVarChar, 50).Value = companyBankAccountID;
                cmd.Parameters.Add("@IsCaughtInFraudControl", SqlDbType.Bit, 1).Value = isCaughtInFraudControl;
                cmd.Parameters.Add("@FraudControlDescription", SqlDbType.NVarChar, 500).Value = fraudDescription;
                cmd.Parameters.Add("@IDOut", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                var ID = cmd.Parameters["@IDOut"].Value.ToString();

                connection.Close();

                return ID;
            }
            catch { }

            return null;
        }

        public static string GetPaymentNotificationTransactionNr(string id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("Select TransactionNr From PaymentNotifications Where Id='{0}'", id);

                    var result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        string transactionNr = result.ToString();

                        connection.Close();

                        return transactionNr;
                    }
                }
            }

            catch { }

            return null;
        }
        #endregion

        #region FraudControl

        public static List<PaymentTransferPoolDescriptionControlModel> GetPaymentTransferPoolDescriptionControls()
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_defaultConecction);
                connection.Open();

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = string.Format("SELECT * FROM PaymentTransferPoolDescriptionControls");

                DataTable dt = new DataTable();
                SqlDataAdapter adptr = new SqlDataAdapter(cmd);
                adptr.Fill(dt);

                connection.Close();

                var list = new List<PaymentTransferPoolDescriptionControlModel>();

                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new PaymentTransferPoolDescriptionControlModel
                    {
                        Name = dr["Name"].ToString()
                    });
                }

                return list;
            }

            catch { }

            return new List<PaymentTransferPoolDescriptionControlModel>();
        }
        public static CompanyFraudControl GetCompanyFraudControl(string serviceId)
        {
            try
            {
                var idCompany = string.Empty;

                using SqlConnection connection = new SqlConnection(_defaultConecction);
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("CompanyIntegrations_GetByServiceId", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ServiceId", SqlDbType.NVarChar, 4).Value = serviceId;

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adptr = new SqlDataAdapter(cmd))
                    {
                        adptr.Fill(dt);
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        idCompany = dr["ID"].ToString();
                    }
                }

                if (string.IsNullOrEmpty(idCompany)) return new CompanyFraudControl();

                using SqlCommand cmd2 = new SqlCommand("CompanyFraudControls_GetSingle", connection);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = idCompany;

                DataTable dt2 = new DataTable();
                using (SqlDataAdapter adptr2 = new SqlDataAdapter(cmd2))
                {
                    adptr2.Fill(dt2);
                }

                foreach (DataRow dr2 in dt2.Rows)
                {
                    return new CompanyFraudControl()
                    {
                        TransferDailyTransactionCount = int.TryParse(dr2["TransferDailyTransactionCount"].ToString(), out var transferDailyCount) ? transferDailyCount : 0,
                        TransferDailyTransactionLimitAmount = decimal.TryParse(dr2["TransferDailyTransactionLimitAmount"].ToString(), out var transferLimitAmount) ? transferLimitAmount : 0,
                        TransferFirstTransactionLimit = decimal.TryParse(dr2["TransferFirstTransactionLimit"].ToString(), out var firstTransactionLimit) ? firstTransactionLimit : 0,
                        BeStoppedTransferDailyTransactionCount = int.TryParse(dr2["BeStoppedTransferDailyTransactionCount"].ToString(), out var beStoppedDailyCount) ? beStoppedDailyCount : 0,
                        IsTransferFraudControlActive = bool.TryParse(dr2["IsTransferFraudControlActive"].ToString(), out var isFraudControlActive) && isFraudControlActive,
                        TransferTimeSpanInRecentTransactionMinutes = int.TryParse(dr2["TransferTimeSpanInRecentTransactionMinutes"].ToString(), out var transferTimeSpan) ? transferTimeSpan : 0,
                        TransferTimeSpanInRecentTransactionMinutesLimitAmount = decimal.TryParse(dr2["TransferTimeSpanInRecentTransactionMinutesLimitAmount"].ToString(), out var transferTimeSpanLimitAmount) ? transferTimeSpanLimitAmount : 0
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return new CompanyFraudControl();
        }
        public static (bool IsTrusted, bool FraudResult, string FraudDescription) TransferCheckFraudControl(string customerName, string referenceNr, string senderName, string serviceId, decimal amount)
        {
            try
            {
                var companyFraudControlSettings = GetCompanyFraudControl(serviceId);

                if (companyFraudControlSettings != null)
                {
                    if (string.IsNullOrEmpty(serviceId) && !companyFraudControlSettings.IsTransferFraudControlActive)
                    {
                        return (false, true, "Üye işyerinde fraud kontrolü devredışı");
                    }

                    using SqlConnection connection = new SqlConnection(_defaultConecction);
                    connection.Open();

                    using SqlCommand cmd = new SqlCommand("PaymentTransferPools_CheckFraudControl", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@CustomerName", SqlDbType.NVarChar, 500).Value = customerName;
                    cmd.Parameters.Add("@ReferenceNr", SqlDbType.NVarChar, 50).Value = referenceNr;
                    cmd.Parameters.Add("@SenderName", SqlDbType.NVarChar, 500).Value = senderName;
                    cmd.Parameters.Add("@TimeSpanInMinutes", SqlDbType.NVarChar, 500).Value = companyFraudControlSettings.TransferTimeSpanInRecentTransactionMinutes;
                    cmd.Parameters.Add("@TransactionLimitToday", SqlDbType.NVarChar, 500).Value = companyFraudControlSettings.TransferDailyTransactionCount;
                    cmd.ExecuteNonQuery();

                    DataTable dt = new DataTable();
                    SqlDataAdapter adptr = new SqlDataAdapter(cmd);
                    adptr.Fill(dt);

                    connection.Close();

                    var transferCheckFraudControlDto = new TransferCheckFraudControlDto();

                    foreach (DataRow dr in dt.Rows)
                    {
                        transferCheckFraudControlDto.DailyTransactionLimitExceeded = Convert.ToBoolean(dr["DailyTransactionLimitExceeded"].ToString());
                        transferCheckFraudControlDto.RecentTransactionLimitExceeded = Convert.ToBoolean(dr["RecentTransactionLimitExceeded"].ToString());
                        transferCheckFraudControlDto.TransactionWithin24Hours = Convert.ToBoolean(dr["TransactionWithin24Hours"].ToString());
                        transferCheckFraudControlDto.TransactionCountToday = Convert.ToInt32(dr["TransactionCountToday"].ToString());
                        transferCheckFraudControlDto.IsCaughtInFraudControlPending = Convert.ToBoolean(dr["IsCaughtInFraudControlPending"].ToString());
                    }

                    if (transferCheckFraudControlDto.IsCaughtInFraudControlPending)
                    {
                        return (false, false, "Daha önceki işlemi fraud kontrolüne takıldı, işlem durduruldu");
                    }

                    if (transferCheckFraudControlDto.TransactionWithin24Hours)
                    {
                        return (true, true, "Aynı kişi 24 saat veya öncesinde ödeme yaptı, kişi güvenilir olduğu için fraud kontrolü yapılmadı");
                    }

                    if (transferCheckFraudControlDto.RecentTransactionLimitExceeded && amount >= companyFraudControlSettings.TransferTimeSpanInRecentTransactionMinutesLimitAmount)
                    {
                        return(false, false, $"Aynı kişi {companyFraudControlSettings.TransferTimeSpanInRecentTransactionMinutes} dakika içinde {companyFraudControlSettings.TransferTimeSpanInRecentTransactionMinutesLimitAmount:n2} veya üstü olarak 2. kez ödeme yaptı, işlem durduruldu.");
                    }

                    if (transferCheckFraudControlDto.DailyTransactionLimitExceeded && transferCheckFraudControlDto.TransactionCountToday >= companyFraudControlSettings.BeStoppedTransferDailyTransactionCount)
                    {
                        return (false, false, $"Aynı kişi 24 saat içinde {transferCheckFraudControlDto.TransactionCountToday}. kez ödeme yaptı, üye işyeri sınırı {companyFraudControlSettings.BeStoppedTransferDailyTransactionCount}, işlem durduruldu.");
                    }

                    if (transferCheckFraudControlDto.DailyTransactionLimitExceeded && amount >= companyFraudControlSettings.TransferDailyTransactionLimitAmount)
                    {
                        return (false, false, $"Aynı kişi 24 saat içinde {transferCheckFraudControlDto.TransactionCountToday}. kez ve {companyFraudControlSettings.TransferDailyTransactionLimitAmount:n2} üstü ödeme yaptı, işlem durduruldu.");
                    }

                    return (false, true, "Fraud kontrolleri başarıyla tamamlandı.");
                }
            }
            catch { }

            return (false, false, "");
        }


        #endregion

        public static Dictionary<string, int> GetManuelNotifyList()
        {
            var result = new Dictionary<string, int>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_defaultConecction))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                    @"SELECT 
                    PaymentMethod, 
                    COUNT(*) AS RecordCount
                  FROM (
                      SELECT 'Kredi Kartı' AS PaymentMethod
                      FROM CreditCardPaymentNotifications
                      WHERE IsAutoNotification = 0 AND Status = 1 AND PaymentInstitutionCommissionRate is not null AND PaymentInstitutionNetAmount is not null

                      UNION ALL

                      SELECT 'Yurt Dışı Kredi Kartı' AS PaymentMethod
                      FROM ForeignCreditCardPaymentNotifications
                      WHERE IsAutoNotification = 0 AND Status = 1 AND PaymentInstitutionCommissionRate is not null AND PaymentInstitutionNetAmount is not null

                      UNION ALL

                      SELECT 'Havale-Eft' AS PaymentMethod
                      FROM PaymentNotifications
                      WHERE IsAutoNotification = 0 AND Status = 1
                  ) AS Combined
                  GROUP BY PaymentMethod";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string paymentMethod = reader.GetString(0);
                            int recordCount = reader.GetInt32(1);
                            result[paymentMethod] = recordCount;
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return result;
        }
    }
}