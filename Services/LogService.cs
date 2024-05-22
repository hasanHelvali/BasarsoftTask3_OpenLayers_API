using BasarSoftTask3_API.Logging;

namespace BasarSoftTask3_API.Services
{
    public class LogService
    {
        private readonly LogProducer _logProducer;

        public LogService()
        {
            _logProducer = new LogProducer();
        }

        public void Log(string message)
        {
            // Log your message
            _logProducer.SendLog("selamlar");
        }
    }
}
