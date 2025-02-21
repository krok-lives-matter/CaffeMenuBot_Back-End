using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using CaffeMenuBot.Data.Models.Bot;
using CaffeMenuBot.Bot.Actions.Interface;

namespace CaffeMenuBot.Bot.Services
{
    /// <summary>
    /// Handles pattern matching for state / command / callback actions
    /// </summary>
    /// <typeparam name="PatternT">Type of action, derived from IPatternAction</typeparam>
    public sealed class PatternManager<PatternT> where PatternT: IPatternAction 
    {
        private readonly IServiceProvider _provider;
        private readonly ILoggerFactory _loggerFactory;

        public PatternManager(IServiceProvider provider, ILoggerFactory loggerFactory)
        {
            _provider = provider;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// fires respective action based on matching pattern (callback id, command text, user state)
        /// </summary>
        /// <param name="user">user object</param>
        /// <param name="update">update object</param>
        /// <param name="ct">cancellation token</param>
        public async Task HandlePatternAsync(BotUser user, Update update, CancellationToken ct)
        {
            using var scope = _provider.CreateScope();
            var scopedProvider = scope.ServiceProvider;

            IEnumerable<PatternT> patterns = scopedProvider.GetServices<PatternT>();
            foreach (var pattern in patterns)
            {
                if (pattern.Contains(user, update))
                {
                    try
                    {
                        await pattern.ExecuteAsync(user, update, ct);
                    }
                    catch (Exception e)
                    {
                        var logger = _loggerFactory.CreateLogger(pattern.GetType());
                        logger.LogError(e, $"Exception occurred while running the pattern of {typeof(PatternT)}");
                    }
                    break;
                }
            }
        }
    }
}