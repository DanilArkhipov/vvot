namespace Cloudphoto.Commands.Interfaces
{
    internal interface ICommand
    {
        public Task<int> InvokeAsync();
    }
}