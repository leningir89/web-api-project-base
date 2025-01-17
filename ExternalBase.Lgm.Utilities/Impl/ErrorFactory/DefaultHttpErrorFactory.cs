﻿using ExternalBase.Lgm.Utilities.Entities;
using ExternalBase.Lgm.Utilities.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ExternalBase.Lgm.Utilities.Impl.ErrorFactory
{
    public class DefaultHttpErrorFactory : IHttpErrorFactory
    {
        private readonly IHostingEnvironment env;
        private readonly IDictionary<Type, Func<Exception, HttpError>> factory;

        public DefaultHttpErrorFactory(IHostingEnvironment env)
        {
            this.env = env;

            factory = new Dictionary<Type, Func<Exception, HttpError>>
            {
                { typeof(Exception), InternalServerError }
            };
        }

        public HttpError CreateFrom(Exception exception)
        {
            if (factory.TryGetValue(exception.GetType(), out Func<Exception, HttpError> func))
            {
                return func(exception);
            }

            return factory[typeof(Exception)](exception);
        }

        private HttpError InternalServerError(Exception exception)
        {
            return HttpError.Create(
                env,
                status: HttpStatusCode.InternalServerError,
                userMessage:  "Ups... al parecer algo salió mal. Por favor inténtalo más tarde." ,
                developerMessage: $"{exception.Message}\r\n{exception.StackTrace}");
        }
    }
}
