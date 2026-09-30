using ChatbotConversacionalStorage.Domain.Interfaces.Data;

namespace ChatbotConversacionalStorage.Domain.Connector
{
    public class APWork : IAPDWork
    {
        private readonly APConnector _session;

        public APWork(APConnector session)
        {
            _session = session;
        }

        public void BeginTransaction()
        {
            _session.Transaction = _session.Connection.BeginTransaction();
        }

        public void Commit()
        {
            _session.Transaction.Commit();
            Dispose();
        }

        public void Rollback()
        {
            _session.Transaction.Rollback();
            Dispose();
        }

        public void Dispose() => _session.Transaction?.Dispose();
    }
}
