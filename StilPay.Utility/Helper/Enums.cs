namespace StilPay.Utility.Helper
{
    public class Enums
    {
        public enum SQLMode
        {
            New = 1,
            Edit = 2,
            Drop = 3,
            Get = 4
        }

        public enum FieldType
        {
            None = 0,
            Int = 1,
            SmallInt = 2,
            NVarChar = 3,
            Decimal = 4,
            Tinyint = 5,
            Bit = 6,
            DateTime = 7,
            VarBinary = 8,
            List = 9,
            Entity = 10
        }

        public enum TransactionType
        {
            Commit = 1,
            RollBack = 2
        }

        public enum ErrorType
        {
            FK = 1,
            UQ = 2,
            UnKnown = 3,
            NULL = 4
        }

        public enum CompanyStatus
        {
            Register = 1,
            Accepted = 2,
            Rejected = 3,
            Cancel = 4
        }

        public enum StatusType
        {
            Pending = 1,
            Confirmed = 2,
            Canceled = 3,
            All = 4,
            PayPool = 5,
            FraudPool = 6,
            Fraud = 7,
            Process = 8,
            Refunded = 9,
            Risk
        }

        public enum ActionType
        {
            PaymentAuto = 10,
            PaymentNotify = 20,
            DealerWithdrawal = 30,
            MemberWithdrawal = 40,
            MoneyTransferOut = 50,
            MoneyTransferIn = 60,
            DealerPayment = 70,
            MemberPayment = 80,
            Rebate = 90,
            CreditCardPaymentNotify = 100,
            ForeignCreditCardPaymentNotify = 140,
            TransferPoolManualMatching = 150
        }

        public enum CompanyIntegrationType
        {
            Param = 1,
            PayNKolay = 2,
        }

        public enum PaymentMethod
        {
            TransferEftHavale = 1,
            CreditCard,
            ForeignCreditCard
        }

        public enum FinanceIncomeType
        {
            MuhtelifGelirler = 1 ,
            BankaGiris,
            KrediKartiGiris
        }


        public enum FinanceExpenseType
        {
            MuhtelifGiderler = 1 ,
            BankaCikis,
            KrediKartiCikis
        }

        public enum TableWithTheTransaction
        {
            PaymentNotification = 1,
            CreditCardPaymentNotification,
            ForeignCreditCardPaymentNotification,
            CompanyWithdrawalRequests,
            CompanyRebateRequests
        }


        public enum CallbackTransacationType
        {
            Transfer = 1,
            CreditCard,
            Withdrawal,
            Rebate,
            ForeignCreditCard
        }

		public enum CreditCardPaymentMethodType
		{
			Param = 1,
			PayNKolay,
            ForeignPayNKolay,
            IsBankSanalPOS,
            Paybull,
            AKODE,
            Tosla,
            EsnekPos,
            LidioPos,
            ForeignLidioPosTL,
            ForeignLidioPosCurrency,
            EfixPos
        }

        public enum CreditCardType
        {
            BankCard = 1,
            CreditCard,
            Tosla
        }
    }
}