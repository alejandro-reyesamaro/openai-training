namespace ChGPTcmd.Application.Handlers
{
    public interface IServiceHandler
    {
        bool Handles(int option);

        Task Handle();
    }
}
