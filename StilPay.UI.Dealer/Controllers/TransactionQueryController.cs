using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StilPay.BLL.Abstract;
using StilPay.BLL;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.UI.Dealer.Models;
using System.Linq;

namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "Process")]
    public class TransactionQueryController : BaseController<CompanyTransaction>
    {
        private readonly ICompanyTransactionManager _manager;

        public TransactionQueryController(ICompanyTransactionManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<CompanyTransaction> Manager()
        {
            return _manager;
        }
        [HttpPost]
        public IActionResult Search(string queryParameter)
        {
            var model = new DealerTransactionQueryEditViewModel()
            {
                DealerTransactionQuery = _manager.GetRecordsByQueryParameter(queryParameter)
            };

            if (model.DealerTransactionQuery.Count == 0 || (model.DealerTransactionQuery.Count == 1 && model.DealerTransactionQuery.Any(x => x.TableWithTheTransaction == (byte)Enums.TableWithTheTransaction.CompanyRebateRequests)))
                return Json(new GenericResponse { Status = "ERROR", Message = "Kayıt Bulunamadı", Data = model });

            var hasRebateEntity = model.DealerTransactionQuery.FirstOrDefault(x => x.TableWithTheTransaction == (byte)Enums.TableWithTheTransaction.CompanyRebateRequests);

            foreach (var item in model.DealerTransactionQuery.Where(x => x.TableWithTheTransaction != (byte)Enums.TableWithTheTransaction.CompanyRebateRequests))
            {
                switch (item.TableWithTheTransaction)
                {
                    case (byte)Enums.TableWithTheTransaction.PaymentNotification:

                        if (hasRebateEntity != null)
                        {
                            if (hasRebateEntity.Status == (byte)Enums.StatusType.Pending)
                                model.SecondUrl = $"/Process/Detail/90/{hasRebateEntity.ID}";
                            else
                                model.SecondUrl = $"/Process/Detail/90/{hasRebateEntity.ID}";
                        }

                        model.Url = $"/Process/Detail/10/{item.ID}";
                        
                        break;

                    case (byte)Enums.TableWithTheTransaction.CreditCardPaymentNotification:

                        if (hasRebateEntity != null)
                        {
                            if (hasRebateEntity.Status == (byte)Enums.StatusType.Pending)
                                model.SecondUrl = $"/Process/Detail/90/{hasRebateEntity.ID}";
                            else
                                model.SecondUrl = $"/Process/Detail/90/{hasRebateEntity.ID}";
                        }

                        model.Url = $"/Process/Detail/100/{item.ID}";
                        
                        break;

                    case (byte)Enums.TableWithTheTransaction.ForeignCreditCardPaymentNotification:
                            model.Url = $"/Process/Detail/140/{item.ID}";
                        break;

                    case (byte)Enums.TableWithTheTransaction.CompanyWithdrawalRequests:
                            model.Url = $"/Process/Detail/30/{item.ID}";
                        break;

                        //case (byte)Enums.TableWithTheTransaction.CompanyRebateRequests:
                        //    if (item.Status == (byte)Enums.StatusType.Pending)
                        //        model.Url = $"/DealerRebateRequest/Edit/{item.ID}";
                        //    else
                        //        model.Url = $"/DealerRebateTransaction/Edit/{item.ID}";
                        //    break;
                }
            }

            return Json(new GenericResponse { Data = model });
        }
    }
}
