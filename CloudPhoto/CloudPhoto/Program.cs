using Amazon.S3;
using Cloudphoto.Commands.Interfaces;
using CommandLine;
using System.Reflection;

namespace Cloudphoto
{
    internal class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var commands = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.GetInterface(nameof(ICommand)) is not null)
                .ToArray();

            return await InvokeCommandsAsync(args, commands);
        }

        private static async Task<int> InvokeCommandsAsync(string[] args, Type[] commands)
        {
            try
            {
                var result = Parser.Default.ParseArguments(args, commands);
                    
                if (result.Tag is ParserResultType.Parsed)
                {
                    return await (result.Value as ICommand).InvokeAsync();
                }
                else
                {
                    return await Task.FromResult(1);
                }
            }
            catch (AmazonS3Exception aEx)
            {
                return await WriteErrorAsync(
                    $"Error encountered on server. Message: {aEx.Message}");
            }
            catch (Exception ex)
            {
                return await WriteErrorAsync(ex.Message);
            }
        }

        private static async Task<int> WriteErrorAsync(string message)
        {
            Console.Error.WriteLine(message);

            return await Task.FromResult(1);
        }
    }
}