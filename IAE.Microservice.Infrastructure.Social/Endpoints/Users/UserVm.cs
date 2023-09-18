using IAE.Microservice.Application.Common.HClient;

namespace IAE.Microservice.Infrastructure.Social.Endpoints.Users
{
    /// <summary>
    /// Properties can be in the PascalCase format.
    /// When serializing or deserializing, the properties will be automatically converted to the camelCase format.
    /// </summary>
    public class UserVm
    {
        public class BaseRequest
        {
            public long Id { get; set; }
        }

        public class CreateOrUpdateRequest : BaseRequest
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Gender { get; set; }
            public string Age { get; set; }
            public string About { get; set; }
            public string[] Hobbies { get; set; }
            public long UiApiId { get; set; }
        }
        
        public class GetRequest : HClientQuery
        {
        }
        
        public class DeleteRequest : BaseRequest
        {
        }

        public class BaseResponse
        {
            public long Id { get; set; }
        }

        public class CreateOrUpdateResponse : BaseResponse
        {
        }
        
        public class GetResponse : HClientQuery
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Gender { get; set; }
            public string Age { get; set; }
            public string About { get; set; }
            public string[] Hobbies { get; set; }
            public long UiApiId { get; set; }
        }
        
        public class DeleteResponse : BaseResponse
        {
        }
    }
}