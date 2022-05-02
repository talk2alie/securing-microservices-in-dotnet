using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloboTicket.Services.Identity.Services
{
    public class TokenExchangeExtensionGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => "urn:ietf:params:oauth:grant_type:token_exchange";

        private readonly string _incomingTokenType = "urn:ietf:params:oauth:token_type:access_token";
        private readonly ITokenValidator _tokenValidator;

        public TokenExchangeExtensionGrantValidator(ITokenValidator tokenValidator)
        {
            _tokenValidator = tokenValidator;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            // The context parameter gives us access to the full request

            // 1. Check whether the request contains all the needed values

            // => Check that the requested grant_type matches the Grant Type for which this
            // extension is being created; in this case, the grant type above
            var requestedGrantType = context.Request.Raw.Get("grant_type");

            // Another way to get the grant type in the request is:
            // requestedGrantType = context.Request.Raw.Get("grant_type");
            if (string.IsNullOrWhiteSpace(requestedGrantType) || requestedGrantType != GrantType)
            {
                // Request failed
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid Grant");
                return;
            }

            // => Check that a subject_token is in the request
            var subjectToken = context.Request.Raw.Get("subject_token");
            if(string.IsNullOrWhiteSpace(subjectToken))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Subject token is missing");
                return;
            }

            // => Check that a subject_token_type is present
            var subjectTokenType = context.Request.Raw.Get("subject_token_type");
            if (string.IsNullOrWhiteSpace(subjectTokenType))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Subject token type is missing");
                return;
            }

            // => Check that the subject token type matches the expected token type; in our case, access_token
            // In our case, the incoming subject token type must be access_token
            if(subjectTokenType != _incomingTokenType)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Subject Token type is invalid");
            }

            // 2. Validate the incoming token
            // If the expected token were an id_token, you would use the
            // ValidateIdentityTokenAsync method
            var validationResult = await _tokenValidator.ValidateAccessTokenAsync(subjectToken);
            if(validationResult.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant,
                    "Subject token is invalid");
                return;
            }

            // 3. Create and return new Token

            // => Get the sub value from the validation result, which has the sub value and other claims
            var subjectClaim = validationResult.Claims.FirstOrDefault(c => c.Type == "sub");
            if(subjectClaim is null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest,
                    "Subject token must contain a sub value");
            }

            // => Finally, create the new token and return it
            // Before generating the new token, you can do additional custom things:
            //  => Check if user is allowed to access the requested scope
            //  => Transform claims to ones your app understand
            //  => Add/Remove claims to/from the incoming claims
            context.Result = new GrantValidationResult(subjectClaim.Value, "access_token", validationResult.Claims);
            return;
        }
    }
}
