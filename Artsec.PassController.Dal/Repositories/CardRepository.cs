﻿using Artsec.PassController.Dal.Models;
using Dapper;

namespace Artsec.PassController.Dal.Repositories;

public class CardRepository
{
    private readonly IConnectionProvider _connectionProvider;

    public CardRepository(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<IEnumerable<CardModel>> GetAsync()
    {
        var sql =
            "select c.id_card, c.id_cardtype, p.id_pep, p.authmode " +
            "from card c join people p on p.id_pep = c.id_pep ";

        using var connection = _connectionProvider.CreateConnection();
        await connection.OpenAsync();
        var result = await connection.QueryAsync<CardModel, PeopleModel, CardModel>(
            sql,
            (card, pep) => { card.People = pep; return card; },
            splitOn: "id_card,id_pep"
        );
        return result ?? Enumerable.Empty<CardModel>();
    }
    public async Task<CardModel?> GetByIdAsync(string id)
    {
        var sql =
            "select c.id_card, c.id_cardtype, p.id_pep, p.authmode " +
            "from card c join people p on p.id_pep = c.id_pep " +
            "WHERE c.id_card = @Id";

        var parameters = new { Id = id };
        using var connection = _connectionProvider.CreateConnection();
        await connection.OpenAsync();
        var result = await connection.Connection.QueryAsync<CardModel, PeopleModel, CardModel>(
            sql,
            (card, pep) => { card.People = pep; return card; },
            param: parameters,
            splitOn: "id_card,id_pep"
        );
        return result.FirstOrDefault();
    }
}
