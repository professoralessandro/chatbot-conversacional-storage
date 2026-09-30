#region IMPORTS
using ChatbotConversacionalStorage.Application.Helper.Static.Generic;
using ChatbotConversacionalStorage.Domain.Connector;
using ChatbotConversacionalStorage.Domain.Context.Postgre;
using ChatbotConversacionalStorage.Domain.Interfaces.Repository.Log;
using ChatbotConversacionalStorage.Infrastructure.Repositorys.Base.Postgre;
using Dapper;
using System.Data;
#endregion IMPORTS

namespace ChatbotConversacionalStorage.Infrastructure.Repositorys.Log
{
    public class LogRepository : RepositoryPostgreBase<ChatbotConversacionalStorage.Domain.Entities.Log>, ILogRepository
    {
        #region CONSTRUCTOR
        public LogRepository(APContextPostgre context, APConnector connector) : base(context, connector)
        {
            _context = context;
            _session = connector;
        }
        #endregion CONSTRUCTOR

        #region GET ALL BY PARAMETER ASYNC
        public async Task<IEnumerable<T>> GetAllPaginatedAsync<T>(DateTime? dateAdded = null, string? paramm = null, int? pagNumber = null, int? rowsPpage = null)
        {
            var parameters = new DynamicParameters();

            #region PAGINATOR CALCULATOR
            int _pagina = pagNumber.HasValue ? pagNumber.Value : 1;
            int _tamanho_pagina = rowsPpage.HasValue ? rowsPpage.Value : 10;

            int _offSet = (_pagina - 1) * _tamanho_pagina;
            #endregion PAGINATOR CALCULATOR

            parameters.Add("@dateadded", !dateAdded.HasValue ? null : dateAdded, dbType: DbType.DateTime);
            parameters.Add("@param", paramm == null || string.IsNullOrEmpty(paramm) ? null : paramm.RemoveInjections(), dbType: DbType.String);
            parameters.Add("@offset", _offSet, dbType: DbType.Int32);
            parameters.Add("@tamanho_pagina", _tamanho_pagina, dbType: DbType.Int32);

            string queryTest = @"
            -- Consulta com filtro pelo fileid e paginação
             SELECT
                logId
				,message
 				,request
 				,method
 				,response
 				,useraddedid
 				,dateadded
            FROM
                public.logs
            WHERE
                   (message LIKE CONCAT('%', @param, '%')           OR        @param IS NULL)
                AND (message     LIKE CONCAT('%', @param, '%')      OR        @param IS NULL)
                AND (method      LIKE CONCAT('%', @param, '%')      OR        @param IS NULL)
                AND (request     LIKE CONCAT('%', @title, '%')      OR        @param IS NULL)
                AND (response    LIKE CONCAT('%', @title, '%')      OR        @param IS NULL)
            ORDER BY
                fileid
            LIMIT
                @tamanho_pagina
            OFFSET
                @offset;";

            return await ReturnListFromQueryAsync<T>(queryTest, parameters);
        }
        #endregion GET ALL BY PARAMETER ASYNC
    }
}
