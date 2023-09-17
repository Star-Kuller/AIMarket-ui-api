namespace IAE.Microservice.Infrastructure.Social.Endpoints.Chats
{
    /// <summary>
    /// Properties can be in the PascalCase format.
    /// When serializing or deserializing, the properties will be automatically converted to the camelCase format.
    /// </summary>
    public class ChatVm
    {
        public class BaseRequest
        {
            /// <summary>
            /// string [required] Имя. Не более 255 символов.
            /// </summary>
            public string FirstName { get; set; }

            /// <summary>
            /// string [required] Фамилия. Не более 255 символов.
            /// </summary>
            public string LastName { get; set; }

            /// <summary>
            /// number [required] Статус. 1 - статус active, 2 - статус stopped.
            /// </summary>
            public int Status { get; set; }

            /// <summary>
            /// string [required] Электронная почта. Не более 255 символов.
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// string [required] Страна. Не более 255 символов.
            /// </summary>
            public string Country { get; set; }

            /*/// <summary>
            /// string Город. Не более 255 символов.
            /// </summary>
            public string City { get; set; }*/

            /// <summary>
            /// string [required] Мобильный телефон в международном формате.
            /// </summary>
            public string Phone { get; set; }

            /*/// <summary>
            /// string Ник в скайпе. Не более 255 символов.
            /// </summary>
            public string Skype { get; set; }*/

            /// <summary>
            /// string Валюта. Фиксированный список валют: [USD, GBP, EUR, RUB]. Только в верхнем регистре.
            /// </summary>
            public string Currency { get; set; }
        }

        public class CreateRequest : BaseRequest
        {
            /// <summary>
            /// string [required] Пароль. Не менее 6 символов, не более 255 символов.
            /// </summary>
            public string Password { get; set; }
        }

        public class CreateResponse :
            BidderIVm.IId,
            BidderIVm.IDmpId
        {
            public long Id { get; set; }

            public string DmpId { get; set; }
        }

        public class EditRequest : BaseRequest
        {
        }

        public class EditResponse : CreateResponse
        {
        }

        #region Balance

        public class GetBalanceResponse : BidderAdvertiserIVm.IGetBalanceResponse
        {
            public decimal Balance { get; set; }
            public string Currency { get; set; }
        }

        public class AddBalanceRequest : BidderAdvertiserIVm.IAddBalanceRequest
        {
            public decimal Amount { get; set; }
        }

        public class AddBalanceResponse : BidderAdvertiserIVm.IAddBalanceResponse
        {
            public decimal Balance { get; set; }
            public string Currency { get; set; }
        }

        #region List

        public class ListBalanceRequest : BidderClientQuery, BidderAdvertiserIVm.IListBalanceRequest
        {
            public int Page { get; set; }

            public int Size { get; set; }

            public string Ids { get; set; }
        }

        public class ListBalanceItemResponse : BidderAdvertiserIVm.IListBalanceItemResponse
        {
            public long Id { get; set; }

            public decimal Balance { get; set; }

            public string Currency { get; set; }
        }

        public class ListBalanceResponse : BidderAdvertiserIVm.IListBalanceResponse<ListBalanceItemResponse>
        {
            public int TotalPages { get; set; }

            public int TotalCount { get; set; }

            public ListBalanceItemResponse[] Data { get; set; }
        }

        #endregion

        #endregion
    }
}