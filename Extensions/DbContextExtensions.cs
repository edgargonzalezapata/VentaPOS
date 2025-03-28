using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using System.Reflection;

namespace VentaPOS.Extensions
{
    public static class DbContextExtensions
    {
        public static DbTransaction GetDbTransaction(this IDbContextTransaction transaction)
        {
            // Obtener la propiedad interna que contiene la transacción de base de datos
            var transactionPropertyInfo = transaction.GetType().GetProperty("DbTransaction", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (transactionPropertyInfo != null)
            {
                return transactionPropertyInfo.GetValue(transaction) as DbTransaction;
            }

            // Alternativa si la reflexión no funciona
            return transaction.GetType()
                .GetField("_dbTransaction", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(transaction) as DbTransaction;
        }
    }
} 