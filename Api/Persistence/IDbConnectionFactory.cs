using System.Data;

namespace Api.Persistence;

public interface IDbConnectionFactory
{
    IDbConnection GetOpenConnection();
}
