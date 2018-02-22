﻿using Cofoundry.Core.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SimpleSite
{
    public class ContactRequestFormViewComponent : ViewComponent
    {
        private readonly IMailService _mailService;
        private readonly SimpleTestSiteSettings _simpleTestSiteSettings;

        public ContactRequestFormViewComponent(
            IMailService mailService,
            SimpleTestSiteSettings simpleTestSiteSettings
            )
        {
            _mailService = mailService;
            _simpleTestSiteSettings = simpleTestSiteSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var contactRequest = new ContactRequest();

            // ModelBinder is not supported in view components so we have to bind
            // this manually. We have an issue open to try and improve the experience here
            // https://github.com/cofoundry-cms/cofoundry/issues/125
            if (Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                contactRequest.Name = Request.Form[nameof(contactRequest.Name)];
                contactRequest.EmailAddress = Request.Form[nameof(contactRequest.EmailAddress)];
                contactRequest.Message = Request.Form[nameof(contactRequest.Message)];
                
                if (ModelState.IsValid)
                {
                    // Send admin confirmation
                    var template = new ContactRequestMailTemplate();
                    template.Request = contactRequest;
                    await _mailService.SendAsync(_simpleTestSiteSettings.ContactRequestNotificationToAddress, template);

                    return View("ContactSuccess");
                }
            }

            return View(contactRequest);
        }

    }
}
