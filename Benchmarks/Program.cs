using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
    public static class FormatChannel_Original
    {
        public static string FormatChannel(string key, string channel, params string[] options)
        {
            // Prefix with the key
            var formatted = key.EndsWith("/")
                ? key + channel
                : key + "/" + channel;

            // Add trailing slash
            if (!formatted.EndsWith("/"))
                formatted += "/";

            // Add options
            if (options != null && options.Length > 0)
            {
                formatted += "?";
                for (int i = 0; i < options.Length; ++i)
                {
                    if (options[i][0] == '+')
                        continue;

                    formatted += options[i];
                    if (i + 1 < options.Length)
                        formatted += "&";
                }
            }

            // We're done compiling the channel name
            return formatted;
        }
    }

    public static class FormatChannel_StringBuilder
    {
        public static string FormatChannel(string key, string channel, params string[] options)
        {
            // Prefix with the key
            var builder = new StringBuilder();
            builder.Append(key.EndsWith("/") ? key + channel : key + "/" + channel);

            // Add trailing slash
            if (!channel.EndsWith("/")) builder.Append("/");

            // Add options
            if (options != null && options.Length > 0)
            {
                builder.Append("?");
                for (int i = 0; i < options.Length; ++i)
                {
                    if (options[i][0] == '+') continue;

                    builder.Append(options[i]);
                    if (i + 1 < options.Length) builder.Append("&");
                }
            }

            // We're done compiling the channel name
            return builder.ToString();
        }
    }

    public class FormantChannel_Benchmarks
    {
        [Benchmark]
        public string original() => FormatChannel_Original.FormatChannel("EckDAy4LHt_T0eTPSBK_0dmOAGhakMgJ", "test/", "ttl=3600", "me=1");

        [Benchmark]
        public string stringBuilder() => FormatChannel_StringBuilder.FormatChannel("EckDAy4LHt_T0eTPSBK_0dmOAGhakMgJ", "test/", "ttl=3600", "me=1");

    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<FormantChannel_Benchmarks>();
            Console.ReadLine();
        }
    }
}
