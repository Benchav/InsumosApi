﻿using Domain.Endpoint.Entities;
using System.Collections.Generic;
using System;
using Domain.Endpoint.Interfaces.Repositories;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Infrastructure.Endpoint.Builders;
using Infrastructure.Endpoint.Interfaces;

namespace Infrastructure.Endpoint.Data.Repositories
{
    public class ProductoRepository : IProductoRepository
    {

        private readonly ISqlDbConnection _sqlDbConnection;
        private readonly ISqlCommandOperationBuilder _operationBuilder;
        public ProductoRepository(ISqlDbConnection sqlDbConnection, ISqlCommandOperationBuilder operationBuilder)
        {
            _sqlDbConnection = sqlDbConnection;
            _operationBuilder = operationBuilder;

        }

        public void CreateProducto(Producto nuevoProducto)
        {
            SqlCommand writeCommand = _operationBuilder.From(nuevoProducto)
                .WithOperation(SqlWriteOperation.Create)
                .BuildWritter();
            _sqlDbConnection.ExecuteNonQueryCommand(writeCommand);
        }

        public void DeleteProducto(Guid Id)
        {

            string delec = "DELETE FROM TblProducto WHERE IdProducto = @IdProducto";
            SqlCommand sqlCommand = _sqlDbConnection.TraerConsulta(delec);
            SqlParameter parameter = new SqlParameter()
            {
                Direction = ParameterDirection.Input,
                ParameterName = "@IdProducto",
                SqlDbType = SqlDbType.UniqueIdentifier,
                Value = Id
            };
            sqlCommand.Parameters.Add(parameter);
            sqlCommand.ExecuteNonQuery();
        }


        public async Task<List<Producto>> Get()
        {

            SqlCommand readCommand = _operationBuilder.Initialize<Producto>()
              .WithOperation(SqlReadOperation.Select)
              .BuildReader();
            DataTable dt = await _sqlDbConnection.ExecuteQueryCommandAsync(readCommand);

            List<Producto> cat = dt.AsEnumerable().Select(row =>
            new Producto
            {
                Id = row.Field<Guid>("IdProducto"),
                NombreProducto = row.Field<string>("NombreProducto"),
                Descripcion = row.Field<string>("Descripcion"),
                IdCategoria = row.Field<Guid>("IdCategoria"),
                Preciocompra = row.Field<int>("PrecioCompra"),
                Precioventa = row.Field<int>("PrecioVenta"),
                Estado = row.Field<int>("Estado"),
                FechaCompra = row.Field<DateTime>("FechaCompra"),
                FechaVencimiento = row.Field<DateTime>("FechaVencimiento"),
            }).ToList();

            return cat;

        }



        public void UpdateProducto(Guid Id, Producto nuevosRegistros)
        {
            SqlCommand writeCommand = _operationBuilder.From(nuevosRegistros)
             .WithOperation(SqlWriteOperation.Update)
             .BuildWritter();
            _sqlDbConnection.ExecuteNonQueryCommandAsync(writeCommand);
        }
    }
}
