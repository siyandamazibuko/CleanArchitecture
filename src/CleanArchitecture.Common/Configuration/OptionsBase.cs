using System;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Common.Configuration
{
    public abstract class OptionsBase<TOptions> where TOptions : OptionsBase<TOptions>, new()
    {
        public static TOptions Bind(IConfigurationRoot configurationRoot)
        {
            var options = new TOptions();
            GetConfigurationSection(configurationRoot).Bind(options);
            return options;
        }


        public static IConfigurationSection GetConfigurationSection(IConfigurationRoot configurationRoot)
        {
            return configurationRoot.GetSection(typeof(TOptions).Name);
        }

        public static Action<TOptions> Configure(IConfigurationRoot configurationRoot)
        {
            return options =>
            {
                configurationRoot.GetSection(typeof(TOptions).Name).Bind(options);
            };
        }
    }
}
