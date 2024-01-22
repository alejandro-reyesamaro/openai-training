namespace ChGPTcmd.Infrastructure.Handlers
{
    public interface IServiceHandler
    {
        bool Handles(int option);

        Task Handle();
    }
}
