#region ATRIBUTTES
using ChatbotConversacionalStorage.Application.Helper.Static.Settings;
using Npgsql;
using System.Data;
#endregion

namespace ChatbotConversacionalStorage.Domain.Connector
{
    public class APConnector
    {
        public IDbConnection Connection { get; }
        public IDbTransaction Transaction { get; set; }

        public APConnector()
        {
            Connection = new NpgsqlConnection(RumtimeSettings.ConnectionStringPostgre);
            Connection.Open();
        }

        public void Dispose() => Connection?.Dispose();
    }
}
