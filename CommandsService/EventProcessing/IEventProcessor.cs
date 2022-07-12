namespace CommandsService.EventProcessing
{
    public interface IEventProcessor
    {
        void Process(string message);
    }
}