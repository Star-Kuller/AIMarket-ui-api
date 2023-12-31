﻿using IAE.Microservice.Application.Common.HClient;

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
            public long Id { get; set; }
        }

        public class CreateOrUpdateRequest : BaseRequest
        {
            public string Name { get; set; }
            public long[] Participants { get; set; }
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
            public long[] Participants { get; set; }
        }
        
        public class DeleteResponse : BaseResponse
        {
        }
    }
}