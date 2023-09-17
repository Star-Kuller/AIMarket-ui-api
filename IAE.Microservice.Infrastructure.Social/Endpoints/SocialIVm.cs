namespace IAE.Microservice.Infrastructure.Social.Endpoints
{
    public static class SocialIVm
    {
        public interface IId
        {
            /// <summary>
            /// number Уникальный идентификатор.
            /// </summary>
            long Id { get; set; }
        }
    }
}