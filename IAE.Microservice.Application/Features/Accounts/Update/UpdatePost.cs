using IAE.Microservice.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Common.Operation;
using IAE.Microservice.Application.Features.Accounts.Update.Models;
using IAE.Microservice.Application.Infrastructure;

namespace IAE.Microservice.Application.Features.Accounts.Update
{
    public class UpdatePost
    {
        public class ResponseItem
        {
            public const string ChangeDefault = "The value has not been changed.";

            public string Name { get; set; }

            public string Change { get; set; }

            public List<string> Errors { get; set; }

            public ResponseItem()
            {
                Change = ChangeDefault;
                Errors = new List<string>();
            }
        }

        public class Response
        {
            public OperationApiResult<List<ResponseItem>> Result { get; set; }
        }

        public class Request : IRequest<Response>, IUpdateUser
        {
            public long? Id { get; set; }

            public string Email { get; set; }

            public List<UpdateKeys> Keys { get; set; }

            public UpdateValuesWithPassword Values { get; set; }
        }

        public class Validator : UpdateUserValidator<Request>
        {
            public Validator()
            {
            }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly UserManager<User> _userManager;

            public Handler(UserManager<User> userManager)
            {
                _userManager = userManager;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response SetupResponse(Response resp, List<ResponseItem> respItems)
                {
                    resp.Result = OperationApiResult<List<ResponseItem>>.Success(respItems, HttpStatusCode.OK);
                    return resp;
                }

                var response = new Response();
                var responseItems = new List<ResponseItem>();
                var commonItem = new ResponseItem {Name = "Common"};
                if (request.Keys == null || request.Keys.Count == 0)
                {
                    commonItem.Change = "No changes occurred because the keys list to update is null or empty.";
                    responseItems.Add(commonItem);
                    return SetupResponse(response, responseItems);
                }

                if (!request.Keys.All(x => Enum.IsDefined(typeof(UpdateKeys), x)))
                {
                    commonItem.Change = "No changes occurred because the keys list to update contains " +
                                        "specific argument(s) which is(are) out of the range of valid values.";
                    responseItems.Add(commonItem);
                    return SetupResponse(response, responseItems);
                }

                if (request.Values == null)
                {
                    commonItem.Change = "No changes occurred because the values object to update is null.";
                    responseItems.Add(commonItem);
                    return SetupResponse(response, responseItems);
                }

                try
                {
                    await Transaction.Do(async () =>
                    {
                        try
                        {
                            var user = await request.GetUserAsync(_userManager);
                            foreach (var key in request.Keys)
                            {
                                try
                                {
                                    var item = await UpdateAsync(key, request.Values, user);
                                    responseItems.Add(item);
                                }
                                catch (Exception ex)
                                {
                                    var item = new ResponseItem {Name = key.ToString()};
                                    item.Errors.Add($"Message: {ex.Message}\nTrace: {ex.StackTrace}");
                                    responseItems.Add(item);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            commonItem.Errors.Add(ex.Message);
                            responseItems.Add(commonItem);
                        }

                        if (responseItems.Exists(x => x.Errors.Count > 0))
                        {
                            responseItems.ForEach(x => x.Change = ResponseItem.ChangeDefault);
                            throw new Exception();
                        }
                    });
                }
                catch
                {
                    // ignored
                }

                return SetupResponse(response, responseItems);
            }

            private async Task<ResponseItem> UpdateAsync(UpdateKeys key, UpdateValuesWithPassword values, User user)
            {
                var item = new ResponseItem {Name = key.ToString()};
                switch (key)
                {
                    case UpdateKeys.Password:
                    {
                        var newPassword = values.Password;
                        if (string.IsNullOrEmpty(newPassword))
                        {
                            item.Errors.Add("The value cannot equals empty or null.");
                            break;
                        }

                        var hash = _userManager.PasswordHasher
                            .VerifyHashedPassword(user, user.PasswordHash, newPassword);
                        if (hash == PasswordVerificationResult.Success)
                        {
                            item.Change = ValuesEqual(newPassword);
                            break;
                        }

                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var iResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
                        if (iResult.Succeeded)
                        {
                            item.Change = ValueChanged(newPassword);
                        }
                        else
                        {
                            item.Errors = iResult.Errors.Select(err => err.Description).ToList();
                        }

                        break;
                    }
                    case UpdateKeys.Email:
                    {
                        var newEmail = values.Email;
                        if (!IsValidEmail(newEmail))
                        {
                            item.Errors.Add("The value is not email address.");
                            break;
                        }

                        var oldEmail = user.Email;
                        if (oldEmail == newEmail)
                        {
                            item.Change = ValuesEqual(newEmail);
                            break;
                        }

                        var iResult = await _userManager.SetEmailAsync(user, newEmail);
                        await _userManager.UpdateNormalizedEmailAsync(user);
                        if (iResult.Succeeded)
                        {
                            item.Change = ValueChanged(newEmail, oldEmail);
                        }
                        else
                        {
                            item.Errors = iResult.Errors.Select(err => err.Description).ToList();
                        }

                        var newUserName = values.Email;
                        iResult = await _userManager.SetUserNameAsync(user, newUserName);
                        await _userManager.UpdateNormalizedUserNameAsync(user);
                        if (!iResult.Succeeded)
                        {
                            item.Errors = iResult.Errors.Select(err => err.Description).ToList();
                        }

                        break;
                    }
                    case UpdateKeys.EmailConfirmed:
                    {
                        var newValue = values.EmailConfirmed;
                        var oldValue = user.EmailConfirmed;
                        if (oldValue == newValue)
                        {
                            item.Change = ValuesEqual(newValue.ToString());
                            break;
                        }

                        user.EmailConfirmed = newValue;
                        var iResult = await _userManager.UpdateAsync(user);
                        if (iResult.Succeeded)
                        {
                            item.Change = ValueChanged(newValue.ToString(), oldValue.ToString());
                        }
                        else
                        {
                            item.Errors = iResult.Errors.Select(err => err.Description).ToList();
                        }

                        break;
                    }
                    case UpdateKeys.Phone:
                    {
                        var newValue = values.Phone;
                        var oldValue = user.PhoneNumber;
                        if (oldValue == newValue)
                        {
                            item.Change = ValuesEqual(newValue);
                            break;
                        }

                        user.PhoneNumber = newValue;
                        var iResult = await _userManager.UpdateAsync(user);
                        if (iResult.Succeeded)
                        {
                            item.Change = ValueChanged(newValue, oldValue);
                        }
                        else
                        {
                            item.Errors = iResult.Errors.Select(err => err.Description).ToList();
                        }

                        break;
                    }
                    case UpdateKeys.FirstName:
                    {
                        var newValue = values.FirstName;
                        var oldValue = user.Name;
                        if (oldValue == newValue)
                        {
                            item.Change = ValuesEqual(newValue);
                            break;
                        }

                        user.Name = newValue;
                        var iResult = await _userManager.UpdateAsync(user);
                        if (iResult.Succeeded)
                        {
                            item.Change = ValueChanged(newValue, oldValue);
                        }
                        else
                        {
                            item.Errors = iResult.Errors.Select(err => err.Description).ToList();
                        }

                        break;
                    }
                    case UpdateKeys.LastName:
                    {
                        var newValue = values.LastName;
                        var oldValue = user.Name;
                        if (oldValue == newValue)
                        {
                            item.Change = ValuesEqual(newValue);
                            break;
                        }

                        user.Name = newValue;
                        var iResult = await _userManager.UpdateAsync(user);
                        if (iResult.Succeeded)
                        {
                            item.Change = ValueChanged(newValue, oldValue);
                        }
                        else
                        {
                            item.Errors = iResult.Errors.Select(err => err.Description).ToList();
                        }

                        break;
                    }
                    case UpdateKeys.Language:
                    {
                        var oldValue = user.Language;
                        var newValue = values.Language;
                        if (oldValue == newValue)
                        {
                            item.Change = ValuesEqual(newValue.ToString());
                            break;
                        }

                        user.Language = newValue;
                        var iResult = await _userManager.UpdateAsync(user);
                        if (iResult.Succeeded)
                        {
                            item.Change = ValueChanged(newValue.ToString(), oldValue.ToString());
                        }
                        else
                        {
                            item.Errors = iResult.Errors.Select(err => err.Description).ToList();
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return item;
            }

            private static string ValueChanged(string newValue, string oldValue = null) =>
                $"The value has been changed {(oldValue == null ? "" : $"from '{oldValue}'")} to '{newValue}'.";

            private static string ValuesEqual(string newValue) =>
                $"The value has not been changed because old and new values ('{newValue}') are equal.";

            private static bool IsValidEmail(string email)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    return addr.Address == email;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}