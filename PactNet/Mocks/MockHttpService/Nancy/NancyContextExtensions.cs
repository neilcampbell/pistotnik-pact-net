﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public static class NancyContextExtensions
    {
        private const string PactMockInteractionsKey = "PactMockInteractions";

        public static void SetMockInteraction(this NancyContext context, IEnumerable<ProviderServiceInteraction> interactions)
        {
            context.Items[PactMockInteractionsKey] = interactions;
        }

        public static ProviderServiceInteraction GetMatchingInteraction(this NancyContext context, HttpVerb method, string path)
        {
            if (!context.Items.ContainsKey(PactMockInteractionsKey))
            {
                throw new InvalidOperationException("No mock interactions have been registered");
            }

            var requestResponsePairs = (IEnumerable<ProviderServiceInteraction>)context.Items[PactMockInteractionsKey];

            if (requestResponsePairs == null)
            {
                throw new ArgumentException("No matching mock interaction has been registered for the current request");
            }

            var matchingRequestResponsePairs = requestResponsePairs.Where(x =>
                x.Request.Method == method &&
                x.Request.Path == path).ToList();

            if (matchingRequestResponsePairs == null || !matchingRequestResponsePairs.Any())
            {
                throw new ArgumentException("No matching mock interaction has been registered for the current request");
            }

            if (matchingRequestResponsePairs.Count() > 1)
            {
                throw new ArgumentException("More than one matching mock interaction has been registered for the current request");
            }

            return matchingRequestResponsePairs.Single();
        }
    }
}
