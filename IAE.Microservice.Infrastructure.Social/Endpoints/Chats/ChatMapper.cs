using AutoMapper;
using IAE.Microservice.Application.Common;
using IAE.Microservice.Application.Interfaces.Mapping;

namespace IAE.Microservice.Infrastructure.Social.Endpoints.Chats
{
    public class ChatMapper : SocialMapperBase, IHaveCustomMapping
    {
        public void CreateMappings(Profile configuration)
        {
            #region Base

            configuration.CreateMap<User, ChatVm.BaseRequest>()
                .ForMember(request => request.FirstName, opt
                    => opt.MapFrom(user => user.FirstName))
                .ForMember(request => request.LastName, opt
                    => opt.MapFrom(user => user.LastName))
                .ForMember(request => request.Status, opt
                    => opt.MapFrom((user, request) => ToInt(user.Status)))
                .ForMember(request => request.Email, opt
                    => opt.MapFrom(user => user.Email))
                .ForMember(request => request.Country, opt
                    => opt.MapFrom(user => user.Country.Name))
                //.ForMember(request => request.City, opt // It isn't necessary
                //    => opt.MapFrom(user => user))
                .ForMember(request => request.Phone, opt
                    => opt.MapFrom(user => user.PhoneNumber))
                //.ForMember(request => request.Skype, opt // It isn't necessary
                //    => opt.MapFrom(user => user))
                .ForMember(request => request.Currency, opt
                    => opt.MapFrom(user => Enum.GetName(typeof(Currency), user.Currency).ToUpper()))
                .ForAllOtherMembers(x => x.Ignore());

            #endregion

            #region Create

            configuration.CreateMap<User, ChatVm.CreateRequest>()
                // Redefinition password mapping in AdvertiserService.
                .ForMember(request => request.Password, opt
                    => opt.MapFrom(user => user.PhoneNumber))
                .IncludeBase<User, ChatVm.BaseRequest>();

            configuration.CreateMap<ChatVm.CreateResponse, User>()
                .ForMember(source => source.BidderId, opt
                    => opt.MapFrom(x => x.Id))
                .ForMember(source => source.BidderDmpId, opt
                    => opt.MapFrom(x => x.DmpId))
                .ForAllOtherMembers(x => x.Ignore());

            #endregion

            #region Edit

            configuration.CreateMap<User, ChatVm.EditRequest>()
                .IncludeBase<User, ChatVm.BaseRequest>();

            configuration.CreateMap<ChatVm.EditResponse, User>()
                .IncludeBase<ChatVm.CreateResponse, User>();

            #endregion

            #region Balance

            configuration.CreateMap<ChatVm.GetBalanceResponse, BalanceGet.Model>();

            configuration.CreateMap<ChatVm.AddBalanceResponse, BalanceAdd.Model>();

            #region List

            configuration.CreateMap<BalanceListById.Query, ChatVm.ListBalanceRequest>()
                .IncludeBase<PagedListQueryBase, BidderCommonIVm.IPaging>()
                .ForMember(dst => dst.Ids, opt
                    => opt.MapFrom(src => string.Join(",", src.BidderAdvertiserId)));

            configuration.CreateMap<ChatVm.ListBalanceItemResponse, BalanceListById.Model>();

            configuration.CreateMap<ChatVm.ListBalanceResponse, BalanceListById.Result>()
                .AfterMap((src, dst, context) =>
                {
                    dst.Clear();
                    foreach (var model in src.Data.Select(response =>
                                 context.Mapper.Map<BalanceListById.Model>(response)))
                    {
                        dst.Add(model.Id, model);
                    }
                });

            #endregion

            #endregion
        }
    }
}