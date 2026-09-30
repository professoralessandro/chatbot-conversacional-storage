#region REFERENCES
using ChatbotConversacionalStorage.Application.Helper.Static.Generic;
using ChatbotConversacionalStorage.Domain.Connector;
using ChatbotConversacionalStorage.Domain.Context.Postgre;
using ChatbotConversacionalStorage.Domain.Entities;
using ChatbotConversacionalStorage.Domain.Interfaces.Repository.FileStorage;
using ChatbotConversacionalStorage.Infrastructure.Repositorys.Base.Postgre;
using Dapper;
using System.Data;
#endregion REFERENCES

namespace ChatbotConversacionalStorage.Infrastructure.Repositorys.FileStorage
{
    public class FileStorageRepository : RepositoryPostgreBase<StorageFile>, IFileStorageRepository
    {
        #region CONSTRUCTOR
        public FileStorageRepository(APContextPostgre context, APConnector connector) : base(context, connector)
        {
            _context = context;
            _session = connector;
        }
        #endregion

        #region GET ALL BY PARAMETER ASYNC
        public async Task<IEnumerable<T>> GetAllPaginatedAsync<T>(Guid? fileId = null, string? title = null, Guid? externalReferenceId = null, int? pagNumber = null, int? rowsPpage = null)
        {
            var parameters = new DynamicParameters();

            #region PAGINATOR CALCULATOR
            int _pagina = pagNumber.HasValue ? pagNumber.Value : 1;
            int _tamanho_pagina = rowsPpage.HasValue ? rowsPpage.Value : 10;

            int _offSet = (_pagina - 1) * _tamanho_pagina;
            #endregion PAGINATOR CALCULATOR

            parameters.Add("@id", !fileId.HasValue ? null : fileId, dbType: DbType.Guid);
            parameters.Add("@external_reference_id", !externalReferenceId.HasValue ? null : externalReferenceId, dbType: DbType.Guid);
            parameters.Add("@title", title == null || string.IsNullOrEmpty(title) ? null : title.RemoveInjections(), dbType: DbType.String);
            parameters.Add("@offset", _offSet, dbType: DbType.Int32);
            parameters.Add("@tamanho_pagina", _tamanho_pagina, dbType: DbType.Int32);

            string queryTest = @"
            -- Consulta com filtro pelo fileid e paginação
             SELECT
                fileid,
                title,
                filepath,
                ""Description"",
                mainfile,
                public,
                externalreferenceid,
                useraddedid,
                userupdatedid,
                dateadded,
                dateupdated
            FROM
                public.storagefiles
            WHERE
                (fileid = @Id OR @Id IS NULL)
                AND (externalreferenceid = @external_reference_id OR @external_reference_id IS NULL)
                AND (title LIKE CONCAT('%', @title, '%') OR @title IS NULL)
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
