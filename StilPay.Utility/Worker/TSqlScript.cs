using ClosedXML.Excel;
using ClosedXML.Graphics;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2010.Drawing.Slicer;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Office2016.Word.Symex;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Zlib;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.Filters;
using RestSharp;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Pipelines;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using static ClosedXML.Excel.XLPredefinedFormat;
using static StilPay.Utility.Helper.Enums;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace StilPay.Utility.Worker
{
    public static class TSqlScript
    {

        // Güncelleme Atıldı. 10.01.2025 17:05 Arda
        //-------------------------------------------------------VERSİYON BAŞLANGIÇ ----------------------------------------------------------------------------------

        //    ALTER PROCEDURE[dbo].[CompanyTransactions_CreditCardDetailedSearch]
        //    @Status TINYINT,
        //    @IDCompany NVARCHAR(50),
        //    @SenderName NVARCHAR(500),
        //    @SenderPhone NVARCHAR(50),
        //    @CardNumber NVARCHAR(50),
        //    @PageLength INT,
        //    @OffsetValue INT,
        //    @SearchValue NVARCHAR(MAX)
        //AS
        //BEGIN
        //    SET NOCOUNT ON;

        //    -- Declare a variable to store the total record count
        //    DECLARE @totalRecords INT;

        //    -- Select records into a temporary table
        //    SELECT
        //        r.ID,
        //        r.CDate,
        //        r.TransactionID,
        //        r.TransactionNr,
        //        r.Company,
        //        r.SenderName,
        //        r.Phone AS SenderPhone,
        //        r.CardNumber,
        //        r.CardTypeId,
        //        r.PaymentInstitutionName,
        //        r.Amount,
        //        r.Status,
        //        r.RebateID,
        //        r.EntityID,
        //        r.IsForeign
        //    INTO #TempTable
        //    FROM
        //    (
        //        -- First query for CreditCardPaymentNotifications
        //        SELECT
        //            r.ID,
        //            r.CDate,
        //            r.TransactionID,
        //            r.TransactionNr,
        //            c.Title AS Company,
        //            r.SenderName,
        //            r.Phone,
        //            r.CardNumber,
        //            r.CardTypeId,
        //            pins.Name AS PaymentInstitutionName,
        //            r.Amount,
        //            r.Status,
        //            crr.ID AS RebateID,
        //            r.ID as EntityID,
        //   0 as IsForeign
        //        FROM CreditCardPaymentNotifications r
        //        INNER JOIN CompanyIntegrations ci ON ci.ServiceID = r.ServiceID
        //        INNER JOIN Companies c ON c.ID = ci.ID
        //        LEFT JOIN CompanyRebateRequests crr ON crr.TransactionID = r.TransactionID
        //        LEFT JOIN PaymentInstitutions pins ON pins.ID = r.CreditCardPaymentMethodID
        //        WHERE
        //            (@Status IS NULL OR r.Status = @Status)
        //            AND((@IDCompany IS NULL or @IDCompany = '0') OR c.ID = @IDCompany)
        //            AND(@SenderName IS NULL OR r.SenderName COLLATE Turkish_CI_AS LIKE '%' + @SenderName + '%')
        //            AND(@SenderPhone IS NULL OR r.Phone COLLATE Turkish_CI_AS LIKE '%' + @SenderPhone + '%')
        //            AND(@CardNumber IS NULL OR r.CardNumber COLLATE Turkish_CI_AS LIKE '%' + @CardNumber + '%' OR EncryptedCardNumber = @CardNumber)

        //        UNION ALL

        //        -- Second query for ForeignCreditCardPaymentNotifications
        //        SELECT
        //            r.ID,
        //            r.CDate,
        //            r.TransactionID,
        //            r.TransactionNr,
        //            c.Title AS Company,
        //            r.SenderName,
        //            r.Phone,
        //            r.CardNumber,
        //            r.CardTypeId,
        //            pins.Name AS PaymentInstitutionName,
        //            r.Amount,
        //            r.Status,
        //            crr.ID AS RebateID,
        //            r.ID as EntityID,
        //            1 as IsForeign
        //        FROM ForeignCreditCardPaymentNotifications r
        //        INNER JOIN CompanyIntegrations ci ON ci.ServiceID = r.ServiceID
        //        INNER JOIN Companies c ON c.ID = ci.ID
        //        LEFT JOIN CompanyRebateRequests crr ON crr.TransactionID = r.TransactionID
        //        LEFT JOIN PaymentInstitutions pins ON pins.ID = r.CreditCardPaymentMethodID
        //        WHERE
        //            (@Status IS NULL OR r.Status = @Status)
        //            AND((@IDCompany IS NULL or @IDCompany = '0') OR c.ID = @IDCompany)
        //            AND(@SenderName IS NULL OR r.SenderName COLLATE Turkish_CI_AS LIKE '%' + @SenderName + '%')
        //            AND(@SenderPhone IS NULL OR r.Phone COLLATE Turkish_CI_AS LIKE '%' + @SenderPhone + '%')
        //            AND(@CardNumber IS NULL OR r.CardNumber COLLATE Turkish_CI_AS LIKE '%' + @CardNumber + '%')
        //    ) AS r;

        //    -- Get the total number of records
        //    SELECT @totalRecords = COUNT(*) FROM #TempTable;

        //    -- Select the paginated results with the total records count
        //    SELECT*, @totalRecords AS TotalRecords
        //    FROM #TempTable
        //    ORDER BY CDate DESC
        //    OFFSET @OffsetValue ROWS FETCH NEXT @PageLength ROWS ONLY;

        //    -- Drop the temporary table
        //    DROP TABLE #TempTable;

        //    SET NOCOUNT OFF;
        //END;


        // Güncellendi

        //------------------------------------------------------- 10.02.2025 Arda Değişiklik Scriptleri ----------------------------------------------------------------------------------

        //ALTER PROCEDURE[dbo].[CompanyTransactions_TransferDetailedSearch]
        //@Status TINYINT,
        //@IDCompany NVARCHAR(50),
        //@SenderName NVARCHAR(500),
        //@SenderPhone NVARCHAR(50),
        //@SenderReferenceNr NVARCHAR(50),
        //@Amount decimal (18,2),
        //@PageLength INT,
        //@OffsetValue INT,
        //@SearchValue NVARCHAR(MAX)
        //AS
        //BEGIN
        //    SET NOCOUNT ON;

        //    DECLARE @totalRecords INT;

        //    SELECT
        //        r.ID,
        //        r.CDate,
        //        r.TransactionID,
        //        r.TransactionNr,
        //        r.Company,
        //        r.SenderName,
        //           r.SenderPhone,
        //        r.SenderReferenceNr,
        //        r.Amount,
        //        r.Status,
        //        r.RebateID,
        //        EntityUrl
        //    INTO #TempTable
        //    FROM
        //    (
        //        SELECT
        //            r.ID,
        //            r.CDate,
        //            r.TransactionID,
        //            r.TransactionNr,
        //            c.Title AS Company,
        //            r.SenderName,
        //               r.Phone as SenderPhone,
        //            ptp.Description as SenderReferenceNr,
        //            r.Amount,
        //            r.Status,
        //            crr.ID AS RebateID,
        //         '/DealerPaymentTransaction/Edit/' + r.ID as EntityUrl
        //        FROM PaymentNotifications r
        //        INNER JOIN CompanyIntegrations ci ON ci.ServiceID = r.ServiceID
        //        INNER JOIN Companies c ON c.ID = ci.ID
        //        INNER JOIN PaymentTransferPools ptp ON ptp.ResponseTransactionId = r.TransactionID
        //        LEFT JOIN CompanyRebateRequests crr ON crr.TransactionID = r.TransactionID
        //        WHERE
        //            (@Status IS NULL OR r.Status = @Status)
        //            AND(@Amount = 0 OR r.Amount = @Amount)
        //            AND((@IDCompany IS NULL or @IDCompany = '0') OR c.ID = @IDCompany)

        //            AND
        //            (
        //                (@SenderName IS NOT NULL AND r.SenderName COLLATE Turkish_CI_AS LIKE '%' + @SenderName + '%')

        //                OR(@SenderPhone IS NOT NULL AND r.Phone COLLATE Turkish_CI_AS LIKE '%' + @SenderPhone + '%')

        //                OR(@SenderReferenceNr IS NOT NULL AND ptp.Description COLLATE Turkish_CI_AS LIKE '%' + @SenderReferenceNr + '%')
        //      )

        //        UNION ALL

        //        SELECT
        //            r.ID,
        //            r.CDate,
        //         null as TransactionID,
        //         null as TransactionNr,
        //         null as Company,
        //            r.SenderName,
        //	        null as SenderPhone,
        //            r.Description as SenderReferenceNr,
        //            r.Amount,
        //            r.Status,
        //         null AS RebateID,
        //         null as EntityUrl
        //        FROM PaymentTransferPools r
        //        WHERE r.Amount > 0 AND
        //            (@Status IS NULL OR r.Status = @Status)
        //            AND(@Amount = 0 OR r.Amount = @Amount)

        //               AND
        //               (
        //                (@SenderName IS NOT NULL AND r.SenderName COLLATE Turkish_CI_AS LIKE '%' + @SenderName + '%')

        //                OR(@SenderReferenceNr IS NOT NULL AND r.Description COLLATE Turkish_CI_AS LIKE '%' + @SenderReferenceNr + '%')
        //      )
        //    ) AS r;

        //        SELECT @totalRecords = COUNT(*) FROM #TempTable;

        //    SELECT*, @totalRecords AS TotalRecords
        //    FROM #TempTable
        //    ORDER BY CDate DESC
        //    OFFSET @OffsetValue ROWS FETCH NEXT @PageLength ROWS ONLY;

        //        DROP TABLE #TempTable;

        //    SET NOCOUNT OFF;
        //END;

        //GO

        //ALTER PROCEDURE[dbo].[CompanyTransactions_CreditCardDetailedSearch]
        //@Status TINYINT,
        //@IDCompany NVARCHAR(50),
        //@SenderName NVARCHAR(500),
        //@SenderPhone NVARCHAR(50),
        //@CardNumber NVARCHAR(50),
        //@Amount decimal (18,2),
        //@PageLength INT,
        //@OffsetValue INT,
        //@SearchValue NVARCHAR(MAX)
        //AS
        //BEGIN
        //    SET NOCOUNT ON;

        //    -- Declare a variable to store the total record count
        //    DECLARE @totalRecords INT;

        //    -- Select records into a temporary table
        //    SELECT
        //        r.ID,
        //        r.CDate,
        //        r.TransactionID,
        //        r.TransactionNr,
        //        r.Company,
        //        r.SenderName,
        //        r.Phone AS SenderPhone,
        //        r.CardNumber,
        //        r.CardTypeId,
        //        r.PaymentInstitutionName,
        //        r.Amount,
        //        r.Status,
        //        r.RebateID,
        //        r.EntityID,
        //        r.IsForeign
        //    INTO #TempTable
        //    FROM
        //    (
        //        -- First query for CreditCardPaymentNotifications
        //        SELECT
        //            r.ID,
        //            r.CDate,
        //            r.TransactionID,
        //            r.TransactionNr,
        //            c.Title AS Company,
        //            r.SenderName,
        //            r.Phone,
        //            r.CardNumber,
        //            r.CardTypeId,
        //            pins.Name AS PaymentInstitutionName,
        //            r.Amount,
        //            r.Status,
        //            crr.ID AS RebateID,
        //            r.ID as EntityID,
			     //   0 as IsForeign
        //        FROM CreditCardPaymentNotifications r
        //        INNER JOIN CompanyIntegrations ci ON ci.ServiceID = r.ServiceID
        //        INNER JOIN Companies c ON c.ID = ci.ID
        //        LEFT JOIN CompanyRebateRequests crr ON crr.TransactionID = r.TransactionID
        //        LEFT JOIN PaymentInstitutions pins ON pins.ID = r.CreditCardPaymentMethodID
        //        WHERE
        //            (@Status IS NULL OR r.Status = @Status)
        //            AND((@IDCompany IS NULL or @IDCompany = '0') OR c.ID = @IDCompany)
        //            AND(@SenderName IS NULL OR r.SenderName COLLATE Turkish_CI_AS LIKE '%' + @SenderName + '%')
        //            AND(@SenderPhone IS NULL OR r.Phone COLLATE Turkish_CI_AS LIKE '%' + @SenderPhone + '%')
        //            AND(@Amount = 0 OR r.Amount = @Amount)
        //            AND(@CardNumber IS NULL OR r.CardNumber COLLATE Turkish_CI_AS LIKE '%' + @CardNumber + '%' OR EncryptedCardNumber = @CardNumber)

        //        UNION ALL

        //        -- Second query for ForeignCreditCardPaymentNotifications
        //        SELECT
        //            r.ID,
        //            r.CDate,
        //            r.TransactionID,
        //            r.TransactionNr,
        //            c.Title AS Company,
        //            r.SenderName,
        //            r.Phone,
        //            r.CardNumber,
        //            r.CardTypeId,
        //            pins.Name AS PaymentInstitutionName,
        //            r.Amount,
        //            r.Status,
        //            crr.ID AS RebateID,
        //            r.ID as EntityID,
        //            1 as IsForeign
        //        FROM ForeignCreditCardPaymentNotifications r
        //        INNER JOIN CompanyIntegrations ci ON ci.ServiceID = r.ServiceID
        //        INNER JOIN Companies c ON c.ID = ci.ID
        //        LEFT JOIN CompanyRebateRequests crr ON crr.TransactionID = r.TransactionID
        //        LEFT JOIN PaymentInstitutions pins ON pins.ID = r.CreditCardPaymentMethodID
        //        WHERE
        //            (@Status IS NULL OR r.Status = @Status)
        //            AND((@IDCompany IS NULL or @IDCompany = '0') OR c.ID = @IDCompany)
        //            AND(@SenderName IS NULL OR r.SenderName COLLATE Turkish_CI_AS LIKE '%' + @SenderName + '%')
        //            AND(@SenderPhone IS NULL OR r.Phone COLLATE Turkish_CI_AS LIKE '%' + @SenderPhone + '%')
        //            AND(@Amount = 0 OR r.Amount = @Amount)
        //            AND(@CardNumber IS NULL OR r.CardNumber COLLATE Turkish_CI_AS LIKE '%' + @CardNumber + '%')
        //    ) AS r;

        //    -- Get the total number of records
        //    SELECT @totalRecords = COUNT(*) FROM #TempTable;

        //    -- Select the paginated results with the total records count
        //    SELECT*, @totalRecords AS TotalRecords
        //    FROM #TempTable
        //    ORDER BY CDate DESC
        //    OFFSET @OffsetValue ROWS FETCH NEXT @PageLength ROWS ONLY;

        //    -- Drop the temporary table
        //    DROP TABLE #TempTable;

        //    SET NOCOUNT OFF;
        //END;



    }
}
