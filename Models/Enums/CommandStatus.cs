namespace ChGPTcmd.Models.Enums
{
    public enum CommandStatus
    {
        Success = 0,        //!- Command recognized and processed
        CommandError = 1,   //!- Command not recognized
        InternalError = 2,  //!- Internal error
        ApiError = 3,       //!- Error from the API 
        Quit = 4,           //!- Quit command
    }
}
