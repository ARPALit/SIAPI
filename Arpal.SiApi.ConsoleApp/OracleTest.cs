using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Arpal.SiApi.ConsoleApp
{
    internal class OracleTest
    {
        String ConnectionString = ";";


        public async Task ExecReadAsync()
        {
            var query = @"SELECT ANNO,ACQUA,ACQUA_PROVINCIA,ACQUA_COMUNE,ACQUA_DESCRIZIONE,STATO,STATO_MOTIVO,STATO_COLORE,STATO_CODICECOLORE,
                           STATO_DATA,CLASSIFICAZIONE,CLASSIFICAZIONE_COLORE, CLASSIFICAZIONE_STELLE,CLASSIFICAZIONE_LOGO,PROFILO_PDF,AREA_OSTREOPSIS from 
                           BALNEREAD.V_PUB_STATIACQUE@BALNEREAD.SVI2DB.REGIONE.LIGURIA.IT WHERE 
                           ((:CODICE_ACQUA is null or ACQUA=:CODICE_ACQUA) and (:ANNO_STATO is null or ANNO=:ANNO_STATO) )";

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("ANNO_STATO", OracleDbType.Int32)).Value = 2023;
                    command.Parameters.Add(new OracleParameter("CODICE_ACQUA", OracleDbType.Varchar2)).Value = null;

                    command.BindByName = true;

                    using (OracleDataReader reader = (OracleDataReader)(command.ExecuteReader()))
                    {
                        var list = new List<dynamic>();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                dynamic dynamicData = new ExpandoObject();
                                var dynamicDictionary = (IDictionary<string, object>)dynamicData;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string columnName = reader.GetName(i);
                                    object value = reader[i];
                                    dynamicDictionary[columnName] = value;
                                }
                                list.Add(dynamicData);
                            }
                        }

                        if (list.Count > 0)
                            await Console.Out.WriteLineAsync(list.ToString());
                    }
                }
            }
        }
    }
}
